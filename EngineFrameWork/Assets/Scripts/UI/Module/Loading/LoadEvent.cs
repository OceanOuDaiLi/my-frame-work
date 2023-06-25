namespace UI
{
    public enum LoadEvent : byte
    {
        LOAD_SCENE_BUNDLE,
        LOAD_LIGHT_BUNDLE,
        LOAD_SCENE,                                 //加载场景

        SET_LOADING_PROGRESS,                       //设置加载进度
        SET_LOADING_TIP,                            //设置加载提示

        SHOW_ACTIVITY_INDICATOR,                    //显示数据加载菊花
        SHOW_ACTIVITY_INDICATOR_IMMEDIATELY,        //立刻显示菊花
        HIDE_ACTIVITY_INDICATOR,                    //隐藏数据加载菊花
    }
}