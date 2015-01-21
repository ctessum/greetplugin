using System.Reflection;
using System.Windows.Forms;
using Greet.DataStructureV3.Interfaces;
using Greet.Model.Interfaces;
using SplitContributions.UI;

namespace SplitContributions
{
    /// <summary>
    /// This plugin allows the user to select a pathway output, then calculates the contributions of all processess
    /// involved into the production of the final product. This works by navigating the flows within the pathways
    /// and splitting the contributions of each processes based on their inputs and outputs amounts
    /// </summary>
    internal class SplitContributions : APlugin
    {
        /// <summary>
        /// Controller that allows access to the data and functions
        /// </summary>
        public static IGREETController Controler;

        /// <summary>
        /// A array of the menu items for this plugin
        /// </summary>
        ToolStripMenuItem[] items = new ToolStripMenuItem[1];

        #region APlugin
        /// <summary>
        /// Initialize the plugin, called once after the DLL is loaded into GREET
        /// </summary>
        /// <param name="controler"></param>
        /// <returns></returns>
        public override bool InitializePlugin(IGREETController controler)
        {
            //init the controller that is used to send action and data requests to GREET
            SplitContributions.Controler = controler;

            //init menu items collection for this example
            ToolStripMenuItem ex = new ToolStripMenuItem("SplitContributions");
            ex.Click += (s, e) =>
            {
                ProductSelectionForm form = new ProductSelectionForm();
                form.InitializeControls();
                form.Show();
            };
            this.items[0] = ex;

            return true;
        }

        public override string GetPluginName
        {
            get { return "SplitContributions : Calculating the contribution for individual processes"; }
        }

        public override string GetPluginDescription
        {
            get { return "Allows the user to select the output of a pathway, and tracks the contribution for each process"; }
        }

        public override string GetPluginVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public override System.Drawing.Image GetPluginIcon
        {
            get { return null; }
        }
        #endregion

        #region menu items

        /// <summary>
        /// Called when the GREET main form is initializing, returns the menu items for this plugin
        /// </summary>
        /// <returns></returns>
        public override System.Windows.Forms.ToolStripMenuItem[] GetMainMenuItems()
        {
            return this.items;
        }

        #endregion
    }
}
