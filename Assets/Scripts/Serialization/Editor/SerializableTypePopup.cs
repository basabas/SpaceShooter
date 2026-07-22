using Bas.Utils.Reflection;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Bas.Serialization.Editor
{
    public static class SerializableTypePopup
    {
        public static bool Popup<T>(Action<Type> onClickAction)
        {
            Type[] types = ReflectionTools.GetDerivedTypes(typeof(T)).ToArray();
            var _typeNames = types.Select(type => type.FullName).ToArray();
            var _popupValues = types.Select(type => type.FullName?.Replace(".", "/")).ToArray();

            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < types.Length; i++)
            {
                Type t = types[i];
                menu.AddItem(new GUIContent(_popupValues[i]), false, () => onClickAction(t));
            }
            menu.ShowAsContext();

            return true;
        }
    }
}