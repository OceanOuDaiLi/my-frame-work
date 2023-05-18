using UnityEngine;

namespace UI
{
    public class DataLoadingImg : MonoBehaviour
    {
        void Update()
        {
            //菊花旋转动画
            transform.Rotate(new Vector3(0, 0, Time.deltaTime * -60));
        }
    }
}

