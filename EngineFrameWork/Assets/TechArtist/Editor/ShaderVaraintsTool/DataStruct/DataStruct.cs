using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class MulitiDict<Key, Value>
{
    [SerializeField]
    private List<Key> keys;
    [SerializeField]
    private List<Value> values;
    [SerializeField]
    private BitMap[] _map;

    public int Count { get { return keys.Count; } }
    [SerializeField]
    private List<Value> valuetemp;

    public MulitiDict(int length)
    {
        keys = new List<Key>(length);
        values = new List<Value>();
        _map = new BitMap[length];
        for (int i = 0; i < length; ++i)
            _map[i] = new BitMap(1);
        valuetemp = new List<Value>();
    }

    public List<Value> this[Key key]
    {
        get
        {
            valuetemp.Clear();
            int length = keys.Count;
            List<int> indexs;
            for (int i = 0; i < length; ++i)
            {
                if (key.Equals(keys[i]))
                {
                    indexs = _map[i].GetAllIndex();
                    int length2 = indexs.Count;
                    for (int j = 0; j < length2; ++j)
                    {
                        valuetemp.Add(values[indexs[j]]);
                    }
                    break;
                }
            }
            return valuetemp;
        }
    }

    //public Value[] this[int n]
    //{
    //    get
    //    {
    //        List<Value> valuetemp = new List<Value>();
    //        uint[] map = _map[n].Get();
    //        for (int i = 0; i < map.Length << 5; ++i)
    //        {
    //            if ((map[i >> 5] & 1 << (i & 0x1f)) != 0)
    //                valuetemp.Add(values[i]);

    //        }
    //        return valuetemp.ToArray();
    //    }
    //}

    public List<Value> GetAllValue()
    {
        return values;
    }

    public List<Key> GetAllKey()
    {
        return keys;
    }

    public void Add(Key key, Value value)
    {
        int origin = keys.IndexOf(key);
        if (origin < 0)
        {
            keys.Add(key);
            origin = keys.Count - 1;
        }
        int aim = values.IndexOf(value);
        if (aim < 0)
        {
            values.Add(value);
            aim = values.Count - 1;
        }

        SetMap(origin, aim, true);
    }

    public bool ContainsKey(Key key)
    {
        return keys.Contains(key);
    }

    public bool Contains(Key key, Value value)
    {
        if (keys.Contains(key))
        {
            List<Value> values = this[key];
            int length = values.Count;
            for (int i = 0; i < length; ++i)
                if (values.Equals(value))
                    return true;
        }
        return false;
    }

    public void Clear()
    {
        values.Clear();
        keys.Clear();
        int length = _map.Length;
        for (int i = 0; i < length; ++i)
            _map[i].Clear();
    }

    public void CloneFrom(MulitiDict<Key, Value> dict)
    {
        int length = _map.Length;
        for (int i = 0; i < length; ++i)
        {
            _map[i].CloneFrom(dict._map[i]);
        }
        keys.Clear();
        keys.AddRange(dict.keys);
        values.Clear();
        values.AddRange(dict.values);
    }

    public void Swap(int i, int j)
    {
        Key keytemp = keys[i];
        keys[i] = keys[j];
        keys[j] = keytemp;

        BitMap temp = new BitMap();
        temp.CloneFrom(_map[i]);
        _map[i].CloneFrom(_map[j]);
        _map[j].CloneFrom(temp);
    }

    public void ChangeValue(Key key, Value oldValue, Value newValue)
    {
        if (oldValue.Equals(newValue))
            return;

        int nKey = FindKeyIndex(key);
        int nOldValue = FindValueIndex(oldValue);

        bool flag = false;      //旧value是否存在其他引用

        for (int i = 0; i < Count; ++i)
        {
            if (nKey == i)
                continue;

            if (_map[i].HasIndex(nOldValue))
            {
                flag = true;
                break;
            }
        }

        if (flag)
        {
            SetMap(nKey, nOldValue, false);
            Add(key, newValue);
        }
        else
        {
            RemoveValueAt(nOldValue);
            Add(key, newValue);
        }
    }

    public Key GetKeyAtIndex(int index)
    {
        return keys[index];
    }

    private void SetMap(int origin, int aim, bool bit)
    {
        if (origin >= _map.Length)
        {
            BitMap[] newMap = new BitMap[_map.Length * 2];
            int length = _map.Length;
            for (int i = 0; i < length; ++i)
                newMap[i] = _map[i];
            for (int i = length; i < newMap.Length; ++i)
                newMap[i] = new BitMap(1);
            _map = null;
            _map = newMap;
        }
        _map[origin].Set(aim, bit);
    }

    private int FindKeyIndex(Key key)
    {
        int length = keys.Count;
        for (int i = 0; i < length; ++i)
        {
            if (keys[i].Equals(key))
                return i;
        }
        return -1;
    }

    private int FindValueIndex(Value value)
    {
        int length = values.Count;
        for (int i = 0; i < length; ++i)
        {
            if (values[i].Equals(value))
                return i;
        }

        return -1;
    }

    private void RemoveValueAt(int n)
    {
        values.RemoveAt(n);
        int length = keys.Count;
        for (int i = 0; i < length; ++i)
        {
            _map[i].RemoveAt(n);

            //_map[i].Check();
        }
    }
}

    [Serializable]
