using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UGUIGridView {
    public class HorizontalGridViewControl<T> : GridViewController<T> {
        public float cellWidth;

        protected override void Awake() {
            base.Awake();
        }

        protected override void Init() {
            base.Init();
            gridView.delegateWidthOfCell = () => {
                return cellWidth;
            };
        }

    }
}