using System.Collections.Generic;

namespace Model
{
    public class ServerInfo : ModelData
    {
        public int lmt { get; set; }

        public int port { get; set; }

        public string ip { get; set; }

        public int current { get; set; }

        public ServerInfo() { }

        public ServerInfo(Dictionary<string, object> dict) : base(dict) { }
    }
}