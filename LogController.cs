using System;
using System.IO;
using System.Windows.Forms;

namespace XPlane_Scenery_Editor
{
    // Inspired from https://stackoverflow.com/a/43541437
    public static class LogController
    {
        private static string path = string.Empty;

        public static void Write(string msg)
        {
            path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            try
            {
                using (StreamWriter w = File.AppendText(Path.Combine(path, "error.log")))
                {
                    Log(msg, w);
                }
            }
            catch (Exception e)
            {
                DialogResult d = MessageBox.Show("Please copy this error and upload your log file to (error.log) to GitHub Issues of this project.\n" + e.ToString(), "Error!", MessageBoxButtons.OK);
                // Why if statement here?
                // I want to execute the below code after the user presses OK
                if (d == DialogResult.OK)
                {
                    //as the code below will start a process (can be used to start executables as well), to make it more secure, I'm making the string const and only accessible in this call stack.
                    const string url = @"https://github.com/cptalpdeniz/X-PlaneSceneryEditor/issues";
                    System.Diagnostics.Process.Start(url);
                }
            }
        }

        private static void Log(string msg, TextWriter w)
        {
            try
            {
                w.Write(Environment.NewLine);
                w.Write("[{0} {1}]", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                w.Write("\t");
                w.WriteLine(" {0}", msg);
                w.WriteLine("-----------------------");
            }
            catch (Exception e)
            {
                DialogResult d = MessageBox.Show("Please copy this error and upload your log file to (error.log) to GitHub Issues of this project.\n" + e.ToString(), "Error!", MessageBoxButtons.OK);
                // Why if statement here?
                // I want to execute the below code after the user presses OK
                if (d == DialogResult.OK)
                {
                    const string url = @"https://github.com/cptalpdeniz/X-PlaneSceneryEditor/issues";
                    System.Diagnostics.Process.Start(url);
                }
            }
        }
    }
}
