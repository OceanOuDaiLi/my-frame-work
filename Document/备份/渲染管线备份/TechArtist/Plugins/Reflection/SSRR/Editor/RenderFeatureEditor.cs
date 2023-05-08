using UnityEngine;
using UnityEditor;

namespace ShinySSRR {

    [CustomEditor(typeof(ShinySSRR))]
    public class RenderFeatureEditor : Editor {

        SerializedProperty useDeferred, renderPassEvent, showInSceneView;
        SerializedProperty smoothnessThreshold, reflectionsMultiplier, reflectionsMinIntensity, reflectionsMaxIntensity;
        SerializedProperty downsampling, depthBias, outputMode, separationPos, lowPrecision, stopNaN;
        SerializedProperty sampleCount, maxRayLength, thickness, binarySearchIterations, refineThickness, thicknessFine, decay, jitter, animatedJitter;
        SerializedProperty fresnel, fuzzyness, contactHardening, minimumBlur;
        SerializedProperty blurDownsampling, blurStrength, specularControl, specularSoftenPower, vignetteSize;
        Reflections[] reflections;
        public Texture bulbOnIcon, bulbOffIcon, deleteIcon, arrowRight;

        private void OnEnable() {
            renderPassEvent = serializedObject.FindProperty("renderPassEvent");
            useDeferred = serializedObject.FindProperty("useDeferred");
            showInSceneView = serializedObject.FindProperty("showInSceneView");
            smoothnessThreshold = serializedObject.FindProperty("smoothnessThreshold");
            reflectionsMultiplier = serializedObject.FindProperty("reflectionsMultiplier");
            reflectionsMinIntensity = serializedObject.FindProperty("reflectionsMinIntensity");
            reflectionsMaxIntensity = serializedObject.FindProperty("reflectionsMaxIntensity");
            downsampling = serializedObject.FindProperty("downsampling");
            depthBias = serializedObject.FindProperty("depthBias");
            outputMode = serializedObject.FindProperty("outputMode");
            separationPos = serializedObject.FindProperty("separationPos");
            lowPrecision = serializedObject.FindProperty("lowPrecision");
            stopNaN = serializedObject.FindProperty("stopNaN");
            sampleCount = serializedObject.FindProperty("sampleCount");
            maxRayLength = serializedObject.FindProperty("maxRayLength");
            binarySearchIterations = serializedObject.FindProperty("binarySearchIterations");
            thickness = serializedObject.FindProperty("thickness");
            thicknessFine = serializedObject.FindProperty("thicknessFine");
            refineThickness = serializedObject.FindProperty("refineThickness");
            decay = serializedObject.FindProperty("decay");
            fresnel = serializedObject.FindProperty("fresnel");
            fuzzyness = serializedObject.FindProperty("fuzzyness");
            contactHardening = serializedObject.FindProperty("contactHardening");
            minimumBlur = serializedObject.FindProperty("minimumBlur");
            jitter = serializedObject.FindProperty("jitter");
            animatedJitter = serializedObject.FindProperty("animatedJitter");
            blurDownsampling = serializedObject.FindProperty("blurDownsampling");
            blurStrength = serializedObject.FindProperty("blurStrength");
            specularControl = serializedObject.FindProperty("specularControl");
            specularSoftenPower = serializedObject.FindProperty("specularSoftenPower");
            vignetteSize = serializedObject.FindProperty("vignetteSize");

#if UNITY_2020_1_OR_NEWER
            reflections = FindObjectsOfType<Reflections>(true);
#else
            reflections = FindObjectsOfType<Reflections>();
#endif
        }

        public override void OnInspectorGUI() {

            int reflectionsCount = reflections != null ? reflections.Length : 0;
            EditorGUILayout.PropertyField(useDeferred);
            EditorGUILayout.PropertyField(renderPassEvent);
            EditorGUILayout.PropertyField(showInSceneView);
            EditorGUILayout.PropertyField(downsampling);
            if (downsampling.intValue > 1 && !useDeferred.boolValue) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(depthBias);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(outputMode);
            if (outputMode.intValue == (int)OutputMode.SideBySideComparison) {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(separationPos);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(lowPrecision);
            EditorGUILayout.PropertyField(stopNaN, new GUIContent("Stop NaN"));

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Raytracing Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Apply Quality Preset:", GUILayout.Width(EditorGUIUtility.labelWidth));
            ShinySSRR ssr = (ShinySSRR)target;
            if (GUILayout.Button("Fast")) {
                ssr.ApplyRaytracingPreset(RaytracingPreset.Fast);
            }
            if (GUILayout.Button("Medium")) {
                ssr.ApplyRaytracingPreset(RaytracingPreset.Medium);
            }
            if (GUILayout.Button("High")) {
                ssr.ApplyRaytracingPreset(RaytracingPreset.High);
            }
            if (GUILayout.Button("Superb")) {
                ssr.ApplyRaytracingPreset(RaytracingPreset.Superb);
            }
            if (GUILayout.Button("Ultra")) {
                ssr.ApplyRaytracingPreset(RaytracingPreset.Ultra);
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
            EditorGUILayout.PropertyField(animatedJitter);

            EditorGUILayout.PropertyField(smoothnessThreshold, new GUIContent("Smoothness Threshold", "Minimum smoothness to receive reflections"));
            EditorGUILayout.PropertyField(reflectionsMultiplier, new GUIContent("Global Multiplier"));
            EditorGUILayout.PropertyField(reflectionsMinIntensity, new GUIContent("Min Intensity"));
            EditorGUILayout.PropertyField(reflectionsMaxIntensity, new GUIContent("Max Intensity"));
            EditorGUILayout.PropertyField(fresnel);
            EditorGUILayout.PropertyField(decay);
            EditorGUILayout.PropertyField(specularControl);
            if (specularControl.boolValue) {
                EditorGUILayout.PropertyField(specularSoftenPower);
            }
            EditorGUILayout.PropertyField(vignetteSize);

            EditorGUILayout.PropertyField(fuzzyness, new GUIContent("Fuzziness"));
            EditorGUILayout.PropertyField(contactHardening);
            EditorGUILayout.PropertyField(minimumBlur);
            EditorGUILayout.PropertyField(blurDownsampling);
            EditorGUILayout.PropertyField(blurStrength);

            if (reflectionsCount > 0) {
                if (!ShinySSRR.isDeferredActive) {
                    EditorGUILayout.HelpBox("Some settings may be overriden by Reflections scripts on specific objects.", MessageType.Info);
                }
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Reflections scripts in Scene", EditorStyles.helpBox);
                if (ShinySSRR.isDeferredActive) {
                    EditorGUILayout.HelpBox("In deferred rendering path, only global SSR settings are used.", MessageType.Warning);
                }
                for (int k = 0; k < reflectionsCount; k++) {
                    Reflections refl = reflections[k];
                    if (refl == null) continue;
                    EditorGUILayout.BeginHorizontal();
                    GUI.enabled = refl.gameObject.activeInHierarchy;
                    if (GUILayout.Button(new GUIContent(refl.enabled ? bulbOnIcon : bulbOffIcon, "Toggle on/off this reflection"), EditorStyles.miniButton, GUILayout.Width(35))) {
                        refl.enabled = !refl.enabled;
                    }
                    GUI.enabled = true;
                    if (GUILayout.Button(new GUIContent(deleteIcon, "Remove this reflection script"), EditorStyles.miniButton, GUILayout.Width(35))) {
                        if (EditorUtility.DisplayDialog("Confirmation", "Remove the reflection script on " + refl.gameObject.name + "?", "Ok", "Cancel")) {
                            DestroyImmediate(refl);
                            reflections[k] = null;
                            continue;
                        }
                    }
                    if (GUILayout.Button(new GUIContent(arrowRight, "Select this reflection script"), EditorStyles.miniButton, GUILayout.Width(35), GUILayout.Width(40))) {
                        Selection.activeObject = refl.gameObject;
                        EditorGUIUtility.PingObject(refl.gameObject);
                        GUIUtility.ExitGUI();
                    }
                    GUI.enabled = refl.isActiveAndEnabled;
                    if (!refl.gameObject.activeInHierarchy) {
                        GUILayout.Label(refl.name + " (hidden gameobject)");
                    } else {
                        GUILayout.Label(refl.name);
                    }
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();
                }
            } else if (reflectionsCount == 0) {
                if (!ShinySSRR.isDeferredActive) {
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("Reflections in Scene", EditorStyles.helpBox);
                    EditorGUILayout.HelpBox("In forward rendering path, add a Reflections script to any object or group of objects that you want to get reflections.", MessageType.Info);
                }

            }

        }

    }
}