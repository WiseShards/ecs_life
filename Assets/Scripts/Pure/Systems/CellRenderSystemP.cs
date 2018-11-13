using Unity.Collections;
using Unity.Jobs;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.PlayerLoop;
using Unity.Rendering;
using System.Collections.Generic;

namespace pure {
    public struct Group {
        [ReadOnly] public EntityArray EntityArray;
        [ReadOnly] public ComponentDataArray<CellStateP> CellState;
        [ReadOnly] public SharedComponentDataArray<MeshInstanceRenderer> MeshInstanceRenderer;
    }

    [UpdateAfter(typeof(MoveNextGenSystemP))]
    public class CellRenderSystemP : ComponentSystem {
        private ComponentGroup group;

        protected override void OnCreateManager() {
            group = GetComponentGroup(typeof(CellStateP), typeof(MeshInstanceRenderer));
        }

        protected override void OnUpdate() {
            ComponentDataArray<CellStateP> cellStates = group.GetComponentDataArray<CellStateP>();
            EntityArray entityArray = group.GetEntityArray();
            SharedComponentDataArray<MeshInstanceRenderer> meshInstanceRenderers = group.GetSharedComponentDataArray<MeshInstanceRenderer>();
            var settings = PureSettings.Inst;

            for(int i = 0; i < cellStates.Length; ++i) {
                var state = cellStates[i].AliveNow;
                var material = state == 0 ? settings.DeadMaterial : settings.AliveMaterial;

                MeshInstanceRenderer ren = meshInstanceRenderers[i];
                if(ren.material != material) {
                    ren.material = material;
                    PostUpdateCommands.SetSharedComponent<MeshInstanceRenderer>(entityArray[i], ren);
                }
            }
        }
    }
}