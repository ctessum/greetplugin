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
        public static KeyValuePair<Guid, Guid> CrawlPathwayOutput(IPathway path, Guid outputID, out Graph g)
        {
            g = new Graph();
            return TracePathway(path, path.MainOutput, g);

        }

        /// <summary>
        /// Creates a complete graph structure of all processes involved in the production of the product in that pathway for the specified output id
        /// </summary>
        /// <param name="path">The pathway we want to crawl</param>
        /// <param name="outputID">The output ID targeted as the starting point</param>
        /// <returns>Graph structure containing all processes</returns>
        public static KeyValuePair<Guid, Guid> CrawlMixOutput(IMix mix, out Graph g)
        {
            g = new Graph();
            return TraceMix(mix, g);
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
            fakeProcess.Name = "Mix?";
            fakeProcess.VertexID = Guid.NewGuid();
            g.AddProcess(fakeProcess);

            POutput fakeOutput = new POutput();
            fakeOutput.Id = Guid.NewGuid();
            fakeProcess.Outputs.Add(fakeOutput);
            fakeOutput.ResourceID = mix.MainOutputResourceID;
            Mix m = mix as Mix;
            fakeOutput.Quantity = new Value(1, m.mixOutputResults.Results.FunctionalUnit);

            foreach (IProductionItem item in mix.FuelProductionEntities)
            {
                PInput fakeInput = new PInput();
                fakeInput.Id = Guid.NewGuid();
                fakeProcess.Inputs.Add(fakeInput);
                fakeInput.Quantity = new Value(1 * item.Share, m.mixOutputResults.Results.FunctionalUnit);
                
                if (item.SourceType == Enumerators.SourceType.Mix)
                {
                    IMix source = SplitContributions.Controler.CurrentProject.Data.Mixes.ValueForKey(item.MixOrPathwayId);
                    fakeInput.ResourceID = source.MainOutputResourceID;
                    KeyValuePair<Guid, Guid> link = TraceMix(source, g);
                    Flow f = new Flow(link.Key, link.Value, fakeProcess.VertexID, fakeInput.Id);
                    g.AddFlow(f);

                }
                else if (item.SourceType == Enumerators.SourceType.Pathway)
                {
                    IPathway source = SplitContributions.Controler.CurrentProject.Data.Pathways.ValueForKey(item.MixOrPathwayId);
                    fakeInput.ResourceID = source.MainOutputResourceID;
                    KeyValuePair<Guid, Guid> link = TracePathway(source, source.MainOutput, g);
                    Flow f = new Flow(link.Key, link.Value, fakeProcess.VertexID, fakeInput.Id);
                    g.AddFlow(f);
                }
                else
                    throw new Exception("Unknown source type");
            }

            return new KeyValuePair<Guid,Guid>(fakeProcess.VertexID, fakeOutput.Id);
        }

        /// <summary>
        /// This method takes as an input a reference to a pathway and the GUID of an output in that pathway in order to recursively 
        /// crawl the processes that are upstream of the selected output
        /// </summary>
        /// <param name="path">Reference to the pathway to be crawled</param>
        /// <param name="output">Desired output to be crawled</param>
        /// <param name="g">Graph object to which processes and flows will be added</param>
        /// <returns>Vertex ID and outptut ID of the clostest process to the selected output</returns>
        private static KeyValuePair<Guid, Guid> TracePathway(IPathway path, Guid output, Graph g)
        {
            IIO pathOutput = path.Outputs.Single(item => item.Id == output);
            PMOutput pOut = pathOutput as PMOutput;
            IEdge eOut = path.Edges.Single(item => item.InputVertexID == pOut.Id && item.InputID == pOut.Id);

            KeyValuePair<Guid,Guid> fakeProcess = TraceProcess(path, eOut.OutputVertexID, eOut.OutputID, g);

            return new KeyValuePair<Guid, Guid>(fakeProcess.Key, fakeProcess.Value);
        }

        /// <summary>
        /// This method uses a reference to an input and a pathway in order to determine which process/pathway/mix 
        /// is connected upstream of the input, then adds recursively processes and flows to the graph object
        /// </summary>
        /// <param name="path">The pathway containing the input we're trying to crawl upsteam</param>
        /// <param name="vertexId">The vertexID containing the process model for which we're trying to crawl an input</param>
        /// <param name="input">The input reference we're trying to crawl</param>
        /// <param name="g">The graph object beeing completed with processes and flows</param>
        /// <returns>Vertex ID and output ID of the closes process to the selected input</returns>
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
                    return TraceProcess(path, previousVertex.ID, edge.OutputID, g);
                }
                if (previousVertex.Type == 1)
                { //a pathway
                    IPathway prevPath = SplitContributions.Controler.CurrentProject.Data.Pathways.ValueForKey(previousVertex.ModelID);
                    return TracePathway(prevPath, prevPath.MainOutput, g);
                }
                if (previousVertex.Type == 2)
                { //a mix
                    IMix prevMix = SplitContributions.Controler.CurrentProject.Data.Mixes.ValueForKey(previousVertex.ModelID);
                    return TraceMix(prevMix, g);
                }
                throw new Exception("Previous vertex must be a process reference, or feed reference (to another mix or pathway)");
            }
            throw new Exception("Input source must be Mix, Pathway or Previous");
        }

        /// <summary>
        /// Creates a new process object in the graph and crawl it's inputs from "previous"
        /// This will be extended later on to crawl on all inputs
        /// </summary>
        /// <param name="path">The pathw containing the vertex/process</param>
        /// <param name="vertexID">The vertex ID containing the process model</param>
        /// <param name="outputId">Outptut ID used to connected to the vertex ID</param>
        /// <param name="g">Graph object beeing completed with the process and flow</param>
        /// <returns>VertexID and OutputID of the process inserted in the graph, a flow is also inserted and connected to that OutputID</returns>
        private static KeyValuePair<Guid, Guid> TraceProcess(IPathway path, Guid vertexID, Guid outputId, Graph g)
        {
            IVertex previousVertex = path.Vertices.Single(item => item.ID == vertexID);
            IProcess processModel = SplitContributions.Controler.CurrentProject.Data.Processes.ValueForKey(previousVertex.ModelID);
            AProcess ap = processModel as AProcess;
            CanonicalProcess cp = (path as Pathway).CanonicalProcesses[vertexID];

            Process fakeProcess;
            bool newProcess = false;
            if (g.Processes.Any(item => item.VertexID == vertexID))
                fakeProcess = g.Processes.Single(item => item.VertexID == vertexID);
            else
            {
                fakeProcess = new Process();
                fakeProcess.Name = processModel.Name;
                fakeProcess.ProcessModelId = processModel.Id;
                fakeProcess.VertexID = vertexID;
                newProcess = g.AddProcess(fakeProcess);
            }
            
            
            if(!fakeProcess.Outputs.Any(item => item.Id == outputId))
            {//adding the output beeing crawled
                POutput fakeOutput = new POutput();
                fakeOutput.Id = outputId;
                fakeOutput.Results = cp.OutputsResults[outputId].Results;
                AOutput output = ap.FlattenAllocatedOutputList.Single(item => item.Id == outputId) as AOutput;
                fakeOutput.Quantity = new Value(output.AmountAfterLossesBufffer.ValueInDefaultUnit, output.AmountAfterLossesBufffer.QuantityName);
                fakeOutput.ResourceID = output.ResourceId;
                fakeProcess.Outputs.Add(fakeOutput);
            }
            
            //adding the displaced outptus
            foreach (IIO outp in ap.FlattenAllocatedOutputList)
            {
                if (outp is CoProduct && (outp as CoProduct).method == CoProductsElements.TreatmentMethod.displacement)
                {
                    if(!fakeProcess.Outputs.Any(item => item.Id == outp.Id))
                    {//adding the output beeing crawled
                        POutput fakeCoProduct = new POutput();
                        fakeCoProduct.Id = outp.Id;
                        fakeCoProduct.Results = null;
                        fakeCoProduct.Quantity = new Value((outp as CoProduct).AmountAfterLossesBufffer.ValueInDefaultUnit, (outp as CoProduct).AmountAfterLossesBufffer.QuantityName);
                        fakeCoProduct.ResourceID = outp.ResourceId;
                        fakeCoProduct.IsDisplaced = true;
                        fakeCoProduct.Id = outputId;
                        fakeProcess.Outputs.Add(fakeCoProduct);
                    }
                }
            }
           
            if (newProcess)
            {//if new we crawl all inputs, otherwise we do not need to as it was already performed before
                foreach (Input inp in ap.FlattenInputList)
                {

                    PInput fakeInput = new PInput();
                    fakeInput.Id = inp.Id;
                    fakeProcess.Inputs.Add(fakeInput);
                    fakeInput.Quantity = new Value(inp.AmountForCalculations.ValueInDefaultUnit, inp.AmountForCalculations.QuantityName);
                    fakeInput.ResourceID = inp.ResourceId;

                    if (!inp.InternalProduct && inp.Source != Enumerators.SourceType.Well && inp.Source == Enumerators.SourceType.Previous)
                    {
                        KeyValuePair<Guid, Guid> link = TraceInput(path, previousVertex.ID, inp, g);
                        Flow f = new Flow(link.Key, link.Value, fakeProcess.VertexID, fakeInput.Id);
                        g.AddFlow(f);
                    }
                }
            }
            
            return new KeyValuePair<Guid, Guid>(fakeProcess.VertexID, outputId);
        }
    }
}
