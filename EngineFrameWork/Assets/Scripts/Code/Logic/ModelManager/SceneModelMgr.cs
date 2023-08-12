
using GameEngine;
using UnityEngine;

namespace Model
{
    public class SceneModelMgr
    {
        public FightMap FightMap { get; set; }
        public SceneMap SceneMap { get; set; }
        public GameObject MapMgrObj { get; set; }
        public Transform MapInstance { get; set; }
        public GameObject FightCanvas { get; set; }
        public GameObject CameraMgrObj { get; set; }

        public SceneModelMgr()
        {
            if (SceneMap != null) { SceneMap.Dispose(); }
            SceneMap = new SceneMap();
        }

        public void OnDispose()
        {
            SceneMap.Dispose();
            SceneMap = null;
            MapMgrObj = null;
            MapInstance = null;
            CameraMgrObj = null;
        }
    }
}