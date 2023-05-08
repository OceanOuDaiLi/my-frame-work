using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace TechArtist.QualitySetting
{
    public enum EQualityLevel
    {
        Low = 0,
        Medium,
        High
    }

    public static class DynamicResolutionManager
    {
        private static float _currentFPS;
        private static float _totalFPS;
        private static float _lastFPS;

        private static float _frameTime;
        private static int _frameCount;
        private static float _deltaTime;

        private static int _lowFpsCount;
        private static int _highFpsCount;
        private static int _fpsBeLowerCount;

        private static bool _firstStart = true;

        public static bool EnterFight = false;
        public static float ExpectTargetFPS = 59.5f;

        static DynamicResolutionManager()
        {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            for (int i = 0; i < playerLoop.subSystemList.Length; ++i)
            {
                int subsystemListLength = playerLoop.subSystemList[i].subSystemList.Length;
                if (playerLoop.subSystemList[i].type == typeof(Update))
                {
                    playerLoop.subSystemList[i].updateDelegate -= Update;
                    playerLoop.subSystemList[i].updateDelegate += Update;
                }
            }
            PlayerLoop.SetPlayerLoop(playerLoop);

            _frameTime = 1f;
            _totalFPS = _lastFPS = _currentFPS = _deltaTime = 0;
            _fpsBeLowerCount = _frameCount = _lowFpsCount = _highFpsCount = 0;
        }

        public static void ExitGameFight()
        {
            EnterFight = false;
            if(GameFightQuality.Instance!=null)
                GameFightQuality.Instance.UpdateDynamicResolution(false);

            _lastFPS = 0;
            _totalFPS = 0;
            _deltaTime = 0;
            _currentFPS = 0;
            _frameCount = 0;
            _lowFpsCount = 0;
            _highFpsCount = 0;
            _fpsBeLowerCount = 0;
        }

        private static void Update()
        {
            if (!EnterFight) { return; }

            _frameCount++;
            _deltaTime += Time.deltaTime;

            if (_deltaTime >= _frameTime)
            {
                _currentFPS = _frameCount / _deltaTime;
                _totalFPS += _currentFPS;

                _frameCount = 0;
                _deltaTime = 0f;

                //If target FPS lower than current FPS, low down the Quality
                if (_currentFPS < ExpectTargetFPS)
                {
                    ++_lowFpsCount;
                    _highFpsCount = 0;
                }
                else
                {
                    ++_highFpsCount;
                    _lowFpsCount = 0;
                }

                if (_lowFpsCount > 30)
                {
                    float aveFPS = _totalFPS / _lowFpsCount;
                    if (_firstStart)
                    {
                        _firstStart = false;
                        _lastFPS = aveFPS;
                        GameFightQuality.Instance.UpdateDynamicResolution(true);
                        //Debug.LogError("***************   Enable DynamicResolutionFeature   ***************");
                    }

                    _lowFpsCount = 0;
                    //UpdateObservers(true);

                    if (aveFPS < _lastFPS)
                        _fpsBeLowerCount++;
                    else
                        _fpsBeLowerCount = 0;

                    if (_fpsBeLowerCount > 3)
                    {
                        GameFightQuality.Instance.UpdateDynamicResolution(false);
                        //Debug.LogError("***************   Disable DynamicResolutionFeature   ***************");
                    }

                    _totalFPS = 0;
                }

                if (_highFpsCount > 30)
                {
                    _highFpsCount = 0;
                    _fpsBeLowerCount = 0;
                    _totalFPS = 0;
                    _firstStart = true;
                }
            }
        }

    }
}