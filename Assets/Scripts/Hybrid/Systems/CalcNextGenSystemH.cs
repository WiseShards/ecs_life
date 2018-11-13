using Unity.Collections;
using Unity.Jobs;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace hybrid {
    [UpdateBefore(typeof(Update))]
    [UpdateAfter(typeof(UpdateNeighborsSystemH))]
    public class CalcNextGenSystemH : ComponentSystem {
        public static bool AutoSimulate = false;
        public static bool SimulateNextGen = false;

        public struct CellData {
            public readonly int Length;
            public ComponentArray<CellStateH> CellStates;
            [ReadOnly] public ComponentArray<NeighborsH> Neighbors;
        }

        [Inject] CellData m_cellData;

        protected override void OnUpdate() {
            // No data to consume
            if(m_cellData.Length == 0) {
                return;
            }

            // No computation needed
            if(!AutoSimulate && !SimulateNextGen) {
                return;
            }
            SimulateNextGen = false;

            for(int cellIndex = 0; cellIndex < m_cellData.Length; ++cellIndex) {
                var state = m_cellData.CellStates[cellIndex];
                var neighbors = m_cellData.Neighbors[cellIndex];
                var AliveNeighborCount = neighbors.AliveNeighbors;

                if(state.AliveNow) {
                    state.AliveNow = ((AliveNeighborCount == 2) || (AliveNeighborCount == 3));
                }
                else {
                    state.AliveNow = (AliveNeighborCount == 3);
                }
                neighbors.AliveNeighbors = 0;
            }
        }
    }
}