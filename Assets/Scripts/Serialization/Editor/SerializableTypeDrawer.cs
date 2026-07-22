using Bas.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Bas.Serialization.Editor
{
    [CustomPropertyDrawer(typeof(SerializableType))]
    [CustomPropertyDrawer(typeof(SerializableTypeAttribute), true)]
    public class SerializableTypeDrawer : PropertyDrawer
    {
        private string[] _typeNames;
        private string[] _assemblyNames;
        private string[] _popupValues;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ValidatePopupValues())
            {
                SerializedProperty type = property.FindPropertyRelative("typeName");
                SerializedProperty assembly = property.FindPropertyRelative("assemblyName");
                int index = string.IsNullOrWhiteSpace(type.stringValue) ? -1 : Array.IndexOf(_typeNames, type.stringValue);

                int newIndex = attribute is SerializableTypeAttribute serializableTypeAttribute && serializableTypeAttribute.Options.HasFlag(SerializableTypeOptions.DrawName)
                        ? EditorGUI.Popup(position, property.displayName, Math.Max(0, index), _popupValues)
                        : EditorGUI.Popup(position, Math.Max(0, index), _popupValues);
                if (newIndex != index)
                {
                    type.stringValue = _typeNames[newIndex];
                    assembly.stringValue = _assemblyNames[newIndex];
                }
            }
        }

        private bool ValidatePopupValues()
        {
            if (_popupValues == null)
            {
                IEnumerable<Type> types;
                if (attribute is SerializableTypeAttribute serializableTypeAttribute)
                {
                    types = serializableTypeAttribute.FilteredTypes;

                    if (serializableTypeAttribute.Options.HasFlag(SerializableTypeOptions.IncludePrimitives))
                    {
                        types = types.Concat(typeof(int).Assembly.GetTypes().Where(t => t.IsPrimitive));
                    }
                }
                else
                {
                    types = ReflectionTools
                        .GetDerivedTypes(ReflectionTools.GetUnityProjectAssemblies(), typeof(object), true, true)
                        .Where(type => type.FullName != null && !type.FullName.Contains("+<>c"));
                    types = types.Concat(typeof(int).Assembly.GetTypes().Where(t => t.IsPrimitive));
                }

                IEnumerable<Type> typesArray = types as Type[] ?? types.ToArray();
                _typeNames = typesArray.Select(type => type.FullName).ToArray();
                _assemblyNames = typesArray.Select(type => type.Assembly.FullName).ToArray();
                _popupValues = typesArray.Select(type => type.FullName?.Replace(".", "/")).ToArray();
            }

            return _popupValues != null;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}