using UnityEngine;
using Unity.Entities;

namespace pure {
    public struct CellStateP : IComponentData {
        public byte AliveNow;   // (bool) Dead or alive (now)
        public byte AliveNext;  // (bool) Dead or alive (next generation)
        public int CellIndex;   // Internal Index
    }
}