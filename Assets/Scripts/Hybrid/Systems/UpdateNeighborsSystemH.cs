using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using UnityEngine.Experimental.PlayerLoop;

namespace hybrid {
    [UpdateBefore(typeof(Update))]
    [UpdateBefore(typeof(CalcNextGenSystemH))]
    public class UpdateNeighborsSystemH : ComponentSystem {
        public static bool AutoSimulate = false;
        public static bool SimulateNextGen = false;

        public struct NeighrosData {
            public readonly int Length;
            [ReadOnly] public ComponentArray<CellStateH> cellState;
            public ComponentArray<NeighborsH> neighbors;
        }

        [Inject] NeighrosData m_neighbors;

        protected override void OnUpdate() {
            if(0 == m_neighbors.Length) {
                return;
            }

            if(!AutoSimulate && !SimulateNextGen) {
                return;
            }
            SimulateNextGen = false;

            var Neighbors = m_neighbors.neighbors;
            var CellStateComponent = m_neighbors.cellState;
            for(int entityIndex = 0; entityIndex < m_neighbors.Length; ++entityIndex) {
                var neighborComponent = Neighbors[entityIndex];
                var neighborsCellState = neighborComponent.NList;

                if(CellStateComponent[entityIndex].AliveNow) {
                    for(int nIndex = 0; nIndex < neighborsCellState.Count; ++nIndex) {
                        ++neighborsCellState[nIndex].AliveNeighbors;
                    }
                }
            }
        }
    }
}