using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

namespace pure {
    public class BoardManagerPure : MonoBehaviour {
        public int BoardHeight = 10;
        public int BoardWidth = 10;

        public static List<Entity> entityList = new List<Entity>();
        public static EntityManager entityManager;

        void Awake() {
            entityManager = World.Active.GetOrCreateManager<EntityManager>();
            Init();
        }
        void Init() {
            var settings = PureSettings.Inst;
            MeshInstanceRenderer defaultMeshInstanceRenderer = new MeshInstanceRenderer {
                mesh = settings.Mesh,
                material = settings.DeadMaterial,
                subMesh = 0,
                castShadows = ShadowCastingMode.Off,
                receiveShadows = false
            };

            var cellArchetype = entityManager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(CellStateP), typeof(MeshInstanceRenderer));

            int cellIndex = 0;
            for(int xCoord = -Mathf.FloorToInt(BoardWidth / 2.0f), xIndex = 0; xIndex < BoardWidth; ++xCoord, ++xIndex) {
                for(int zCoord = -Mathf.FloorToInt(BoardHeight / 2.0f), zIndex = 0; zIndex < BoardHeight; ++zCoord, ++zIndex) {

                    Entity cellEntity = entityManager.CreateEntity(cellArchetype);

                    entityList.Add(cellEntity);

                    entityManager.SetComponentData<Position>(cellEntity, new Position { Value = new float3(xCoord, 0, zCoord) });
                    entityManager.SetComponentData<Rotation>(cellEntity, new Rotation { Value = quaternion.Euler(Mathf.PI * 0.5f, 0, 0) });
                    entityManager.SetComponentData<CellStateP>(cellEntity, new CellStateP {
                        AliveNow = 0,
                        AliveNext = 0,
                        CellIndex = cellIndex++,
                    });
                    entityManager.SetSharedComponentData<MeshInstanceRenderer>(cellEntity, defaultMeshInstanceRenderer);
                }
            }
        }

        void Start() {
            Restart();
        }

        public void SetAutoSimulation(bool autoSimulation) {
            CalcNextGenSystemP.AutoSimulate = autoSimulation;
        }

        public void NextGeneration() {
            CalcNextGenSystemP.SimulateNextGen = true;
        }

        public void Restart() {
            CalcNextGenSystemP.RestartBoard = true;
            for(int index = 0; index < BoardWidth * BoardHeight; ++index) {
                byte cellState = (UnityEngine.Random.value < 0.1f) ? (byte) 1 : (byte) 0;
                var cellStateComponent = entityManager.GetComponentData<CellStateP>(entityList[index]);
                cellStateComponent.AliveNow = cellState;
                cellStateComponent.AliveNext = cellState;
                entityManager.SetComponentData<CellStateP>(entityList[index], cellStateComponent);
            }
        }

    }
}