using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;        //so we can use lists
using Random = UnityEngine.Random;      //Tells random to use unity random generator

namespace Completed
{
	public class BoardManager : Singleton<BoardManager>
    {
       
        public static int columns = 150;
        public static int rows = 50;
        int simulations = 7;
        float chanceCellIsOnPath = 0.40f;
        //public GameObject[] floorTiles;
        public GameObject[] obstacleTiles;
        //public GameObject[] wallObstacles;

        //private Transform boardHolder;
        //private List<Vector3> gridPositions = new List<Vector3>();
        private List<Point> gridPositions = new List<Point>();
        private List<bool> liveCells = new List<bool>();
		private List<Point> deadCells = new List<Point>();
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
                    else if(x_neighbor < 0 || y_neighbor < 0 || x_neighbor >= columns || y_neighbor >= rows)
                    {
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
                        //if cell has less than 3 live neighbors keep it alive
                        else //if (liveNeighbors < 4)
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

        //create game board list data structure and list of same size with values set to true or false based on game of life gird
        void InitializeList()
        {
            gridPositions.Clear();
            int count = 0;
            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    //create grid position list of vector3 elements
                    //gridPositions.Add(new Vector3(x, y, 0f));
                    //gridPositions.Add(new Point(x, y));
                    //if gridPath item is true set list value to true
                    if (gridPath[x,y] == true)
                    {
                        liveCells.Add(true);
                        
                    }
                    else
                    {
                        liveCells.Add(false);
						deadCells.Add(new Point(x, y));
						count = count + 1;
                    }
                }
            }
            print("Live Count " + count);
        }

