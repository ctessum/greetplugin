using Greet.Plugins.SplitContributions.Buisness.Entities;
using Greet.DataStructureV3.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Greet.DataStructureV3.Entities;
using Greet.DataStructureV3;

namespace Greet.Plugins.SplitContributions.Buisness
{
    /// <summary>
    /// This class defines methods that are using a graph structure and starting point in order to extract the contribution of all processes
    /// </summary>
    public static class ContributionExtraction
    {
        /// <summary>
        /// Navigates the graph and extract the contribution for all processes
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="startingPoint"></param>
        /// <param name="type"></param>
        /// <param name="gasOrResourceID"></param>
        /// <returns></returns>
        public static void ExtractContributions(Graph g, Guid startingPoint, int type, int[] gasOrResourceIDs, Value functionalUnit)
        {           
            //int resourceId = -1;
            //IResource resource = SplitContributions.Controler.CurrentProject.Data.Resources.ValueForKey(resourceId);
            //IValue value = resource.ConvertTo("kilograms", new Value(1, "energy"));

            NormalizeGraph(g, functionalUnit.Unit);
            
            Process startingProcess = g.Processes.Single(item => item.Outputs.Any(oo => oo.Id == startingPoint));
            startingProcess.IsStartingProcess = true;

            // Find the amount of each process that is required to create the output process.
            bool firstIteration = true;
            bool keepGoing = true;
            while(keepGoing) 
            {
                foreach(Process p in g.Processes) // Prepare for this iteration
                {
                    if (p.Quantity != null)
                        p.PreviousQuantity = p.Quantity.Copy();
                    if (p.IsStartingProcess)
                    {
                         POutput startingOutput = p.Outputs.Single(oo => oo.Id == startingPoint);
                         p.Quantity = startingOutput.Quantity;
                    }
                    else
                    {
                        p.Quantity = new Value(0, Unit2PluralUnit(functionalUnit.Unit)); // Amount for this iteration will be calculated based on previous amount.
                    }
                }
                foreach(Process p in g.Processes) 
                {
                    foreach(PInput input in p.Inputs)
                    {
                        // Some flows and processes are not currently tracked. If the flow is not tracked, continue on.
                        if(!g.Flows.Any(item => item.EndVertex == p.VertexID && item.EndInput == input.Id))
                            continue; // Change to exception later.

                        // The flow describes the link between and process requiring an input (p)
                        // and the process providing the output (previousProcess).
                        Flow flow = g.Flows.Single(item => item.EndVertex == p.VertexID && item.EndInput == input.Id);

                        // Some flows and processes are not currently tracked. If the flow is not tracked, continue on.
                        if (!g.Processes.Any(item => item.VertexID == flow.StartVertex))
                            continue; // Change to exception later.

                        // previousProcess is the process providing the output. It is at the start vertex of the flow.
                        Process previousProcess = g.Processes.Single(item => item.VertexID == flow.StartVertex);
                      
                        // PreviousProcessOutput is the output of the previous process.
                        POutput previousProcessOutput = previousProcess.Outputs.Single(item => item.Id == flow.StartOutput);
                        // PreviousProcessQuantity is the quantity that is output by default.
                        Value previousProcessQuantity = previousProcessOutput.Quantity; 
                        
                        // The new quantity of the process providing the output (op) is the amount of the current
                        // process (p) times the amount required by the input (input), divided by the amount that is
                        // output by default. This new quantity is added to whatever quantity was previously calculated.
                        previousProcess.Quantity += input.Quantity * p.Quantity / previousProcessQuantity; // Does this include loss factors?
                    }
                    //foreach(POutput output in p.Outputs)
                    //{
                    //    Process op = g.Processes.Single(item => item.Inputs.Any(oo => oo.Id == output.Id));
                    //    op.Quantity += output.Quantity * p.Quantity;         
                    //}
                }
                if (!firstIteration) // Check for convergence
                {
                    keepGoing = false;
                    foreach (Process p in g.Processes)
                    {
                        if (!p.CheckConverged()) 
                        {
                            keepGoing = true;
                            break;
                        }
                    }
                }
                else 
                {
                    firstIteration = false;
                }
            }

            // After we know the amount of each process that's required, we can multiply that
            // amount by the onsite emissions intensity to get the emissions contribution of 
            // that process.
            foreach(Process p in g.Processes)
            {
                p.EmissionsContribution = new double[gasOrResourceIDs.Length];
                for (int j = 1; j < gasOrResourceIDs.Length; j++)
                {
                    double emissions = 0;
                    // The onsite emissions are allocated among the outputs. Since we're interested in
                    // the ...........problem here.
                    foreach (POutput o in p.Outputs)
                    {
                        if (o.Results!=null && o.Results.onsiteEmissions.ContainsKey(gasOrResourceIDs[j]))
                            emissions += o.Results.onsiteEmissions[gasOrResourceIDs[j]];
                    }
                    p.EmissionsContribution[j] = emissions;
                } 
            }
            return;
        }

