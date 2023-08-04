using System.Collections.Generic;

namespace Model
{
    public class ServerZone : ModelData
    {
        public int port { get; set; }

        public string ip { get; set; }

        public int status { get; set; }

        public string name { get; set; }

        public long newDate { get; set; }

        public int hasPlayer { get; set; }

        public ServerZone() { }

        public ServerZone(Dictionary<string, object> dict) : base(dict) { }
    }
}