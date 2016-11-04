using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public static class AStar
{
	private static Dictionary<Point, Node> nodes;
	
	private static void CreateNodes()
	{
		nodes = new Dictionary<Point, Node>();
		
		foreach (TileScript tile in layoutmanager.Instance.Tiles.Values)
		{
			nodes.Add(tile.GridPosition, new Node(tile));
		}
	}
	
	public static Stack<Node> GetPath(Point start, Point goal)
	{
		if (nodes == null)
		{
			CreateNodes();
		}
		
		//openList for AStar algorithm
		HashSet<Node> openList = new HashSet<Node>();
		
		//closedList for AStar algorithm
		HashSet<Node> closedList = new HashSet<Node>();
		
		Stack<Node> finalPath = new Stack<Node>();
		
		//finds start node, sets to currentNode
		Node currentNode = nodes[start];
		
		//add start node to openList
		openList.Add(currentNode);
		
		while(openList.Count > 0)
		{
			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					Point neighborPos = new Point(currentNode.GridPosition.x - x, currentNode.GridPosition.y - y);
				
					if(layoutmanager.Instance.Inbounds(neighborPos) && layoutmanager.Instance.Tiles[neighborPos].Walkable && neighborPos != currentNode.GridPosition)
					{
						int gCost = 0;
					
						if (Math.Abs(x-y) == 1)
						{
							gCost = 10;
						}
						else 
						{
							if(!ConnectedDiag(currentNode, nodes[neighborPos]))
							{
								continue;
							}
							gCost = 14;
						}
					
						//adds neighbor to openList
						Node neighbor = nodes[neighborPos];
					
						if (openList.Contains(neighbor))
						{
							if(currentNode.G + gCost < neighbor.G)
							{
								neighbor.CalcValues(currentNode, nodes[goal], gCost);
							}
						}
					
						else if (!closedList.Contains(neighbor))
						{
							openList.Add(neighbor);
							neighbor.CalcValues(currentNode, nodes[goal], gCost);
						}					
					}				
				}
			}
		
			//moves currentNode from openList to closedList
			openList.Remove(currentNode);
			closedList.Add(currentNode);
		
			if(openList.Count > 0)
			{
				//sorts list by F value, first is lowest
				currentNode = openList.OrderBy(n => n.F).First();
			}
			
			if(currentNode == nodes[goal])
			{
				while(currentNode.GridPosition != start)
				{
					finalPath.Push(currentNode);
					currentNode = currentNode.Parent;
				}
				break;
			}
		}
		
		return finalPath;
		
		//debugging, remove later		
		//GameObject.Find("Debugger").GetComponent<Debugger>().DebugPath(openList, closedList, finalPath);
	}
	
	
	private static bool ConnectedDiag(Node currentNode, Node neighbor)
	{
		Point direction = neighbor.GridPosition - currentNode.GridPosition;
		
		Point first = new Point(currentNode.GridPosition.x + direction.x, currentNode.GridPosition.y);
		
		Point second = new Point(currentNode.GridPosition.x, currentNode.GridPosition.y + direction.y);
		
		if(layoutmanager.Instance.Inbounds(first) && !layoutmanager.Instance.Tiles[first].Walkable)
		{
			return false;
		}
		
		if(layoutmanager.Instance.Inbounds(second) && !layoutmanager.Instance.Tiles[second].Walkable)
		{
			return false;
		}
		
		return true;
	}
	
	
}
