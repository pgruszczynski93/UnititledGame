using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fullscreen {
    public class MainViewFullscreen : FullscreenView {

        public override Type Type { get { return ReflectionUtility.FindClass("UnityEditor.ContainerWindow"); } }

        public override bool HasToolbar { get { return true; } }

        public override bool RequiresLayoutReload { get { return true; } }

        public override string MenuItemPath { get { return Shortcut.MAIN_VIEW_PATH; } }

        public override Object DefaultView { get { return GetMainView(); } }

        protected override void OnEnable() {
            base.OnEnable();

            if(IsOpen)
                CurrentOpenObject.InvokeMethod("Internal_BringLiveAfterCreation", true, true);
        }

        public static Object GetMainView() {
            var containers = Resources.FindObjectsOfTypeAll(containerWindowType);

            for(var i = 0; i < containers.Length; i++)
                if(containers[i] && containers[i].GetPropertyValue<int>("showMode") == 4)
                    return containers[i];

            throw new Exception("Couldn't find main view");
        }

    }
}