using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Fullscreen {
    public class FocusedWindowFullscreen : FullscreenView {

        public override Type Type { get { return typeof(EditorWindow); } }

        public override bool HasToolbar { get { return true; } }

        public override bool RequiresLayoutReload { get { return false; } }

        public override string MenuItemPath { get { return Shortcut.CURRENT_VIEW_PATH; } }

        public override Object DefaultView { get { return EditorWindow.mouseOverWindow ?? EditorWindow.focusedWindow; } }

        public override void Open(Object reference) {
            if(!reference)
                return;

            base.Open(reference);

            if(reference is SceneView)
                FullscreenUtility.ShowFullscreenNotification(CurrentOpenWindow, "Use {0} when opening a fullscreen Scene View to prevent unexpected behaviour", Shortcut.SCENE_VIEW_PATH);
            else if(reference.GetType() == GameViewFullscreen.gameViewType)
                FullscreenUtility.ShowFullscreenNotification(CurrentOpenWindow, "Use {0} when opening a fullscreen Game View to prevent unexpected behaviour", Shortcut.GAME_VIEW_PATH);
        }

    }
}