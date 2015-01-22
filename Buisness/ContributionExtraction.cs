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
        public static Dictionary<string, float[]> ExtractContributions(Graph g, Guid startingOutput, int type, int[] gasOrResourceIDs, Value functionalUnit)
        {
           // foreach (Process p in g.Processes) 
           // {
           //     string name = p.Name;
           //     foreach (PInput i in p.Inputs) {
           //         IProcess inputProcess = SplitContributions.Controler.CurrentProject.Data.Processes.ValueForKey(i.Id);
           //         string inputName = inputProcess.Name;
           //     }
           //     foreach (POutput o in p.Outputs)
           //     {
           //         IProcess outputProcess = SplitContributions.Controler.CurrentProject.Data.Processes.ValueForKey(o.Id);
           //         string outputName = outputProcess.Name;
           //     }
           // }

            // Sample data ////////////////////////////////////
            
            Dictionary<string, float[]> data = new Dictionary<string, float[]>();
            Random random = new Random();
            int i = 0;
            foreach (Process p in g.Processes)
            {
                float[] vals = new float[gasOrResourceIDs.Length];
                for (int j = 0; j<gasOrResourceIDs.Length; j++)
                {
                    double v = random.NextDouble();
                    vals[j] = (float)v;
                }
                data.Add(i.ToString() + " " + p.Name + "," + p.ProcessModelId, vals);
                i++;
            }
            ///////////////////////////////////////////////////

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
            varline.Append("Process name, Process Model ID");
            foreach (string var in outputVars)
            {
                varline.Append("," + var);
            }
            fid.WriteLine(varline.ToString());

            // Write data to file.
            Dictionary<string, float[]> data = ExtractContributions(graph, startingOutputId, 0, gasOrResourceIDs, functionalUnit);
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
