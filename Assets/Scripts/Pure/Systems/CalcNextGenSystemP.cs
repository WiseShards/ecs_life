using Unity.Collections;
using Unity.Jobs;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace pure {
    [Unity.Burst.BurstCompile]
    public struct CalculateNextGenJobP : IJobProcessComponentData<CellStateP> {
        public bool calculateNextGen;
        [ReadOnly] public NativeArray<byte> neighboursState;

        public void Execute(ref CellStateP state) {
            if(!calculateNextGen) {
                state.AliveNext = state.AliveNow;
                return;
            }

            int AliveNeighborCount = 0;

            int startIndex = state.CellIndex * 8;
            for(int i = startIndex; i < startIndex + 8; ++i) {
                int neighbourState = neighboursState[i];
                if(neighbourState == 1) {
                    ++AliveNeighborCount;
                }
            }

            if(state.AliveNow == 1) {
                state.AliveNext = ((AliveNeighborCount == 2) || (AliveNeighborCount == 3)) ? (byte) 1 : (byte) 0;
            }
            else {
                state.AliveNext = (AliveNeighborCount == 3) ? (byte) 1 : (byte) 0;
            }
        }
    }

    public class CalcNextGenSystemP : JobComponentSystem {
        public static bool AutoSimulate = false;
        public static bool SimulateNextGen = false;
        public static bool RestartBoard = false;
        public NativeArray<byte> nState;    // neighbours State

        int BoardWidth = 100;
        int BoardHeight = 100;

        protected override void OnCreateManager() {
            nState = new NativeArray<byte>(8 * BoardWidth * BoardHeight, Allocator.Persistent);
        }

        protected override void OnDestroyManager() {
            nState.Dispose();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            CalculateNextGenJobP nextGenJob = new CalculateNextGenJobP {
                calculateNextGen = (!RestartBoard && (AutoSimulate || SimulateNextGen)),
                neighboursState = nState
            };
            JobHandle nextGenHandle = nextGenJob.Schedule(this, inputDeps);
            SimulateNextGen = false;
            RestartBoard = false;
            return nextGenHandle;
        }
    }
}