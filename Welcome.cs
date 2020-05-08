using System;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace XPlane_Scenery_Editor
{
    public partial class Welcome : Form
    {
        FontFamily ff;
        Font font;
        private delegate void CloseDelegate();
        private static Welcome welcomeSplash;
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);
        private void InitCustomLabelFont()
        {
            byte[] fontArr = Properties.Resources.PeaceSans;
            int length = Properties.Resources.PeaceSans.Length;

            IntPtr intPtr = Marshal.AllocCoTaskMem(length);

            Marshal.Copy(fontArr, 0, intPtr, length);
            uint cFonts = 0;
            AddFontMemResourceEx(intPtr, (uint)fontArr.Length, IntPtr.Zero, ref cFonts);

            PrivateFontCollection pfc = new PrivateFontCollection();
            pfc.AddMemoryFont(intPtr, length);

            Marshal.FreeCoTaskMem(intPtr);

            ff = pfc.Families[0];
            font = new Font(ff, 15f, FontStyle.Regular);
        }

        private void AllocateFont(Font f, Control c, float size)
        {
            FontStyle fontStyle = FontStyle.Regular;
            c.Font = new Font(ff, size, fontStyle);
        }

        private void Welcome_Load(object sender, EventArgs e)
        {
            InitCustomLabelFont();
            AllocateFont(font, this.label1, 30);
        }

        static public void ShowFirstRunBox()
        {
            MessageBox.Show("Welcome to X-Plane 11 Scenery Config Editor. Since this is your first time running the program, the program will try to find the location of scenery_packs.ini file's location. If it fails, it will ask you to select it manually", "Welcome to X-Plane 11 Scenery Config Editor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            var xplaneLocationFile = Path.Combine(Path.GetDirectoryName(Environment.GetEnvironmentVariable("LocalAppData")), "Local");
            xplaneLocationFile = Path.Combine(xplaneLocationFile, "x-plane_install_11.txt");
            if (File.Exists(xplaneLocationFile))
            {
                var xplaneLocation = File.ReadAllText(xplaneLocationFile);
                xplaneLocation = Regex.Replace(xplaneLocation, @"\t|\n|\r", "").Replace('/', '\\');
                var sceneryPacksFile = Path.Combine(xplaneLocation, @"Custom Scenery\scenery_packs.ini");
                if (File.Exists(sceneryPacksFile))
                {
                    Properties.Settings.Default.sceneryINILocation = sceneryPacksFile;
                    Properties.Settings.Default.xpLocation = xplaneLocation;
                    MessageBox.Show("scenery_packs.ini file has been found.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                else
                {
                    MessageBox.Show("There seems to be no scenery_packs.ini file in Custom Scenery folder. XPSCE will try to start X-Plane automatically so that the scenery_packs.ini can be generated.\n\nPlease start XPSCE when the scenery_packs.ini file is generated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    var PROCESS = new Process();
                    var xpEXE = Path.Combine(xplaneLocation, "X-Plane.exe");
                    PROCESS.StartInfo.FileName = xpEXE;
                    PROCESS.Start();
                    Thread.Sleep(2000);
                    Program.closefromInternal = true;
                }
            }
        }

        public Welcome()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.ControlBox = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            picturePanel1.Anchor = AnchorStyles.None;
            picturePanel1.Location = new Point((picturePanel1.Parent.ClientSize.Width / 2) - (picturePanel1.Width / 2), 40);
            picturePanel1.Refresh();

            InitCustomLabelFont();
            label1.Anchor = AnchorStyles.None;
            label1.Location = new Point(0, 420);
            label1.Size = new Size(label1.Parent.ClientSize.Width, 60);
            label1.Text = "X-Plane 11 Scenery Config Editor";
            label1.ForeColor = Color.FromArgb(27, 162, 220);
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.BackColor = Color.White;
            label1.Refresh();
        }

        static private void showForm()
        {
            welcomeSplash = new Welcome();
            Application.Run(welcomeSplash);
        }

        static public void showSplashScreen()
        {
            // Need to make sure it is only launched once
            if (welcomeSplash != null)
                return;
            Thread thread = new Thread(new ThreadStart(showForm));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        static public void closeForm()
        {
            welcomeSplash.Invoke(new CloseDelegate(Welcome.CloseFormInternal));
        }

        static private void CloseFormInternal()
        {
            welcomeSplash.Close();
            welcomeSplash = null;
        }
    }
}