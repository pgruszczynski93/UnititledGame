using System.Linq;
using UnityEditor;

namespace Fullscreen {
    /// <summary>
    /// Class that open and close a <see cref="GameViewFullscreen"/> instance if play mode state changes and <see cref="FullscreenPreferences.FullscreenOnPlayEnabled"/> is true.
    /// </summary>
    [InitializeOnLoad]
    internal static class FullscreenOnPlay {

        static FullscreenOnPlay() {
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += state => {
                switch(state) {
                    case PlayModeStateChange.ExitingEditMode:
                        SetIsPlaying(true);
                        break;

                    case PlayModeStateChange.ExitingPlayMode:
                        SetIsPlaying(false);
                        break;
                }
            };

            EditorApplication.pauseStateChanged += state => SetIsPlaying(state == PauseState.Unpaused);
#else 
            EditorApplication.playmodeStateChanged += () => SetIsPlaying(EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPaused);
#endif
        }

        private static void SetIsPlaying(bool playing) {
            if(!FullscreenPreferences.FullscreenOnPlayEnabled)
                return;

            if(playing && FullscreenPreferences.FullscreenOnPlayGiveWay && Tracker<FullscreenView>.GetAllOpen().Length > 0)
                return;

            var openGameViews = Tracker<GameViewFullscreen>.GetAllOpen();

            if(openGameViews.Length > 0 && !playing)
                openGameViews.First().Close();

            else if(openGameViews.Length == 0 && playing)
                Tracker<GameViewFullscreen>.GetOrCreate(true).Open();
        }
    }
}