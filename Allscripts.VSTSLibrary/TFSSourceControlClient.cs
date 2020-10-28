using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Windows.Forms;

namespace Allscripts.VSTSLibrary
{
    //this is not needed as of now
    public class TFSSourceControlClient
    {
        public Item frmSelectedItem;
        public Item toSelectedItem;
        public string sfromBranch;
        public string stoBranch;
        private TfsTeamProjectCollection tfsTeamProjectCollection;
        private VersionControlServer versionControl;
        //public TFSSourceControlClient(VSTS objVSTS)
        //{
        //    try
        //    {
        //        tfsTeamProjectCollection = new TfsTeamProjectCollection(objVSTS.ServerUri);
        //        tfsTeamProjectCollection.Authenticate();
        //        versionControl = tfsTeamProjectCollection.GetService<VersionControlServer>();

        //    }
        //    catch (Exception ex)
        //    {
                
        //        MessageBox.Show("An error occured while establishing connection to tfs server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        LogFile.WriteLine(String.Format("{0} - TFS source Control Initialization failed", System.DateTime.Now.ToString(@"MM/dd/yyyy HH:mm:ss.ffff")), objVSTS);
        //        LogFile.WriteLine(string.Format("\t Exception {0}", ex.Message), objVSTS);

        //        tfsTeamProjectCollection = null;
        //        versionControl = null;
        //        return;
        //    }
        //}
        //~TFSSourceControlClient()
        //{
        //    if (tfsTeamProjectCollection != null) tfsTeamProjectCollection.Dispose();
        //}
    }
}
