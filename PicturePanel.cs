using System.Drawing;
using System.Windows.Forms;

namespace XPlane_Scenery_Editor
{
    internal class PicturePanel : Panel
    {
        public PicturePanel()
        {
            this.DoubleBuffered = true;
            this.AutoScroll = true;
            this.BackgroundImageLayout = ImageLayout.Center;
        }
        public override Image BackgroundImage
        {
            get { return base.BackgroundImage; }
            set
            {
                base.BackgroundImage = value;
                if (value != null) this.AutoScrollMinSize = value.Size;
            }
        }
    }
}
