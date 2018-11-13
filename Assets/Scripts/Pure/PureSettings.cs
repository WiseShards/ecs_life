using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pure {
    public class PureSettings : ScriptableObject {
        public Material AliveMaterial;
        public Material DeadMaterial;
        public Mesh Mesh;

        private static PureSettings Instance;

        public static PureSettings Inst {
            get {
                if(Instance == null) {
                    Instance = Resources.Load<PureSettings>("PureSettings");
                }
                return Instance;
            }
        }
    }
}