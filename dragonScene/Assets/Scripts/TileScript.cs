using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour 
{
	
	public Point GridPosition { get; private set; }
	
	public bool IsEmpty { get; private set; }
	
	private Color32 fullColor = new Color32(255, 118, 118, 255);
	
	private Color32 emptyColor = new Color32(96, 255, 90, 255);
	
	private SpriteRenderer spriteRenderer;
	
	public bool Walkable { get; set; }
	
	public bool Debugging { get; set; }
	
	public Vector2 WorldPosition
	{
		get
		{
				return new Vector2(transform.position.x + (GetComponent<SpriteRenderer>().bounds.size.x/2), transform.position.y - (GetComponent<SpriteRenderer>().bounds.size.y/2));
		}
	}

	// Use this for initialization
	void Start () 
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Setup(Point gridPos, Vector3 worldPos, Transform parent)
	{
		Walkable = true;
		IsEmpty = true;
		this.GridPosition = gridPos;
		transform.position = worldPos;
		transform.SetParent(parent);		
		Completed.BoardManager.Instance.Tiles.Add(gridPos, this);
		
	}
	/*
	private void OnMouseOver()
	{
		
		if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn != null)
		{
			if(IsEmpty && !Debugging)
			{
				ColorTile(emptyColor);
			}
			if (!IsEmpty && !Debugging)
			{
				ColorTile(fullColor);
			}
			else if (Input.GetMouseButtonDown(0))
			{
				PlaceTower();
			}
		}
	}
	*/
	private void OnMouseExit()
	{
		if (!Debugging)
		{
			ColorTile(Color.white);
		}
	}
	/*
	private void PlaceTower()
	{
		Instantiate(GameManager.Instance.ClickedBtn.TowerPrefab, transform.position, Quaternion.identity);
		
		IsEmpty = false;
		
		ColorTile(Color.white);
		GameManager.Instance.BuyTower();
		Walkable = false;
	}
	*/
	
	private void ColorTile(Color newColor)
	{
		spriteRenderer.color = newColor;
	}
	
}
