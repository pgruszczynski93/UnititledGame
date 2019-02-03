using UnityEditor;

namespace Fullscreen {
    internal static class MenuItems {

        private const int MENUITEM_ORDER = 100;

        [MenuItem(Shortcut.TOOLBAR_PATH, true)]
        [MenuItem(Shortcut.FULLSCREEN_ON_PLAY_PATH, true)]
        private static bool SetCheckMarks() {
            Menu.SetChecked(Shortcut.TOOLBAR_PATH, FullscreenPreferences.ToolbarVisible);
            Menu.SetChecked(Shortcut.FULLSCREEN_ON_PLAY_PATH, FullscreenPreferences.FullscreenOnPlayEnabled);
            return true;
        }

        [MenuItem(Shortcut.TOOLBAR_PATH, false, 0)]
        private static void Toolbar() {
            FullscreenPreferences.ToolbarVisible.Value = !FullscreenPreferences.ToolbarVisible;
        }

        [MenuItem(Shortcut.FULLSCREEN_ON_PLAY_PATH, false, 0)]
        private static void FullscreenOnPlay() {
            FullscreenPreferences.FullscreenOnPlayEnabled.Value = !FullscreenPreferences.FullscreenOnPlayEnabled;
        }

        [MenuItem(Shortcut.CURRENT_VIEW_PATH, false, MENUITEM_ORDER)]
        private static void CVMenuItem() {
            Tracker<FocusedWindowFullscreen>.ToggleView();
        }

        [MenuItem(Shortcut.GAME_VIEW_PATH, false, MENUITEM_ORDER)]
        private static void GVMenuItem() {
            Tracker<GameViewFullscreen>.ToggleView();
        }

        [MenuItem(Shortcut.SCENE_VIEW_PATH, false, MENUITEM_ORDER)]
        private static void SVMenuItem() {
            Tracker<SceneViewFullscreen>.ToggleView();
        }

        [MenuItem(Shortcut.MAIN_VIEW_PATH, false, MENUITEM_ORDER)]
        private static void MVMenuItem() {
            Tracker<MainViewFullscreen>.ToggleView();
        }


    }
}