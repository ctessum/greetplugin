using Greet.DataStructureV3.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Greet.Plugins.SplitContributions.Buisness
{
    /// <summary>
    /// This static class defines methods that are crawling though the pathways and mixes and returns a complete structure of all processes and flows used
    /// </summary>
    public static class Crawler
    {
        /// <summary>
        /// Creates a complete graph structure of all processes involved in the production of the product in that pathway for the specified output id
        /// </summary>
        /// <param name="path">The pathway we want to crawl</param>
        /// <param name="outputID">The output ID targeted as the starting point</param>
        /// <returns>Graph structure containing all processes</returns>
        public static Graph CrawlPathwayOutput(IPathway path, Guid outputID)
        {
        //    //if the retrieved object is a pathway
        //    IPathway path = tag as IPathway;
        //    //We ask the pathway what is the product defined as the main product for this pathway
        //    //then store an integer that corresponds to an IResource.ID
        //    productID = SplitContributions.Controler.CurrentProject.Data.Helper.PathwayMainOutputResouce(path.Id);
        //    //We use the ID of the Resource that corresponds to the main output of the pathway to get the correct results
        //    Dictionary<IIO, IResults> availableResults = path.GetUpstreamResults(SplitContributions.Controler.CurrentProject.Data);
        //    Guid desiredOutput = new Guid();
        //    if (null == availableResults.Keys.SingleOrDefault(item => item.ResourceId == productID))
        //    {
        //        MessageBox.Show("Selected pathway does not produce the fuel selected. Please remove it from the Fuel Types list");
        //        return;
        //    }
        //    else
        //    {
        //        foreach (IIO io in availableResults.Keys.Where(item => item.ResourceId == productID))
        //        {
        //            desiredOutput = io.Id;
        //            result = availableResults.SingleOrDefault(item => item.Key.Id == desiredOutput).Value;
        //            if (io.Id == path.MainOutput)
        //            {
        //                desiredOutput = io.Id;
        //                break;
        //            }
        //        }
        //    }
        //    result = availableResults.SingleOrDefault(item => item.Key.Id == desiredOutput).Value;
        //    //We set the string variable as the name of the pathway
        //    name = path.Name;


        //    foreach (IVertex vertex in path.Vertices)
        //    {
        //        IProcess processModel = SplitContributions.Controler.CurrentProject.Data.Processes.AllValues.Where(item => item.Id == vertex.ModelID) as IProcess;
        //        AProcess processModelReal = processModel as AProcess;
        //        Guid vertexId = vertex.ID;

        //        Pathway p = path as Pathway;
        //        foreach (KeyValuePair<Guid, CanonicalProcess> pair in p.CanonicalProcesses)
        //        {
        //            if (pair.Key == vertexId)
        //            {
        //                CanonicalProcess processResultsStorage = pair.Value;
        //                //will give you results associated with all outputs of a process (Vertex) in the pathway
        //                Dictionary<IIO, Results> processResults = processResultsStorage.GetResults(SplitContributions.Controler.CurrentProject.Data as GData);

        //            }
        //        }


        //        if (processModel is StationaryProcess)
        //        {
        //            StationaryProcess sp = processModelReal as StationaryProcess;

        //            foreach (IIO output in sp.FlattenAllocatedOutputList)
        //            {
        //                Guid outID = output.Id;
        //            }
        //            foreach (IIO input in sp.FlattenInputList)
        //            {
        //                Guid inpID = input.Id;
        //            }
        //        }
        //        else
        //        {
        //            TransportationProcess tp = processModelReal as TransportationProcess;
        //        }


        //    }

        //    foreach (IEdge edge in path.Edges)
        //    {
        //        Guid vertexFlowStart = edge.OutputVertexID;
        //        Guid outputFlowStart = edge.OutputID;

        //        Guid vertexFlowEnd = edge.InputVertexID;
        //        Guid inputFlowEnd = edge.InputID;
        //    }

                      
            return null;

        }

        /// <summary>
        /// Creates a complete graph structure of all processes involved in the production of the product in that pathway for the specified output id
        /// </summary>
        /// <param name="path">The pathway we want to crawl</param>
        /// <param name="outputID">The output ID targeted as the starting point</param>
        /// <returns>Graph structure containing all processes</returns>
        public static Graph CrawlMixOutput(IMix mix)
        {
            ////We ask the mix what is the product defined as the main product for this mix
            //        //then store an integer that corresponds to an IResource.ID
            //        productID = mix.MainOutputResourceID;
            //        //We use the ID of the Resource that corresponds to the main output of the pathway to get the correct results
            //        var upstream = mix.GetUpstreamResults(SplitContributions.Controler.CurrentProject.Data);

            //        if (null == upstream.Keys.SingleOrDefault(item => item.ResourceId == productID))
            //        {
            //            MessageBox.Show("Selected mix does not produce the fuel selected. Please remove it from the Fule Types list");
            //            return;
            //        }

            //        //a mix has a single output so we can safely do the folowing
            //        result = upstream.SingleOrDefault(item => item.Key.ResourceId == productID).Value;

            //        //We set the string variable as the name of the pathway
            //        name = mix.Name;

            return null;
        }

    }
}
