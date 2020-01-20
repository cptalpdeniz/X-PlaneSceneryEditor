using System;
using System.Windows.Forms;
using XPlane_Scenery_Editor.Properties;

namespace XPlane_Scenery_Editor
{
    public partial class Forms : Form
    {

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (Settings.Default.WindowLocation != null)
            {
                this.Location = Settings.Default.WindowLocation;
            }

            if (Settings.Default.WindowSize != null)
            {
                this.Size = Settings.Default.WindowSize;
            }
        }
    }
}