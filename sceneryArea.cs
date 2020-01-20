namespace XPlane_Scenery_Editor
{
    public class sceneryArea
    {
        //improvements here suchs as library required, which one etc.
        private int index { get; set; }
        private bool enabled { get; set; }
        private string name { get; set; }
        private string path { get; set; }
        private bool hasError { get; set; }

        public sceneryArea(int index, bool enabled, string name, string path, bool hasError)
        {
            this.index = index;
            this.enabled = enabled;
            this.name = name;
            this.path = path;
            this.hasError = hasError;
        }
    }
    
}
