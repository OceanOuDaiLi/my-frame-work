using FrameWork;
using UnityEngine;

namespace Model
{
    public class HeroModelMgr : BaseModelMgr
    {
        public Vector2Int m_vServerPos;
        public Vector2 m_vClientPos;
        // Todo: Cache DB Config Data 
        // Todo: Cache User Model Data

        public void SetClientPos(Vector2 vPos)
        {
            m_vClientPos = vPos;
        }

        public void SetServerPos(Vector2Int vPos)
        {
            m_vServerPos.x = vPos.x;
            m_vServerPos.y = vPos.y;

            App.Instance.Trigger("OnHeroPosChanged");
        }

        ///////////////////////////////////////////////
        ////////////HeroModelMgr Initialize///////////
        ///////////////////////////////////////////////

        public HeroModelMgr() : base()
        {
            OnRegister();

            // 获取 DB数据
        }

        public override void OnDispose()
        {
            OnRemove();
            base.OnDispose();
        }

        #region Auto Event Binding.
        private void OnRegister()
        {
        }

        private void OnRemove()
        {

        }
        #endregion


        ///////////////////////////////////////////////
        ////////////Proto Response Event///////////////
        ////////////动态生成接口，实现自定义///////////
    }
}