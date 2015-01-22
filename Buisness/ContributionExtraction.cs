using Greet.Plugins.SplitContributions.Buisness.Entities;
using Greet.DataStructureV3.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            Process startingProcess = g.Processes.Single(item => item.Outputs.Any(oo => oo.Id == startingPoint));

            // Find the amount of each process that is required to create the output process.
            bool firstIteration = true;
            while(true) 
            {
                foreach(Process p in g.Processes) // Prepare for this iteration
                {
                    p.PreviousQuantity = p.Quantity.Copy(); 
                    if (p.VertexID==startingProcess.VertexID)
                    {
                         POutput startingOutput = p.Outputs.Single(oo => oo.Id == startingPoint);
                         p.Quantity = startingOutput.Quantity;
                    }
                    else
                    {
                        p.Quantity = new Value(0,p.Quantity.Unit); // Amount for this iteration will be calculated based on previous amount.
                    }
                }
                foreach(Process p in g.Processes) 
                {
                    foreach(PInput input in p.Inputs)
                    {
                        // The flow describes the link between and process requiring an input (p)
                        // and the process providing the output (previousProcess).
                        if(!g.Flows.Any(item => item.EndVertex == p.VertexID && item.EndInput == input.Id))
                            continue; // Change to exception later.

                        Flow flow = g.Flows.Single(item => item.EndVertex == p.VertexID && item.EndInput == input.Id);
                        
                        // previousProcess is the process providing the output. It is at the start vertex of the flow.
                        Process previousProcess = g.Processes.Single(item => item.VertexID == flow.StartVertex);
                      
                        // PreviousProcessOutput is the output of the previous process.
                        POutput previousProcessOutput = previousProcess.Outputs.Single(item => item.Id == flow.StartOutput);
                        // PreviousProcessQuantity is the quantity that is output by default.
                        Value previousProcessQuantity = previousProcessOutput.Quantity; 
                        
                        // The new quantity of the process providing the output (op) is the amount of the current
                        // process (p) times the amount required by the input (input), divided by the amount that is
                        // output by default. This new quantity is added to whatever quantity was previously calculated.
                        previousProcess.Quantity += input.Quantity * p.Quantity / previousProcessQuantity;
                    }
                    //foreach(POutput output in p.Outputs)
                    //{
                    //    Process op = g.Processes.Single(item => item.Inputs.Any(oo => oo.Id == output.Id));
                    //    op.Quantity += output.Quantity * p.Quantity;         
                    //}
                }
                if (!firstIteration) // Check for convergence
                {
                    bool converged = true;
                    foreach (Process p in g.Processes)
                    {
                        if (!p.CheckConverged()) 
                        {
                            converged = false;
                        }
                    }
                    if (converged) 
                    {
                        break;
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
                    line.Append(","+val.ToString());
                }
                fid.WriteLine(line.ToString());
            }
            fid.Close();
        }

    }
}
