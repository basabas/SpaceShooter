using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Bas.Utils.Reflection.Extensions
{
    public static class ReflectionExtensions
    {
        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            return memberInfo switch
            {
                FieldInfo fieldInfo => fieldInfo.FieldType,
                PropertyInfo propertyInfo => propertyInfo.PropertyType,
                _ => null
            };
        }

        public static object GetValue(this MemberInfo memberInfo, object o) => memberInfo switch
        {
            FieldInfo fieldInfo => fieldInfo.GetValue(o),
            PropertyInfo propertyInfo => propertyInfo.GetValue(o),
            _ => default
        };

        public static void SetValue(this MemberInfo memberInfo, object o, object value)
        {
            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                    fieldInfo.SetValue(o, value);
                    break;
                case PropertyInfo propertyInfo:
                    propertyInfo.SetValue(o, value);
                    break;
            }
        }

        public static IEnumerable<MemberInfo> GetMemberInfos(this Type type)
        {
            MemberInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            MemberInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return fields.Where(f => !f.Name.Contains("Backing")).Concat(properties);
        }

        public static IEnumerable<MemberInfo> FilterSerializable(this IEnumerable<MemberInfo> members)
        {
            return members.Where(member =>
            {
                if (member is PropertyInfo property)
                {
                    return property.GetGetMethod()?.IsPublic == true;
                }

                if (member is FieldInfo field)
                {
                    return field.IsPublic || field.GetCustomAttribute<SerializeField>() != null;
                }

                return false;
            });
        }

        public static bool IsPublic(this MemberInfo memberInfo) => memberInfo switch
        {
            FieldInfo fieldInfo => fieldInfo.IsPublic,
            PropertyInfo propertyInfo => propertyInfo.GetAccessors().Any(a => a.IsPublic),
            _ => false
        };

        public static bool HasAttribute(this MemberInfo memberInfo, params Type[] attributeTypes)
        {
            return memberInfo.CustomAttributes.Any(attribute => attributeTypes.Contains(attribute.AttributeType));
        }
    }
}