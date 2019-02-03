using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Fullscreen {
    public class SceneViewFullscreen : FullscreenView {

        public override Type Type { get { return typeof(SceneView); } }

        public override bool HasToolbar { get { return true; } }

        public override bool RequiresLayoutReload { get { return false; } }

        public override string MenuItemPath { get { return Shortcut.SCENE_VIEW_PATH; } }

        public override Object DefaultView { get { return SceneView.lastActiveSceneView; } }

    }
}
