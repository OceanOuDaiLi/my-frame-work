using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGUIGridView {
    public class VerticalGridViewControl<T> : GridViewController<T>
    {
        public float cellHeight;

        protected override void Init() {
            base.Init();
            if (gridView != null)
            {
                gridView.delegateHeightOfCell = () =>
                {
                    return cellHeight;
                };
            }
        }
    }
}