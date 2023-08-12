using UnityEngine;
using UnityEngine.UI;
namespace GameEngine
{
    public class HUDText : HUDObject
    {
        [SerializeField]
        Text text = null;

        public System.Action onDespawn = null;

        public void SetText(string txt)
        {
            text.text = txt;
        }

        public override void Despawn()
        {
            if (onDespawn != null) onDespawn();
            onDespawn = null;
            base.Despawn();
        }
    }
}