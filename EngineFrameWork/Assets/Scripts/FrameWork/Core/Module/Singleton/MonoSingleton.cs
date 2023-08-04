using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private const string mRootName = "CodeV";
    private static T mIns = null;

    public static T Ins
    {
        get
        {
            if (mIns == null)
            {
                mIns = FindObjectOfType(typeof(T)) as T;
                if (mIns == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    mIns = go.AddComponent<T>();
                    GameObject parent = GameObject.Find(mRootName);
                    if (parent == null)
                    {
                        parent = new GameObject(mRootName);
                        DontDestroyOnLoad(parent);
                    }
                    if (parent != null)
                    {
                        go.transform.SetParent(parent.transform, false);
                    }
                }
            }

            return mIns;
        }
    }

    /*
     * 没有任何实现的函数，用于保证MonoSingleton在使用前已创建
     */
    public void Startup()
    {

    }

    private void Awake()
    {
        if (mIns == null)
        {
            mIns = this as T;
        }

        DontDestroyOnLoad(gameObject);
        Init();
    }

    protected virtual void Init()
    {

    }

    public void DestroySelf()
    {
        Dispose();
        MonoSingleton<T>.mIns = null;
        UnityEngine.Object.Destroy(gameObject);
    }

    public virtual void Dispose()
    {

    }
}