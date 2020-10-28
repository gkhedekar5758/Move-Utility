using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allscripts.VSTSLibrary
{
    public class VSTS
    {

        public Uri ServerUri { get; set; }
        public string LocalPath { get; set; }
        public VSTS()
        {
            ServerUri = new Uri("http://alm-prod-app1:8080/tfs/boc_projects/");
            LocalPath = @"C:\APMMoveUtility\";
        }
    }
}
