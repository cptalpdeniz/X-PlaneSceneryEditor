namespace XPlane_Scenery_Editor
{
    public class SceneryArea
    {
        //improvements here suchs as library required, which one etc.
        internal int index { get; set; }
        internal bool enabled { get; set; }
        internal string name { get; set; }
        internal string path { get; set; }
        internal bool hasError { get; set; }

        public SceneryArea(int index, bool enabled, string name, string path, bool hasError)
        {
            this.index = index;
            this.enabled = enabled;
            this.name = name;
            this.path = path;
            this.hasError = hasError;
        }
    }
    
}
