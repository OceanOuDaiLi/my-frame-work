using System.Collections.Generic;

namespace Model
{
    public class ModelName : ModelData
    {
        public string name { get; set; }

        public ModelName() { }

        public ModelName(Dictionary<string, object> dict) : base(dict)
        {

        }
    }
}
