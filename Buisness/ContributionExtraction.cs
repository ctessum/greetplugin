using Greet.Plugins.SplitContributions.Buisness.Entities;
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
        public static Dictionary<Guid, float[]> ExtractContributions(Graph graph, Guid startingPoint, int type, int[] gasOrResourceIDs, Value functionalUnit)
        {

            // Sample data ////////////////////////////////////
            
            Dictionary<Guid, float[]> data = new Dictionary<Guid, float[]>();
            Random random = new Random();
            for (int i = 0; i < 10; i++ )
            {
                float[] vals = new float[gasOrResourceIDs.Length];
                for (int j = 0; j<gasOrResourceIDs.Length; j++)
                {
                    double v = random.NextDouble();
                    vals[j] = (float)v;
                }
                data.Add(Guid.NewGuid(), vals);
            }
            ///////////////////////////////////////////////////

            return data;
        }

        /// <summary>
        /// Saves the extracted values to a file
        /// </summary>
        /// <param name="dictionary"></param>
        public static void SaveToFile(System.IO.StreamWriter fid, string[] outputVars, Graph graph)
        {
            Greet.Plugins.SplitContributions.Buisness.Entities.Value v = new Greet.Plugins.SplitContributions.Buisness.Entities.Value();


            int[] gasOrResourceIDs = new int[outputVars.Length];

            // This should be replaced with code that gets the actual IDs of the variable names.
            int i = 0;
            foreach (string var in outputVars)
            {
                gasOrResourceIDs[i] = i;
                i++;
            }

            // Write header to file.
            fid.WriteLine("# Process: xxxxxxxxxxxxxxx");
            fid.WriteLine("# Functional unit: xxxxxxxxxxxxxxx");
            fid.WriteLine("# GREET version: xxxxxxxxxxxxxxxxxx");
            fid.WriteLine("# Database version: xxxxxxxxxxxxxxxx");

            // Write variable names to file.
            StringBuilder varline = new StringBuilder();
            varline.Append("Process GUID");

            // Write data to file.
            Dictionary<Guid, float[]> data = ExtractContributions(graph, Guid.NewGuid(), 0, gasOrResourceIDs, v);
            foreach (var pair in data)
            {
                StringBuilder line = new StringBuilder();
                line.Append(pair.Key.ToString());
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
