using UnityEngine;

namespace Core
{

    /// <summary>
    /// 驱动脚本
    /// </summary>
    public sealed class DriverBehaviour : MonoBehaviour
    {
        /// <summary>
        /// 驱动器
        /// </summary>
        private Driver driver;

        /// <summary>
        /// 设定驱动器
        /// </summary>
        /// <param name="driver">驱动器</param>
        public void SetDriver(Driver driver)
        {
            this.driver = driver;
        }

        /// <summary>
        /// 每帧更新时
        /// </summary>
        public void Update()
        {
            if (driver != null)
            {
                driver.Update();
            }
        }

        /// <summary>
        /// 在每帧更新时之后
        /// </summary>
        public void LateUpdate()
        {
            if (driver != null)
            {
                driver.LateUpdate();
            }
        }

        /// <summary>
        /// 当释放时
        /// </summary>
        public void OnDestroy()
        {
            if (driver != null)
            {
                driver.OnDestroy();
            }
        }
    }
}