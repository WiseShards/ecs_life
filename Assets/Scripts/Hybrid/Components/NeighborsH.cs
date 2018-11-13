using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;

namespace hybrid {
    public class NeighborsH : MonoBehaviour {
        public List<NeighborsH> NList = new List<NeighborsH>(); // Always 8 neighbors

        public int AliveNeighbors = 0;
    }
}