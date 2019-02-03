using System;
using System.Reflection;
using UnityEditor;

namespace Fullscreen {
    /// <summary>
    /// Class containing method extensions for getting private and internal members.
    /// </summary>
    public static class ReflectionUtility {

        public const BindingFlags FULL_BINDING = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public static Type FindClass(string name) { return typeof(Editor).Assembly.GetType(name, false, true); }

        public static FieldInfo FindField(this Type type, string fieldName) { return type.GetField(fieldName, FULL_BINDING); }

        public static PropertyInfo FindProperty(this Type type, string propertyName) { return type.GetProperty(propertyName, FULL_BINDING); }

        public static MethodInfo FindMethod(this Type type, string methodName) { return type.GetMethod(methodName, FULL_BINDING); }

        public static T GetFieldValue<T>(this Type type, string fieldName) { return (T)type.FindField(fieldName).GetValue(null); }

        public static T GetFieldValue<T>(this object obj, string fieldName) { return (T)obj.GetType().FindField(fieldName).GetValue(obj); }

        public static void SetFieldValue(this Type type, string fieldName, object value) { type.FindField(fieldName).SetValue(null, value); }

        public static void SetFieldValue(this object obj, string fieldName, object value) { obj.GetType().FindField(fieldName).SetValue(obj, value); }

        public static T GetPropertyValue<T>(this Type type, string propertyName) { return (T)type.FindProperty(propertyName).GetValue(null, null); }

        public static T GetPropertyValue<T>(this object obj, string propertyName) { return (T)obj.GetType().FindProperty(propertyName).GetValue(obj, null); }

        public static void SetPropertyValue(this Type type, string propertyName, object value) { type.FindProperty(propertyName).SetValue(null, value, null); }

        public static void SetPropertyValue(this object obj, string propertyName, object value) { obj.GetType().FindProperty(propertyName).SetValue(obj, value, null); }

        public static T InvokeMethod<T>(this Type type, string methodName, params object[] args) { return (T)type.FindMethod(methodName).Invoke(null, args); }

        public static T InvokeMethod<T>(this object obj, string methodName, params object[] args) { return (T)obj.GetType().FindMethod(methodName).Invoke(obj, args); }

        public static void InvokeMethod(this Type type, string methodName, params object[] args) { type.FindMethod(methodName).Invoke(null, args); }

        public static void InvokeMethod(this object obj, string methodName, params object[] args) { obj.GetType().FindMethod(methodName).Invoke(obj, args); }

    }
}