#if UNITY_EDITOR
    public
#else
internal
#endif
struct Set<T>
    {
        public T[] Data
        {
            get
            {
                T[] temp = new T[_count];
                for (int i = 0; i < _count; ++i)
                    temp[i] = _date[i];
                return temp;
            }
        }
        [SerializeField]
        private T[] _date;
        public int Count
        {
            get
            {
                return _count;
            }
        }
        [SerializeField]
        private int _count;

        public Set(int length)
        {
            _date = new T[length];
            _count = 0;
        }

        public int Add(T date)
        {
            int length = _date.Length;
            for (int i = 0; i < _count; ++i)
                if (_date[i].Equals(date))
                    return i;

            if (length <= _count)
            {
                T[] temp = new T[_date.Length * 2];
                for (int i = 0; i < _count; ++i)
                    temp[i] = _date[i];
                _date = temp;
            }
            _date[_count] = date;
            ++_count;

            return _count - 1;
        }

        public void ExChange(int i, int j)
        {
            T temp = _date[i];
            _date[i] = _date[j];
            _date[j] = temp;
        }

        public T this[int n]
        {
            get
            {
                if (n >= _count)
                    return default(T);
                return _date[n];
            }

            set
            {
                if (n < _count)
                    _date[n] = value;
            }
        }

        public T[] ToArray()
        {
            return _date;
        }

        public void Clear()
        {
            _date = null;
            _date = new T[64];
            _count = 0;
        }

        public void CloneFrom(Set<T> set)
        {
            Clear();
            for (int i = 0; i < _count; ++i)
            {
                Add(set._date[i]);
            }
        }

        public bool Contains(T value)
        {
            for (int i = 0; i < _count; ++i)
                if (_date[i].Equals(value))
                    return true;
            return false;
        }

        public void RemoveAt(int n)
        {
            if (n > _count - 1)
                return;

            for (int i = n; i < _count - 1; ++i)
                _date[i] = _date[i + 1];

            --_count;
        }

        public int IndexOf(T value)
        {
            for (int i = 0; i < _count; ++i)
                if (_date[i].Equals(value))
                    return i;
            return -1;
        }
    }

/// <summary>
/// 字典映射类
/// </summary>
[Serializable]
public struct BitMap
{
    //每个元素高8位为索引位，低24位为标记位，理论上最多能存6144个映射，够用了
    [SerializeField]
    private UInt32[] _bit;
    [SerializeField]
    private int _lenth;

    public BitMap(int length)
    {
        _lenth = length;
        _bit = new UInt32[length];
    }

