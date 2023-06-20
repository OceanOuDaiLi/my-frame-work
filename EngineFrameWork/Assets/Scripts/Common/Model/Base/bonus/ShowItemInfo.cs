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



namespace cfg.bonus
{ 

public sealed partial class ShowItemInfo :  Bright.Config.BeanBase 
{
    public ShowItemInfo(JSONNode _json) 
    {
        { if(!_json["item_id"].IsNumber) { throw new SerializationException(); }  ItemId = _json["item_id"]; }
        { if(!_json["item_num"].IsNumber) { throw new SerializationException(); }  ItemNum = _json["item_num"]; }
        PostInit();
    }

    public ShowItemInfo(int item_id, long item_num ) 
    {
        this.ItemId = item_id;
        this.ItemNum = item_num;
        PostInit();
    }

    public static ShowItemInfo DeserializeShowItemInfo(JSONNode _json)
    {
        return new bonus.ShowItemInfo(_json);
    }

    public int ItemId { get; private set; }
    public item.Item ItemId_Ref { get; private set; }
    public long ItemNum { get; private set; }

    public const int __ID__ = -1496363507;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        this.ItemId_Ref = (_tables["item.TbItem"] as item.TbItem).GetOrDefault(ItemId);
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "ItemId:" + ItemId + ","
        + "ItemNum:" + ItemNum + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
