/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

/**
 * @class strange.extensions.mediation.impl.EventView
 * 
 * Injects a local event bus into this View. Intended
 * for local communication between the View and its
 * Mediator.
 * 
 * Caution: we recommend against injecting the context-wide
 * dispatcher into a View.
 */

#if UNITY_EDITOR
using UnityEngine;
#endif

using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.api;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.U2D;

namespace strange.extensions.mediation.impl
{
    public class EventView : View
    {
        [Inject]
        public IEventDispatcher dispatcher { get; set; }

#if UNITY_EDITOR

        [HideInInspector]
        public List<string> _atlasDependence = null;

        public void RefreshForGenAtlasConfig()
        {
            GameObject view = gameObject;
            var imgs = view.GetComponentsInChildren<Image>(true);
            var rImgs = view.GetComponentsInChildren<RawImage>(true);

            _atlasDependence = new List<string>();

            foreach (var item in imgs)
            {
                string path = AssetDatabase.GetAssetPath(item.sprite);

                if (path.Contains("unity_builtin_extra") || item.sprite == null) { continue; }         // flit unity default resources.

                if (!string.IsNullOrEmpty(path))
                {
                    string dir = System.IO.Path.GetDirectoryName(path);
                    dir = dir.Remove(0, dir.LastIndexOf(@"\") + 1);

                    if (!_atlasDependence.Contains(dir))
                        _atlasDependence.Add(dir);

                    // Debug.Log($"path:{dir} ");
                }
                else
                {
                    Debug.LogError($"Sprite Gen Error: Can't find {item.sprite.name} file path.");
                }
            }

            foreach (var item in rImgs)
            {
                string path = AssetDatabase.GetAssetPath(item.texture);

                if (path.Contains("unity_builtin_extra") || path.Contains("AssetBundle/live2d") || item.texture == null) { continue; }         // flit unity default resources.

                if (!string.IsNullOrEmpty(path))
                {
                    string dir = System.IO.Path.GetDirectoryName(path);
                    dir = dir.Remove(0, dir.LastIndexOf(@"\") + 1);

                    if (!_atlasDependence.Contains(dir))
                        _atlasDependence.Add(dir);

                    // Debug.LogWarning($"path:{path}  ");
                }
                else
                {
                    Debug.LogError($"Sprite Gen Error: Can't find {item.texture.name} file path.");
                }
            }
        }

#endif
    }
}
