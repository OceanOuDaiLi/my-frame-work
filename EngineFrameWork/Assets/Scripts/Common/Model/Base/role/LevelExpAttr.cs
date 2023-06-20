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



namespace cfg.role
{ 

public sealed partial class LevelExpAttr :  Bright.Config.BeanBase 
{
    public LevelExpAttr(JSONNode _json) 
    {
        { if(!_json["level"].IsNumber) { throw new SerializationException(); }  Level = _json["level"]; }
        { if(!_json["need_exp"].IsNumber) { throw new SerializationException(); }  NeedExp = _json["need_exp"]; }
        { var __json0 = _json["clothes_attrs"]; if(!__json0.IsArray) { throw new SerializationException(); } ClothesAttrs = new System.Collections.Generic.List<int>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  ClothesAttrs.Add(__v0); }   }
        PostInit();
    }

    public LevelExpAttr(int level, long need_exp, System.Collections.Generic.List<int> clothes_attrs ) 
    {
        this.Level = level;
        this.NeedExp = need_exp;
        this.ClothesAttrs = clothes_attrs;
        PostInit();
    }

    public static LevelExpAttr DeserializeLevelExpAttr(JSONNode _json)
    {
        return new role.LevelExpAttr(_json);
    }

    public int Level { get; private set; }
    public long NeedExp { get; private set; }
    public System.Collections.Generic.List<int> ClothesAttrs { get; private set; }

    public const int __ID__ = -1569837022;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "Level:" + Level + ","
        + "NeedExp:" + NeedExp + ","
        + "ClothesAttrs:" + Bright.Common.StringUtil.CollectionToString(ClothesAttrs) + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
