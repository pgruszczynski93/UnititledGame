using System.Linq;
using UnityEngine;

namespace Fullscreen {
    /// <summary>
    /// Keeps track of all the <see cref="FullscreenView"/>.
    /// </summary>
    public static class Tracker<T> where T : FullscreenView {

        /// <summary>
        /// Return all the <see cref="FullscreenView"/> of type <see cref="{T}"/>.
        /// </summary>
        public static T[] GetAll() {
            return Resources.FindObjectsOfTypeAll<T>();
        }

        /// <summary>
        /// Return all the closed <see cref="FullscreenView"/> of type <see cref="{T}"/>.
        /// </summary>
        public static T[] GetAllClosed() {
            return (from view in GetAll()
                    where view && !view.IsOpen
                    select view).ToArray();
        }

        /// <summary>
        /// Return all the open <see cref="FullscreenView"/> of type <see cref="{T}"/>.
        /// </summary>
        public static T[] GetAllOpen() {
            return (from view in GetAll()
                    where view && view.IsOpen
                    select view).ToArray();
        }

        /// <summary>
        /// Get one instance <see cref="{T}"/> or create a new if none exists.
        /// </summary>
        /// <param name="allowGettingOpen">Allow returning a <see cref="FullscreenView"/> that is open.</param>
        public static T GetOrCreate(bool allowGettingOpen) {
            var closedOnes = GetAllClosed();
            var openedOnes = GetAllOpen();

            if(closedOnes.Length > 0)
                return closedOnes.First();
            else if(allowGettingOpen && openedOnes.Length > 0)
                return openedOnes.First();

            var instance = ScriptableObject.CreateInstance<T>();
            instance.hideFlags = HideFlags.HideAndDontSave;
            return instance;
        }

        /// <summary>
        /// Get the <see cref="{T}"/> under the mouse, or null if there is none.
        /// </summary>
        /// <returns></returns>
        public static T GetFullscreenViewUnderMouse() {
            var mousePos = FullscreenUtility.MousePosition;

            return (from view in GetAll()
                    where view is T && view.IsOpen && view.Rect.Contains(mousePos)
                    select view as T).FirstOrDefault();
        }

        /// <summary>
        /// Toggle the fullscreen state of <see cref="{T}"/> based on <see cref="FullscreenPreferences.MutipleWindowMode"/>.
        /// </summary>
        public static T ToggleView() {
            switch(FullscreenPreferences.MutipleWindowMode.Value) {
                case MutipleWindow.OneOfEachType:
                    if(CloseAll())
                        return null;
                    else
                        break;

                case MutipleWindow.OnlyOne:
                    if(Tracker<FullscreenView>.CloseAll())
                        return null;
                    else
                        break;

                case MutipleWindow.OnlyOneImmediate:
                    if(CloseAll())
                        return null;

                    Tracker<FullscreenView>.CloseAll();
                    break;

                default:
                    if(typeof(T) == typeof(MainViewFullscreen))
                        if(CloseAll())
                            return null;
                        else
                            break;

                    var view = GetFullscreenViewUnderMouse();

                    if(view != null && view.IsOpen) {
                        view.Close();
                        return null;
                    }
                    else
                        break;
            }

            var newView = GetOrCreate(false);
            newView.Open();
            return newView;
        }

        /// <summary>
        /// Close all the <see cref="{T}"/>. Returns true if any was closed.
        /// </summary>
        public static bool CloseAll() {
            var closed = false;

            foreach(var view in GetAllOpen()) {
                closed = true;
                view.Close();
            }

            return closed;
        }

    }
}