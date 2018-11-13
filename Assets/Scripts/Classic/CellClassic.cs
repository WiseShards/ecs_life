using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace classic {
    public enum CELL_STATE {
        ALIVE,
        DEAD,
    }

    public class CellClassic : MonoBehaviour {
        public static bool ContinuousSimulation = false;
        public static int ComputeNextSimulation = 0;

        public Material AliveMaterial;
        public Material DeadMaterial;
        private List<CellClassic> Neighbors = new List<CellClassic>();  // Always 8 neighbors
        private MeshRenderer CellSkin;
        private bool AliveNext = false;     // Dead or alive (next generation)
        private bool AliveNow = false;      // Dead or alive (now)

        public void SetState(bool newState) {
            AliveNow = newState;
            if(newState) {
                CellSkin.sharedMaterial = AliveMaterial;
            }
            else {
                CellSkin.sharedMaterial = DeadMaterial;
            }
        }

        void Awake() {
            CellSkin = GetComponent<MeshRenderer>();
            SetState(AliveNow);
        }

        public void AddNeighbor(CellClassic cell) {
            Neighbors.Add(cell);
        }

        public void CalculateNextGeneration() {
            int AliveNeighborCount = 0;
            for(int i = 0; i < Neighbors.Count; ++i) {
                if(Neighbors[i].AliveNow) {
                    ++AliveNeighborCount;
                }
            }

            if(AliveNow) {
                AliveNext = ((AliveNeighborCount == 2) || (AliveNeighborCount == 3));
            }
            else {
                AliveNext = AliveNeighborCount == 3;
            }
        }

        void Update() {
            if(ContinuousSimulation || (ComputeNextSimulation != 0)) {
                CalculateNextGeneration();
            }
        }

        void LateUpdate() {
            if(ContinuousSimulation || (ComputeNextSimulation != 0)) {
                NextGeneration();
            }
            ComputeNextSimulation = Mathf.Max(0, --ComputeNextSimulation);
        }

        public void NextGeneration() {
            SetState(AliveNext);
        }
    }
}