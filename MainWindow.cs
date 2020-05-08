using System;
using System.Net;
using System.Windows.Forms;
using XPlane_Scenery_Editor.Properties;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace XPlane_Scenery_Editor
{
    public partial class Forms : Form
    {
        //You can change a line from SCENERY_PACK to SCENERY_PACK_DISABLED to disable the loading of a pack entirely.
        private string fileName;
        private OpenFileDialog openFileDialog;
        private FolderBrowserDialog folderBrowserDialog;
        private List<SceneryArea> sceneryList;
        private bool fileOpened;
        private bool configFileChanged;
        private bool hasError = false;

        private bool checkForLocation()
        {
            if (File.Exists(Settings.Default.sceneryINILocation))
            {
                fileName = Settings.Default.sceneryINILocation;
                return true;
            }
            return false;
        }

        public static bool checkInternetConnection()
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

        //run dsf tool to convert to txt.
        //read all lines that start with OBJECT_DEF and ! (lib && objects)
        //take the first folder of the path (misterX/...) only the misterX and add it to the array
        //array of libraries required
        private List<string> readLibraryRequirements(string sceneryPath)
        {
            string DSF_EXE_LOC = AppDomain.CurrentDomain.BaseDirectory + @"DSFTool.exe";
            List<string> libraryList = new List<String>();
            try
            {
                foreach (var file in Directory.EnumerateFiles(Settings.Default.xpLocation + sceneryPath + @"earth nav data", "*", SearchOption.AllDirectories)
                                    .Where(s => s.EndsWith(".dsf", StringComparison.OrdinalIgnoreCase)))
                {
                    //ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", @"\c" + DSF_EXE_LOC + "--dsf2text " + file + " " + file + ".txt")
                    ProcessStartInfo startInfo = new ProcessStartInfo(DSF_EXE_LOC, "--dsf2text " + file + " " + file + ".txt")
                    {
                        CreateNoWindow = false,
                        UseShellExecute = false,
                        WindowStyle = ProcessWindowStyle.Hidden,
                    };

                    using (Process dsfTool = Process.Start(startInfo))
                    {
                        dsfTool.WaitForExit();
                    }
                    string[] libTXT = File.ReadAllLines(file + ".txt");
                    foreach (string line in libTXT)
                    {
                        if (line.Contains("OBJECT_DEF ") && !(line.ToLower().Contains("lib") || line.ToLower().Contains("objects")))
                        {
                            string s = line.Split(' ', '/')[1];
                            libraryList.Add(Regex.Replace(s, @"(^\w)|(\s\w)", m => m.Value.ToUpper()));
                        }
                    }
            }
            }
            catch (Exception e)
            {
                LogController.Write(e.ToString());
            }
            
            return libraryList;
        }

        //after List<SceneryArea> is initialized, need to draw the listView
        private void initSceneryList()
        {
            File.SetAttributes(Settings.Default.sceneryINILocation, FileAttributes.Normal);
            sceneryList = new List<SceneryArea>();

            string[] t_fileArray = File.ReadAllLines(fileName);

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
            for (int i = 0; i < t_fileArray.Length; i++)
            {
                if (t_fileArray[i].Contains("SCENERY_PACK") || t_fileArray[i].Contains("SCENERY_PACK_DISABLED"))
                    tempArr = t_fileArray[i].Split(new[] { ' ' }, 2);
                else
                {
                    MessageBox.Show("An error occured, please regenerate your scenery config file by launching your simulator", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                var isEnabled = (tempArr[0] == "SCENERY_PACK") ? true : false;
                string name = tempArr[1].Substring(tempArr[1].IndexOf('\\') + 1);
                string path = tempArr[1].Replace('/', '\\');
                sceneryList.Add(new SceneryArea(i, isEnabled, name, path, false, readLibraryRequirements(path))); //tempArr[1] represents the path
            }
        }

        private void saveSceneryFile()
        {
            string file = "I\n1000 Version\nSCENERY\n";
            if (! hasError )
            {
                foreach (SceneryArea scenery in sceneryList)
                {
                    file += "\n" + (scenery.enabled ? "SCENERY_PACK" : "SCENERY_PACK_DISABLED") + scenery.path;
                }
            }
            else
            {
                MessageBox.Show("Please correct errors before saving.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            File.WriteAllText(Settings.Default.sceneryINILocation, file);
        }

        public Forms()
        {
            InitializeComponent();
            menuStrip1.RenderMode = ToolStripRenderMode.System;
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form_KeyDown);

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
            this.configFileChanged = false;

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
                initSceneryList();
            }
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)       // Ctrl-S Save
            {
                
                e.SuppressKeyPress = true;  // Stops other controls on the form receiving event.
            }
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
            saveSceneryFile();
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string currentVersion = "0.0.0.0";
            var error = false;
            if (checkInternetConnection())
            {
                var client = new WebClient();
                string currentVersionURL = "https://raw.githubusercontent.com/cptalpdeniz/X-PlaneSceneryEditor/master/version.md";
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
            MessageBox.Show("This feature is not implemented yet.", "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void openSceneryPacksFIleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
                try
                {
                    initSceneryList();
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

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (configFileChanged)
            {
                DialogResult result = MessageBox.Show("Would you like to save your current file before reading the config file again?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    initSceneryList();
                }
            }
            else
            {
                MessageBox.Show("Please select config file first.", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void mainListView_Layout(object sender, LayoutEventArgs e)
        {
            foreach (var scenery in sceneryList)
            {
                //var row = new string[] { scenery.index.ToString(), };
            }
        }
    }
}
