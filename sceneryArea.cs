using System.Collections.Generic;

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
        internal List<string> lib { get; set; } //instead of using array or IEnumurable<T> I'm using List since we don't know how many libs the scenery requires

        public SceneryArea(int index, bool enabled, string name, string path, bool hasError, List<string> libs)
        {
            this.index = index;
            this.enabled = enabled;
            this.name = name;
            this.path = path;
            this.hasError = hasError;
            this.lib = libs;
        }
    }
    
}
