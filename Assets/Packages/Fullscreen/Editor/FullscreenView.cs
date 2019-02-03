using System;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fullscreen {
    /// <summary>
    /// Class for creating and closing fullscreen windows.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class FullscreenView : ScriptableObject {

        protected const float TOOLBAR_HEIGHT = 17f;
        protected const string CLOSE_METHOD = "Close";
        protected const string SHOW_METHOD = "ShowPopup";
        protected const string RECT_PROPERTY = "position";

        internal static readonly Type containerWindowType = ReflectionUtility.FindClass("UnityEditor.ContainerWindow");

        [SerializeField]
        private Rect rect;
        [SerializeField]
        private Object currentOpen;

        /// <summary>
        /// Is this instance in fullscreen mode?
        /// </summary>
        public bool IsOpen { get { return CurrentOpenObject; } }

        /// <summary>
        /// The rect of the window.
        /// </summary>
        public Rect Rect {
            get { return rect; }
            set {
                rect = value;

                if(CurrentOpenWindow) {
                    CurrentOpenWindow.maximized = true;
                    CurrentOpenWindow.maxSize = value.size;
                    CurrentOpenWindow.minSize = value.size;
                }

                CurrentOpenObject.SetPropertyValue(RECT_PROPERTY, value);
            }
        }

        /// <summary>
        /// The current object in fullscreen mode, can be an <see cref="EditorWindow"/> or a <see cref="ContainerWindow"/>.
        /// </summary>
        public Object CurrentOpenObject {
            get { return currentOpen; }
            private set { currentOpen = value; }
        }

        /// <summary>
        /// The current open object as an <see cref="EditorWindow"/>.
        /// </summary>
        public EditorWindow CurrentOpenWindow {
            get { return CurrentOpenObject as EditorWindow; }
        }

        /// <summary>
        /// The open <see cref="ContainerWindow"/> of <see cref="CurrentOpenObject"/>.
        /// </summary>
        public Object CurrentOpenContainerView {
            get {
                if(CurrentOpenObject is EditorWindow)
                    return CurrentOpenObject.GetFieldValue<Object>("m_Parent").GetPropertyValue<Object>("window");
                else
                    return CurrentOpenObject;
            }
        }

        /// <summary>
        /// The view that was used as a reference for cloning the last time <see cref="Open"/> was called.
        /// </summary>
        public Object ViewPassedAsReference { get; private set; }

        /// <summary>
        /// The type of window the class that inherits from this creates.
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// Does the <see cref="EditorWindow"/> has a toolbar at the top?
        /// </summary>
        public abstract bool HasToolbar { get; }

        /// <summary>
        /// Does opening and closing this view requires the layout to reload? 
        /// It's important to keep always only one <see cref="FullscreenView"/> that requires layout reload opened.
        /// </summary>
        public abstract bool RequiresLayoutReload { get; }

        /// <summary>
        /// The <see cref="MenuItem"/> path for opening/closing this view.
        /// </summary>
        public abstract string MenuItemPath { get; }

        /// <summary>
        /// The default reference for cloning when showing this view.
        /// </summary>
        public abstract Object DefaultView { get; }

        /// <summary>
        /// Start the fullscreen view.
        /// </summary>
        public void Open() {
            Open(DefaultView);
        }

        /// <summary>
        /// Start the fullscreen view.
        /// </summary>
        /// <param name="reference">A reference to clone properties from.</param>
        public virtual void Open(Object reference) {
            if(IsOpen)
                return;

            if(RequiresLayoutReload)
                FullscreenUtility.SaveLayout(false);

            ViewPassedAsReference = reference;

            if(reference)
                if(reference.GetType() == Type || reference.GetType().IsSubclassOf(Type))
                    CurrentOpenObject = Instantiate(reference);
                else {
                    ViewPassedAsReference = null;
                    Debug.LogWarning("Reference object is not inherited from " + Type + ", it will be ignored");
                }

            if(!CurrentOpenObject)
                CurrentOpenObject = CreateInstance(Type);

            CurrentOpenObject.InvokeMethod(SHOW_METHOD);
            Rect = FullscreenRects.GetFullscreenRect(CurrentOpenObject);

            if(!FullscreenPreferences.ToolbarVisible)
                SetToolbarStatus(false);

            Focus();

            FullscreenUtility.ShowFullscreenNotification(CurrentOpenWindow, MenuItemPath);

            for(var i = 0; i < 3; i++) //Prevents the editor from being completely black when switching fullscreen mode of main window.
                InternalEditorUtility.RepaintAllViews(); //But it won't work every time, so we do it multiple times.

            CurrentOpenContainerView.SetFieldValue("m_DontSaveToLayout", true);

#if UNITY_2018_1_OR_NEWER
            EditorApplication.wantsToQuit += WantsToQuit;
#endif
        }

        /// <summary>
        /// Close this fullscreen view.
        /// </summary>
        public virtual void Close() {
            if(!IsOpen)
                return;

#if UNITY_2018_1_OR_NEWER
            EditorApplication.wantsToQuit -= WantsToQuit;
#endif
            CurrentOpenObject.InvokeMethod(CLOSE_METHOD);

            if(RequiresLayoutReload)
                FullscreenUtility.LoadLayout(false);
        }

#if UNITY_2018_1_OR_NEWER
        private bool WantsToQuit() {
            Close();
            return true;
        }
#endif

        /// <summary>
        /// Focus the window associated with this fullscreen view if there is any.
        /// </summary>
        public virtual void Focus() {
            if(!CurrentOpenWindow)
                return;

            CurrentOpenWindow.Focus();
            FullscreenUtility.WaitFrames(2, CurrentOpenWindow.Focus);
        }

        protected virtual void OnEnable() {
            if(CurrentOpenWindow)
                CurrentOpenWindow.Repaint();

            FullscreenPreferences.ToolbarVisible.OnValueSaved += SetToolbarStatus;
        }

        protected virtual void OnDisable() { //Called on editor quit
            FullscreenPreferences.ToolbarVisible.OnValueSaved -= SetToolbarStatus;

#if !UNITY_2018_1_OR_NEWER
            if(File.Exists(FullscreenUtility.TEMP_LAYOUT_PATH))
                File.Copy(FullscreenUtility.TEMP_LAYOUT_PATH, FullscreenUtility.UNITY_LAYOUT_PATH, true);
#endif
        }

        /// <summary>
        /// Enable or disable the window toolbar if <see cref="HasToolbar"/> is true, otherwise does nothing.
        /// </summary>
        /// <param name="toolbarVisible"></param>
        public void SetToolbarStatus(bool toolbarVisible) {
            if(!HasToolbar || !IsOpen)
                return;

            var newRect = Rect;

            if(toolbarVisible)
                newRect.yMin += TOOLBAR_HEIGHT;
            else
                newRect.yMin -= TOOLBAR_HEIGHT;

            Rect = newRect;
        }

        /// <summary>
        /// Is the given type valid for creating a fullscreen view?
        /// </summary>
        public static bool IsValidTypeForFullscreen(Type type) {
            if(type == null)
                return false;
            if(type == containerWindowType || type == typeof(EditorWindow))
                return true;
            if(type.IsSubclassOf(containerWindowType) || type.IsSubclassOf(typeof(EditorWindow)))
                return true;
            return false;
        }

    }
}