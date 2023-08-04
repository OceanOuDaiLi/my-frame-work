using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace UGUIGridView
{
    public class GridViewController<T> : EventView
    {
        public List<T> dataList;
        public GridView gridView = null;
        public RectTransform cellPrefab;
        public int countInAGroup;

        protected override void Awake()
        {
            base.Awake();
            Init();
        }

        protected virtual void Init()
        {
            if (gridView != null)
            {
                gridView.delegateCellsNumInGroup = ()
                    =>
                {
                    return countInAGroup;
                };
                gridView.delegateNumberOfCells = () =>
                {
                    return dataList.Count;
                };
                gridView.delegateNewCellOfIndex = (g, i) =>
                {
                    return OnNewCell(i);
                };
                gridView.delegateUpdateCellOfIndex = (g, cell, i) =>
                {
                    OnUpdateCell(cell, i);
                };
            }
        }

        protected virtual RectTransform OnNewCell(int i)
        {
            return null;
        }

        protected virtual void OnUpdateCell(RectTransform cell, int i)
        {

        }

        public virtual void Reload()
        {
            gridView.ReloadData(-2);
        }

        public virtual void Reload(int index)
        {
            gridView.ReloadData(index);
        }

        public void ClearCells()
        {
            gridView.Clear();
        }
    }
}