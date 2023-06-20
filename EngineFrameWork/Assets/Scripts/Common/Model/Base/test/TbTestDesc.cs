//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;
using SimpleJSON;



namespace cfg.test
{ 

public sealed partial class TbTestDesc
{
    private readonly List<test.TestDesc> _dataList;
    
    private Dictionary<int, test.TestDesc> _dataMap_id;
    private Dictionary<string, test.TestDesc> _dataMap_name;

    public TbTestDesc(JSONNode _json)
    {
        _dataList = new List<test.TestDesc>();
        
        foreach(JSONNode _row in _json.Children)
        {
            var _v = test.TestDesc.DeserializeTestDesc(_row);
            _dataList.Add(_v);
        }
        _dataMap_id = new Dictionary<int, test.TestDesc>();
        _dataMap_name = new Dictionary<string, test.TestDesc>();
    foreach(var _v in _dataList)
    {
        _dataMap_id.Add(_v.Id, _v);
        _dataMap_name.Add(_v.Name, _v);
    }
        PostInit();
    }

    public List<test.TestDesc> DataList => _dataList;

    public test.TestDesc GetById(int key) => _dataMap_id.TryGetValue(key, out test.TestDesc __v) ? __v : null;
    public test.TestDesc GetByName(string key) => _dataMap_name.TryGetValue(key, out test.TestDesc __v) ? __v : null;

    public void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var v in _dataList)
        {
            v.Resolve(_tables);
        }
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var v in _dataList)
        {
            v.TranslateText(translator);
        }
    }

    
    partial void PostInit();
    partial void PostResolve();
}

}