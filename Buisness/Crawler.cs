using Greet.DataStructureV3.Interfaces;
using Greet.Plugins.SplitContributions.Buisness.Entities;
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
            Graph g = new Graph();

            TraceMix(mix, g);

            return g;
        }

        /// <summary>
        /// <para>Completes the Graph by adding the upstream found in a mix attached as a feed of the process P</para>
        /// <para>Mix -> P</para>
        /// </summary>
        /// <param name="mix">The mix we want to trace upstream</param>
        /// <param name="g">The graph that is beeing completed</param>
        /// <param name="p">The process P that uses Mix as a feed</param>
        /// <returns>Couple of the <ExitVertex, ExitOutput></returns>
        private static KeyValuePair<Guid, Guid> TraceMix(IMix mix, Graph g)
        {
            Process fakeProcess = new Process();
            g.AddProcess(fakeProcess);

            POutput fakeOutput = new POutput();
            fakeOutput.Id = Guid.NewGuid();
            fakeProcess.Outputs.Add(fakeOutput);

            foreach (IProductionItem item in mix.FuelProductionEntities)
            {
                PInput fakeInput = new PInput();
                fakeInput.Id = Guid.NewGuid();
                fakeProcess.Inputs.Add(fakeInput);

                if (item.SourceType == Enumerators.SourceType.Mix)
                {
                    IMix source = SplitContributions.Controler.CurrentProject.Data.Mixes.ValueForKey(item.MixOrPathwayId);

                    Flow f = new Flow(Guid.Empty,Guid.Empty,Guid.Empty,Guid.Empty);
                    g.AddFlow(f);

                }
                else if (item.SourceType == Enumerators.SourceType.Pathway)
                {
                    IPathway source = SplitContributions.Controler.CurrentProject.Data.Pathways.ValueForKey(item.MixOrPathwayId);
                    KeyValuePair<Guid, Guid> link = TracePathway(source, source.MainOutput, g);
                    Flow f = new Flow(link.Key, link.Value, fakeProcess.VertexID, fakeInput.Id);
                    g.AddFlow(f);
                }
                else
                    throw new Exception("Unknown source type");
            }

            return new KeyValuePair<Guid,Guid>(fakeProcess.VertexID, fakeOutput.Id);
        }

        private static KeyValuePair<Guid, Guid> TracePathway(IPathway path, Guid output, Graph g)
        {
            return new KeyValuePair<Guid,Guid>(Guid.Empty,Guid.Empty);
        }

        private static KeyValuePair<Guid, Guid> TraceInput(IPathway path, Guid vertexId, Guid inputId, Graph g)
        {
            return new KeyValuePair<Guid,Guid>(Guid.Empty, Guid.Empty);
        }
    }
}
