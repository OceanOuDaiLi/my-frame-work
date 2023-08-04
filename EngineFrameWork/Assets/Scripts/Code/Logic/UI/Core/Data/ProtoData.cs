using System;
using System.Collections.Generic;

public class ProtoData : ProtoConst
{
    public static Type GetProtoClass(short nKey)
    {
        if (m_dicS2CProto.ContainsKey(nKey))
        {
            return m_dicS2CProto[nKey];
        }

        CDebug.LogError($"[Class:ProtoConst] => 协议Id不存在： {nKey}");

        return null;
    }

    public static short GetC2SMsgID(Type tClass)
    {
        if (m_dicC2SProto.ContainsKey(tClass.FullName) == true)
        {
            return m_dicC2SProto[tClass.FullName];
        }

        CDebug.LogError($"[Class:ProtoConst] => 协议未生成： {tClass.FullName}");

        return 0;
    }

    public static Dictionary<short, Type> GetS2CProtoDic()
    {
        return m_dicS2CProto;
    }

}
