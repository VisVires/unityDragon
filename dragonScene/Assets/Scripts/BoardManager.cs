using UnityEngine;
using System.Collections;
using System.Collections.Generic;        //so we can use lists
using Random = UnityEngine.Random;      //Tells random to use unity random generator

namespace Completed
{
    public class BoardManager : MonoBehaviour
    {
       
        public static int columns = 35;
        public static int rows = 35;
        int simulations = 5;
        float chanceCellIsOnPath = 0.45f;
        public GameObject[] floorTiles;
        public GameObject[] obstacleTiles;

        private Transform boardHolder;
        private List<Vector3> gridPositions = new List<Vector3>();
        private List<bool> liveCells = new List<bool>();
        private bool [,] gridPath = new bool[columns, rows];

        void InitializeGrid()
        {
            for (int x = 0; x < columns; x++)
            {
                for(int y = 0; y < rows; y++)
                {
                    float rand = Random.Range(0.0f, 1.0f);
                    if (rand < chanceCellIsOnPath)
                    {
                        gridPath[x, y] = true;
                    }
                    else
                    {
                        gridPath[x, y] = false;
                    }
                }
            }
        }

        void GameOfLifeSim()
        {
            InitializeGrid();
            while (simulations > 0)
            {
                bool[,] newGrid = new bool[columns, rows];
                //populate newGrid
                newGrid = populateNewGrid(newGrid);
                //replace old grid
                for (int x = 0; x < columns; x++)
                {
                    for (int y = 0; y < rows; y++)
                    {
                        //replace each position in gridPath with updated position
                        gridPath[x, y] = newGrid[x, y];
                    }
                }
                simulations--;
            }
        }

        //count number of live neighbors
        int CountLiveNeighbors(int x, int y)
        {
            //keep count
            int liveNeighbors = 0;
            //move through x = -1 to x = 1
            for (int i = -1; i < 2; i++)
            {
                //move through y = -1 to y = 1
                for (int j = -1; j < 2; j++)
                {
                    //get x and y coords of neighbors
                    int x_neighbor = x + i;
                    int y_neighbor = y + j;
                    //if at the home cell ignore
                    if (i == 0 && j == 0)
                    {
                        //do nothing
                    }
                    // check if neighboring cell is off the map or at edge
                    else if(x_neighbor < 1 || y_neighbor < 1 || x_neighbor >= columns - 1 || y_neighbor >= rows - 1){
                        liveNeighbors = liveNeighbors + 1;
                    }
                    //check if neighbor is alive
                    else if(gridPath[x_neighbor, y_neighbor]){
                        liveNeighbors = liveNeighbors + 1;
                    }
                }
            }
            //return number of live neighbors, or edge neighbors
            return liveNeighbors;
        }


        //populate new grid for next simulation
        bool [,] populateNewGrid(bool [,] newGrid)
        {
            //for each column
            for(int x = 1; x < columns; x++)
            {
                //and each row
                for(int y = 1; y < rows; y++)
                {
                    //count live neighbors
                    int liveNeighbors = CountLiveNeighbors(x, y);
                    //if cell is alive
                    if (gridPath[x, y])
                    {
                        //if cell has more than 4 live neighbors kill it
                        if(liveNeighbors > 3)
                        {
                            newGrid[x, y] = false;
                        }
                        //if cell has less than 4 live neighbors keep it alive
                        else
                        {
                            newGrid[x, y] = true;
                        }
                        /*
                        if (liveNeighbors < 2)
                        {
                            newGrid[x, y] = false;
                        }
                        //if cell alive and live neighbors equal to 2 or 3 keep cell alive
                        else if (liveNeighbors == 2 || liveNeighbors == 3)
                        {
                            newGrid[x, y] = true;
                        }
                        //if cell is alive and there are more than 3 live neighbors kill cell
                        else if (liveNeighbors > 3)
                        {
                            newGrid[x, y] = false;
                        }
                        */
                    }
                    //if dead
                    else {  
                        //if cell is dead and it has less than four live neighbors lazarus
                        if (liveNeighbors < 3)
                        {
                            newGrid[x, y] = true;
                        }
                        //if cell is dead and has 4 or more live neighbors keep it dead 
                        else
                        {
                            newGrid[x, y] = false;
                        }

                    }
                }
            }
            //return updated and populated new grid
            return newGrid;
        }



        void InitializeList()
        {
            GameOfLifeSim();
            gridPositions.Clear();
            for (int x = 1; x < columns; x++)
            {
                for (int y = 1; y < rows; y++)
                {
                    gridPositions.Add(new Vector3(x, y, 0f));
                    if (gridPath[x,y] == true)
                    {
                        liveCells.Add(true);
                    }
                    else
                    {
                        liveCells.Add(false);
                    }
                }
            }
        }

        void BoardSetup()
        {
            boardHolder = new GameObject("Board").transform;

            for (int x = 1; x < columns; x++)
            {
                for (int y = 1; y < rows; y++)
                {
                    GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                    GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(boardHolder);
                }
            }
        }

        

        void LayoutObject(GameObject[] tileArray)
        {
            for (int x = 0; x < gridPositions.Count; x++)
            {
                Vector3 position = gridPositions[x];
                if (liveCells[x])
                {
                    GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
                    Instantiate(tileChoice, position, Quaternion.identity);
                }
            }
        }


        public void SetupScene(int level)
        {
            BoardSetup();
            InitializeList();
            LayoutObject(obstacleTiles);
            

        }
    }
}