        void createTextFile()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("Level.txt");
            for (int x = 0; x < rows; x++)
            {
                string rowx = "";
                for (int y = 0; y < columns; y++)
                {
                    //set random floor tiles to gameObjects with parent being the board holder
                    if (x == 0 || y == 0 || x == rows - 1 || y == columns - 1)
                    {
                        //write random number between 0 and wallObstacles.length to file
                        rowx = string.Concat(rowx + "4");
                    }
                    else
                    {

                        rowx = string.Concat(rowx + (Random.Range(0, 3)).ToString());
                    }
                }
                rowx = string.Concat(rowx, "-");
                file.WriteLine(rowx);
            }
            file.Close();
        }


		//read text file
		private string[] ReadLevelText()
		{
			//save data from file to binddata
			TextAsset bindData = Resources.Load("Level") as TextAsset;
			//replace newLine characters with an empty string
			string data = bindData.text.Replace(Environment.NewLine, string.Empty);
			//split the data into an array using the '-' delimiter
			return data.Split('-');
		}
        



		//creat dictionary of TileScript each with the key coordinates 
		public Dictionary<Point, TileScript> Tiles { get; set; }

		//holds maximum x and y coordinates
		private Point mapSize;

		//Camera Movement Variable
		[SerializeField]
		private CameraMovement cameraMovement;

		//create spawn portal object
		public Portal SpawnPortal { get; set; }

		//coordinates for spawn point
		private Point spawn;

		//coordinates fo end point
		private Point ending;

		//add spawns object
		[SerializeField]
		private GameObject spawns;

		//add ending object
		[SerializeField]
		private GameObject endings;
		//Transform tool is map
		[SerializeField]
		private Transform map;

		//prefabs of tiles for floor
		[SerializeField]
		private GameObject[] tilePrefabs;

		//returns width of the box of a sprite
		public float TileSize
		{
			get {return tilePrefabs[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x; }
		}


		private void CreateLevel()
		{
			//create dictionary to hold map coordinates and TileScript
			Tiles = new Dictionary<Point, TileScript>();

			//read level from file to mapData array
			string[] mapData = ReadLevelText();

			//set map size to the size of the max x and y coordinates of the graph
			mapSize = new Point(mapData[0].ToCharArray().Length, mapData.Length);

			//set mapX (number of columns) by changing string to char array and taking length
			int mapX = mapData[0].ToCharArray().Length;
			//set mapY (number of rows) to length of mapData array
			int mapY = mapData.Length;

			//set maxtile to (0,0,0)
			Vector3 maxTile = Vector3.zero;

			//calculate start point, top left corner
			Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));

			//for each row
			for (int y = 0; y < mapY - 1; y++)
			{
				//separate each row into individual characters
				char[] newTiles = mapData[y].ToCharArray();

				//for each column
				for (int x = 0; x < mapX - 1; x++)
				{
					//place tile on map
					PlaceTile(newTiles[x].ToString(),x,y, worldStart);
				}
			}

			//set maxTile to the tile at the bottom corner of the map
			//maxTile = Tiles[ new Point (mapX - 1, mapY - 1)].transform.position;

			//set limit of camera movement to vector3 0,0,0 plus 
			//cameraMovement.SetLimits(new Vector3(maxTile.x + TileSize, maxTile.y - TileSize));

			//Spawn();
		}


		//function to place tile on map
		private void PlaceTile(string tileType, int x, int y, Vector3 worldStart)
		{
			//cast tile to tileIndex
			int tileIndex = int.Parse(tileType);

			//create new tile as tileIndex
			TileScript newTile = Instantiate(tilePrefabs[tileIndex]).GetComponent<TileScript>();
			Point next = new Point (x, y);
			Vector3 worldSpot = new Vector3 ((worldStart.x + (TileSize * x)), (worldStart.y - (TileSize * y)),0);

			newTile.Setup (next, worldSpot, map);		

			//set position of tile at point x,y in the world at the vector3 position (worldstart + x, worldstart + y, 0), making the Transform map the parent
		}

		//place spawn point on map
		private void Spawn()
		{
			//set spawn point
			spawn = new Point (1, 5);

			//place spawn point sprite on map
			GameObject tmp = (GameObject)Instantiate(spawns, Tiles[spawn].GetComponent<TileScript>().WorldPosition, Quaternion.identity);

			//initialize portal
			SpawnPortal = tmp.GetComponent<Portal>();
			SpawnPortal.name = "teleporter-small_31";


			//set ending spawn point
			ending = new Point (15, 1);

			//place ending spawn point on map
			Instantiate(endings, Tiles[ending].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
		}


		public bool Inbounds(Point position)
		{
			return position.x >= 0 && position.y >= 0 && position.x < mapSize.x && position.y < mapSize.y;
		}

		private void addObstacles(){
			int deadCount = 0;
			for (int x = 0; x < deadCells.Count; x++)
			{
				print ("Tiles: " + x);
				if (Tiles.ContainsKey(deadCells[x])) 
				{
					deadCount++;
					GameObject tileChoice = obstacleTiles [Random.Range (0, obstacleTiles.Length)];
					Instantiate (tileChoice, Tiles [deadCells [x]].WorldPosition, Quaternion.identity);
				
				}
			}
			print ("DeadCells: " + deadCount);
		}


        //create floor of game board with random floor tiles
        void BoardSetup()
        {
            
            /*boardHolder = new GameObject("Board").transform;
            for (int x = -1; x <= columns + 1; x++)
            {
                for (int y = -1; y <= rows + 1; y++)
                {
                    //set random floor tiles to gameObjects with parent being the board holder
                    GameObject toInstantiate;
                    //create walls
                    if (x == -1 || y == -1 || x == columns + 1 || y == rows + 1)
                    {
                        //write random number between 0 and wallObstacles.length to file
                        toInstantiate = wallObstacles[Random.Range(0, wallObstacles.Length)];
                    }
                    //write random number between 1 and 4 to text file
                    else
                    {
                        toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                        
                    }
                  
                    GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(boardHolder);
                }
           }*/
			//CreateLevel ();
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
                int i = gridPositions[x].x;
                int j = gridPositions[x].y;
                Vector3 pos = new Vector3(i, j, 0f);


                //Vector3 position = gridPositions[x];
                //Point pos = gridPositions[x];
                //if cell in grid at position is dead then place obstacle
                if (!liveCells[x])
                {
                    liveCount = liveCount + 1;
                    GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
                    Instantiate(tileChoice, pos, Quaternion.identity);
                }
                else
                {
                    count = count + 1;
                }
            }
            print("Final Count " + count);
            print("Dead Count " + liveCount);
        }


		void Start(){
			GameOfLifeSim ();
			createTextFile ();
			CreateLevel ();
			InitializeList ();
			addObstacles ();
			//LayoutObject (obstacleTiles);		
		}

        /*public void SetupScene(int level)
        {
            //create grid 
            GameOfLifeSim();
            //create floor
            createTextFile();
            //BoardSetup();
			CreateLevel();
            //initialize grid and set to liveCells then set up gridpositions list
            //InitializeList();
            //add obstacles
            //LayoutObject(obstacleTiles);
        }*/
    }
}