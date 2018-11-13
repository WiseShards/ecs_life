using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;

namespace hybrid {
    public class BoardManagerHybrid : MonoBehaviour {
        public int BoardHeight = 10;
        public int BoardWidth = 10;
        public GameObject CellPrefab;

        public static List<CellStateH> CellStateList = new List<CellStateH>();
        public static List<MeshRenderer> MeshRendererList = new List<MeshRenderer>();
        public static List<NeighborsH> NeighborList = new List<NeighborsH>();

        void Awake() {
            Init();
        }
        void Init() {

            Quaternion prefabRotation = CellPrefab.transform.rotation;
            for(int xCoord = -Mathf.FloorToInt(BoardWidth / 2.0f), xIndex = 0; xIndex < BoardWidth; ++xCoord, ++xIndex) {
                for(int zCoord = -Mathf.FloorToInt(BoardHeight / 2.0f), zIndex = 0; zIndex < BoardHeight; ++zCoord, ++zIndex) {
                    GameObject newCellGameObject = GameObject.Instantiate(CellPrefab, new Vector3(xCoord, 0, zCoord), prefabRotation, transform);
                    newCellGameObject.name = "CellHybrid (" + xCoord + "," + zCoord + ")";

                    MeshRendererList.Add(newCellGameObject.GetComponent<MeshRenderer>());

                    var neighborComponent = newCellGameObject.GetComponent<NeighborsH>();
                    if(!neighborComponent) {
                        neighborComponent = newCellGameObject.AddComponent<NeighborsH>();
                    }
                    NeighborList.Add(neighborComponent);

                    var cellStateComponent = newCellGameObject.GetComponent<CellStateH>();
                    if(!cellStateComponent) {
                        cellStateComponent = newCellGameObject.AddComponent<CellStateH>();
                    }

                    CellStateList.Add(cellStateComponent);
                }
            }

            for(int index = 0; index < BoardWidth * BoardHeight; ++index) {
                int xIndex = index / BoardWidth;
                int zIndex = index % BoardWidth;

                var currentNeighbor = NeighborList[index];

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

                            currentNeighbor.NList.Add(NeighborList[currentIndex]);
                        }
                    }
                }
            }
        }

        void Start() {
            Restart();
        }

        public void SetAutoSimulation(bool autoSimulation) {
            CalcNextGenSystemH.AutoSimulate = autoSimulation;
            UpdateNeighborsSystemH.AutoSimulate = autoSimulation;
        }

        public void NextGeneration() {
            CalcNextGenSystemH.SimulateNextGen = true;
            UpdateNeighborsSystemH.SimulateNextGen = true;
        }

        public void Restart() {
            for(int index = 0; index < CellStateList.Count; ++index) {
                CellStateList[index].AliveNow = (Random.value < 0.1f);
            }
            UpdateNeighborsState();
        }

        void UpdateNeighborsState() {
            for(int index = 0; index < CellStateList.Count; ++index) {
                SetColor(MeshRendererList[index], CellStateList[index].AliveNow);
            }
        }
        void Update() {
            UpdateNeighborsState();
        }

        public void SetColor(MeshRenderer meshRenderer, bool state) {
            var settings = BootstrapHybrid.Settings;

            if(state) {
                meshRenderer.sharedMaterial = settings.AliveMaterial;
            }
            else {
                meshRenderer.sharedMaterial = settings.DeadMaterial;
            }
        }
    }
}