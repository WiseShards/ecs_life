using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace classic {
    public class BoardManagerClassic : MonoBehaviour {
        public int BoardHeight = 10;
        public int BoardWidth = 10;
        public GameObject CellPrefab;

        private static CellClassic[] boardCells;

        void Awake() {
            boardCells = new CellClassic[BoardWidth * BoardHeight];
            Quaternion prefabRotation = CellPrefab.transform.rotation;
            for(int xCoord = -Mathf.FloorToInt(BoardWidth / 2.0f), xIndex = 0; xIndex < BoardWidth; ++xCoord, ++xIndex) {
                for(int zCoord = -Mathf.FloorToInt(BoardHeight / 2.0f), zIndex = 0; zIndex < BoardHeight; ++zCoord, ++zIndex) {
                    GameObject newCellGameObject = GameObject.Instantiate(CellPrefab, new Vector3(xCoord, 0, zCoord), prefabRotation, transform);
                    newCellGameObject.name = "CellClassic (" + xCoord + "," + zCoord + ")";
                    CellClassic newCell = newCellGameObject.GetComponent<CellClassic>();
                    boardCells[xIndex * BoardWidth + zIndex] = newCell;
                }
            }

            Restart();

            for(int index = 0; index < BoardWidth * BoardHeight; ++index) {
                int xIndex = index / BoardWidth;
                int zIndex = index % BoardWidth;

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
                            boardCells[index].AddNeighbor(boardCells[currentIndex]);
                        }
                    }
                }
            }
        }

        public void SetAutoSimulation(bool autoSimulation) {
            CellClassic.ContinuousSimulation = autoSimulation;
        }

        public void NextGeneration() {
            CellClassic.ComputeNextSimulation = BoardHeight * BoardWidth;
        }

        public void Restart() {
            for(int index = 0; index < BoardWidth * BoardHeight; ++index) {
                bool cellState = (Random.value < 0.1f);
                boardCells[index].SetState(cellState);
            }
        }
    }
}