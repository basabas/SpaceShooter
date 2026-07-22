using Bas.Serialization.Editor;
using Bas.Thoth.Models;
using Bas.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using static Bas.Thoth.Editor.Layouts;

namespace Bas.Thoth.Editor
{
    [CustomEditor(typeof(EntityPrefab))]
    public class EntityPrefabEditor : UnityEditor.Editor
    {
        private static Type baseComponentType { get; } = typeof(Component);

        private SerializedProperty types;
        private SerializedProperty keys;
        private SerializedProperty values;

        private SerializedProperty graphics;

        private List<bool> visible;

        private void OnEnable()
        {
            types = serializedObject.FindProperty("types");
            graphics = serializedObject.FindProperty("graphicsInformation");
            keys = types.FindPropertyRelative("keys");
            values = types.FindPropertyRelative("values");
            visible = new List<bool>();
            for (int i = 0; i < keys.arraySize; i++)
            {
                visible.Add(true);
            }
        }

        public override void OnInspectorGUI()
        {
            int removeIndex = -1;
            for (int i = 0; i < keys.arraySize; i++)
            {
                SerializedProperty component = keys.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(visible[i] ? "⇑" : "⇓", ToggleButtonStyle))
                {
                    visible[i] = !visible[i];
                }
                GUILayout.Space(2);
                Type type = GetSerializableType(component);

                GUILayout.Label(type.Name, CenteredComponentStyle);

                GUILayout.Space(2);
                if (GUILayout.Button("x", DisposeButtonStyle))
                {
                    removeIndex = i;
                }

                EditorGUILayout.EndHorizontal();

                if (visible[i])
                {
                    DrawAnything(values.GetArrayElementAtIndex(i));
                }

                EditorGUILayout.Space(i < keys.arraySize - 1 ? 10 : 0);
            }

            if (removeIndex != -1)
            {
                Type type = GetSerializableType(keys.GetArrayElementAtIndex(removeIndex));
                if (type == typeof(Graphics))
                {
                    Debug.LogError("YEAH, removing graphics, we need to Clear the GraphicsInformation");
                }

                keys.DeleteArrayElementAtIndex(removeIndex);
                values.DeleteArrayElementAtIndex(removeIndex);
                visible.RemoveAt(removeIndex);
            }

            GUILayout.Space(6);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(40);
            if (GUILayout.Button("Add Component", GUILayout.Height(26)))
            {
                SerializableTypePopup.Popup<Component>(AddComponent);
            }
            GUILayout.Space(40);
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }

        private void AddComponent(Type componentType)
        {
            if (HasComponentType(componentType))
            {
                EditorUtility.DisplayDialog("Duplicate detected", $"Component with type \'{componentType.FullName}\' was already on the Entity", "Close");
                return;
            }

            int index = keys.arraySize;
            keys.InsertArrayElementAtIndex(index);
            values.InsertArrayElementAtIndex(index);
            visible.Add(true);

            SetComponent(keys.GetArrayElementAtIndex(index), values.GetArrayElementAtIndex(index), componentType);
        }

        private void SetComponent(SerializedProperty serializableTypeProperty, SerializedProperty anything, Type componentType)
        {
            SetSerializableType(serializableTypeProperty, componentType);

            if (!baseComponentType.IsAssignableFrom(componentType))
            {
                throw new Exception("This should not be possble, wrong type");
            }

            while (!componentType.FullName.StartsWith("Bas.Thoth.Topdown.Component"))
            {
                componentType = componentType.BaseType;
            }

            if (componentType.IsGenericType)
            {
                Type[] genericTypeArguments = componentType.GenericTypeArguments;
                if (genericTypeArguments.Length > 1)
                {
                    Debug.LogWarning("GenericArguments should never be not 1, ignoring the other arguments");
                }
                SetNewAnything(anything, genericTypeArguments[0]);
            }
            else
            {
                SetEmptyAnything(anything);
            }
        }

        private void SetNewAnything(SerializedProperty anything, Type type)
        {
            SetSerializableType(anything.FindPropertyRelative("type"), type);
            object o = Activator.CreateInstance(type);
            anything.FindPropertyRelative("json").stringValue = AnyThing.ToJson(o);
        }

        private void SetEmptyAnything(SerializedProperty anything)
        {
            anything.FindPropertyRelative("json").stringValue = string.Empty;
        }

        private void DrawAnything(SerializedProperty anything)
        {
            SerializedProperty jsonProperty = anything.FindPropertyRelative("json");
            string json = jsonProperty.stringValue;
            if (json == string.Empty)
            {
                return;
            }

            Type type = GetSerializableType(anything.FindPropertyRelative("type"));
            object o = AnyThing.FromJson(json, type);

            if (type == typeof(DrawInformation))
            {
                MemberInfo member = type.GetMember("color")[0];
                GUI.changed |= NonSerializedObjectsHelper.DrawMember<Color>(o, member, value => EditorGUILayout.ColorField(new GUIContent(member.Name), value));
                EditorGUILayout.PropertyField(graphics);
            }
            else
            {
                NonSerializedObjectsHelper.Draw(o);
            }

            if (GUI.changed)
            {
                jsonProperty.stringValue = AnyThing.ToJson(o);
            }
        }

        private bool HasComponentType(Type type)
        {
            for (int i = 0; i < keys.arraySize; i++)
            {
                if (GetSerializableType(keys.GetArrayElementAtIndex(i)) == type)
                {
                    return true;
                }
            }
            return false;
        }

        private void SetSerializableType(SerializedProperty serializableTypeProperty, Type type)
        {
            string assemblyName = type.Assembly.FullName;
            string typeName = type.FullName;

            serializableTypeProperty.FindPropertyRelative("assemblyName").stringValue = assemblyName;
            serializableTypeProperty.FindPropertyRelative("typeName").stringValue = typeName;
        }

        private static Type GetSerializableType(SerializedProperty serializableTypeProperty)
        {
            string assemblyName = serializableTypeProperty.FindPropertyRelative("assemblyName").stringValue;
            string typeName = serializableTypeProperty.FindPropertyRelative("typeName").stringValue;

            Type type;
            try
            {
                if (ReflectionTools.TryGetType(typeName, assemblyName, out type))
                {
                    return type;
                }
            }
            catch (WrongAssemblyException)
            {
            }

            //Hail mary but it helps if namespaces have been updated
            if (ReflectionTools.TryGetType(typeName, out type))
            {
                serializableTypeProperty.FindPropertyRelative("assemblyName").stringValue = type.Assembly.FullName;
                serializableTypeProperty.FindPropertyRelative("typeName").stringValue = typeName;
                return type;
            }

            UnityEngine.Debug.LogError($"Failed to get: assemblyName:{assemblyName}, typeName:{typeName}.");

            throw new Exception("Sumting wong");
        }
    }
}