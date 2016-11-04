using UnityEngine;
using System.Collections.Generic;
using System;

public class layoutmanager : Singleton<layoutmanager> 
{
	[SerializeField]
	private GameObject[] tilePrefabs;
	
	[SerializeField]
	private CameraMovement cameraMovement;
	
	[SerializeField]
	private Transform map;
	
	private Point spawn;
	private Point ending;
	
	[SerializeField]
	private GameObject spawns;
	
	[SerializeField]
	private GameObject endings;
	
	public Portal SpawnPortal { get; set; }
	
	private Point mapSize;
	
	private Stack<Node> path;
	
	public Stack<Node> Path
	{
		get 
		{
			if(path == null)
			{
				GeneratePath();
			}
		
			return new Stack<Node>(new Stack<Node>(path));
		}
	}
	
	public Dictionary<Point, TileScript> Tiles { get; set; }
	
	public float TileSize
	{
		get {return tilePrefabs[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x; }
	}

	// Use this for initialization
	void Start () 
	{
		
		CreateLevel();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	
	private void CreateLevel()
	{
		
		Tiles = new Dictionary<Point, TileScript>();
		string[] mapData = ReadLevelText();
		
		mapSize = new Point(mapData[0].ToCharArray().Length, mapData.Length);
		
		int mapX = mapData[0].ToCharArray().Length;
		int mapY = mapData.Length;
		
		Vector3 maxTile = Vector3.zero;
	
		//calculate start point, top left corner
		Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));
		for (int y=0; y<mapY; y++)
		{
			char[] newTiles = mapData[y].ToCharArray();
			
			for (int x=0; x<mapX; x++)
			{
				PlaceTile(newTiles[x].ToString(),x,y,worldStart);
			}
		}
		
		maxTile = Tiles[ new Point (mapX - 1, mapY - 1)].transform.position;
		
		cameraMovement.SetLimits(new Vector3(maxTile.x + TileSize, maxTile.y - TileSize));
		
		Spawn();
	}
	
	private void PlaceTile(string tileType, int x, int y, Vector3 worldStart)
	{
		int tileIndex = int.Parse(tileType);
		
		//create new tile
		TileScript newTile = Instantiate(tilePrefabs[tileIndex]).GetComponent<TileScript>();
				
		//change position of tile
		newTile.Setup(new Point(x, y), new Vector3(worldStart.x + (TileSize*x), worldStart.y - (TileSize*y), 0), map);
		
	}
	
	private string[] ReadLevelText()
	{
		TextAsset bindData = Resources.Load("Level") as TextAsset;
		
		string data = bindData.text.Replace(Environment.NewLine, string.Empty);
		
		return data.Split('-');
	}
	
	private void Spawn()
	{
		//set spawn point
		spawn = new Point (1, 5);	
		GameObject tmp = (GameObject)Instantiate(spawns, Tiles[spawn].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
		SpawnPortal = tmp.GetComponent<Portal>();
		SpawnPortal.name = "teleporter-small_31";
		
		
		//set end point
		ending = new Point (15, 1);
		Instantiate(endings, Tiles[ending].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
	}
	
	public bool Inbounds(Point position)
	{
		return position.x >= 0 && position.y >= 0 && position.x < mapSize.x && position.y < mapSize.y;
	}
	
	public void GeneratePath()
	{
		path = AStar.GetPath(spawn, ending);
	}
	
	
}