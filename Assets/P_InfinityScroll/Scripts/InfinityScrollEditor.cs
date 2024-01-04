#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

namespace MyInfinityScrollUI
{
    [CustomEditor(typeof(InfinityScroll), true)]
    [CanEditMultipleObjects]
    public class InfinityScrollEditor : ScrollRectEditor
    {
        private SerializedProperty contentInactiveMaxPosY;
        private SerializedProperty contentsInactiveMinPosY;

        private SerializedProperty imageInactiveTop;
        private SerializedProperty imageInactiveBot;

        private SerializedProperty spacing;
        private SerializedProperty scrollItemSize;

        private SerializedProperty horizontal;
        private SerializedProperty vertical;

        private SerializedProperty itemPrefab;

        protected override void OnEnable()
        {
            base.OnEnable();

            contentInactiveMaxPosY = serializedObject.FindProperty("scrollItemVisibleExtendMaxValue");
            contentsInactiveMinPosY = serializedObject.FindProperty("scrollItemVisibleExtendMinValue");

            imageInactiveTop = serializedObject.FindProperty("image_InactiveMaxPos");
            imageInactiveBot = serializedObject.FindProperty("image_InactiveMinPos");

            spacing = serializedObject.FindProperty("spacing");
            scrollItemSize = serializedObject.FindProperty("scrollItemSize");

            horizontal = serializedObject.FindProperty("m_Horizontal");
            vertical = serializedObject.FindProperty("m_Vertical");

            itemPrefab = serializedObject.FindProperty("scrollItemPrefab");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(contentInactiveMaxPosY);
            EditorGUILayout.PropertyField(contentsInactiveMinPosY);

            EditorGUILayout.PropertyField(imageInactiveTop);
            EditorGUILayout.PropertyField(imageInactiveBot);

            EditorGUILayout.PropertyField(spacing);
            EditorGUILayout.PropertyField(scrollItemSize);

            vertical.boolValue = !horizontal.boolValue;

            EditorGUILayout.PropertyField(itemPrefab);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif