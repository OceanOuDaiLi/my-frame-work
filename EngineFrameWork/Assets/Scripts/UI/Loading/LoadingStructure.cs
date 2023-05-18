
public class LoadingStructure
{
    public string bundleName;
    public string sceneName;                //场景名
    public bool isAsync;                    //异步加载
    public bool sceneActivation;            //预加载
    public float progressRate;              //进度占比
    public bool ignoreRepeat;               //忽略与当前场景重复

    public static LoadingStructure LoadingDirective(string sceneName, bool async, bool activation, float progressRate)
    {
        LoadingStructure s = new LoadingStructure();
        s.bundleName = string.Empty;
        s.sceneName = sceneName;
        s.isAsync = async;
        s.sceneActivation = activation;
        s.progressRate = progressRate;
        return s;
    }

    public static LoadingStructure LoadingBundle(string bundleName, string sceneName, float progressRate, bool ignoreRepeat = false)
    {
        LoadingStructure s = new LoadingStructure();
        s.bundleName = bundleName;
        s.sceneName = sceneName;
        s.isAsync = true;
        s.sceneActivation = true;
        s.progressRate = progressRate;
        s.ignoreRepeat = ignoreRepeat;
        return s;
    }
}
