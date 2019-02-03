using System;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Fullscreen {

    /// <summary>
    /// Helper class for suppressing unity logs when calling a method that may show unwanted logs.
    /// </summary>
    internal class SuppressLog : IDisposable {

        private readonly bool lastState;

        internal static ILogger Logger {
            get {
#if UNITY_2017_1_OR_NEWER
                return Debug.unityLogger;
#else
                return Debug.logger;
#endif
            }
        }

        public SuppressLog() {
            lastState = Logger.logEnabled;
            Logger.logEnabled = false;
        }

        public void Dispose() {
            Logger.logEnabled = lastState;
        }

    }

    /// <summary>
    /// Miscellaneous utilities for Fullscreen Editor.
    /// </summary>
    [InitializeOnLoad]
    public static class FullscreenUtility {

        public const string UNITY_LAYOUT_PATH = "Library/CurrentLayout.dwlt";
        public const string TEMP_LAYOUT_PATH = "Library/FullscreenTempLayout.dwlt";
        public const string SAFE_LAYOUT_PATH = "Library/FullscreenSafeLayout.dwlt";

        private static readonly byte[] fullscreenIconSource = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 0, 46, 0, 0, 0, 46, 8, 6, 0, 0, 0, 87, 185, 43, 55, 0, 0, 0, 6, 98, 75, 71, 68, 0, 255, 0, 255, 0, 255, 160, 189, 167, 147, 0, 0, 0, 9, 112, 72, 89, 115, 0, 0, 11, 19, 0, 0, 11, 19, 1, 0, 154, 156, 24, 0, 0, 0, 7, 116, 73, 77, 69, 7, 225, 10, 26, 18, 5, 32, 61, 99, 110, 78, 0, 0, 4, 79, 73, 68, 65, 84, 104, 222, 237, 152, 91, 136, 85, 85, 24, 199, 127, 235, 140, 215, 49, 209, 188, 219, 100, 88, 144, 93, 30, 42, 162, 130, 34, 173, 193, 94, 42, 81, 66, 104, 178, 146, 16, 4, 161, 32, 122, 43, 170, 7, 9, 137, 232, 162, 62, 42, 61, 68, 133, 15, 82, 80, 82, 42, 130, 209, 75, 150, 73, 118, 49, 187, 48, 74, 161, 99, 205, 152, 78, 99, 102, 142, 195, 204, 57, 191, 30, 90, 7, 54, 155, 189, 207, 57, 179, 207, 116, 33, 206, 7, 155, 25, 206, 119, 89, 255, 189, 246, 119, 135, 22, 181, 168, 69, 255, 73, 10, 163, 85, 80, 39, 0, 151, 0, 179, 129, 25, 192, 183, 33, 132, 158, 81, 232, 47, 4, 22, 1, 253, 192, 105, 224, 100, 8, 225, 194, 152, 0, 87, 67, 8, 65, 181, 29, 184, 18, 184, 1, 184, 28, 152, 15, 204, 3, 22, 196, 255, 39, 3, 221, 192, 67, 33, 132, 35, 13, 128, 190, 25, 120, 21, 88, 8, 252, 1, 244, 198, 231, 23, 224, 39, 224, 8, 240, 5, 112, 36, 132, 48, 84, 232, 83, 168, 143, 168, 39, 212, 1, 117, 200, 218, 212, 173, 94, 87, 199, 222, 29, 106, 111, 29, 59, 67, 241, 188, 110, 245, 166, 162, 192, 31, 83, 43, 54, 78, 39, 212, 206, 28, 91, 203, 212, 193, 81, 216, 58, 167, 46, 174, 133, 175, 84, 11, 59, 80, 169, 193, 175, 242, 134, 129, 65, 96, 10, 176, 81, 157, 158, 2, 61, 13, 120, 46, 186, 198, 32, 48, 146, 210, 207, 179, 109, 45, 224, 227, 154, 8, 236, 3, 192, 182, 232, 163, 199, 128, 115, 241, 37, 210, 129, 54, 4, 172, 0, 218, 129, 105, 192, 21, 192, 28, 96, 29, 112, 109, 209, 195, 155, 1, 190, 0, 248, 46, 132, 240, 65, 205, 232, 255, 43, 99, 36, 179, 206, 1, 117, 57, 208, 209, 76, 58, 44, 53, 161, 219, 1, 236, 82, 215, 141, 34, 110, 74, 234, 51, 192, 142, 120, 251, 255, 8, 240, 44, 159, 155, 0, 108, 81, 159, 85, 199, 213, 1, 221, 14, 188, 2, 108, 168, 19, 51, 99, 14, 252, 60, 240, 98, 204, 183, 105, 90, 15, 188, 20, 139, 83, 30, 232, 173, 192, 227, 25, 236, 175, 128, 205, 163, 253, 250, 181, 132, 219, 50, 226, 225, 67, 117, 73, 12, 198, 180, 236, 19, 192, 235, 234, 212, 20, 232, 233, 192, 94, 224, 225, 140, 243, 62, 5, 238, 6, 14, 53, 112, 126, 195, 254, 120, 111, 42, 247, 94, 136, 65, 133, 186, 72, 221, 175, 150, 19, 252, 106, 206, 63, 88, 189, 121, 117, 150, 218, 147, 83, 104, 182, 171, 179, 163, 92, 87, 170, 102, 244, 171, 139, 10, 221, 120, 8, 97, 39, 208, 5, 156, 72, 248, 184, 145, 215, 13, 44, 7, 182, 3, 229, 84, 251, 112, 52, 241, 219, 80, 116, 133, 100, 140, 252, 6, 188, 0, 172, 9, 33, 156, 74, 224, 168, 234, 31, 3, 238, 143, 103, 20, 39, 245, 122, 245, 203, 120, 19, 203, 82, 188, 41, 49, 48, 71, 34, 127, 91, 58, 72, 213, 201, 234, 174, 200, 239, 85, 239, 83, 67, 74, 166, 43, 242, 247, 169, 87, 143, 89, 11, 169, 118, 168, 239, 87, 93, 37, 197, 27, 175, 62, 170, 190, 173, 206, 202, 209, 95, 160, 190, 166, 222, 89, 163, 189, 120, 43, 79, 127, 44, 94, 96, 210, 223, 100, 119, 114, 107, 194, 104, 209, 255, 149, 42, 149, 74, 161, 66, 83, 106, 32, 112, 218, 212, 7, 212, 29, 89, 253, 136, 58, 67, 221, 160, 174, 82, 75, 57, 54, 186, 212, 221, 213, 130, 147, 170, 23, 243, 212, 3, 89, 25, 171, 153, 104, 159, 160, 62, 21, 115, 236, 123, 106, 91, 122, 240, 85, 223, 137, 252, 62, 245, 182, 12, 27, 43, 19, 21, 113, 183, 122, 85, 138, 63, 51, 214, 137, 97, 117, 141, 58, 190, 41, 208, 229, 114, 121, 174, 250, 102, 162, 172, 239, 72, 2, 87, 151, 168, 159, 167, 74, 121, 143, 122, 75, 66, 102, 149, 122, 54, 37, 115, 40, 57, 150, 197, 182, 224, 96, 228, 157, 87, 159, 87, 47, 42, 122, 211, 65, 125, 35, 213, 63, 188, 91, 5, 174, 62, 168, 254, 156, 51, 47, 174, 72, 216, 121, 58, 71, 102, 64, 93, 157, 1, 188, 74, 91, 139, 2, 111, 87, 119, 166, 140, 237, 81, 39, 86, 42, 149, 39, 115, 166, 254, 97, 245, 158, 12, 91, 107, 115, 134, 238, 225, 88, 113, 47, 206, 0, 254, 89, 225, 10, 25, 125, 58, 73, 251, 212, 77, 57, 55, 248, 163, 186, 180, 78, 167, 121, 42, 71, 119, 83, 162, 23, 170, 210, 254, 177, 4, 158, 71, 223, 168, 183, 54, 96, 243, 118, 245, 135, 28, 27, 35, 163, 5, 94, 106, 50, 241, 124, 12, 44, 13, 33, 124, 82, 119, 101, 22, 194, 71, 177, 21, 62, 60, 22, 67, 67, 51, 192, 183, 3, 157, 33, 132, 190, 134, 247, 125, 33, 28, 6, 58, 129, 61, 205, 166, 234, 162, 235, 137, 223, 227, 202, 97, 165, 122, 28, 248, 53, 206, 164, 3, 33, 132, 179, 201, 236, 148, 216, 49, 78, 2, 102, 198, 237, 192, 81, 224, 174, 194, 227, 89, 19, 62, 94, 142, 153, 98, 36, 142, 116, 253, 113, 5, 183, 34, 195, 214, 102, 245, 164, 122, 38, 202, 150, 163, 94, 173, 245, 94, 97, 31, 15, 117, 220, 168, 58, 106, 181, 1, 19, 227, 186, 121, 47, 176, 47, 67, 118, 99, 252, 125, 90, 148, 45, 69, 189, 208, 204, 250, 187, 84, 99, 199, 113, 26, 56, 19, 247, 125, 245, 246, 45, 91, 128, 181, 33, 132, 211, 25, 126, 125, 28, 88, 29, 215, 203, 245, 236, 156, 7, 6, 226, 83, 108, 177, 31, 27, 166, 203, 128, 27, 129, 107, 128, 75, 129, 185, 113, 169, 63, 31, 152, 26, 111, 110, 83, 8, 97, 125, 3, 238, 55, 14, 120, 57, 190, 68, 117, 104, 238, 137, 195, 248, 169, 248, 247, 123, 224, 235, 16, 194, 177, 234, 142, 190, 240, 39, 201, 56, 124, 78, 124, 102, 2, 67, 49, 205, 53, 170, 63, 30, 88, 28, 191, 104, 63, 208, 151, 152, 244, 91, 212, 162, 22, 181, 232, 95, 164, 63, 1, 103, 50, 107, 61, 123, 183, 171, 65, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66, 96, 130 };
        private static readonly byte[] fullscreenIconSourceSmallDarkSkin = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 0, 15, 0, 0, 0, 15, 8, 6, 0, 0, 0, 59, 214, 149, 74, 0, 0, 0, 6, 98, 75, 71, 68, 0, 255, 0, 255, 0, 255, 160, 189, 167, 147, 0, 0, 0, 9, 112, 72, 89, 115, 0, 0, 11, 19, 0, 0, 11, 19, 1, 0, 154, 156, 24, 0, 0, 0, 7, 116, 73, 77, 69, 7, 225, 10, 27, 13, 17, 7, 25, 1, 143, 88, 0, 0, 1, 156, 73, 68, 65, 84, 40, 207, 165, 210, 79, 136, 14, 96, 16, 199, 241, 207, 251, 238, 187, 254, 164, 197, 150, 172, 148, 180, 114, 147, 11, 69, 28, 184, 200, 58, 140, 131, 184, 104, 215, 193, 197, 69, 14, 14, 14, 136, 139, 118, 15, 194, 5, 7, 78, 74, 78, 82, 180, 147, 11, 185, 40, 185, 109, 56, 8, 201, 170, 245, 103, 195, 214, 218, 20, 251, 7, 151, 217, 188, 119, 83, 79, 207, 51, 211, 111, 166, 103, 230, 59, 141, 204, 236, 197, 14, 172, 197, 58, 172, 194, 249, 136, 120, 4, 153, 217, 192, 110, 28, 199, 39, 140, 226, 29, 30, 180, 176, 15, 23, 253, 179, 95, 88, 154, 153, 7, 34, 226, 35, 86, 227, 28, 54, 98, 81, 155, 174, 191, 137, 153, 182, 192, 12, 6, 176, 23, 95, 43, 246, 5, 123, 112, 4, 179, 109, 218, 185, 38, 26, 229, 124, 67, 7, 174, 97, 75, 68, 76, 67, 221, 59, 113, 5, 77, 76, 204, 103, 183, 48, 93, 239, 65, 116, 227, 12, 134, 51, 179, 31, 183, 113, 8, 55, 74, 115, 2, 203, 113, 26, 191, 155, 200, 58, 93, 17, 113, 22, 167, 240, 25, 91, 35, 226, 55, 182, 227, 13, 142, 70, 196, 5, 44, 198, 29, 60, 110, 69, 196, 88, 102, 14, 84, 16, 206, 227, 86, 21, 128, 99, 88, 137, 15, 229, 95, 194, 84, 68, 124, 247, 63, 214, 40, 150, 61, 152, 140, 136, 159, 153, 185, 0, 43, 48, 30, 17, 115, 153, 217, 133, 101, 17, 49, 86, 218, 13, 120, 21, 17, 179, 141, 204, 220, 140, 235, 56, 140, 151, 24, 194, 254, 66, 243, 4, 247, 176, 6, 103, 34, 226, 118, 102, 206, 212, 192, 174, 182, 138, 235, 166, 154, 252, 48, 250, 170, 191, 241, 226, 250, 12, 187, 112, 43, 51, 39, 11, 233, 32, 158, 182, 138, 45, 60, 44, 84, 163, 232, 139, 136, 215, 245, 205, 147, 248, 129, 179, 184, 91, 172, 255, 160, 179, 217, 214, 127, 55, 166, 106, 143, 39, 50, 115, 190, 104, 87, 17, 184, 89, 235, 185, 112, 62, 161, 217, 238, 148, 240, 121, 225, 88, 82, 177, 109, 120, 81, 115, 152, 223, 198, 78, 116, 180, 112, 25, 35, 232, 197, 250, 106, 99, 168, 141, 227, 125, 244, 224, 32, 222, 226, 125, 45, 205, 200, 95, 236, 203, 133, 198, 101, 201, 25, 7, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66, 96, 130 };
        private static readonly byte[] fullscreenIconSourceSmallWhiteSkin = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 0, 15, 0, 0, 0, 15, 8, 6, 0, 0, 0, 59, 214, 149, 74, 0, 0, 0, 6, 98, 75, 71, 68, 0, 255, 0, 255, 0, 255, 160, 189, 167, 147, 0, 0, 0, 9, 112, 72, 89, 115, 0, 0, 11, 19, 0, 0, 11, 19, 1, 0, 154, 156, 24, 0, 0, 0, 7, 116, 73, 77, 69, 7, 225, 10, 27, 13, 21, 20, 249, 211, 11, 130, 0, 0, 1, 170, 73, 68, 65, 84, 40, 207, 165, 210, 63, 72, 215, 97, 16, 199, 241, 215, 239, 231, 207, 254, 16, 86, 66, 100, 180, 132, 209, 22, 45, 125, 161, 168, 161, 150, 200, 150, 134, 168, 37, 180, 245, 150, 112, 104, 104, 168, 200, 37, 116, 16, 107, 169, 165, 155, 130, 16, 130, 8, 2, 163, 165, 104, 169, 161, 69, 236, 31, 65, 69, 100, 96, 166, 84, 130, 73, 84, 254, 171, 229, 145, 126, 123, 7, 15, 207, 115, 199, 231, 142, 187, 231, 125, 181, 136, 232, 196, 1, 108, 195, 118, 108, 193, 96, 102, 62, 130, 136, 168, 225, 48, 206, 224, 51, 198, 241, 1, 15, 26, 56, 134, 203, 254, 217, 111, 172, 143, 136, 19, 153, 57, 137, 173, 184, 132, 93, 88, 211, 164, 235, 174, 99, 161, 41, 176, 128, 30, 28, 197, 215, 18, 251, 130, 35, 8, 44, 54, 105, 151, 234, 168, 21, 231, 27, 90, 112, 29, 123, 50, 115, 30, 202, 125, 16, 215, 80, 199, 204, 74, 118, 3, 243, 229, 221, 143, 118, 92, 196, 72, 68, 116, 227, 54, 78, 225, 70, 209, 156, 197, 70, 92, 192, 114, 29, 247, 202, 105, 203, 204, 62, 156, 199, 20, 246, 102, 230, 50, 246, 227, 29, 78, 103, 230, 16, 214, 226, 14, 30, 55, 50, 115, 34, 34, 122, 74, 16, 6, 49, 92, 10, 64, 47, 54, 227, 83, 241, 175, 96, 46, 51, 191, 251, 31, 171, 21, 150, 29, 152, 205, 204, 95, 17, 177, 10, 155, 48, 157, 153, 75, 17, 209, 134, 13, 153, 57, 81, 180, 59, 241, 38, 51, 23, 91, 34, 162, 194, 45, 60, 169, 170, 106, 6, 3, 24, 194, 171, 170, 170, 166, 112, 23, 189, 85, 85, 77, 143, 142, 142, 190, 174, 170, 106, 18, 63, 171, 170, 122, 214, 40, 92, 119, 151, 159, 31, 65, 87, 153, 111, 186, 112, 125, 142, 67, 24, 142, 136, 217, 130, 180, 31, 79, 27, 133, 45, 60, 44, 168, 198, 209, 149, 153, 111, 75, 155, 231, 240, 3, 125, 165, 139, 58, 254, 160, 181, 222, 52, 127, 59, 230, 202, 30, 207, 68, 196, 74, 209, 182, 66, 224, 102, 89, 207, 213, 43, 9, 245, 102, 167, 8, 95, 20, 28, 235, 74, 108, 31, 94, 226, 120, 211, 54, 182, 162, 165, 129, 171, 24, 67, 39, 118, 148, 49, 6, 154, 56, 222, 71, 7, 78, 226, 61, 62, 150, 165, 25, 251, 11, 100, 253, 140, 217, 146, 43, 155, 251, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66, 96, 130 };

        private static Vector2 mousePosition;
        private static Texture2D fullscreenIcon;
        private static Texture2D fullscreenIconSmallDarkSkin;
        private static Texture2D fullscreenIconSmallWhiteSkin;

        static FullscreenUtility() {
            var lastUpdate = EditorApplication.timeSinceStartup;

            EditorApplication.update += () => {
                if(EditorApplication.timeSinceStartup - lastUpdate > 0.5f) {
                    EditorApplication.RepaintHierarchyWindow();
                    EditorApplication.RepaintProjectWindow();
                    lastUpdate = EditorApplication.timeSinceStartup;
                }
            };

            EditorApplication.hierarchyWindowItemOnGUI += (rect, id) => RecalculateMousePosition();
            EditorApplication.projectWindowItemOnGUI += (rect, id) => RecalculateMousePosition();
            SceneView.onSceneGUIDelegate += sceneView => RecalculateMousePosition();
        }

        private static void RecalculateMousePosition() {
            mousePosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        }

        /// <summary>
        /// The mouse position, can be called outside an OnGUI method.
        /// </summary>
        public static Vector2 MousePosition {
            get {
                InternalEditorUtility.RepaintAllViews();
                return mousePosition;
            }
        }

        /// <summary>
        /// The icon of this plugin.
        /// </summary>
        public static Texture2D FullscreenIcon { get { return fullscreenIcon ?? (fullscreenIcon = FindOrLoad(fullscreenIconSource, "FullscreenEditorIcon")); } }

        /// <summary>
        /// A smaller icon of this plugin for the dark skin, used in the preferences window.
        /// </summary>
        public static Texture2D FullscreenIconSmallDarkSkin { get { return fullscreenIconSmallDarkSkin ?? (fullscreenIconSmallDarkSkin = FindOrLoad(fullscreenIconSourceSmallDarkSkin, "FullscreenEditorIconSmallB")); } }

        /// <summary>
        /// A smaller icon of this plugin for the white skin, used in the preferences window.
        /// </summary>
        public static Texture2D FullscreenIconSmallWhiteSkin { get { return fullscreenIconSmallWhiteSkin ?? (fullscreenIconSmallWhiteSkin = FindOrLoad(fullscreenIconSourceSmallWhiteSkin, "FullscreenEditorIconSmallW")); } }

        /// <summary>
        /// Save the current window layout.
        /// </summary>
        /// <param name="safe">Should the layout be saved as a backup?</param>
        public static void SaveLayout(bool safe) {
            try {
                ReflectionUtility.FindClass("UnityEditor.WindowLayout").InvokeMethod("SaveWindowLayout", safe ? SAFE_LAYOUT_PATH : TEMP_LAYOUT_PATH);
            }
            catch(Exception e) {
                Debug.LogError("Failed to save current layout: " + e);
            }
        }

        /// <summary>
        /// Load a layout saved previously and then deletes the file.
        /// </summary>
        /// <param name="safe">Should we load the backup layout?</param>
        public static void LoadLayout(bool safe) {
            try {
                if(!File.Exists(safe ? SAFE_LAYOUT_PATH : TEMP_LAYOUT_PATH))
                    return;

                using(new SuppressLog()) //Supress the following errors about failing to destroy views.
                    ReflectionUtility.FindClass("UnityEditor.WindowLayout").InvokeMethod("LoadWindowLayout", safe ? SAFE_LAYOUT_PATH : TEMP_LAYOUT_PATH, true);

                File.Delete(safe ? SAFE_LAYOUT_PATH : TEMP_LAYOUT_PATH);
            }
            catch(Exception e) {
                InternalEditorUtility.LoadDefaultLayout();
                Debug.LogException(e);
                Debug.LogError("Error while loading the previous layout, default layout was loaded instead");
            }
        }

        /// <summary>
        /// Show the notification of a newly opened <see cref="FullscreenView"/> about using the shortcut to close.
        /// </summary>
        /// <param name="window">The fullscreen window.</param>
        /// <param name="menuItemPath">The <see cref="MenuItem"/> path containing a shortcut.</param>
        public static void ShowFullscreenNotification(EditorWindow window, string menuItemPath) {
            ShowFullscreenNotification(window, "Press {0} to exit fullscreen", menuItemPath);
        }

        /// <summary>
        /// Show a notification in an <see cref="EditorWindow"/>.
        /// </summary>
        /// <param name="window">The host of the notification.</param>
        /// <param name="message">The message to show.</param>
        /// <param name="menuItemPath">The <see cref="MenuItem"/> path containing a shortcut.</param>
        public static void ShowFullscreenNotification(EditorWindow window, string message, string menuItemPath) {
            if(!window || FullscreenPreferences.DisableNotifications)
                return;

            var notification = string.Format(message, TextifyMenuItemShortcut(menuItemPath));

            window.ShowNotification(new GUIContent(notification, FullscreenIcon));
            window.Repaint();

            if(EditorWindow.mouseOverWindow)
                EditorWindow.mouseOverWindow.Repaint();
        }

        /// <summary>
        /// Does the given <see cref="MenuItem"/> path contains a key binding?
        /// </summary>
        public static bool MenuItemHasShortcut(string menuItemPath) {
            var index = menuItemPath.LastIndexOf(" ");

            if(index++ == -1)
                return false;

            var shortcut = menuItemPath.Substring(index).Replace("_", "");
            var evt = Event.KeyboardEvent(shortcut);

            shortcut = InternalEditorUtility.TextifyEvent(evt);

            return !shortcut.Equals("None", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Gets a human-readable shortcut.
        /// </summary>
        /// <param name="menuItemPath">The <see cref="MenuItem"/> path containing a shortcut.</param>
        /// <returns></returns>
        public static string TextifyMenuItemShortcut(string menuItemPath) {
            var index = menuItemPath.LastIndexOf(" ");

            if(index++ == -1)
                return "None";

            var shortcut = menuItemPath.Substring(index).Replace("_", "");
            var evt = Event.KeyboardEvent(shortcut);

            shortcut = InternalEditorUtility.TextifyEvent(evt);

            return shortcut;
        }

        internal static Texture2D FindOrLoad(byte[] bytes, string name) {
            return FindTextureFromName(name) ?? LoadTexture(bytes, name);
        }

        internal static Texture2D LoadTexture(byte[] bytes, string name) {
            try {
                var texture = new Texture2D(0, 0, TextureFormat.ARGB32, false, true);

                texture.name = name;
                texture.hideFlags = HideFlags.HideAndDontSave;

                texture.LoadImage(bytes);

                return texture;
            }
            catch(Exception e) {
                Debug.LogError(string.Format("Failed to load texture \"{0}\": {1}", name, e));
                return null;
            }
        }

        internal static Texture2D FindTextureFromName(string name) {
            try {
                var textures = Resources.FindObjectsOfTypeAll<Texture2D>();

                for(var i = 0; i < textures.Length; i++)
                    if(textures[i].name == name)
                        return textures[i];

                return null;
            }
            catch(Exception e) {
                Debug.LogError(string.Format("Failed to find texture \"{0}\": {1}", name, e));
                return null;
            }
        }

        internal static void WaitFrames(int frames, Action callback) {
            var update = new EditorApplication.CallbackFunction(() => { });
            var framesPassed = 0;

            update = () => {
                if(framesPassed++ >= frames) {
                    EditorApplication.update -= update;
                    callback();
                }
            };

            EditorApplication.update += update;
        }

        //[InitializeOnLoadMethod]
        //private static void LoadLayoutOnStart() {
        //    var all = Tracker<FullscreenView>.GetAll();

        //    WaitFrames(3, () => {
        //        if(all.Length == 0 && File.Exists(TEMP_LAYOUT_PATH)) {
        //            LoadLayout(false);
        //            Debug.LogWarning("Closed fullscreen window that was opened last time editor application ran");
        //        }
        //    });

        //}

    }
}