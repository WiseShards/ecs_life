using UnityEngine;
using Unity.Entities;
using UnityEngine.SceneManagement;

namespace hybrid {
    public sealed class BootstrapHybrid {
        public static HybridSettings Settings;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitializeAfterSceneLoad() {
            var settingsGO = GameObject.Find("Settings");
            if(settingsGO == null) {
                SceneManager.sceneLoaded += OnSceneLoaded;
                return;
            }

            InitializeWithScene();
        }

        private static void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) {
            InitializeWithScene();
        }

        public static void InitializeWithScene() {
            var settingsGO = GameObject.Find("Settings");
            Settings = settingsGO?.GetComponent<HybridSettings>();
            if(!Settings)
                return;
        }
    }
}