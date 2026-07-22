using Bas.Utils.Reflection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Bas.Thoth.Editor
{
    public static class NonSerializedObjectsHelper
    {
        public static Rect Draw(Rect rect, object o)
        {
            rect.height = EditorGUIUtility.singleLineHeight;
            IEnumerable<MemberInfo> members = o.GetType().GetMemberInfos().FilterSerializable();

            foreach (MemberInfo member in members)
            {
                Type type = member.GetMemberType();
                switch (type.Name)
                {
                    case "Byte":
                        GUI.changed |= DrawMember<byte>(o, member, value => (byte)EditorGUI.IntField(rect, new GUIContent(member.Name), value));
                        break;
                    case "Boolean":
                        GUI.changed |= DrawMember<bool>(o, member, value => EditorGUI.Toggle(rect, new GUIContent(member.Name), value));
                        break;
                    case "Char":
                        GUI.changed |= DrawMember<char>(o, member, value => EditorGUI.TextField(rect, new GUIContent(member.Name), $"{(char)member.GetValue(o)}").First());
                        break;
                    case "Double":
                        GUI.changed |= DrawMember<double>(o, member, value => EditorGUI.DoubleField(rect, new GUIContent(member.Name), value));
                        break;
                    case "Single":
                        GUI.changed |= DrawMember<float>(o, member, value => EditorGUI.FloatField(rect, new GUIContent(member.Name), value));
                        break;
                    case "Int32":
                        GUI.changed |= DrawMember<int>(o, member, value => EditorGUI.IntField(rect, new GUIContent(member.Name), value));
                        break;
                    case "Int64":
                        GUI.changed |= DrawMember<long>(o, member, value => EditorGUI.LongField(rect, new GUIContent(member.Name), value));
                        break;
                    case "Int16":
                        GUI.changed |= DrawMember<short>(o, member, value => (short)EditorGUI.IntField(rect, new GUIContent(member.Name), value));
                        break;
                    case "UInt32":
                        GUI.changed |= DrawMember<uint>(o, member, value => (uint)EditorGUI.IntField(rect, new GUIContent(member.Name), (int)value));
                        break;
                    case "UInt64":
                        GUI.changed |= DrawMember<ulong>(o, member, value => (ulong)EditorGUI.LongField(rect, new GUIContent(member.Name), (long)value));
                        break;
                    case "UInt16":
                        GUI.changed |= DrawMember<ushort>(o, member, value => (ushort)EditorGUI.IntField(rect, new GUIContent(member.Name), value));
                        break;
                    case "String":
                        GUI.changed |= DrawMember<string>(o, member, value => EditorGUI.TextField(rect, new GUIContent(member.Name), value));
                        break;
                    default:
                        if (typeof(Enum).IsAssignableFrom(type))
                        {
                            GUI.changed |= DrawMember<Enum>(o, member, value => EditorGUI.EnumPopup(rect, new GUIContent(member.Name), value));
                        }
                        else if (typeof(Color) == type)
                        {
                            GUI.changed |= DrawMember<Color>(o, member, value => EditorGUI.ColorField(rect, new GUIContent(member.Name), value));
                        }
                        else
                        {
                            GUI.Label(rect, $"[{type.Name}] {member.Name} is not supported");
                        }
                        break;
                }

                rect.y += EditorGUIUtility.singleLineHeight;
            }

            return rect;
        }

        public static void Draw(object o)
        {
            IEnumerable<MemberInfo> members = o.GetType().GetMemberInfos().FilterSerializable();

            foreach (MemberInfo member in members)
            {
                Type type = member.GetMemberType();
                switch (type.Name)
                {
                    case "Byte":
                        GUI.changed |= DrawMember<byte>(o, member, value => (byte)EditorGUILayout.IntField(new GUIContent(member.Name), value));
                        break;
                    case "Boolean":
                        GUI.changed |= DrawMember<bool>(o, member, value => EditorGUILayout.Toggle(new GUIContent(member.Name), value));
                        break;
                    case "Char":
                        GUI.changed |= DrawMember<char>(o, member, value => EditorGUILayout.TextField(new GUIContent(member.Name), $"{(char)member.GetValue(o)}").First());
                        break;
                    case "Double":
                        GUI.changed |= DrawMember<double>(o, member, value => EditorGUILayout.DoubleField(new GUIContent(member.Name), value));
                        break;
                    case "Single":
                        GUI.changed |= DrawMember<float>(o, member, value => EditorGUILayout.FloatField(new GUIContent(member.Name), value));
                        break;
                    case "Int32":
                        GUI.changed |= DrawMember<int>(o, member, value => EditorGUILayout.IntField(new GUIContent(member.Name), value));
                        break;
                    case "Int64":
                        GUI.changed |= DrawMember<long>(o, member, value => EditorGUILayout.LongField(new GUIContent(member.Name), value));
                        break;
                    case "Int16":
                        GUI.changed |= DrawMember<short>(o, member, value => (short)EditorGUILayout.IntField(new GUIContent(member.Name), value));
                        break;
                    case "UInt32":
                        GUI.changed |= DrawMember<uint>(o, member, value => (uint)EditorGUILayout.IntField(new GUIContent(member.Name), (int)value));
                        break;
                    case "UInt64":
                        GUI.changed |= DrawMember<ulong>(o, member, value => (ulong)EditorGUILayout.LongField(new GUIContent(member.Name), (long)value));
                        break;
                    case "UInt16":
                        GUI.changed |= DrawMember<ushort>(o, member, value => (ushort)EditorGUILayout.IntField(new GUIContent(member.Name), value));
                        break;
                    case "String":
                        GUI.changed |= DrawMember<string>(o, member, value => EditorGUILayout.TextField(new GUIContent(member.Name), value));
                        break;
                    default:
                        if (typeof(Enum).IsAssignableFrom(type))
                        {
                            GUI.changed |= DrawMember<Enum>(o, member, value => EditorGUILayout.EnumPopup(new GUIContent(member.Name), value));
                        }
                        else if (typeof(Color) == type)
                        {
                            GUI.changed |= DrawMember<Color>(o, member, value => EditorGUILayout.ColorField(new GUIContent(member.Name), value));
                        }
                        else
                        {
                            GUILayout.Label($"[{type.FullName}] {member.Name} is not supported");
                        }
                        break;
                }
            }
        }

        public static bool DrawMember<T>(object o, MemberInfo member, Func<T, T> drawMember)
        {
            T startValue = (T)member.GetValue(o);

            T newValue = drawMember(startValue);

            if (newValue != null && !newValue.Equals(startValue))
            {
                member.SetValue(o, newValue);
                return true;
            }

            return false;
        }
    }
}