    /// <summary>
    /// 添加映射
    /// </summary>
    /// <param name="n"></param>
    /// <param name="bit"></param>
    public void Set(int n, bool bit)
    {
        int m = FindIndex(n);

        //未查找到索引则创建
        if (m == -1)
        {
            int newLenth = 1 + _lenth;
            UInt32[] temp = new UInt32[newLenth];
            for (int i = 0; i < _lenth; ++i)
                temp[i] = _bit[i];
            _lenth = newLenth;
            _bit = temp;
            m = _lenth - 1;

            //高8位记录索引
            _bit[m] = (UInt32)(n / 24) << 24;
        }

        UInt32 a = 1;
        if (bit)
            _bit[m] |= a << (n % 24);
        else
        {
            _bit[m] &= ~(a << (n % 24));
            Check();
        }
    }

    /// <summary>
    /// 查询是否存在该元素
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public bool HasIndex(int n)
    {
        UInt32 m = (UInt32)(n / 24);

        UInt32 index;
        UInt32 a = 0x1;
        for (int i = 0; i < _lenth; ++i)
        {
            index = _bit[i] >> 24;
            if (index == m)
            {
                if ((_bit[i] & (a << (n % 24))) != 0)
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 获得所有元素索引
    /// </summary>
    /// <returns></returns>
    public List<int> GetAllIndex()
    {
        List<int> tempList = new List<int>(128);
        tempList.Clear();
        UInt32 a = 0x1;
        int length = _bit.Length;
        for (int i = 0; i < length; ++i)
        {
            int n = (int)(_bit[i] >> 24);
            for (int j = 0; j < 24; ++j)
            {
                if ((_bit[i] & (a << j)) != 0)
                {
                    tempList.Add(24 * n + j);
                }
            }
        }
        return tempList;
    }

    public void CloneFrom(BitMap bitmap)
    {
        UInt32[] bits = bitmap._bit;
        _lenth = bits.Length;
        _bit = null;
        _bit = new UInt32[_lenth];
        UInt32 a = 0xffffffff;
        for (int i = 0; i < _lenth; ++i)
        {
            _bit[i] = a & bits[i];
        }
    }

    /// <summary>
    /// 移除映射
    /// </summary>
    /// <param name="n"></param>
    public void RemoveAt(int n)
    {
        int m = FindIndex(n);
        int k = n % 24;
        int a = -1;
        UInt32 b = 0x01;
        if (m != -1)
        {
            UInt32 low = _bit[m] & (~(0xffffffff << k));
            UInt32 high = _bit[m] & 0xff000000;
            _bit[m] = low | high | (((_bit[m] & ~(b << k)) - low - high) >> 1);
        }

        //索引高于移除元素索引的向前挪一位
        for (int i = 0; i < _lenth; ++i)
        {
            a = (int)(_bit[i] >> 24);
            if (a > n / 24)
            {
                for (int j = 0; j < 24; ++j)
                {
                    if ((_bit[i] & (b << j)) != 0)
                    {
                        _bit[i] &= ~(b << j);
                        int c = a * 24 + j - 1;
                        if (c < 0)
                            continue;
                        Set(c, true);
                    }
                }
            }
        }
        Check();
    }

    public void Clear()
    {
        int length = _bit.Length;
        for (int i = 0; i < length; ++i)
            _bit[i] = 0;
    }

    private int FindIndex(int n)
    {
        UInt32 m = (UInt32)(n / 24);
        UInt32 index;
        for (int i = 0; i < _lenth; ++i)
        {
            index = _bit[i] >> 24;
            if (index == m)
                return i;
        }

        return -1;
    }

    /// <summary>
    /// 检查 移除空索引数组元素
    /// </summary>
    public void Check()
    {
        for (int i = 0; i < _lenth; ++i)
        {
            if ((_bit[i] << 8) == 0)
            {
                UInt32[] temp = new UInt32[_lenth - 1];
                for (int j = i + 1; j < _lenth; ++j)
                    _bit[j - 1] = _bit[j];
                for (int j = 0; j < _lenth - 1; ++j)
                    temp[j] = _bit[j];
                _bit = temp;
                --_lenth;
                --i;
            }
        }
    }
}
