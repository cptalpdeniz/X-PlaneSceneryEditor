using System.Windows.Forms;

namespace XPlane_Scenery_Editor
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            label1.Text = "Scenery Library Editor for X-Plane 11 \nVersion: " + Application.ProductVersion.ToString();
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        }
    }
}
