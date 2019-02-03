using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using Object = UnityEngine.Object;

namespace Fullscreen {
    public class GameViewFullscreen : FullscreenView {

        internal static readonly Type gameViewType = ReflectionUtility.FindClass("UnityEditor.GameView");

        public override Type Type { get { return gameViewType; } }

        public override bool HasToolbar { get { return true; } }

        public override bool RequiresLayoutReload { get { return FullscreenPreferences.GameViewInputFix; } }

        public override string MenuItemPath { get { return Shortcut.GAME_VIEW_PATH; } }

        public override Object DefaultView { get { return GetMainGameView(); } }

        public override void Open(Object reference) {
            base.Open(reference);

            if(FullscreenPreferences.GameViewInputFix)
                foreach(var gameView in GetGameViews())
                    if(gameView && gameView != CurrentOpenWindow)
                        gameView.Close();
        }

        public override void Focus() {
            if(!CurrentOpenWindow)
                return;

            base.Focus();
            InternalEditorUtility.OnGameViewFocus(true);
            InternalEditorUtility.OnGameViewFocus(true); //UI may not work properly if this is not called.
            FullscreenUtility.WaitFrames(2, () => InternalEditorUtility.OnGameViewFocus(true));
        }

        protected override void OnEnable() {
            base.OnEnable();
            Focus();
        }

        public static EditorWindow GetMainGameView() {
            return ReflectionUtility.FindClass("UnityEditor.GameView").InvokeMethod<EditorWindow>("GetMainGameView");
        }

        public static EditorWindow[] GetGameViews() {
            return ReflectionUtility.FindClass("UnityEditor.GameView").GetFieldValue<IList>("s_GameViews").Cast<EditorWindow>().ToArray();
        }

    }
}
