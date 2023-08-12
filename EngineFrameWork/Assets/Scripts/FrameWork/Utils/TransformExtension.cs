using UnityEngine;

public static class TransformExtension
{

    /// <summary>
    /// 是否是我的祖先物体
    /// </summary>
    /// <param name="self"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool IsMyAncestor(this Transform self,Transform other)
    {
        if (self == null || other == null) return false;

        var ptr = self.parent;
        while (ptr != null) 
        {
            if(ptr.Equals(other))
                return true;
            ptr = ptr.parent;
        }
        return false;
    }

    /// <summary>
    /// 是否是我的子物体
    /// </summary>
    /// <param name="self"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool IsMyGrandson(this Transform self, Transform other)
    {
        return IsMyGrandson_Internal(self, other);
    }

    private static bool IsMyGrandson_Internal(Transform self, Transform other)
    {
        if (self == null || other == null) return false;

        int childCount = self.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            var trans = self.GetChild(i);
            if (trans.Equals(other))
                return true;
            if (IsMyGrandson_Internal(trans, other))
                return true;
        }
        return false;
    }
}
