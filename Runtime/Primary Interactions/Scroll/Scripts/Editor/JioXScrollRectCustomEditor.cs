using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace JioXSDK
{
    [CustomEditor(typeof(JioXScrollRect))]
    [CanEditMultipleObjects]
    public class JioXScrollRectCustomEditor : Editor
    {
        // Serialized properties for your custom fields
        SerializedProperty handScrollSensitivity;
        SerializedProperty JioXScrollBar;
        SerializedProperty velocityDecayRate;
        SerializedProperty minVelocityThreshold;
        SerializedProperty scrollMultiplier;
        
        SerializedProperty _leftControllerTracking;
        SerializedProperty _rightControllerTracking;
        SerializedProperty movementThreshold;

        // Properties from ScrollRect class
        SerializedProperty horizontal;
        SerializedProperty vertical;
        SerializedProperty movementType;
        SerializedProperty inertia;
        SerializedProperty scrollSensitivity;
        SerializedProperty content;
        SerializedProperty viewport;
        SerializedProperty horizontalScrollbar;
        SerializedProperty verticalScrollbar;
        SerializedProperty horizontalScrollbarVisibility;
        SerializedProperty verticalScrollbarVisibility;
        SerializedProperty horizontalScrollbarSpacing;
        SerializedProperty verticalScrollbarSpacing;
        SerializedProperty elasticity;
        SerializedProperty decelerationRate;
        SerializedProperty scrollEvent; // onValueChanged event

        private void OnEnable()
        {
            // Get references to custom fields
            handScrollSensitivity = serializedObject.FindProperty("handScrollSensitivity");
            JioXScrollBar = serializedObject.FindProperty("JioXScrollBar");
            velocityDecayRate = serializedObject.FindProperty("velocityDecayRate");
            minVelocityThreshold = serializedObject.FindProperty("minVelocityThreshold");
            scrollMultiplier = serializedObject.FindProperty("scrollMultiplier");
            _leftControllerTracking = serializedObject.FindProperty("_leftControllerTracking");
            _rightControllerTracking = serializedObject.FindProperty("_rightControllerTracking");
            movementThreshold = serializedObject.FindProperty("movementThreshold");

            // Get references to inherited fields from ScrollRect
            horizontal = serializedObject.FindProperty("m_Horizontal");
            vertical = serializedObject.FindProperty("m_Vertical");
            movementType = serializedObject.FindProperty("m_MovementType");
            inertia = serializedObject.FindProperty("m_Inertia");
            scrollSensitivity = serializedObject.FindProperty("m_ScrollSensitivity");
            content = serializedObject.FindProperty("m_Content");
            viewport = serializedObject.FindProperty("m_Viewport");
            horizontalScrollbar = serializedObject.FindProperty("m_HorizontalScrollbar");
            verticalScrollbar = serializedObject.FindProperty("m_VerticalScrollbar");
            horizontalScrollbarVisibility = serializedObject.FindProperty("m_HorizontalScrollbarVisibility");
            verticalScrollbarVisibility = serializedObject.FindProperty("m_VerticalScrollbarVisibility");
            horizontalScrollbarSpacing = serializedObject.FindProperty("m_HorizontalScrollbarSpacing");
            verticalScrollbarSpacing = serializedObject.FindProperty("m_VerticalScrollbarSpacing");
            elasticity = serializedObject.FindProperty("m_Elasticity");
            decelerationRate = serializedObject.FindProperty("m_DecelerationRate");
            scrollEvent = serializedObject.FindProperty("m_OnValueChanged");
        }

        public override void OnInspectorGUI()
        {
            // Update the serialized object
            serializedObject.Update();

            // Display default ScrollRect fields
            EditorGUILayout.LabelField("ScrollRect Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(content, new GUIContent("Content"));
            EditorGUILayout.PropertyField(viewport, new GUIContent("Viewport"));
            EditorGUILayout.PropertyField(horizontal, new GUIContent("Horizontal"));
            EditorGUILayout.PropertyField(vertical, new GUIContent("Vertical"));
            EditorGUILayout.PropertyField(movementType, new GUIContent("Movement Type"));
            EditorGUILayout.PropertyField(elasticity, new GUIContent("Elasticity"));
            EditorGUILayout.PropertyField(inertia, new GUIContent("Inertia"));
            EditorGUILayout.PropertyField(decelerationRate, new GUIContent("Deceleration Rate"));
            EditorGUILayout.PropertyField(scrollSensitivity, new GUIContent("Scroll Sensitivity"));
            EditorGUILayout.Space(); // Add some spacing

            // Display scrollbar-related fields
            EditorGUILayout.LabelField("Scrollbars", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(horizontalScrollbar, new GUIContent("Horizontal Scrollbar"));
            EditorGUILayout.PropertyField(verticalScrollbar, new GUIContent("Vertical Scrollbar"));
            EditorGUILayout.PropertyField(horizontalScrollbarVisibility, new GUIContent("Horizontal Scrollbar Visibility"));
            EditorGUILayout.PropertyField(verticalScrollbarVisibility, new GUIContent("Vertical Scrollbar Visibility"));
            EditorGUILayout.PropertyField(horizontalScrollbarSpacing, new GUIContent("Horizontal Scrollbar Spacing"));
            EditorGUILayout.PropertyField(verticalScrollbarSpacing, new GUIContent("Vertical Scrollbar Spacing"));
            EditorGUILayout.Space(); // Add some spacing

            // Display the OnValueChanged event
            EditorGUILayout.PropertyField(scrollEvent, new GUIContent("On Value Changed"));

            EditorGUILayout.Space(); // Add some spacing

            // Display custom fields
            EditorGUILayout.LabelField("JioXScrollRect Custom Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(handScrollSensitivity, new GUIContent("Hand Scroll Sensitivity"));
            EditorGUILayout.PropertyField(JioXScrollBar, new GUIContent("JioX Scroll Bar"));
            EditorGUILayout.PropertyField(velocityDecayRate, new GUIContent("Velocity Decay Rate"));
            EditorGUILayout.PropertyField(minVelocityThreshold, new GUIContent("Min Velocity Threshold"));
            EditorGUILayout.PropertyField(scrollMultiplier, new GUIContent("Scroll Multiplier"));
            EditorGUILayout.PropertyField(_leftControllerTracking, new GUIContent("Left Controller Tracking"));
            EditorGUILayout.PropertyField(_rightControllerTracking, new GUIContent("Right Controller Tracking"));
            EditorGUILayout.PropertyField(movementThreshold, new GUIContent("movementThreshold"));

            // Apply changes to the serialized object
            serializedObject.ApplyModifiedProperties();
        }
    }
}
