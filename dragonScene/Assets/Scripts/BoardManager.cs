using UnityEngine;
using System.Collections;
using System.Collections.Generic;        //so we can use lists
using Random = UnityEngine.Random;      //Tells random to use unity random generator

namespace Completed
{
    public class BoardManager : MonoBehaviour
    {
       
        public static int columns = 50;
        public static int rows = 50;
        int simulations = 7;
        float chanceCellIsOnPath = 0.4f;
        public GameObject[] floorTiles;
        public GameObject[] obstacleTiles;

        private Transform boardHolder;
        private List<Vector3> gridPositions = new List<Vector3>();
        private List<bool> liveCells = new List<bool>();
        private bool [,] gridPath = new bool[columns, rows];

        //initializes game of life grid
        void InitializeGrid()
        {
            for (int x = 0; x < columns; x++)
            {
                for(int y = 0; y < rows; y++)
                {
                    //get random float between 0 and 1
                    float rand = Random.Range(0.0f, 1.0f);
                    //compare float to chance cell is on path
                    //if rand is less than chance, set to true
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
            //OutputGridToConsole();
        }

        //create game of life grid
        void GameOfLifeSim()
        {
            InitializeGrid();
            //run until all simulations are done
            while (simulations > 0)
            {
                //create second temp grid  
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
                //OutputGridToConsole();
                //decrement simulations
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
                    else if(x_neighbor < 0 || y_neighbor < 0 || x_neighbor >= columns - 1 || y_neighbor >= rows - 1){
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
                        if(liveNeighbors < 4)
                        {
                            newGrid[x, y] = false;
                        }
                        //if cell has less than 4 live neighbors keep it alive
                        else
                        {
                            newGrid[x, y] = true;
                        }
                    }
                    //if dead
                    else {  
                        //if cell is dead and it has less than three live neighbors lazarus
                        if (liveNeighbors > 3)
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

        /*
        void OutputGridToConsole()
        {
            int count = 0;
            for (int x = 1; x < columns; x++)
            {
                for (int y = 1; y < rows; y++)
                {
                    if (gridPath[x, y])
                    {
                        count = count + 1;
                        //print(".  ");
                    }
                    else
                    {
                        //print("X  ");
                    }
                }
                //print("\r\n");
            }
            print(count);
        }
        */

        //create game board list data structure and list of same size with values set to true or false based on game of life gird
        void InitializeList()
        {
            GameOfLifeSim();
            gridPositions.Clear();
            int count = 0;
            for (int x = 1; x < columns; x++)
            {
                for (int y = 1; y < rows; y++)
                {
                    //create grid position list of vector3 elements
                    gridPositions.Add(new Vector3(x, y, 0f));
                    //if gridPath item is true set list value to true
                    if (gridPath[x,y] == true)
                    {
                        liveCells.Add(true);
                        count = count + 1;
                    }
                    else
                    {
                        liveCells.Add(false);
                    }
                }
            }
            print("Live Count " + count);
        }

        //create floor of game board with random floor tiles
        void BoardSetup()
        {
            boardHolder = new GameObject("Board").transform;

            for (int x = 1; x < columns; x++)
            {
                for (int y = 1; y < rows; y++)
                {
                    //set random floor tiles to gameObjects with parent being the board holder
                    GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                    GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(boardHolder);
                }
            }
        }

        
        //add random object to boardholder as obstacles
        void LayoutObject(GameObject[] tileArray)
        {
            int count = 0;
            int liveCount = 0;
            //for each item in grid positions list
            for (int x = 0; x < gridPositions.Count; x++)
            {
                //get position
                Vector3 position = gridPositions[x];
                //if cell in grid at position is dead then place obstacle
                if (!liveCells[x])
                {
                    liveCount = liveCount + 1;
                    GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
                    Instantiate(tileChoice, position, Quaternion.identity);
                }
                else
                {
                    count = count + 1;
                }
                //gridPositions.RemoveAt(x);
                //liveCells.RemoveAt(x);
            }
            print("Final Count " + count);
            print("Dead Count " + liveCount);
        }


        public void SetupScene(int level)
        {
            //create floor
            BoardSetup();
            //initialize grid and set to liveCells then set up gridpositions list
            InitializeList();
            //add obstacles
            LayoutObject(obstacleTiles);
        }
    }
}