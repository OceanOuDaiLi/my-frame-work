using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UGUIGridView
{
    public class GridView : ScrollRect
    {
        /// <summary>
        /// 说明：获取一组的单元数
        /// </summary>
        public Func<int> delegateCellsNumInGroup = null;
        /// <summary>
        /// 说明：获取全部单元数
        /// </summary>
        public Func<int> delegateNumberOfCells = null;
        /// <summary>
        /// 说明：获取单元高度
        /// </summary>
        public Func<float> delegateHeightOfCell = null;
        /// <summary>
        /// 说明：获取单元宽度
        /// </summary>
        public Func<float> delegateWidthOfCell = null;
        /// <summary>
        /// 说明：根据数据单元Index获取实例
        /// 参数：Transform表示当前Content，int表示数据单元index
        /// </summary>
        public Func<Transform, int, RectTransform> delegateNewCellOfIndex = null;
        /// <summary>
        /// 说明：根据数据单元Index修改实例
        /// 参数：RectTransform 表示要修改的实例，int表示数据单元index
        /// </summary>
        public Action<Transform, RectTransform, int> delegateUpdateCellOfIndex = null;

        float cellwidth;
        float cellheight;
        float currentGroup;

        int numberOfCells;
        int numberOfGroups;
        int cellsNumInGroup;

        new Rect viewRect;
        List<List<Rect>> rects = new List<List<Rect>>();

        Dictionary<Vector2, RectTransform> visibilityCellList = new Dictionary<Vector2, RectTransform>();
        Queue<RectTransform> invisibilityCellQueue = new Queue<RectTransform>();

        protected override void Awake()
        {
            base.Awake();
            viewRect = (transform as RectTransform).rect;
        }

        protected override void Start()
        {
            base.Start();
            onValueChanged.AddListener(OnMove);
        }

        protected override void OnDestroy()
        {
            delegateNumberOfCells = null;
            delegateHeightOfCell = null;

            base.OnDestroy();
        }

        void OnMove(Vector2 value)
        {
            if (numberOfCells == 0) return;

            UpdateMinMaxVisibleCell();
        }

        void UpdateMinMaxVisibleCell()
        {
            Vector2 minMaxGroupIndex = GetMinMaxVisibleIndex();
            int minGroupIndex = (int)minMaxGroupIndex.x;
            int maxGroupIndex = (int)minMaxGroupIndex.y;

            RemoveCellAtIndex(minGroupIndex, maxGroupIndex);

            for (int i = minGroupIndex; i < maxGroupIndex; i++)
            {
                for (int j = 0; j < cellsNumInGroup; j++)
                {
                    AddCellAtIndex(i, j);
                }
            }
            currentGroup = minMaxGroupIndex.x;
        }

        bool Init()
        {
            if (delegateNumberOfCells == null)
                return false;
            if (delegateHeightOfCell == null && delegateWidthOfCell == null)
                return false;
            if (delegateNewCellOfIndex == null)
                return false;
            if (delegateCellsNumInGroup == null)
                return false;
            if (delegateUpdateCellOfIndex == null)
                return false;

            numberOfCells = delegateNumberOfCells();
            cellsNumInGroup = delegateCellsNumInGroup();
            numberOfGroups = numberOfCells / cellsNumInGroup + (numberOfCells % cellsNumInGroup > 0 ? 1 : 0);

            if (vertical)
            {
                cellwidth = viewRect.width / cellsNumInGroup;
                cellheight = delegateHeightOfCell();

                content.pivot = new Vector2(0.5f, 1.0f);
                content.anchorMin = new Vector2(0.0f, 1.0f);
                content.anchorMax = new Vector2(1.0f, 1.0f);
                SetContentAnchoredPosition(new Vector2(0.0f, 0.0f));
            }
            else if (horizontal)
            {
                cellheight = viewRect.height / cellsNumInGroup;
                cellwidth = delegateWidthOfCell();

                content.pivot = new Vector2(0.0f, 0.5f);
                content.anchorMin = new Vector2(0.0f, 0.0f);
                content.anchorMax = new Vector2(0.0f, 1.0f);
                SetContentAnchoredPosition(new Vector2(0.0f, 0.0f));
            }
            else
                return false;
            return true;
        }

        public void ReloadData(int index = default(int)/*,bool cleanCell=default(bool)*/)
        {
            foreach (RectTransform rt in visibilityCellList.Values)
            {
                invisibilityCellQueue.Enqueue(rt);
                rt.gameObject.SetActive(false);
            }
            visibilityCellList.Clear();

            if (!Init())
            {
                //Debug.Log("Grid Init Error");
                return;
            }
            SetRects();

            if (index == -2)
            {
                EnqueueReusabelCell();
                if (vertical)
                    SetContentAnchoredPosition(new Vector2(0, currentGroup * cellheight));
                else if (horizontal)
                    SetContentAnchoredPosition(new Vector2(-currentGroup * cellwidth, 0));
            }

            if (index >= 0)
            {
                if (vertical)
                    SetContentAnchoredPosition(new Vector2(0, index * cellheight));
                else if (horizontal)
                    SetContentAnchoredPosition(new Vector2(-index * cellwidth, 0));
            }

            if (index == -1)
            {
                if (movementType == MovementType.Elastic)
                {
                    if (vertical)
                        SetContentAnchoredPosition(new Vector2(0, -2 * cellheight));
                    else if (horizontal)
                        SetContentAnchoredPosition(new Vector2(2 * cellwidth, 0));
                }
                else
                {
                    if (vertical)
                        SetContentAnchoredPosition(new Vector2(0, 0));
                    else if (horizontal)
                        SetContentAnchoredPosition(new Vector2(0, 0));
                }
            }

            Vector2 minMaxGroupIndex = GetMinMaxVisibleIndex();
            int minGroupIndex = (int)minMaxGroupIndex.x;
            int maxGroupIndex = (int)minMaxGroupIndex.y;

            if (numberOfCells > 0)
            {
                for (int i = minGroupIndex; i < maxGroupIndex; i++)
                {
                    for (int j = 0; j < cellsNumInGroup; j++)
                    {
                        AddCellAtIndex(i, j);
                    }
                }
            }
        }

        void SetRects()
        {
            foreach (List<Rect> group in rects)
            {
                group.Clear();
            }
            rects.Clear();

            int countOfCells = numberOfCells;
            Vector2 contentSize = content.sizeDelta;

            float x = 0.0f;
            float y = 0.0f;
            if (vertical)
            {
                for (int i = 0; i < numberOfGroups; i++)
                {
                    x = -(viewRect.width + cellwidth) / 2;
                    y += cellheight;
                    List<Rect> group = new List<Rect>();

                    for (int j = 0; j < cellsNumInGroup && countOfCells > 0; j++, countOfCells--)
                    {
                        x += cellwidth;
                        group.Add(new Rect(x, -y, 0, cellheight));
                    }
                    rects.Add(group);
                }
                contentSize.x = 0.0f;
                contentSize.y = y;
            }
            else if (horizontal)
            {
                for (int i = 0; i < numberOfGroups; i++)
                {
                    y = (viewRect.height + cellheight) / 2;
                    List<Rect> group = new List<Rect>();
                    for (int j = 0; j < cellsNumInGroup && countOfCells > 0; j++, countOfCells--)
                    {
                        y -= cellheight;
                        group.Add(new Rect(x, y, cellwidth, 0));
                    }
                    x += cellwidth;
                    rects.Add(group);
                }
                contentSize.x = x;
                contentSize.y = 0.0f;
            }
            content.sizeDelta = contentSize;
        }

        /// <summary>
        /// 说明：返回可视区域的行数 Form minIndex To maxIndex(Exclude)
        /// </summary>
        /// <returns></returns>
        Vector2 GetMinMaxVisibleIndex()
        {
            float min = 0.0f;
            float max = 0.0f;
            if (vertical)
            {
                Vector2 offsetMax = content.offsetMax;
                min = offsetMax.y / cellheight;
                Vector2 offsetMin = -content.offsetMin;
                max = (offsetMin.y - viewRect.height) / cellheight;
            }
            else if (horizontal)
            {
                Vector2 offsetMin = -content.offsetMin;
                min = offsetMin.x / cellwidth;
                Vector2 offsetMax = content.offsetMax;
                max = (offsetMax.x - viewRect.width) / cellwidth;
            }
            float minIndex = min;
            float maxIndex = numberOfGroups - (int)max;

            return new Vector2(
                Mathf.Clamp(minIndex, 0, numberOfGroups),
                Mathf.Clamp(maxIndex, 0, numberOfGroups)
            );
        }

        void AddCellAtIndex(int groupIndex, int inGroupIndex)
        {
            int index = groupIndex * cellsNumInGroup + inGroupIndex;
            if ((index + 1) > numberOfCells || inGroupIndex >= cellsNumInGroup) return;

            Vector2 cellIndex = new Vector2(groupIndex, inGroupIndex);
            if (visibilityCellList.ContainsKey(cellIndex)) return;

            RectTransform trans = DequeueReusabelCell();
            if (trans == null)
            {
                trans = delegateNewCellOfIndex(content, index) as RectTransform;

                if (trans.parent != content)
                {
                    trans.SetParent(content);
                    trans.localPosition = Vector3.zero;
                    trans.localScale = Vector3.one;
                }
            }
            delegateUpdateCellOfIndex(content, trans, index);

            Rect rect = rects[groupIndex][inGroupIndex];
            trans.anchoredPosition = new Vector2(rect.x + rect.width / 2, rect.y + rect.height / 2);
            trans.pivot = new Vector2(0.5f, 0.5f);

            if (vertical)
            {
                trans.anchorMin = trans.anchorMax = new Vector2(0.5f, 1.0f);
            }
            else if (horizontal)
            {
                trans.anchorMin = trans.anchorMax = new Vector2(0.0f, 0.5f);
            }

            visibilityCellList[cellIndex] = trans;
            trans.gameObject.SetActive(true);
        }

        void RemoveCellAtIndex(int rowMinIndex, int rowMaxIndex)
        {
            List<Vector2> removeList = new List<Vector2>();
            foreach (Vector2 cellIndex in visibilityCellList.Keys)
            {
                if (cellIndex.x < rowMinIndex || cellIndex.x >= rowMaxIndex)
                {
                    RectTransform trans = visibilityCellList[cellIndex];
                    invisibilityCellQueue.Enqueue(trans);
                    removeList.Add(cellIndex);
                    trans.gameObject.SetActive(false);
                }
            }

            foreach (var index in removeList)
            {
                visibilityCellList.Remove(index);
            }

            removeList.Clear();
            removeList = null;
        }

        RectTransform DequeueReusabelCell()
        {
            if (invisibilityCellQueue.Count > 0)
                return invisibilityCellQueue.Dequeue();

            return null;
        }

        void EnqueueReusabelCell()
        {
            foreach (Vector2 cellIndex in visibilityCellList.Keys)
            {
                RectTransform cell = visibilityCellList[cellIndex];
                if (cell == null)
                    continue;

                invisibilityCellQueue.Enqueue(cell);
                cell.gameObject.SetActive(false);
            }
            visibilityCellList.Clear();
        }

        public void Clear()
        {
            visibilityCellList.Clear();
            invisibilityCellQueue.Clear();
        }
    }
}