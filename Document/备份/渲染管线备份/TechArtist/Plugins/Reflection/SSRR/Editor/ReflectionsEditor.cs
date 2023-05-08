using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ShinySSRR {

    [CustomEditor(typeof(Reflections))]
    public class ReflectionsEditor : Editor {

        SerializedProperty ignore, scope, layerMask, nameFilter, subMeshMask;
        SerializedProperty smoothness, perSubMeshSmoothness, subMeshSettings;
        SerializedProperty useMaterialSmoothness, materialSmoothnessMapPropertyName, materialSmoothnessIntensityPropertyName;
        SerializedProperty useMaterialNormalMap, materialNormalMapPropertyName;
        SerializedProperty fresnel, fuzzyness, contactHardening;
        SerializedProperty overrideGlobalSettings;
        SerializedProperty sampleCount, maxRayLength, thickness, binarySearchIterations, refineThickness, thicknessFine, decay, jitter;

        private void OnEnable() {
            ignore = serializedObject.FindProperty("ignore");
            scope = serializedObject.FindProperty("scope");
            layerMask = serializedObject.FindProperty("layerMask");
            nameFilter = serializedObject.FindProperty("nameFilter");
            subMeshMask = serializedObject.FindProperty("subMeshMask");
            smoothness = serializedObject.FindProperty("smoothness");
            useMaterialSmoothness = serializedObject.FindProperty("useMaterialSmoothness");
            materialSmoothnessMapPropertyName = serializedObject.FindProperty("materialSmoothnessMapPropertyName");
            materialSmoothnessIntensityPropertyName = serializedObject.FindProperty("materialSmoothnessIntensityPropertyName");
            perSubMeshSmoothness = serializedObject.FindProperty("perSubMeshSmoothness");
            subMeshSettings = serializedObject.FindProperty("subMeshSettings");
            useMaterialNormalMap = serializedObject.FindProperty("useMaterialNormalMap");
            materialNormalMapPropertyName = serializedObject.FindProperty("materialNormalMapPropertyName");
            fresnel = serializedObject.FindProperty("fresnel");
            fuzzyness = serializedObject.FindProperty("fuzzyness");
            contactHardening = serializedObject.FindProperty("contactHardening");
            overrideGlobalSettings = serializedObject.FindProperty("overrideGlobalSettings");
            sampleCount = serializedObject.FindProperty("sampleCount");
            maxRayLength = serializedObject.FindProperty("maxRayLength");
            binarySearchIterations = serializedObject.FindProperty("binarySearchIterations");
            thickness = serializedObject.FindProperty("thickness");
            refineThickness = serializedObject.FindProperty("refineThickness");
            thicknessFine = serializedObject.FindProperty("thicknessFine");
            decay = serializedObject.FindProperty("decay");
            jitter = serializedObject.FindProperty("jitter");
        }

        public override void OnInspectorGUI() {

            UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset pipe = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
            if (pipe == null) {
                EditorGUILayout.HelpBox("Universal Rendering Pipeline asset is not set in 'Project Settings / Graphics or Quality' !", MessageType.Error);
                EditorGUILayout.Separator();
                GUI.enabled = false;
            } else if (!ShinySSRR.installed) {
                EditorGUILayout.HelpBox("Shiny SSRR Render Feature must be added to the rendering pipeline renderer.", MessageType.Error);
                if (GUILayout.Button("Go to Universal Rendering Pipeline Asset")) {
                    Selection.activeObject = pipe;
                }
                EditorGUILayout.Separator();
                GUI.enabled = false;
            } else {
                if (!pipe.supportsCameraDepthTexture) {
                    EditorGUILayout.HelpBox("Depth Texture option is required. Check Universal Rendering Pipeline asset!", MessageType.Warning);
                    if (GUILayout.Button("Go to Universal Rendering Pipeline Asset")) {
                        Selection.activeObject = pipe;
                    }
                    EditorGUILayout.Separator();
                    GUI.enabled = false;
                }
                EditorGUILayout.BeginVertical(GUI.skin.box);
                if (GUILayout.Button("Show Global Settings")) {
                    if (pipe != null) {
                        var so = new SerializedObject(pipe);
                        var prop = so.FindProperty("m_RendererDataList");
                        if (prop != null && prop.arraySize > 0) {
                            var o = prop.GetArrayElementAtIndex(0);
                            if (o != null) {
                                Selection.SetActiveObjectWithContext(o.objectReferenceValue, null);
                                GUIUtility.ExitGUI();
                            }
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }

            bool isForwardPath = true;
            if (ShinySSRR.isDeferredActive) {
                EditorGUILayout.HelpBox("In deferred mode, material properties like smoothness and normal map plus global SSR settings are used.", MessageType.Warning);
                isForwardPath = false;
                GUI.enabled = false;
            }

            // ensure submesh array size matches materials count
            Reflections refl = (Reflections)target;
            if (refl.ssrRenderers != null && refl.ssrRenderers.Count == 1 && refl.ssrRenderers[0].originalMaterials != null) {
                List<Material> materials = refl.ssrRenderers[0].originalMaterials;
                if (refl.subMeshSettings == null) {
                    refl.subMeshSettings = new SubMeshSettingsData[materials.Count];
                } else if (refl.subMeshSettings.Length < materials.Count) {
                    System.Array.Resize(ref refl.subMeshSettings, materials.Count);
                }
            }

            serializedObject.Update();

            EditorGUILayout.PropertyField(ignore);
            if (!ignore.boolValue) {

                if (refl.renderers?.Count == 0) {
                    if (scope.intValue == (int)Scope.OnlyThisObject) {
                        EditorGUILayout.HelpBox("No renderers found on this gameobject. Switch to 'Include Children' or add this script to another object which contains a renderer.", MessageType.Warning);
                    } else {
                        EditorGUILayout.HelpBox("No renderers found under this gameobject.", MessageType.Warning);
                    }
                }

                EditorGUILayout.PropertyField(scope);
                if (scope.intValue == (int)Scope.IncludeChildren) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(layerMask);
                    EditorGUILayout.PropertyField(nameFilter);
                    EditorGUILayout.PropertyField(subMeshMask);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.PropertyField(useMaterialSmoothness);
                GUI.enabled = !useMaterialSmoothness.boolValue;
                EditorGUILayout.PropertyField(perSubMeshSmoothness);
                if (perSubMeshSmoothness.boolValue) {
                    EditorGUILayout.PropertyField(subMeshSettings, new GUIContent("Smoothness Values"), true);
                } else {
                    EditorGUILayout.PropertyField(smoothness);
                }
                GUI.enabled = isForwardPath;
                EditorGUILayout.PropertyField(useMaterialNormalMap);

                if (useMaterialSmoothness.boolValue || perSubMeshSmoothness.boolValue || useMaterialNormalMap.boolValue) {
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("Material Property Names", EditorStyles.miniBoldLabel);
                    if (useMaterialSmoothness.boolValue || perSubMeshSmoothness.boolValue) {
                        EditorGUILayout.PropertyField(materialSmoothnessMapPropertyName, new GUIContent("Smoothness Map", "The material property name for the smoothness map"));
                        EditorGUILayout.PropertyField(materialSmoothnessIntensityPropertyName, new GUIContent("Smoothness Intensity", "The material property name for the smoothness intensity"));
                    }
                    if (useMaterialNormalMap.boolValue) {
                        EditorGUILayout.PropertyField(materialNormalMapPropertyName, new GUIContent("NormalMap", "The material property name for the normal map"));
                    }
                }

                EditorGUILayout.PropertyField(overrideGlobalSettings);
                if (overrideGlobalSettings.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Apply Quality Preset:", GUILayout.Width(EditorGUIUtility.labelWidth));
                    if (GUILayout.Button("Fast")) {
                        ApplyRaytracingPreset(RaytracingPreset.Fast);
                    }
                    if (GUILayout.Button("Medium")) {
                        ApplyRaytracingPreset(RaytracingPreset.Medium);
                    }
                    if (GUILayout.Button("High")) {
                        ApplyRaytracingPreset(RaytracingPreset.High);
                    }
                    if (GUILayout.Button("Superb")) {
                        ApplyRaytracingPreset(RaytracingPreset.Superb);
                    }
                    if (GUILayout.Button("Ultra")) {
                        ApplyRaytracingPreset(RaytracingPreset.Ultra);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(sampleCount);
                    EditorGUILayout.PropertyField(maxRayLength);
                    EditorGUILayout.PropertyField(thickness);
                    EditorGUILayout.PropertyField(binarySearchIterations);
                    EditorGUILayout.PropertyField(refineThickness);
                    if (refineThickness.boolValue) {
                        EditorGUILayout.PropertyField(thicknessFine);
                    }
                    EditorGUILayout.PropertyField(jitter);
                    EditorGUILayout.PropertyField(fresnel);
                    EditorGUILayout.PropertyField(decay);
                    EditorGUILayout.PropertyField(fuzzyness, new GUIContent("Fuzziness"));
                    EditorGUILayout.PropertyField(contactHardening);
                    EditorGUI.indentLevel--;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }


        void ApplyRaytracingPreset(RaytracingPreset preset) {
            Reflections ssr = (Reflections)target;
            ssr.ApplyRaytracingPreset(preset);
            EditorUtility.SetDirty(ssr);
        }

    }
}