using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allscripts.VSTSLibrary
{
    public static class LogFile
    {
        public static void WriteLine(string sLine,VSTS objVSTS=null)
        {
            if (sLine == null) return;
            if (objVSTS == null) return;

            string sFileName;

            sFileName = objVSTS.LocalPath + "log" + System.DateTime.Today.ToString("MMddyyyy") + ".txt";

            try
            {
                if (Directory.Exists(objVSTS.LocalPath) == false) Directory.CreateDirectory(objVSTS.LocalPath);

                StreamWriter sw = File.AppendText(sFileName);

                sw.WriteLine(sLine);

                sw.Flush();

                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Logfile.WriteLine Error: {0} - {1}", ex.ToString(), ex.Message);
            }

            return;
        }
    }
}
