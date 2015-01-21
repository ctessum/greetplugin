using Greet.DataStructureV3.Entities;
using Greet.DataStructureV3.Interfaces;
using Greet.DataStructureV3.ResultsStorage;
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
            Graph g = new Graph();

            TracePathway(path, path.MainOutput, g);

            return g;
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
            fakeProcess.VertexID = Guid.NewGuid();
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
                    KeyValuePair<Guid, Guid> link = TraceMix(source, g);
                    Flow f = new Flow(link.Key, link.Value, fakeProcess.VertexID, fakeInput.Id);
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
            IIO pathOutput = path.Outputs.Single(item => item.Id == output);
            PMOutput pOut = pathOutput as PMOutput;
            IEdge eOut = path.Edges.Single(item => item.InputVertexID == pOut.Id && item.InputID == pOut.Id);

            KeyValuePair<Guid,Guid> fakeProcess = TraceProcess(path, eOut.OutputVertexID, eOut.OutputID, g);

            return new KeyValuePair<Guid, Guid>(fakeProcess.Key, fakeProcess.Value);
        }

        private static KeyValuePair<Guid, Guid> TraceInput(IPathway path, Guid vertexId, Input input, Graph g)
        {
            if (input.Source == Enumerators.SourceType.Mix)
            {
                IMix source = SplitContributions.Controler.CurrentProject.Data.Mixes.ValueForKey(input.SourceMixOrPathwayID);
                KeyValuePair<Guid, Guid> link = TraceMix(source, g);
                return new KeyValuePair<Guid, Guid>(link.Key, link.Value);
            }
            else if (input.Source == Enumerators.SourceType.Pathway)
            {
                IPathway source = SplitContributions.Controler.CurrentProject.Data.Pathways.ValueForKey(input.SourceMixOrPathwayID);
                KeyValuePair<Guid, Guid> link = TracePathway(source, source.MainOutput, g);
                return new KeyValuePair<Guid, Guid>(link.Key, link.Value);
            }
            else if (input.Source == Enumerators.SourceType.Previous)
            {
                IEdge edge = path.Edges.Single(item => item.InputVertexID == vertexId && item.InputID == input.Id);
                IVertex previousVertex = path.Vertices.Single(item => item.ID == edge.OutputVertexID);

                if (previousVertex.Type == 0)
                { //a process
                    TraceProcess(path, previousVertex.ID, edge.OutputID, g);
                }
                if (previousVertex.Type == 1)
                { //a pathway
                    IPathway prevPath = SplitContributions.Controler.CurrentProject.Data.Pathways.ValueForKey(previousVertex.ModelID);
                    TracePathway(prevPath, prevPath.MainOutput, g);
                }
                if (previousVertex.Type == 2)
                { //a mix
                    IMix prevMix = SplitContributions.Controler.CurrentProject.Data.Mixes.ValueForKey(previousVertex.ModelID);
                    TraceMix(prevMix, g);
                }

                return new KeyValuePair<Guid, Guid>(edge.OutputVertexID, edge.OutputID);
            }
            throw new Exception("Input source must be Mix, Pathway or Previous");
        }

        private static KeyValuePair<Guid, Guid> TraceProcess(IPathway path, Guid vertexID, Guid outputId, Graph g)
        {
            IVertex previousVertex = path.Vertices.Single(item => item.ID == vertexID);
            IProcess processModel = SplitContributions.Controler.CurrentProject.Data.Processes.ValueForKey(previousVertex.ModelID);
            CanonicalProcess cp = (path as Pathway).CanonicalProcesses[vertexID];

            Process fakeProcess = new Process();
            fakeProcess.Name = processModel.Name;
            fakeProcess.ProcessResults = cp;
            fakeProcess.ProcessModelId = processModel.Id;
            fakeProcess.VertexID = vertexID;
            g.AddProcess(fakeProcess);

            POutput fakeOutput = new POutput();
            fakeOutput.Id = outputId;
            fakeProcess.Outputs.Add(fakeOutput);

            if (processModel is AProcess)
            {
                AProcess ap = processModel as AProcess;
                foreach (Input inp in ap.FlattenInputList)
                {
                    PInput fakeInput = new PInput();
                    fakeInput.Id = inp.Id;
                    fakeProcess.Inputs.Add(fakeInput);
                    if (!inp.InternalProduct && inp.Source != Enumerators.SourceType.Well && inp.Source == Enumerators.SourceType.Previous)
                    {
                        KeyValuePair<Guid, Guid> link = TraceInput(path, previousVertex.ID, inp, g);
                        Flow f = new Flow(link.Key, link.Value, fakeProcess.VertexID, fakeInput.Id);
                        g.AddFlow(f);
                    }
                }
            }

            return new KeyValuePair<Guid, Guid>(fakeProcess.VertexID, fakeOutput.Id);
        }
    }
}
