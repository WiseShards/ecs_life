using Unity.Collections;
using Unity.Jobs;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using Unity.Collections.LowLevel.Unsafe;

namespace pure {
    [Unity.Burst.BurstCompile]
    struct MoveNextGenJobP : IJobProcessComponentData<CellStateP> {
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<byte> neighboursState;
        [ReadOnly]
        public NativeArray<int> neighboursIndexes;  // indexes of each cell's neighbours

        public void Execute(ref CellStateP state) {
            state.AliveNow = state.AliveNext;
            int startIndex = state.CellIndex * 8;
            for(int i = startIndex; i < startIndex + 8; ++i) {
                neighboursState[neighboursIndexes[i]] = state.AliveNow;
            }
        }
    }

    [UpdateAfter(typeof(CalcNextGenSystemP))]
    public class MoveNextGenSystemP : JobComponentSystem {
        public NativeArray<int> neighbours;
        [Inject] CalcNextGenSystemP calcNextGenSystem;

        int BoardWidth = 100;
        int BoardHeight = 100;

        protected override void OnCreateManager() {
            neighbours = new NativeArray<int>(8 * BoardWidth * BoardHeight, Allocator.Persistent);

            for(int index = 0; index < BoardWidth * BoardHeight; ++index) {
                int xIndex = index / BoardWidth;
                int zIndex = index % BoardWidth;

                int curNeighbourIndex = 0;

                for(int deltaX = -1; deltaX <= 1; ++deltaX) {
                    for(int deltaZ = -1; deltaZ <= 1; ++deltaZ) {
                        if(deltaX != 0 || deltaZ != 0) {
                            int currentXIndex = xIndex + deltaX;
                            if(currentXIndex < 0) {
                                currentXIndex += BoardWidth;
                            }
                            else if(currentXIndex >= BoardWidth) {
                                currentXIndex -= BoardWidth;
                            }

                            int currentZIndex = zIndex + deltaZ;

                            if(currentZIndex < 0) {
                                currentZIndex += BoardHeight;
                            }
                            else if(currentZIndex >= BoardHeight) {
                                currentZIndex -= BoardHeight;
                            }
                            int currentIndex = currentXIndex * BoardWidth + currentZIndex;

                            neighbours[index * 8 + curNeighbourIndex] = currentIndex * 8 + 7 - curNeighbourIndex;
                            curNeighbourIndex++;
                        }
                    }
                }
            }

        }

        protected override void OnDestroyManager() {
            neighbours.Dispose();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            MoveNextGenJobP moveNextGenJob = new MoveNextGenJobP { neighboursState = calcNextGenSystem.nState, neighboursIndexes = neighbours };

            JobHandle moveNextGenHandle = moveNextGenJob.Schedule(this, inputDeps);

            return moveNextGenHandle;
        }
    }
}