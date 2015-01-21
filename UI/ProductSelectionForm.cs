using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Greet.DataStructureV3.Interfaces;
using Greet.DataStructureV3.Entities;
using Greet.DataStructureV3.ResultsStorage;
using Greet.DataStructureV3;

namespace SplitContributions.UI
{
    /// <summary>
    /// A simple form that shows a list of available pathways and mixes
    /// and display the associated GHGs results when an element is selected
    /// </summary>
    internal partial class ProductSelectionForm : Form
    {
        public ProductSelectionForm()
        {
            InitializeComponent();

            this.treeView1.MouseDown += new MouseEventHandler(treeView1_MouseDown);
        }

        /// <summary>
        /// Creates the list of available pathways and mixes
        /// </summary>
        public void InitializeControls()
        {
            //Gets the dictionary of IResource object indexed by IResource.Id
            IGDataDictionary<int, IResource> resources = SplitContributions.Controler.CurrentProject.Data.Resources;
            //Gets the dictionary of IPathways object indexed by IPathway.Id
            IGDataDictionary<int, IPathway> pathways = SplitContributions.Controler.CurrentProject.Data.Pathways;
            //Gets the dictionary of IMixes object indexed by IMid.Id
            IGDataDictionary<int, IMix> mixes = SplitContributions.Controler.CurrentProject.Data.Mixes;

            this.treeView1.Nodes.Clear();

            //Adds pathways and mixes to the list so the user can select one
            foreach (IResource resource in resources.AllValues.OrderBy(item => item.Name))
            {
                TreeNode resourceTreeNode = new TreeNode(resource.Name);
                resourceTreeNode.Tag = resource;

                foreach (IPathway pathway in pathways.AllValues.Where(item => SplitContributions.Controler.CurrentProject.Data.Helper.PathwayMainOutputResouce(item.Id) == resource.Id))
                {
                    TreeNode pathwayNode = new TreeNode("Pathway: "+ pathway.Name);
                    pathwayNode.Tag = pathway;
                    resourceTreeNode.Nodes.Add(pathwayNode);
                }

                foreach (IMix mix in mixes.AllValues.Where(item => item.MainOutputResourceID == resource.Id))
                {
                    TreeNode mixNode = new TreeNode("Mix: " + mix.Name);
                    mixNode.Tag = mix;
                    resourceTreeNode.Nodes.Add(mixNode);
                }

                if(resourceTreeNode.Nodes.Count >0)
                    this.treeView1.Nodes.Add(resourceTreeNode);
            }
        }

        /// <summary>
        /// Invoked when the user click on an item in the tree list view
        /// Retrieve the IPathway or IMix object stored in the tag and sends it to the ResultsControl 
        /// for displaying the results associated with that pathway or mix main output
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            treeView1.SelectedNode = treeView1.GetNodeAt(e.Location);

            if (this.treeView1.SelectedNode != null)
            {
                Object tag = this.treeView1.SelectedNode.Tag;
                IResults result = null;
                int productID = -1;
                string name = "";

                if (tag is IPathway)
                {//if the retrieved object is a pathway
                    IPathway path = tag as IPathway;
                    //We ask the pathway what is the product defined as the main product for this pathway
                    //then store an integer that corresponds to an IResource.ID
                    productID = SplitContributions.Controler.CurrentProject.Data.Helper.PathwayMainOutputResouce(path.Id);
                    //We use the ID of the Resource that corresponds to the main output of the pathway to get the correct results
                    Dictionary<IIO, IResults> availableResults = path.GetUpstreamResults(SplitContributions.Controler.CurrentProject.Data);
                    Guid desiredOutput = new Guid();
                    if (null == availableResults.Keys.SingleOrDefault(item => item.ResourceId == productID))
                    {
                        MessageBox.Show("Selected pathway does not produce the fuel selected. Please remove it from the Fuel Types list");
                        return;
                    }
                    else
                    {
                        foreach (IIO io in availableResults.Keys.Where(item => item.ResourceId == productID))
                        {
                            desiredOutput = io.Id;
                            result = availableResults.SingleOrDefault(item => item.Key.Id == desiredOutput).Value;
                            if (io.Id == path.MainOutput)
                            {
                                desiredOutput = io.Id;
                                break;
                            }
                        }
                    }
                    result = availableResults.SingleOrDefault(item => item.Key.Id == desiredOutput).Value;
                    //We set the string variable as the name of the pathway
                    name = path.Name;


                    
                    
                        
                        
                    foreach (IVertex vertex in path.Vertices)
                    {
                        IProcess processModel = SplitContributions.Controler.CurrentProject.Data.Processes.AllValues.Where(item => item.Id == vertex.ModelID) as IProcess;
                        AProcess processModelReal = processModel as AProcess;
                        Guid vertexId = vertex.ID;

                        Pathway p = path as Pathway;
                        foreach (KeyValuePair<Guid, CanonicalProcess> pair in p.CanonicalProcesses)
                        {
                           if (pair.Key == vertexId)
                           {
                                CanonicalProcess processResultsStorage = pair.Value;
                                //will give you results associated with all outputs of a process (Vertex) in the pathway
                                Dictionary<IIO, Results> processResults = processResultsStorage.GetResults(SplitContributions.Controler.CurrentProject.Data as GData);

                            }
                        }


                        if (processModel is StationaryProcess)
                        {
                            StationaryProcess sp = processModelReal as StationaryProcess;

                            foreach(IIO output in sp.FlattenAllocatedOutputList)
                            {
                                Guid outID = output.Id;
                            }
                            foreach(IIO input in sp.FlattenInputList)
                            {
                                Guid inpID = input.Id;
                            }
                        }
                        else
                        {
                            TransportationProcess tp = processModelReal as TransportationProcess;
                        }


                    }

                    foreach (IEdge edge in path.Edges)
                    {
                        Guid vertexFlowStart = edge.OutputVertexID;
                        Guid outputFlowStart = edge.OutputID;

                        Guid vertexFlowEnd = edge.InputVertexID;
                        Guid inputFlowEnd = edge.InputID;
                    }

                      


                }
                else if (tag is IMix)
                {//if the retrieved object is a mix
                    IMix mix = tag as IMix;
                    //We ask the mix what is the product defined as the main product for this mix
                    //then store an integer that corresponds to an IResource.ID
                    productID = mix.MainOutputResourceID;
                    //We use the ID of the Resource that corresponds to the main output of the pathway to get the correct results
                    var upstream = mix.GetUpstreamResults(SplitContributions.Controler.CurrentProject.Data);

                    if (null == upstream.Keys.SingleOrDefault(item => item.ResourceId == productID))
                    {
                        MessageBox.Show("Selected mix does not produce the fuel selected. Please remove it from the Fule Types list");
                        return;
                    }

                    //a mix has a single output so we can safely do the folowing
                    result = upstream.SingleOrDefault(item => item.Key.ResourceId == productID).Value;

                    //We set the string variable as the name of the pathway
                    name = mix.Name;
                }


            }
        }
    }
}
