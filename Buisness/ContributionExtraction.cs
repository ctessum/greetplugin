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
        public static Dictionary<string, double[]> ExtractContributions(Graph g, Guid startingPoint, int type, int[] gasOrResourceIDs, Value functionalUnit)
        {           
            //int resourceId = -1;
            //IResource resource = SplitContributions.Controler.CurrentProject.Data.Resources.ValueForKey(resourceId);
            //IValue value = resource.ConvertTo("kilograms", new Value(1, "energy"));

            NormalizeGraph(g, functionalUnit.Unit);
            
            Process startingProcess = g.Processes.Single(item => item.Outputs.Any(oo => oo.Id == startingPoint));

            // Find the amount of each process that is required to create the output process.
            bool firstIteration = true;
            bool keepGoing = true;
            while(keepGoing) 
            {
                foreach(Process p in g.Processes) // Prepare for this iteration
                {
                    p.PreviousQuantity = p.Quantity.Copy();
                    if (p.VertexID==startingProcess.VertexID) // Does this work?
                    {
                         POutput startingOutput = p.Outputs.Single(oo => oo.Id == startingPoint);
                         p.Quantity = startingOutput.Quantity;
                    }
                    else
                    {
                        p.Quantity  = new Value(0,p.Quantity.Unit); // Amount for this iteration will be calculated based on previous amount.
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

            Dictionary<string, double[]> data = new Dictionary<string, double[]>(); 
            Random random = new Random(); 
            int i = 0; 
            foreach (Process p in g.Processes)
            { 
                int numVars = gasOrResourceIDs.Length+1;
                double[] vals = new double[numVars];
                vals[0] = p.Quantity.Val;

                for (int j = 1; j<numVars; j++) 
                { 
                    double v = random.NextDouble(); 
                    vals[j] = v; 
                } 
                data.Add(i.ToString() + " " + p.Name + "," + p.ProcessModelId, vals); 
                i++; 
            } 

            return data;
        }

        /// <summary>
        /// Saves the extracted values to a file
        /// </summary>
        /// <param name="dictionary"></param>
        public static void SaveToFile(System.IO.StreamWriter fid, string[] outputVars, Graph graph, Guid startingOutputId, Value functionalUnit)
        {
            int[] gasOrResourceIDs = new int[outputVars.Length];

            // This should be replaced with code that gets the actual IDs of the variable names.
            int i = 0;
            foreach (string var in outputVars)
            {
                gasOrResourceIDs[i] = i;
                i++;
            }

            // Write header to file.
            fid.WriteLine("# Output process: xxxxxxxxxxxxxxx");
            fid.WriteLine("# Functional unit: xxxxxxxxxxxxxxx");
            fid.WriteLine("# GREET version: xxxxxxxxxxxxxxxxxx");
            fid.WriteLine("# Database version: xxxxxxxxxxxxxxxx");

            // Write variable names to file.
            StringBuilder varline = new StringBuilder();
            varline.Append("Process name, Process Model ID, amount");
            foreach (string var in outputVars)
            {
                varline.Append("," + var);
            }
            fid.WriteLine(varline.ToString());

            // Write data to file.
            Dictionary<string, double[]> data = ExtractContributions(graph, startingOutputId, 0, gasOrResourceIDs, functionalUnit);
            foreach (var pair in data)
            {
                StringBuilder line = new StringBuilder();
                line.Append(pair.Key);
                foreach (float val in pair.Value)
                {
                    line.Append("," + val.ToString());
                }
                fid.WriteLine(line.ToString());
            }
            fid.Close();
        }


        /// <summary>
        /// Normalizes a graph and convert all the quantities and results to the same unit or functional unit
        /// </summary>
        /// <param name="g">The graph to be normalized</param>
        /// <param name="unit">The desired unit "joule", "kilogram" or "cubic_meter"</param>
        /// <returns>Modified graph</returns>
        private static void NormalizeGraph(Graph g, string unit)
        {
            foreach (Process p in g.Processes)
                NormalizeProcess(p, unit);
        }


        /// <summary>
        /// Normalize IOs as well as results objects stored within that process
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

                output.Results = r.ConvertTo(SplitContributions.Controler.CurrentProject.Data as GData
                    , Unit2PluralUnit(unit)
                    , output.Results);
            }
        }

        private static string Unit2Group(string unit)
        {
            if (unit == "joule")
                return "energy";
            else if (unit == "cubic_meter")
                return "volume";
            else if (unit == "kilogram")
                return "mass";
            else
                throw new Exception("Unknow given unit");
        }

        private static string Group2Unit(string unit)
        {
            if (unit == "energy")
                return "joule";
            else if (unit == "volume")
                return "cubic_meter";
            else if (unit == "mass")
                return "kilogram";
            else
                throw new Exception("Unknow given unit");
        }

        private static string Unit2PluralUnit(string unit)
        {
             if (unit == "joule")
                return "joules";
            else if (unit == "cubic_meter")
                return "cubic_meters";
            else if (unit == "kilogram")
                return "kilograms";
            else
                throw new Exception("Unknow given unit");
        }
    }
}
