using System;
using System.Net;
using System.Windows.Forms;
using XPlane_Scenery_Editor.Properties;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace XPlane_Scenery_Editor
{
    public partial class Forms : Form
    {
        //From XPlane website
        //You can change a line from SCENERY_PACK to SCENERY_PACK_DISABLED to disable the loading of a pack entirely.
        private string fileName;
        private OpenFileDialog openFileDialog;
        private FolderBrowserDialog folderBrowserDialog;
        private List<sceneryArea> sceneryList;
        private bool fileOpened;

        private bool checkForLocation()
        {
            if (File.Exists(Settings.Default.sceneryINILocation))
            {
                fileName = Settings.Default.sceneryINILocation;
                return true;
            }
            return false;
        }

        public static bool CheckInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("https://github.com/cptalpdeniz/"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        private void initSceneryList()
        {
            File.SetAttributes(Settings.Default.sceneryINILocation, FileAttributes.Normal);
            if (sceneryList == null)
                sceneryList = new List<sceneryArea>();

            var t_fileArray = File.ReadAllLines(Settings.Default.sceneryINILocation);

            int count = 0;
            for (count = 0; count < t_fileArray.Length; count++)
            {
                if (t_fileArray[count].Contains("SCENERY_PACK"))
                {
                    break;
                }
            }
            t_fileArray = t_fileArray.Skip(count).ToArray();

            string[] tempArr = new string[2];
            for (int i = t_fileArray.Length -1; i > -1; i--)
            {
                if (t_fileArray[i].Contains("SCENERY_PACK") || t_fileArray[i].Contains("SCENERY_PACK_DISABLED"))
                    tempArr = t_fileArray[i].Split(new[] { ' ' }, 2);
                else
                {
                    MessageBox.Show("An error occured, please regenerate your scenery config file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                var isEnabled = (tempArr[0] == "SCENERY_PACK") ? true : false;
                var name = tempArr[1].Substring(tempArr[1].IndexOf('\\') + 1); ;
                sceneryList.Add(new sceneryArea(i, isEnabled, name, tempArr[1], false));
            }
        }

        public Forms()
        {
            InitializeComponent();
            menuStrip1.RenderMode = ToolStripRenderMode.System;

            this.folderBrowserDialog = new FolderBrowserDialog();
            this.openFileDialog = new OpenFileDialog
            {
                DefaultExt = "ini",
                Filter = "ini files (*.ini)|*.ini"
            };

            this.folderBrowserDialog.Description = "Please select the X-Plane 11 Root folder";
            this.folderBrowserDialog.ShowNewFolderButton = false;
            this.folderBrowserDialog.RootFolder = Environment.SpecialFolder.ProgramFiles;

            this.fileOpened = false;

            if (!checkForLocation())
            {
                openFileDialog.InitialDirectory = Settings.Default.xpLocation;
                openFileDialog.FileName = null;
                
                DialogResult resultMessage = DialogResult.Retry;

                while (resultMessage == DialogResult.Retry)
                {
                    DialogResult result = openFileDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        fileName = openFileDialog.FileName;
                        try
                        {
                            Stream s = openFileDialog.OpenFile();
                            Settings.Default.sceneryINILocation = fileName;
                            initSceneryList();
                            fileOpened = true;
                            break;
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show("An error occured while trying to open scenery_packs.ini file. Error is:" + Environment.NewLine + exp.ToString() + Environment.NewLine, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Application.Exit();
                        }
                        Invalidate();
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        resultMessage = MessageBox.Show("You need to select the scenery_packs.ini file otherwise this program cannot work.", "File not selected", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        if (resultMessage == DialogResult.Cancel)
                        {
                            Load += (s, e) => Close();
                            return;
                        }
                        else if (resultMessage == DialogResult.Retry)
                        {
                            continue;
                        }
                    }
                }
            }
            else
            {
                
/*                foreach (var item in sceneryListArray)
                {
                    richTextBox1.AppendText(Environment.NewLine + item);
                }
  */          }

    }


        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string currentVersion = "0.0.0.0";
            var error = false;
            if (CheckInternetConnection())
            {
                var client = new WebClient();
                string currentVersionURL = "https://raw.githubusercontent.com/cptalpdeniz/XPlaneSceneryEditor/master/version.md";
                DialogResult result = DialogResult.Retry;
                while (result == DialogResult.Retry)
                {
                    try
                    {
                        currentVersion = client.DownloadString(currentVersionURL);
                        if (currentVersion.Contains(@"\n"))
                        {
                            currentVersion = currentVersion.Replace("\n", String.Empty);
                        }
                        break;
                    }
                    catch (Exception)
                    {
                        error = true;
                        result = MessageBox.Show("There's a problem with the internet connection. Can't check the most up-to-date version", "Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
                        if (result == DialogResult.Abort && result == DialogResult.Ignore)
                        {
                            break;
                        }
                    }
                }
            }
            Cursor.Current = Cursors.Default;

            var versionApp = new Version(Resources.version);
            var versionCurrent = new Version(currentVersion);
            
            if (!error)
            {
                int result = versionApp.CompareTo(versionCurrent);
                if (result < 0)
                {
                    MessageBox.Show("There's a newer version available. Please head to Github page to download the newer version", "Update Available", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("There are no new updates available", "Up to Date", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("There's a new version available. Please head to Github page to download the newer version", "Update Available", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void openSceneryPacksFIleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!fileOpened)
            {

                DialogResult result = openFileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    fileName = openFileDialog.FileName;
                    try
                    {
                        Stream s = openFileDialog.OpenFile();
                        s.Close();

                        fileOpened = true;
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show("An error occured while trying to open scenery_packs.ini file. Error is:" + Environment.NewLine + exp.ToString() + Environment.NewLine);
                    }
                    Invalidate();

                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }
            else
            {

            }
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
