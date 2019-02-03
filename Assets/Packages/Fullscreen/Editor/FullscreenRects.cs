using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fullscreen {
    /// <summary>
    /// Class for getting fullscreen rectangles.
    /// </summary>
    public static class FullscreenRects {

        /// <summary>
        /// The number of monitors attached to this machine, returns -1 if the platform is not supported.
        /// </summary>
        public static int ScreenCount {
            get {
#if UNITY_EDITOR_WIN
                const int SM_CMONITORS = 80;
                return GetSystemMetrics(SM_CMONITORS);
#else
                return -1;
#endif
            }
        }

        /// <summary>
        /// Returns a fullscreen rect for a given <see cref="EditorWindow"/> or <see cref="ContainerWindow"/>, respecting the settings of <see cref="FullscreenPreferences.RectSource"/>.
        /// </summary>
        /// <param name="window">The window to get the rect for, must be an <see cref="EditorWindow"/> or <see cref="ContainerWindow"/>.</param>
        public static Rect GetFullscreenRect(Object window) {
            switch(FullscreenPreferences.RectSource.Value) {
                case RectSourceMode.PrimaryScreen:
                    return GetMainDisplayRect();

                case RectSourceMode.AtMousePosition:
                    return GetDisplayRectAtPoint(FullscreenUtility.MousePosition);

                case RectSourceMode.VirtualSpace:
                    return GetVirtualScreenRect();

                case RectSourceMode.WorkArea:
                    return GetWorkAreaRect(window);

                case RectSourceMode.CustomRect:
                    return FullscreenPreferences.CustomRect;

                default:
                    Debug.LogWarning("Invalid fullscreen mode, please fix this changing the rect mode in preferences.");
                    return new Rect(Vector2.zero, Vector2.one * 300f);
            }
        }

        /// <summary>
        /// Returns a rect with the dimensions of the main screen.
        /// (Note that the position may not be right for multiple screen setups)
        /// </summary>
        public static Rect GetMainDisplayRect() {
            return new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height);
        }

        /// <summary>
        /// Returns a rect covering all the screen, except for the taskbar.
        /// This is the Rect that works best with Mac OS X, although the dock must be set to auto hide for it to work.
        /// </summary>
        public static Rect GetWorkAreaRect() {
            return GetWorkAreaRect(true);
        }

        /// <summary>
        /// Returns a rect covering all the screen, except for the taskbar.
        /// This is the Rect that works best with Mac OS X, although the dock must be set to auto hide for it to work.
        /// </summary>
        /// <param name="mouseScreen">Should we get the rect on the screen where the mouse pointer is?</param>
        public static Rect GetWorkAreaRect(bool mouseScreen) {
            return FullscreenView.containerWindowType.InvokeMethod<Rect>("FitRectToScreen", new Rect(Vector2.zero, Vector2.one * 100000f), true, mouseScreen);
        }

        /// <summary>
        /// Returns a rect covering all the screen, except for the taskbar.
        /// This is the Rect that works best with Mac OS X, although the dock must be set to auto hide for it to work.
        /// </summary>
        /// <param name="window">The window that will be used as reference for calulating border error.</param>
        public static Rect GetWorkAreaRect(Object window) {
            return GetWorkAreaRect(window, true);
        }

        /// <summary>
        /// Returns a rect covering all the screen, except for the taskbar.
        /// This is the Rect that works best with Mac OS X, although the dock must be set to auto hide for it to work.
        /// </summary>
        /// <param name="window">The window that will be used as reference for calulating border error.</param>
        /// <param name="mouseScreen">Should we get the rect on the screen where the mouse pointer is?</param>
        public static Rect GetWorkAreaRect(Object window, bool mouseScreen) {
            if(!window)
                return GetWorkAreaRect(mouseScreen);

            if(window is EditorWindow)
                window = window.GetFieldValue<Object>("m_Parent").GetPropertyValue<Object>("window");

            return window.InvokeMethod<Rect>("FitWindowRectToScreen", new Rect(Vector2.zero, Vector2.one * 100000f), true, mouseScreen);
        }

        /// <summary>
        /// Returns a fullscreen rect for the screen that contains the passed point.
        /// </summary>
        /// <param name="point">The point relative to <see cref="RectSourceMode.VirtualSpace"/></param>
        public static Rect GetDisplayRectAtPoint(Vector2 point) {
            switch(Application.platform) {
                case RuntimePlatform.WindowsEditor: //Gets the rect of the screen where the mouse cursor is, useful if you're using multiple screens.
                    return InternalEditorUtility.GetBoundsOfDesktopAtPoint(point);

                default: //GetBoundsOfDesktopAtPoint throws a "NotImplementedException" on OSX, so we won't use it.
                    return GetMainDisplayRect();
            }
        }

        /// <summary>
        /// Full rect of the screen, spanning across all monitors. (Windows Only)
        /// </summary>
        public static Rect GetVirtualScreenRect() {
#if UNITY_EDITOR_WIN
            const int SM_XVIRTUALSCREEN = 76;
            const int SM_YVIRTUALSCREEN = 77;
            const int SM_CXVIRTUALSCREEN = 78;
            const int SM_CYVIRTUALSCREEN = 79;

            var top = GetSystemMetrics(SM_XVIRTUALSCREEN);
            var left = GetSystemMetrics(SM_YVIRTUALSCREEN);
            var width = GetSystemMetrics(SM_CXVIRTUALSCREEN);
            var height = GetSystemMetrics(SM_CYVIRTUALSCREEN);

            return new Rect {
                yMin = top,
                xMin = left,
                width = width,
                height = height,
            };
#else
            return GetMainDisplayRect();
#endif
        }

#if UNITY_EDITOR_WIN
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetSystemMetrics(int smIndex);
#endif

    }
}