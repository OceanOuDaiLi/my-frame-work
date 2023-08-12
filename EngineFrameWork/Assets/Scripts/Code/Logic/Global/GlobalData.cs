using UI;

namespace Model
{
    public class GlobalData
    {
        public static GlobalData instance = null;

        // net services.
        public HttpService httpService = null;
        public WebSocketService websocketService = null;

        // ui common tools managers.
        public ConfigMgr configMgr = null;

        // ui module model manager.
        public LoginModelMgr loginMgr = null;

        // game relate managers
        public HeroModelMgr heroMgr = null;
        public SceneModelMgr sceneModelMgr = null;
        public FightModelMgr fightModelMgr = null;
        public CharacterModelMgr characterModelMgr = null;

        public GlobalData()
        {
            if (instance != null)
            {
                CDebug.LogError("More than one GlobalData existed");
                instance = null;
            }

            instance = this;
        }

        ~GlobalData()
        {
            instance = null;
        }

        public void Initialize()
        {
            if (configMgr != null) configMgr.OnDispose();
            configMgr = new ConfigMgr();

            if (loginMgr != null) loginMgr.OnDispose();
            loginMgr = new LoginModelMgr();


            if (heroMgr != null) heroMgr.OnDispose();
            heroMgr = new HeroModelMgr();

            if (characterModelMgr != null) characterModelMgr.OnDispose();
            characterModelMgr = new CharacterModelMgr();

            if (sceneModelMgr != null) sceneModelMgr.OnDispose();
            sceneModelMgr = new SceneModelMgr();

            if (fightModelMgr != null) fightModelMgr.OnDispose();
            fightModelMgr = new FightModelMgr();
        }
    }
}