using System;
using System.Windows;
using System.Windows.Forms;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using System.Reflection;
using Allscripts.VSTSLibrary;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;

namespace Move_Utility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string sFromFullPath = string.Empty;
        string sToFullPath = string.Empty;
        string sFromFullPathLocal = string.Empty;
        string sToFullPathLocal = string.Empty;
        TfsTeamProjectCollection tfsTeamProjectCollection = null;
        VersionControlServer versionControlServer = null;
        VSTS objVSTS;
        public MainWindow()
        {
            InitializeComponent();
            CheckTfs();
            
        }

        
        private void CheckTfs()
        {
            try
            {
                txtLogTextBox.Text = "Connecting to TFS...";
                objVSTS = new VSTS();
                //objTFSSourceControlClient = new TFSSourceControlClient();

                tfsTeamProjectCollection = new TfsTeamProjectCollection(objVSTS.ServerUri);
                                                   
                tfsTeamProjectCollection.EnsureAuthenticated();
                versionControlServer = tfsTeamProjectCollection.GetService<VersionControlServer>();
                txtLogTextBox.Text = txtLogTextBox.Text + "\n" + "Connected..";
            }
            catch (Exception ex)
            {
                txtLogTextBox.Text = "Could Not establish connection to TFS server. Try After Sometime";
                System.Windows.MessageBox.Show(string.Format( "An error occured while establishing connection to tfs server \n Please review Log File"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LogFile.WriteLine(String.Format("{0} - TFS source Control Initialization failed", System.DateTime.Now.ToString(@"MM/dd/yyyy HH:mm:ss.ffff")), objVSTS);
                LogFile.WriteLine(string.Format("{0} Exception :-  {1}", DateTime.Now.ToString(@"MM/dd/yyyy HH:mm:ss.ffff"), ex.Message), objVSTS);

                tfsTeamProjectCollection = null;
                versionControlServer = null;
                return;
            }
        }

        private void BtnBrowseSource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                    #region TextBox
                    //This region Get the Source item from User which he would like to change or Compare in this utility
                    Form _chooseItemDialog = new Form();
                    Assembly controlAssembly = Assembly.GetAssembly(typeof(Microsoft.TeamFoundation.VersionControl.Controls.ControlAddItemsExclude));
                    Type vcChooseItem = controlAssembly.GetType("Microsoft.TeamFoundation.VersionControl.Controls.DialogChooseItem");

                    ConstructorInfo ci = vcChooseItem.GetConstructor(
                           BindingFlags.Instance | BindingFlags.NonPublic,
                           null,
                           new Type[] { typeof(VersionControlServer) },
                           null);

                    _chooseItemDialog = (Form)ci.Invoke(new object[] { versionControlServer });
                    _chooseItemDialog.ShowDialog();


                    DialogResult DialogResult = _chooseItemDialog.DialogResult;

                    var fullpath = vcChooseItem.GetProperty("SelectedItem", BindingFlags.Instance | BindingFlags.NonPublic);
                    Item selecteditem = (Item)fullpath.GetValue(_chooseItemDialog, null);
                    //objTFSSourceControlClient.frmSelectedItem = selecteditem;
                    
                    if (_chooseItemDialog.DialogResult == DialogResult.OK && string.Equals(selecteditem.ItemType.ToString(), "File", StringComparison.CurrentCultureIgnoreCase))
                    {
                        txtSourceFile.Text = selecteditem.ServerItem;
                       
                        sFromFullPath = selecteditem.ServerItem.ToString();

                        MakeTOFullPath();
                    }
                    else
                    {
                        //if user has not selected anything and then again clicks the browse button then paths are added twice
                        return;
                    }
                    #endregion

                    #region ComboBox
                    //This region will fill the comboBox for target/Destination

                    ICommonStructureService structureService = (ICommonStructureService)tfsTeamProjectCollection.GetService(typeof(ICommonStructureService));
                    ProjectInfo projects = structureService.GetProjectFromName("Allscripts PM");

                    Item[] items = null;
                    string path = "$/" + projects.Name.ToString();
                    ItemSet itemSetTemp = versionControlServer.GetItems(path, RecursionType.OneLevel);
                    items = itemSetTemp.Items;

                    foreach (var item in items)
                    {
                    this.cmbDestinationFolders.SelectedValuePath = "Key";
                    this.cmbDestinationFolders.DisplayMemberPath = "Value";
                    if (item.ServerItem.Contains("Main")) cmbDestinationFolders.Items.Add(new KeyValuePair<string, string>(item.ServerItem, item.ServerItem.Replace("$/Allscripts PM/", string.Empty)));
                        
                        if (item.ServerItem.Contains("Development") || item.ServerItem.Contains("Release"))
                        {
                            path = item.ServerItem;
                            itemSetTemp = versionControlServer.GetItems(path, RecursionType.OneLevel);
                            foreach (var item1 in itemSetTemp.Items)
                            {
                            if (item1 != itemSetTemp.Items[0])
                                
                                cmbDestinationFolders.Items.Add(new KeyValuePair<string, string>(item1.ServerItem, item1.ServerItem.Replace("$/Allscripts PM/", string.Empty)));
                            }
                        }

                    }
                    #endregion

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(string.Format("Error Occured \n Please review Log File"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                  LogFile.WriteLine(string.Format("{0} Exception :-  {1}", DateTime.Now.ToString(@"MM/dd/yyyy HH:mm:ss.ffff"), ex.Message), objVSTS);
            }

            
        }




        /// <summary>
        /// BtnCompare_Click- This will compare the code from source to destination branch
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCompare_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                //user has selected nothing or user has selected same branch
                if (GetBranchFromPath(sFromFullPath) == GetBranchFromPath(sToFullPath) || sFromFullPath == string.Empty || sToFullPath == string.Empty)
                {
                    System.Windows.MessageBox.Show(string.Format("This wont work! \n Comparing from {0} to {1} ", sFromFullPath == string.Empty ? "nothing" : GetBranchFromPath(sFromFullPath),
                        sToFullPath == string.Empty ? "nothing" : GetBranchFromPath(sToFullPath)),
                         "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                //FROM file from FROM branch
                var fromItemToBeDownloaded = versionControlServer.GetItem(sFromFullPath);
                string sServerFileName = sFromFullPath.Substring(sFromFullPath.LastIndexOf('/') + 1);

                string fromLocalPath = objVSTS.LocalPath + sFromFullPath.Substring(2, (sFromFullPath.LastIndexOf('/') - 1)).Replace('/', '\\');
                if (Directory.Exists(fromLocalPath) == false) Directory.CreateDirectory(fromLocalPath);
                sFromFullPathLocal = Path.Combine(fromLocalPath, sServerFileName);
                fromItemToBeDownloaded.DownloadFile(sFromFullPathLocal);
                //remove the read only attribute
                FileInfo fileInfo = new FileInfo(sFromFullPathLocal);
                if (fileInfo.IsReadOnly) fileInfo.IsReadOnly = false;

                //TO file from TO branch
                var isToFileExist = versionControlServer.ServerItemExists(sToFullPath, ItemType.File);
                if (!isToFileExist)
                {
                    System.Windows.MessageBox.Show(string.Format("File do not exist in 'TO' folder \n It is a newly added file in 'FROM' Branch \n Can Not Compare!!"), "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                    
                var toItemToBeDownloaded = versionControlServer.GetItem(sToFullPath);
                string toLocalPath = objVSTS.LocalPath + sToFullPath.Substring(2, (sToFullPath.LastIndexOf('/') - 1)).Replace('/', '\\');
                if (Directory.Exists(toLocalPath) == false) Directory.CreateDirectory(toLocalPath);
                sToFullPathLocal = Path.Combine(toLocalPath, sServerFileName);
                toItemToBeDownloaded.DownloadFile(sToFullPathLocal);
                //remove the read only attribute
                fileInfo = new FileInfo(sToFullPathLocal);
                if (fileInfo.IsReadOnly) fileInfo.IsReadOnly = false;

                txtLogTextBox.Text += "\n"+"Comparing Files ...";

                //start the process of comparing the files
              
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                //************* Diff.Net*******************************************
                //process.StartInfo.FileName = @"C:\APM Move Utility\Diff.Net\Diff.Net.exe";
                //process.StartInfo.Arguments = "\"" + sFromFullPathLocal + "\"" + " " + "\"" + sToFullPathLocal + "\"";
                //************* Diff.Net *******************************************

                //**************** SGDM.exe ****************************************
                //process.StartInfo.FileName = @"C:\APM Move Utility\SGDM\sgdm.exe";
                //path contains space so need to enclose them, we have 'Allscripts PM' which has space
                // at this pont of time this is read only
                //process.StartInfo.Arguments = "-ro2 "+ "\"" + sFromFullPathLocal + "\"" + " " + "\"" + sToFullPathLocal + "\"";
                // ************** SGDM.exe ******************************************

                // ********************* Visual studio's  vsdiffmerge.exe **************************************
                process.StartInfo.FileName = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\vsdiffmerge.exe";
                process.StartInfo.Arguments = "\"" + sFromFullPathLocal + "\"" + " " + "\"" + sToFullPathLocal + "\"" + " /t";  // /t will make it happened in the tab that is on right side

                // ********************   visuall studio's vsdiffmerge.exe ***********************************
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.WorkingDirectory = @"C:\";
                process.Start();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(string.Format("Error Occured \n Please review Log File"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LogFile.WriteLine(string.Format("{0} Exception :-  {1}", DateTime.Now.ToString(@"MM/dd/yyyy HH:mm:ss.ffff"), ex.Message), objVSTS);
            }
        }

        private string GetBranchFromPath(string path)
        {
            if (path == string.Empty) return "";
            string returnString = string.Empty;
            var splitArray = path.Split('/');
            if (splitArray[2].Equals("Main", StringComparison.CurrentCultureIgnoreCase))
                returnString = splitArray[2].ToString();

            if (splitArray[2].Equals("Development", StringComparison.CurrentCultureIgnoreCase) || splitArray[2].Equals("Release", StringComparison.CurrentCultureIgnoreCase))
                returnString = splitArray[2].ToString() + "/" + splitArray[3].ToString();

                return returnString;
        }

        private void cmbDestinationFolders_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                MakeTOFullPath();
                                
            }
            catch (Exception ex)
            {

                LogFile.WriteLine(string.Format("{0} Exception :-  {1}", DateTime.Now.ToString(@"MM/dd/yyyy HH:mm:ss.ffff"), ex.Message), objVSTS);
            }
        }
        /// <summary>
        /// This method will make the to path. when user selects the from using Browse button
        /// </summary>
        private void MakeTOFullPath()
        {
            try
            {
                //for the very first time this will be empty.
                if (cmbDestinationFolders.SelectedValue == null) return;

                string sToBranch = string.Empty;

                string stempBranch = string.Empty;
                if ((cmbDestinationFolders.SelectedItem != null) && (!cmbDestinationFolders.SelectedItem.ToString().Equals(string.Empty)))
                    sToBranch = cmbDestinationFolders.SelectedValue.ToString();

                var splitArray = sFromFullPath.Split('/');
                if (splitArray[2].Equals("Main", StringComparison.CurrentCultureIgnoreCase))
                {
                    for (int i = 3; i < splitArray.Length; i++)
                    {
                        stempBranch = stempBranch + '/' + splitArray[i];
                    }
                }
                else if (splitArray[2].Equals("Development", StringComparison.CurrentCultureIgnoreCase) || splitArray[2].Equals("Release", StringComparison.CurrentCultureIgnoreCase))
                {
                    for (int i = 4; i < splitArray.Length; i++)
                    {
                        stempBranch = stempBranch + '/' + splitArray[i];
                    }
                }

                sToFullPath = sToBranch + stempBranch;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        private void BtnMove_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult;// = null;
            try
            {
                Workspace workspace = null;
                //user has selected nothing or user has selected same branch
                if (GetBranchFromPath(sFromFullPath) == GetBranchFromPath(sToFullPath) || sFromFullPath == string.Empty || sToFullPath == string.Empty)
                {
                    System.Windows.MessageBox.Show(string.Format("This wont work! \n Moving code from {0} to {1} ", sFromFullPath == string.Empty ? "nothing" : GetBranchFromPath(sFromFullPath),
                        sToFullPath == string.Empty ? "nothing" : GetBranchFromPath(sToFullPath)),
                         "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //start the movement, 
                txtLogTextBox.Text += "\n" + "Starting Movement of Code ...";

                //this will help us that we need to move everything as the source file is new
                var isToFileExist = versionControlServer.ServerItemExists(sToFullPath, ItemType.File);
                if (!isToFileExist)
                {
                    System.Windows.MessageBox.Show(string.Format("File do not exist in 'TO' folder \n It is a newly added file in 'FROM' Branch \n New File will be created in 'TO' folder and checked-in."), "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    messageBoxResult = MessageBoxResult.Yes;

                }
                else
                {
                    // we need to ask user if (s)he wants to move all the code changes or selected code changes. Take the input
                    messageBoxResult = CustomMessageBox.ShowYesNo("Do you want to change all lines or Selected lines?", "Everything", "Selected");
                }

                //Yes measn user has said move everything from FROM to TO
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    string fileContent = string.Empty;
                    using (StreamReader sr = new StreamReader(sFromFullPathLocal))
                    {
                        fileContent = sr.ReadToEnd();
                    }
                    using (StreamWriter sw = new StreamWriter(sToFullPathLocal))
                    {
                        sw.WriteLine(fileContent);
                    }

                    //create workspace and then map them
                    workspace = versionControlServer.CreateWorkspace("TFSWORKSPACE");
                    workspace.Map(sToFullPath, sToFullPathLocal);
                    workspace.Get();
                }
                //No mens user said selected part only
                else if (messageBoxResult == MessageBoxResult.No)
                {
                    //create workspace and then map them
                    workspace = versionControlServer.CreateWorkspace("TFSWORKSPACE");
                    workspace.Map(sToFullPath, sToFullPathLocal);
                    workspace.Get();

                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\vsdiffmerge.exe";
                    process.StartInfo.Arguments = "\"" + sToFullPathLocal + "\"" + " " + "\"" + sFromFullPathLocal + "\"" + " " + "\"" + sToFullPathLocal + "\"" + " " + "\"" + sToFullPathLocal + "\"" + " /m";  // /m means merge it
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.WorkingDirectory = @"C:\";
                    process.Start();
                }

               

               
                

                //// Now we have both file so read it from FROM and then put the contenet of that to TO file

               
                //TODO: we need to make sure that file is new or old. as of now we believe that file is old and some changes are there only.


                //workspace.SetLock(sToFullPath, LockLevel.CheckOut); //checkout  // this is to check if the file is new  and need to be added in source control
                //WorkItem workItem = tfsTeamProjectCollection.GetService<WorkItemStore>().GetWorkItem(Convert.ToInt32(txtWorkItem.Text));
                //var workItemCheckInInfo = new[] { new WorkItemCheckinInfo(workItem,WorkItemCheckinAction.Associate)};
                workspace.PendEdit(sToFullPathLocal);
                WorkItemStore workItemStore = tfsTeamProjectCollection.GetService<WorkItemStore>();
                WorkItem workItem = workItemStore.GetWorkItem(Convert.ToInt32(txtWorkItem.Text));
                //WorkItem workItem = tfsTeamProjectCollection.GetService<WorkItemStore>().GetWorkItem(Convert.ToInt32(txtWorkItem.Text));
                var workItemCheckInInfo = new[] { new WorkItemCheckinInfo(workItem, WorkItemCheckinAction.Associate) };
                PendingChange[] pendingChange = workspace.GetPendingChanges();
                var changesetNumber = workspace.CheckIn(pendingChange, txtCheckInComment.Text.Length>0 ? txtCheckInComment.Text : Environment.UserName+" "+"Checked in using Move Utility",null,workItemCheckInInfo,null);
                //var changesetNumber = workspace.CheckIn(pendingChange, txtCheckInComment.Text.Length > 0 ? txtCheckInComment.Text : Environment.UserName + " " + "Checked in using Move Utility");
                txtLogTextBox.Text += "\n" + "Changeset Number returned by TFS : "+changesetNumber.ToString();
                txtLogTextBox.Text += "\n" + "Changeset Number : " + changesetNumber.ToString() + " is associated with Work Item # " + txtWorkItem.Text;
               
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(string.Format("Error Occured \n Please review Log File"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LogFile.WriteLine(string.Format("{0} Exception :-  {1}", DateTime.Now.ToString(@"MM/dd/yyyy HH:mm:ss.ffff"), ex.Message), objVSTS);
            }
            finally
            {
                

            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //delete the workspace

            Workspace localWorkspace = null; 
            try
            {
               localWorkspace= versionControlServer.GetWorkspace("TFSWORKSPACE", Environment.UserName);
            }
            catch(Exception ex)
            {

            }

            if(localWorkspace!=null)
            versionControlServer.DeleteWorkspace("TFSWORKSPACE", Environment.UserName);
            //delete the folders and files those are created on the local machine

            //TODO if time permits then delete the folder from root level
            // as of now keeping it like this would like to invest on core functionality
            if (!sFromFullPathLocal.Equals(string.Empty))
            {
                if (Directory.Exists(sFromFullPathLocal.Substring(0, sFromFullPathLocal.LastIndexOf('\\'))))
                {
                    Directory.Delete(sFromFullPathLocal.Substring(0, sFromFullPathLocal.LastIndexOf('\\')), true);
                } 
            }

            if (!sToFullPathLocal.Equals(string.Empty))
            {
                if (Directory.Exists(sToFullPathLocal.Substring(0, sToFullPathLocal.LastIndexOf('\\'))))
                {
                    Directory.Delete(sToFullPathLocal.Substring(0, sToFullPathLocal.LastIndexOf('\\')), true);
                } 
            }

        }
    }
}

#region Comment Region
//visual stuido movement**************************
//cut from move item

////TODO-as of now making the changes assuming user has selected "Selected changes"
//System.Diagnostics.Process process = new System.Diagnostics.Process();
//// ****************** sgdm.exe *************************************
////process.StartInfo.FileName = @"C:\APM Move Utility\SGDM\sgdm.exe";                
////process.StartInfo.Arguments = "\"" + sFromFullPathLocal + "\"" + " " + "\"" + sToFullPathLocal + "\"";
//// ****************** sgdm.exe **************************************

////*********************** visual stuido merger **********************
//process.StartInfo.FileName = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\vsdiffmerge.exe";
//                process.StartInfo.Arguments = "\"" + sToFullPathLocal + "\""+ " " + "\"" + sFromFullPathLocal + "\""+ " " + "\"" + sToFullPathLocal + "\""+ " " + "\"" + sToFullPathLocal + "\""+ " /m";  // /m means merge it
//                //********************* visual stuido merger ***********************
//                process.StartInfo.UseShellExecute = true;
//                process.StartInfo.WorkingDirectory = @"C:\";
//                process.Start();


    //****************************************************
#endregion
