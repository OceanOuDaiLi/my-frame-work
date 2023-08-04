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



namespace cfg
{ 

public sealed partial class ItemReward :  Bright.Config.BeanBase 
{
    public ItemReward(JSONNode _json) 
    {
        { if(!_json["level"].IsNumber) { throw new SerializationException(); }  Level = _json["level"]; }
        { var __json0 = _json["itemgroups"]; if(!__json0.IsArray) { throw new SerializationException(); } Itemgroups = new System.Collections.Generic.List<ItemGroup>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { ItemGroup __v0;  { if(!__e0.IsObject) { throw new SerializationException(); }  __v0 = ItemGroup.DeserializeItemGroup(__e0);  }  Itemgroups.Add(__v0); }   }
        PostInit();
    }

    public ItemReward(int level, System.Collections.Generic.List<ItemGroup> itemgroups ) 
    {
        this.Level = level;
        this.Itemgroups = itemgroups;
        PostInit();
    }

    public static ItemReward DeserializeItemReward(JSONNode _json)
    {
        return new ItemReward(_json);
    }

    public int Level { get; private set; }
    public System.Collections.Generic.List<ItemGroup> Itemgroups { get; private set; }

    public const int __ID__ = -343558078;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var _e in Itemgroups) { _e?.Resolve(_tables); }
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var _e in Itemgroups) { _e?.TranslateText(translator); }
    }

    public override string ToString()
    {
        return "{ "
        + "Level:" + Level + ","
        + "Itemgroups:" + Bright.Common.StringUtil.CollectionToString(Itemgroups) + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
