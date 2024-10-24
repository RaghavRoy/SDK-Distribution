using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.UI;

namespace JioXSDK
{
    [CustomEditor(typeof(ExtendedClick))]
    public class ExtendedClickEditor : SelectableEditor
    {
        SerializedProperty singleClickEnableProp;
        SerializedProperty doubleClickTypeProp;
        SerializedProperty longClickTypeProp;

        protected override void OnEnable()
        {
            base.OnEnable();

            singleClickEnableProp = serializedObject.FindProperty("_enableClick");
            doubleClickTypeProp = serializedObject.FindProperty("_doubleClickType");
            longClickTypeProp = serializedObject.FindProperty("_longClickType");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();

            RenderClickOptions();
            RenderDoubleClickOptions();
            RenderLongClickOptions();

            serializedObject.ApplyModifiedProperties();
        }

        private void RenderClickOptions()
        {
            bool isClickEnabled = singleClickEnableProp.boolValue;

            EditorGUILayout.Space(15);
            EditorGUILayout.PropertyField(singleClickEnableProp);

            if (!isClickEnabled)
                return;

            SerializedProperty timeIntervalProp = serializedObject.FindProperty("_pressTime");
            EditorGUILayout.PropertyField(timeIntervalProp);

            SerializedProperty eventPro = serializedObject.FindProperty("_clickPerformed");
            EditorGUILayout.PropertyField(eventPro);

        }

        private void RenderDoubleClickOptions()
        {
            if (singleClickEnableProp.boolValue)
                EditorGUILayout.Space(15);

            EditorGUILayout.PropertyField(doubleClickTypeProp);
            DoubleClickType propValue = doubleClickTypeProp.GetEnumValue<DoubleClickType>();

            if (propValue == DoubleClickType.None)
                return;

            SerializedProperty timeIntervalProp = serializedObject.FindProperty("_maxSpaceTime");
            EditorGUILayout.PropertyField(timeIntervalProp);

            SerializedProperty eventProp = serializedObject.FindProperty("_doubleClickPerformed");
            EditorGUILayout.PropertyField(eventProp);

        }

        private void RenderLongClickOptions()
        {
            if (doubleClickTypeProp.GetEnumValue<DoubleClickType>() != DoubleClickType.None)
                EditorGUILayout.Space(15);

            EditorGUILayout.PropertyField(longClickTypeProp);
            LongClickType propValue = longClickTypeProp.GetEnumValue<LongClickType>();

            if (propValue == LongClickType.None)
                return;

            SerializedProperty timeIntervalProp = serializedObject.FindProperty("_continuosInterval");
            EditorGUILayout.PropertyField(timeIntervalProp);

            SerializedProperty eventProp = serializedObject.FindProperty("_longClickPerformed");
            EditorGUILayout.PropertyField(eventProp);
        }
    }
}