        /// <summary>
        /// Saves the extracted values to a file
        /// </summary>
        /// <param name="dictionary"></param>
        public static void SaveToFile(System.IO.StreamWriter fid, string[] outputVars, Graph g, Guid startingOutputId, Value functionalUnit)
        {

            // Get the IDs associated with the variable names.
            int[] gasOrResourceIDs = new int[outputVars.Length];
            int i = 0;
            foreach (string var in outputVars)
            {
                bool found = false;
                foreach (IGas item in SplitContributions.Controler.CurrentProject.Data.Gases.AllValues)
                {
                    if (item.Name==var)
                    {
                        gasOrResourceIDs[i] = item.Id;
                        found = true;
                        break;
                    }
                }
                if (!found)
                    throw new Exception("Can't match output variable.");
                i++;
            }

            // Calculate the emissions contributions
            ExtractContributions(g, startingOutputId, 0, gasOrResourceIDs, functionalUnit);

            // Find the starting process
            Process startingProcess = null;
            foreach (Process p in g.Processes)
            {
                if (p.IsStartingProcess)
                {
                    startingProcess = p;
                    break;
                }
            }

            // Write header to file.
            fid.WriteLine("# Output process name:, " + startingProcess.Name);
            fid.WriteLine("# Output process model ID:, " + startingProcess.ProcessModelId.ToString());
            fid.WriteLine("# Functional unit:, xxxxxxxxxxxxxxx");
            fid.WriteLine("# GREET version:, xxxxxxxxxxxxxxxxxx");
            fid.WriteLine("# Database version:, xxxxxxxxxxxxxxxx");

            // Write variable names to file.
            StringBuilder varline = new StringBuilder();
            varline.Append("Process name, Process Model ID, amount");
            foreach (string var in outputVars)
            {
                varline.Append("," + var);
            }
            fid.WriteLine(varline.ToString());

            // Write data to file.
            foreach (Process p in g.Processes)
            {
                StringBuilder line = new StringBuilder();
                line.Append(p.Name + "," + p.ProcessModelId.ToString() + "," + p.Quantity.Val.ToString());
                foreach (float val in p.EmissionsContribution)
                {
                    line.Append("," + val.ToString());
                }
                fid.WriteLine(line.ToString());
            }
            fid.Close();
        }


        /// <summary>
        /// <para>Normalizes a graph and convert all the quantities and results to the same unit or functional unit</para>
        /// <para>WARNING: This method modifies the content of the instance of the Graph</para>
        /// </summary>
        /// <param name="g">The graph to be normalized</param>
        /// <param name="unit">The desired unit "joule", "kilogram" or "cubic_meter"</param>
        private static void NormalizeGraph(Graph g, string unit)
        {
            foreach (Process p in g.Processes)
                NormalizeProcess(p, unit);
        }


        /// <summary>
        /// <para>Normalize IOs as well as results objects stored within that process</para>
        /// <para>WARNING: This method modifies the content of the instance of the Process</para>
        /// </summary>
        /// <param name="p">The process to be normalized</param>
        /// <param name="unit">The desired unit "joule", "kilogram" or "cubic_meter"</param>
        private static void NormalizeProcess(Process p, string unit)
        {
            //Normalizes inputs and outputs
            foreach (PInput input in p.Inputs)
            {
                IResource resource = SplitContributions.Controler.CurrentProject.Data.Resources.ValueForKey(input.ResourceID);
                ResourceData r = resource as ResourceData;
                input.Quantity.Unit = Unit2Group(input.Quantity.Unit);
                IValue converted = resource.ConvertTo(Unit2PluralUnit(unit), input.Quantity);
                input.Quantity.Val = converted.Value;
                input.Quantity.Unit = converted.Unit;       
            }

            //Normalize results
            foreach (POutput output in p.Outputs)
            {
                IResource resource = SplitContributions.Controler.CurrentProject.Data.Resources.ValueForKey(output.ResourceID);
                ResourceData r = resource as ResourceData;
                output.Quantity.Unit = Unit2Group(output.Quantity.Unit);
                IValue converted = resource.ConvertTo(Unit2PluralUnit(unit), output.Quantity);
                output.Quantity.Val = converted.Value;
                output.Quantity.Unit = converted.Unit;

                if (output.Results != null)
                    output.Results = r.ConvertTo(SplitContributions.Controler.CurrentProject.Data as GData
                        , Unit2PluralUnit(unit)
                        , output.Results);
            }
        }


        /// <summary>
        /// Method to help with inconsistencies with unit names
        /// Converts units or groups to "energy", "volume" or "mass"
        /// </summary>
        /// <param name="unit">Unit name or group name</param>
        /// <returns>"energy", "volume" or "mass"</returns>
        private static string Unit2Group(string unit)
        {
            if (unit == "joule" || unit == "joules" || unit == "energy")
                return "energy";
            else if (unit == "cubic_meter" || unit == "cubic_meters" || unit == "volume")
                return "volume";
            else if (unit == "kilogram" || unit == "kilograms" || unit == "mass")
                return "mass";
            else
                throw new Exception("Unknown given unit");
        }

        /// <summary>
        /// Method to help with inconsistencies with unit names
        /// Converts units or groups to "joule", "cubic_meter" or "kilogram"
        /// </summary>
        /// <param name="unit">Unit name or group name</param>
        /// <returns>"joule", "cubic_meter" or "kilogram"</returns>
        private static string Group2UnitSingular(string unit)
        {
            if (unit == "energy" || unit == "joule" || unit == "joules")
                return "joule";
            else if (unit == "volume" || unit == "cubic_meter" || unit == "cubic_meters")
                return "cubic_meter";
            else if (unit == "mass" || unit == "kilogram" || unit == "kilograms")
                return "kilogram";
            else
                throw new Exception("Unknown given unit");
        }

        /// <summary>
        /// Method to help with inconsistencies with unit names
        /// Converts units or groups to "joules", "cubic_meters" or "kilograms"
        /// </summary>
        /// <param name="unit">Unit name or group name</param>
        /// <returns>"joules", "cubic_meters" or "kilograms"</returns>
        private static string Unit2PluralUnit(string unit)
        {
             if (unit == "joule" || unit == "joules" || unit == "energy")
                return "joules";
            else if (unit == "cubic_meter" || unit == "cubic_meters" || unit == "volume")
                return "cubic_meters";
            else if (unit == "kilogram" || unit == "kilograms" || unit == "mass")
                return "kilograms";
            else
                throw new Exception("Unknown given unit");
        }
    }
}
