using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class UIEventTrigger : UnityEngine.EventSystems.EventTrigger
{
    //��������¼��а󶨵�ί��
    public delegate void PointEventDelegate(PointerEventData eventData);
    public delegate void BaseEventDelegate(BaseEventData eventData);

    public Action<PointerEventData> onClick;

    public Action<string, PointerEventData> onTouch;
    /*public PointEventDelegate onDown;
    public PointEventDelegate onUp;
    public BaseEventDelegate onSelect;
    public BaseEventDelegate onUpdateSelect;*/




    /// <summary>
    /// �õ��������������
    /// </summary>
    /// <param name="go">��������Ϸ����</param>
    /// <returns>
    /// ������
    /// </returns>
    public static UIEventTrigger Get(GameObject go)
    {
        UIEventTrigger lister = go.GetComponent<UIEventTrigger>();
        if (lister == null)
        {
            lister = go.AddComponent<UIEventTrigger>();
        }
        return lister;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
        {
            onClick.Invoke(eventData);
        }

        //RaycastThrough(eventData);
    }



    public override void OnScroll(PointerEventData eventData)
    {
        if (onClick != null)
        {
            onClick(eventData);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (onTouch != null)
        {
            onTouch("down", eventData);
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (onTouch != null)
        {
            onTouch("drag", eventData);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (onTouch != null)
        {
            onTouch("end_drag", eventData);
        }
    }

    public void RaycastThrough(BaseEventData baseEventData)
    {
        PointerEventData pointerEventData = (PointerEventData)baseEventData;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        //��ǰ�����gameobject
        GameObject currentObj = pointerEventData.pointerCurrentRaycast.gameObject ?? pointerEventData.pointerDrag;
        //��ȡ��ǰ���߼�⵽�����н��
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        foreach (var item in raycastResults)
        {
            GameObject nextObj = item.gameObject;
            if (nextObj != null)
            {
                pointerEventData.pointerCurrentRaycast = item;
                ExecuteEvents.ExecuteHierarchy(nextObj, baseEventData, ExecuteEvents.scrollHandler);
            }
        }

    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (onTouch != null)
        {
            onTouch("up", eventData);
        }
    }

    /*   public override void OnSelect(BaseEventData eventBaseData)
       {
           if (onSelect != null)
           {
               onSelect(eventBaseData);
           }
       }

       public override void OnUpdateSelected(BaseEventData eventBaseData)
       {
           if (onUpdateSelect != null)
           {
               onUpdateSelect(eventBaseData);
           }
       }*/

}//Class_end