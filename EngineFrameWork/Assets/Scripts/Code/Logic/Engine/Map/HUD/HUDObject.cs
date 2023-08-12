using UnityEngine;

namespace GameEngine
{
    public enum HUDPosition
    {
        Top,
        Middle,
        Bottom
    }

    public class HUDObject : MonoBehaviour
    {
        public RectTransform Rect = null;

        [HideInInspector]
        public Transform m_Target;
        [HideInInspector]
        public Bounds m_TargetBounds;

        public Vector2 offset = Vector2.zero;
        public Vector2 constantOffset = Vector2.zero;
        public Vector2 randomOffset = Vector2.zero;
        public float Duration = 1f;
        public SpawnPool pool = null;
        public HUDPosition position = HUDPosition.Top;

        public void SetTarget(Transform target, Bounds bounds)
        {
            this.m_Target = target;
            this.m_TargetBounds = bounds;
        }

        public virtual void Despawn()
        {
            if (pool != null) pool.Despawn(Rect);
        }
    }
}