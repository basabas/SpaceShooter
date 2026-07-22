using UnityEditor;
using UnityEngine;

namespace Bas.Thoth.Editor
{
    [CustomPropertyDrawer(typeof(GraphicsInformation))]
    public class GraphicsInformationDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("fps"));
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("width"));
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("height"));
            position.y += EditorGUIUtility.singleLineHeight;
            SerializedProperty frames = property.FindPropertyRelative("frames");
            position.height = frames.arraySize * EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, frames);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty frames = property.FindPropertyRelative("frames");
            return (4 + (frames.isExpanded ? (Math.Max(1, frames.arraySize) * 1.1f + 1.8f) : 0)) * EditorGUIUtility.singleLineHeight;
        }
    }
}
