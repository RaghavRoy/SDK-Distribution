using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;

namespace JioXSDK
{

    [CustomEditor(typeof(InteractionUIState))]
    public class InteractionUIStateEditor : Editor
    {
        private static readonly string[] OPTIONS = { "None", "Z Elevation", "Material Swap", "Texture Swap", "Color Swap", "Scale Change" };
        private static readonly string[] PROP_NAMES = { "zElevation", "materialSwap", "textureSwap", "colorChange", "scaleChange" };
        private static readonly string[] PROP_SUFFIX = { "PokeHoverData", "GazeHoverData", "PokePressedData", "GazePressedData" };

        private SerializedProperty[] _pokeHoverProps = new SerializedProperty[OPTIONS.Length];
        private SerializedProperty[] _gazeHoverProps = new SerializedProperty[OPTIONS.Length];
        private SerializedProperty[] _pokePressedProps = new SerializedProperty[OPTIONS.Length];
        private SerializedProperty[] _gazePressedProps = new SerializedProperty[OPTIONS.Length];

        #region Unity Messages
        private void OnEnable()
        {
            FetchAllSerializedProperties();
        }

        public override void OnInspectorGUI()
        {
            DrawGazeProps("_gazeTransition", "Gaze Transition", _gazeHoverProps, _gazePressedProps);
            EditorGUILayout.Space(10);
            DrawGazeProps("_pokeTransition", "Poke Transition", _pokeHoverProps, _pokePressedProps);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGazeProps(string transitionPropName, string displayName, SerializedProperty[] hoverProps, SerializedProperty[] pressedProps)
        {
            SerializedProperty transitionProp = serializedObject.FindProperty(transitionPropName);
            int transition = EditorGUILayout.Popup($"{displayName}: ", transitionProp.intValue, OPTIONS);
            transitionProp.intValue = transition;

            for (int i = 0; i < OPTIONS.Length; i++)
            {
                if (i == 0)
                    continue;

                int optionIndex = i - 1;

                SerializedProperty hoverProp = hoverProps[optionIndex];
                SerializedProperty pressedProp = pressedProps[optionIndex];

                serializedObject.FindProperty(hoverProp.name + ".isActive").boolValue = false;
                serializedObject.FindProperty(pressedProp.name + ".isActive").boolValue = false;

                if (i == transition)
                {
                    serializedObject.FindProperty(hoverProp.name + ".isActive").boolValue = true;
                    serializedObject.FindProperty(pressedProp.name + ".isActive").boolValue = true;

                    EditorGUILayout.PropertyField(hoverProp, new GUIContent("Hover Effect"));
                    EditorGUILayout.PropertyField(pressedProp, new GUIContent("Pressed Effect"));
                }
            }
        }
        #endregion Unity Messages

        #region Private Methods
        private void FetchAllSerializedProperties()
        {
            _pokeHoverProps = GetSerializedProperties(0);
            _gazeHoverProps = GetSerializedProperties(1);
            _pokePressedProps = GetSerializedProperties(2);
            _gazePressedProps = GetSerializedProperties(3);
        }

        private void PrintAllProperties(SerializedProperty[] gazeHoverProps)
        {
            foreach (var prop in gazeHoverProps)
                Debug.Log($"{prop.displayName}");

            Debug.Log($"-----------------------");
        }

        private SerializedProperty[] GetSerializedProperties(int suffixIndex)
        {
            string propSuffix = PROP_SUFFIX[suffixIndex];
            string[] propNames = PROP_NAMES;
            List<SerializedProperty> props = new();
            foreach (var name in propNames)
                props.Add(serializedObject.FindProperty(name + propSuffix));

            return props.ToArray();
        }

        #endregion Private Methods
    }
}
