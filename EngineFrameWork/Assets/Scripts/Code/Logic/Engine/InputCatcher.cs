using System;
using FrameWork;
using GameEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InputCatcher : MonoSingleton<InputCatcher>
{
    EventSystem currentEventSystem;
    EventDispatcher _eventDispatcher = new EventDispatcher();
    List<RaycastResult> UIRaycastResults = new List<RaycastResult>();

    void Update()
    {
        if (currentEventSystem != EventSystem.current)
        {
            currentEventSystem = EventSystem.current;
        }

        MouseClick();
    }

    void MouseClick()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        GameObject target = null;
        SingleClickData.TargetType targetType = SingleClickData.TargetType.None;

        Vector2 screenPosition = Input.mousePosition;
        Vector2 screemPivot = new Vector2(screenPosition.x / Screen.width, screenPosition.y / Screen.height);

        if (currentEventSystem != null)
        {
            UIRaycastResults.Clear();
            var pointerEvent = new PointerEventData(currentEventSystem)
            {
                position = screenPosition
            };
            currentEventSystem.RaycastAll(pointerEvent, UIRaycastResults);

            for (int i = 0; i < UIRaycastResults.Count; ++i)
            {
                GameObject t = UIRaycastResults[i].gameObject;
                if (t != null)
                {
                    target = t;
                    targetType = SingleClickData.TargetType.UI;
                    break;
                }
            }
        }

        if (target == null && CameraMgr.Instance != null && CameraMgr.Instance.BaseCamera != null)
        {
            Camera mainCamera = CameraMgr.Instance.BaseCamera;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hitResult = Physics2D.Raycast(ray.origin, ray.direction);
            if (hitResult.collider != null)
            {
                target = hitResult.collider.gameObject;
                targetType = SingleClickData.TargetType.World;
            }
            else if (Physics.Raycast(ray, out RaycastHit hitinfo))
            {
                target = hitinfo.collider.gameObject;
                targetType = SingleClickData.TargetType.World;
            }
        }

        //CDebug.Log(string.Format("Catch {0}, screenPosition {2}, screemPivot {3}, targetType {4}, Fream {1}", target != null ? target.name : "null", Time.frameCount, screenPosition, screemPivot, targetType));
        SingleClickData data = new SingleClickData()
        {
            clickTarget = target,
            targetType = targetType,
            screenPosition = screenPosition,
            screemPivot = screemPivot
        };
        App.Instance.Trigger(GameEvent.INPUT_EVENT_SINGLE_CLICK, data);
        _eventDispatcher.Dispatch(data, UIRaycastResults);
    }

    public struct SingleClickData
    {
        /// <summary>
        /// �������Ŀ��
        /// </summary>
        public GameObject clickTarget;
        /// <summary>
        /// �������Ŀ������
        /// </summary>
        public TargetType targetType;
        /// <summary>
        /// �����Ļ�ռ�λ��
        /// </summary>
        public Vector2 screenPosition;
        /// <summary>
        /// �����Ļ�ռ����λ�ã�����Ϊ��0��0��������Ϊ��1��1��
        /// </summary>
        public Vector2 screemPivot;

        public enum TargetType
        {
            None,
            UI,
            World,
        }
    }

    public IEventDispatcher Event
    {
        get
        {
            return _eventDispatcher;
        }
    }

    public interface IEventHandle
    {
    }

    public interface IEventDispatcher
    {
        /// <summary>
        /// ע�ᵥ���¼�
        /// </summary>
        /// <param name="listenObject"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEventHandle AddListenerClick(GameObject listenObject, Action<SingleClickData> callback);

        /// <summary>
        /// �Ƴ������¼�
        /// </summary>
        /// <param name="eventHandle"></param>
        public void RemoveListenerClick(IEventHandle eventHandle);

        /// <summary>
        /// �������ߴ�͸
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="isPenetration"></param>
        public void SetRayPenetration(GameObject gameObject, bool isPenetration);

        /// <summary>
        /// ɾ�����ߴ�͸��Ϣ
        /// </summary>
        /// <param name="gameObject"></param>
        public void RemoveRayPenetration(GameObject gameObject);
    }

    public class EventDispatcher : IEventDispatcher
    {
        private class EventHandle : IEventHandle
        {
            public GameObject listenObject;
            public Action<SingleClickData> callback;
        }

        public IEventHandle AddListenerClick(GameObject listenObject, Action<SingleClickData> callback)
        {
            if (listenObject == null || callback == null)
                throw new ArgumentException("��������Ϊ��");

            EventHandle eventHandle = new EventHandle()
            {
                listenObject = listenObject,
                callback = callback,
            };

            if (listenerCount.ContainsKey(listenObject))
            {
                listenerCount[listenObject].Add(eventHandle);
            }
            else
            {
                List<EventHandle> eventHandles = new List<EventHandle>();
                eventHandles.Add(eventHandle);
                listenerCount.Add(listenObject, eventHandles);
            }
            return eventHandle;
        }

        public void RemoveListenerClick(IEventHandle eventHandle)
        {
            if (_dispatching)
            {
                needRemoveCaches.Add(eventHandle as EventHandle);
            }
            else
            {
                InternalRemoveListener(eventHandle as EventHandle);
            }
        }

        public void SetRayPenetration(GameObject GO, bool isPenetration)
        {
            if (RayPenetrationDicContains(GO))
            {
                rayPenetrationDic[GO] = isPenetration;
            }
            else
            {
                rayPenetrationDic.Add(GO, isPenetration);
            }
        }

        public void RemoveRayPenetration(GameObject GO)
        {
            if (RayPenetrationDicContains(GO))
            {
                rayPenetrationDic.Remove(GO);
            }
        }

        private void InternalRemoveListener(EventHandle eventHandle)
        {
            if (listenerCount.ContainsKey(eventHandle.listenObject))
            {
                var list = listenerCount[eventHandle.listenObject];
                list.Remove(eventHandle);
                if (list.Count <= 0)
                {
                    listenerCount.Remove(eventHandle.listenObject);
                }
            }
        }

        private bool RayPenetrationDicContains(GameObject GO)
        {
            return rayPenetrationDic.ContainsKey(GO);
        }

        Dictionary<GameObject, List<EventHandle>> listenerCount = new Dictionary<GameObject, List<EventHandle>>();
        Dictionary<GameObject, bool> rayPenetrationDic = new Dictionary<GameObject, bool>();


        bool _dispatching = false;
        List<EventHandle> needRemoveCaches = new List<EventHandle>();
        public void Dispatch(SingleClickData data, List<RaycastResult> UIRaycastResults)
        {
            if (data.clickTarget == null || data.targetType == SingleClickData.TargetType.None)
                return;

            try
            {
                _dispatching = true;

                if (UIRaycastResults.Count > 0)
                {
                    for (int i = 0; i < UIRaycastResults.Count; i++)
                    {
                        var raycastResult = UIRaycastResults[i];
                        if (raycastResult.gameObject == null)
                            continue;

                        ///���¼�
                        if (listenerCount.ContainsKey(raycastResult.gameObject))
                        {
                            var eventHandleList = listenerCount[raycastResult.gameObject];

                            SingleClickData dispatchData = new SingleClickData()
                            {
                                clickTarget = raycastResult.gameObject,
                                targetType = SingleClickData.TargetType.UI,
                                screenPosition = data.screenPosition,
                                screemPivot = data.screemPivot
                            };
                            for (int j = 0; j < eventHandleList.Count; j++)
                            {
                                EventHandle eventHandle = eventHandleList[j];
                                try
                                {
                                    eventHandle.callback?.Invoke(dispatchData);
                                }
                                catch (Exception e)
                                {
                                    CDebug.LogException(e);
                                }
                            }
                        }

                        ///�Ƿ�͸
                        if (rayPenetrationDic.ContainsKey(raycastResult.gameObject))
                        {
                            //������Զ��崩͸����Ϊ ���ɴ�͸����ô�ͶϿ�
                            if (!rayPenetrationDic[raycastResult.gameObject])
                                break;
                        }
                        else
                        {
                            //���û���Զ��崩͸���ԣ�Ĭ�ϲ���͸
                            break;
                        }
                    }
                }
                else
                {
                    if (listenerCount.ContainsKey(data.clickTarget))
                    {
                        var eventHandleList = listenerCount[data.clickTarget];
                        for (int j = 0; j < eventHandleList.Count; j++)
                        {
                            EventHandle eventHandle = eventHandleList[j];
                            try
                            {
                                eventHandle.callback?.Invoke(data);
                            }
                            catch (Exception e)
                            {
                                CDebug.LogException(e);
                            }
                        }
                    }
                }


            }
            finally
            {
                _dispatching = false;
                if (needRemoveCaches.Count > 0)
                {
                    for (int i = 0; i < needRemoveCaches.Count; ++i)
                    {
                        InternalRemoveListener(needRemoveCaches[i]);
                    }
                    needRemoveCaches.Clear();
                }
            }
        }
    }
}

