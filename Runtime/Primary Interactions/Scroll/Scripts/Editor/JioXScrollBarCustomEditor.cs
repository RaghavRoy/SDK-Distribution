using UnityEditor;
using UnityEngine;

namespace JioXSDK
{
    [CustomEditor(typeof(JioXScrollBar))]
    public class JioXScrollBarCustomEditor : Editor
    {
        // Custom properties
        SerializedProperty scrollRectProp;
        SerializedProperty handScrollSensitivityProp;
        SerializedProperty scrollMultiplierProp;
        SerializedProperty leftControllerTrackingProp;
        SerializedProperty rightControllerTrackingProp;

        void OnEnable()
        {
            // Link the serialized properties to the fields in your script
            scrollRectProp = serializedObject.FindProperty("scrollRect");
            handScrollSensitivityProp = serializedObject.FindProperty("handScrollSensitivity");
            scrollMultiplierProp = serializedObject.FindProperty("scrollMultiplier");
            leftControllerTrackingProp = serializedObject.FindProperty("_leftControllerTracking");
            rightControllerTrackingProp = serializedObject.FindProperty("_rightControllerTracking");
        }

        public override void OnInspectorGUI()
        {
            // Start updating the serialized object
            serializedObject.Update();

            // Draw Unity's default Scrollbar properties
            EditorGUILayout.LabelField("Default Scrollbar Settings", EditorStyles.boldLabel);
            DrawDefaultInspector();  // This draws all the default Scrollbar properties

            // Custom properties section
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Custom Scrollbar Settings", EditorStyles.boldLabel);

            // Section to display the custom scrollRect
            EditorGUILayout.PropertyField(scrollRectProp, new GUIContent("Scroll Rect"));

            // Section to display hand sensitivity and scroll multiplier
            EditorGUILayout.PropertyField(handScrollSensitivityProp, new GUIContent("Hand Scroll Sensitivity"));
            EditorGUILayout.PropertyField(scrollMultiplierProp, new GUIContent("Scroll Multiplier"));

            // Section for controller and gaze settings
            EditorGUILayout.PropertyField(leftControllerTrackingProp, new GUIContent("Left Controller Tracking"));
            EditorGUILayout.PropertyField(rightControllerTrackingProp, new GUIContent("Right Controller Tracking"));

            // Apply changes to the serialized object
            serializedObject.ApplyModifiedProperties();
        }
    }
}
