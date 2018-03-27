using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabyrinthBehavior : MonoBehaviour {

	private GameObject[,] cells;
	private int size = 10;
	private int startX = -1;
	private int startY = -1;
	private Camera _playerCamera;
	private Camera _globalCamera;
	private GameObject _player;
	private GameObject _entryPoint;
	private GameObject _endPoint;
	private Stack<GameObject> path = new Stack<GameObject>();
	private List<GameObject> nogo = new List<GameObject>();
	private System.Random randomGen = new System.Random();
	
	// Use this for initialization
	void Start ()
	{
		//Create the cells with all walls raised. 
		this.cells = new GameObject[this.size,this.size];
		for (int x = 0; x < this.size; x++)
		{
			for (int y = 0; y < this.size; y++)
			{
				this.cells[x, y] = (GameObject) GameObject.Instantiate(Resources.Load("Cell"));
				this.cells[x, y].GetComponent<CellBehaviour>().Init(x, y);
				this.cells[x, y].transform.parent = gameObject.transform;
			}
		}

		
		this.UpdateExtremities();
		//Define a generation point if there is none.  
		if (this.startX < 0)
			this.startX = this.randomGen.Next(this.size);
		if (this.startY < 0)
			this.startY = this.randomGen.Next(this.size);
		
		//Break the wall to make a path. 
		this.Generate(this.startX,this.startY);

		//Define entry and exit point if there is none. 
		if (this._entryPoint == null)
		{
			this.SetEntryPoint(this.cells[this.startX, this.startY]);
		}
		else
		{
			this.SetEntryPoint(this._entryPoint);
		}
		if (this._endPoint == null)
		{
			this.SetEndPoint(this.cells[this.randomGen.Next(this.size), this.randomGen.Next(this.size)]);
		}
		else
		{
			this.SetEndPoint(this._endPoint);
		}
		
		//Create the player.
		this._player = (GameObject) GameObject.Instantiate(Resources.Load("Player"));
		this._player.GetComponent<PlayerBehaviour>().Init(this._entryPoint);
		this._player.transform.parent = gameObject.transform;

		//Create the cam that will follow the player
		GameObject cam = (GameObject) GameObject.Instantiate(Resources.Load("PlayerCam"));
		this._playerCamera = cam.GetComponent<Camera>();
		cam.GetComponent<CameraBehavior>().Init(this._player);
		cam.transform.parent = gameObject.transform;
		this._playerCamera.enabled = false;
		
		//Adjust the main camera
		this._globalCamera = Camera.main;
		this._globalCamera.transform.position = new Vector3(this.size *(2f/3f),this.size,this.size/2f);
	}

	/// <summary>
	/// Reset extremities and remove all cells. 
	/// </summary>
	public void Reset()
	{
		this._entryPoint = null;
		this._endPoint = null;
		this.startX = -1;
		this.startY = -1; 
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}
	}
	
	public void Generate(int x, int y)
	{
		GameObject currentCell = null;
		GameObject neighborCell = null; 
		Stack<GameObject> history = new Stack<GameObject>();
		     
		currentCell = this.cells[x, y];
	
		while (true)
		{
			List<GameObject> neigbors = this.GetNeighbors(currentCell);
			if (neigbors.Count > 0)
			{
				neighborCell = neigbors[randomGen.Next(neigbors.Count)];
				BreakTheWall(currentCell, neighborCell);
				currentCell.GetComponent<CellBehaviour>().Visited = true;
				neighborCell.GetComponent<CellBehaviour>().Visited = true;

				history.Push(currentCell);
				currentCell = neighborCell;
			}
			else
			{
				if (history.Count > 0)
				{
					currentCell = history.Pop();
				}
				else
				{
					break;
				}
			}
		}            
	}
	
	public void Resolve()
	{
		GameObject currentCell = null;
		GameObject neighborCell = null; 

		currentCell = this._entryPoint;
		while (true)
		{
			List<GameObject> neigbors = this.GetPathNeigbors(currentCell);
			if (neigbors.Count > 0)
			{
				path.Push(currentCell);
				neighborCell = neigbors[randomGen.Next(neigbors.Count)];
				currentCell.GetComponent<CellBehaviour>().Direction = this.getDirection(currentCell, neighborCell);
				currentCell = neighborCell;
			}
			else
			{
				if (currentCell == this._endPoint)
				{
					break;
				}
				else
				{
					nogo.Add(currentCell);
					currentCell = path.Pop();
				}  
			}
		}
		
		//Display arrows in the path cells. 
		foreach (var cell in path)
		{
			cell.GetComponent<CellBehaviour>().MarkDirection();
			this._entryPoint.GetComponent<CellBehaviour>().MarkAsStart();
		}
	}
	
	/// <summary>
	/// Break the wall between two cells. 
	/// </summary>
	/// <param name="cellA"></param>
	/// <param name="cellB"></param>
	private void BreakTheWall(GameObject cellA, GameObject cellB)
	{
		float aPosX = cellA.GetComponent<CellBehaviour>().PosX;
		float aPosY = cellA.GetComponent<CellBehaviour>().PoxY;
		float bPosX = cellB.GetComponent<CellBehaviour>().PosX;
		float bPoxY = cellB.GetComponent<CellBehaviour>().PoxY;
		if (aPosX > bPosX)
		{
			cellA.GetComponent<CellBehaviour>().BreakRightWall();
			cellB.GetComponent<CellBehaviour>().BreakLeftWall();
		}
		if (aPosX < bPosX)
		{
			cellB.GetComponent<CellBehaviour>().BreakRightWall();
			cellA.GetComponent<CellBehaviour>().BreakLeftWall();
		}
		if (aPosY > bPoxY)
		{
			cellA.GetComponent<CellBehaviour>().BreakBackWall();
			cellB.GetComponent<CellBehaviour>().BreakFrontWall();
		}
             
		if (aPosY < bPoxY)
		{
			cellB.GetComponent<CellBehaviour>().BreakBackWall();
			cellA.GetComponent<CellBehaviour>().BreakFrontWall();
		}
	}
	
	/// <summary>
	/// Get the neighbors of a cell
	/// </summary>
	/// <param name="cell"></param>
	/// <returns>List Object of neigbors cells (GameObject) </returns>
	private List<GameObject> GetNeighbors(GameObject cell)
	{
		List<GameObject> neighbors = new List<GameObject>();
		int y = cell.GetComponent<CellBehaviour>().Y;
		int x = cell.GetComponent<CellBehaviour>().X;
		//Upper neighbor. 
		if (y < this.cells.GetLength(1) - 1 && !this.cells[x,y+1].GetComponent<CellBehaviour>().Visited)
		{
			neighbors.Add(this.cells[x,y+1]);
		}
             
		//Lower neighbor. 
		if (y > 0 && !this.cells[x,y - 1].GetComponent<CellBehaviour>().Visited)
		{
			neighbors.Add(this.cells[x,y -1]);
		}
             
		//Right neighbor. 
		if (x < this.cells.GetLength(0) - 1 && !this.cells[x +1,y].GetComponent<CellBehaviour>().Visited)
		{
			neighbors.Add(this.cells[x +1, y]);
		}
             
		//Left neighbor.  
		if (x > 0 && !this.cells[x -1,y].GetComponent<CellBehaviour>().Visited)
		{
			neighbors.Add(this.cells[x -1,y]);
		}
             
		return neighbors;
	}
	
	/// <summary>
	/// Get the nested cell wich are accessible (no wall)
	/// </summary>
	/// <param name="cellGameObject"></param>
	/// <returns> List of accessible nested cells (GameObject) </returns>
	private List<GameObject> GetPathNeigbors(GameObject cellGameObject)
	{
		List<GameObject> neighbors = new List<GameObject>();
		CellBehaviour cell = cellGameObject.GetComponent<CellBehaviour>();
		int y = cell.Y;
		int x = cell.X;

		if (!cell.IsRightWallRaised())
		{
			if ( !this.path.Contains(this.cells[x - 1, y]) && !this.nogo.Contains(this.cells[x - 1, y]))
			{
				neighbors.Add(this.cells[x - 1, y]);
			}
		}
             
		if (!cell.IsLeftWallRaised())
		{
			if (!this.path.Contains(this.cells[x + 1, y]) && !this.nogo.Contains(this.cells[x + 1, y]))
			{           
				neighbors.Add(this.cells[x + 1, y]);
			}
		}
             
		if (!cell.IsUpperWallRaised())
		{
			if (!this.path.Contains(this.cells[x, y + 1]) && !this.nogo.Contains(this.cells[x, y + 1]))
			{                     
				neighbors.Add(this.cells[x, y + 1]);
			}
		}
             
		if (!cell.IsLowerWallRaised())
		{
			if ( !this.path.Contains(this.cells[x, y - 1]) && !this.nogo.Contains(this.cells[x, y - 1]))
			{           
				neighbors.Add(this.cells[x, y - 1]);
			}
		}
      
		return neighbors;
	}
	
	/// <summary>
	/// Calculate the direction to follow to go from CellA to CellB
	/// </summary>
	/// <param name="cellAGameObject"></param>
	/// <param name="cellBGameObject"></param>
	/// <returns>Direction to follow to fo from CellA to CellB</returns>
	private CellBehaviour.DIRECTION getDirection(GameObject cellAGameObject, GameObject cellBGameObject)
	{
		CellBehaviour cellA = cellAGameObject.GetComponent<CellBehaviour>();
		CellBehaviour cellB = cellBGameObject.GetComponent<CellBehaviour>();
		if (cellA.X < cellB.X)
			return CellBehaviour.DIRECTION.LEFT;
		if (cellA.X > cellB.X)
			return CellBehaviour.DIRECTION.RIGHT;
		if (cellA.Y < cellB.Y)
			return CellBehaviour.DIRECTION.UP;
             
		return CellBehaviour.DIRECTION.DOWN;
	}
	
	// Update is called once per frame
	/// <summary>
	/// Manage keys input and winning. 
	/// R : Resolve() 
	/// </summary>
	void Update()
	{
		if (Input.GetKey("r"))
			this.Resolve();

		//Check if finish point has been reached. 
		if (this._player.transform.position.x <= this._endPoint.transform.position.x + 1 &&
		    this._player.transform.position.x >= this._endPoint.transform.position.x
		    && this._player.transform.position.z <= this._endPoint.transform.position.z + 1 &&
		    this._player.transform.position.z >= this._endPoint.transform.position.z)
		{
			this._player.GetComponent<PlayerBehaviour>().MarkAsWinner();
		}

	}

	/// <summary>
	/// Manage key input 
	/// D : Switch display. 
	/// </summary>
	private void FixedUpdate()
	{
		if (Input.GetKeyDown("d"))
		{
			this._playerCamera.enabled = !this._playerCamera.enabled;
			this._globalCamera.enabled = !this._playerCamera.enabled;
		}
	}

	public void SetEntryPoint(GameObject cell)
	{
		if (this._entryPoint != null)
		{
			this._entryPoint.GetComponent<CellBehaviour>().MarkAsNeutral();
		}
		this._entryPoint = cell;
		this._entryPoint.GetComponent<CellBehaviour>().MarkAsStart();
	}
	
	public void SetEndPoint(GameObject cell)
	{
		if (this._endPoint != null)
		{
			this._endPoint.GetComponent<CellBehaviour>().MarkAsNeutral();
		}
		this._endPoint = cell;
		this._endPoint.GetComponent<CellBehaviour>().MarkAsFinish();
	}

	
	public void PlayButtonOnClick()
	{
		try
		{
			this.Reset();
			String sizeEntered = GameObject.Find("SizeField").GetComponent<InputField>().text;
			if (sizeEntered != "")
			{
				this.size = Int32.Parse(sizeEntered);
			}
			
			String xGenPoint = GameObject.Find("GenXField").GetComponent<InputField>().text;
			String yGenPoint = GameObject.Find("GenYField").GetComponent<InputField>().text;
			if (xGenPoint != "" && yGenPoint != "")
			{
				int x = Int32.Parse(xGenPoint);
				int y = Int32.Parse(yGenPoint);
				if (x < this.size && y < this.size)
				{
					this.startX = x;
					this.startY = y; 
				}
				else
				{
					throw new Exception("Valeur incorrectes pour le point de génération");
				}
			}
			this.Start();
		}
		catch (Exception exception)
		{
			//TODO Display error on UI. 
			Debug.Log(exception.Message + exception.StackTrace);
			this.Reset();
			this.Start();
		}
	}

	public void UpdateExtremities()
	{
		try
		{
			String xExitPoint = GameObject.Find("FinishXField").GetComponent<InputField>().text;
			String yExitPoint = GameObject.Find("FinishYField").GetComponent<InputField>().text;
			if (xExitPoint != "" && yExitPoint != "")
			{
				int x = Int32.Parse(xExitPoint);
				int y = Int32.Parse(yExitPoint);
				if (x < this.size && y < this.size)
				{
					this.SetEndPoint(this.cells[x, y]);
				}
				else
				{
					throw new Exception("Valeur incorrectes pour le point de départ");
				}
			}

			String xStartPoint = GameObject.Find("StartXField").GetComponent<InputField>().text;
			String yStartPoint = GameObject.Find("StartYField").GetComponent<InputField>().text;
			if (xStartPoint != "" && yStartPoint != "")
			{
				int x = Int32.Parse(xStartPoint);
				int y = Int32.Parse(yStartPoint);
				if (x < this.size && y < this.size)
				{
					this.SetEntryPoint(this.cells[x, y]);
				}
				else
				{
					throw new Exception("Valeur incorrectes pour le point d'arrivée");
				}
			}
		}
		catch (Exception exception)
		{
			Debug.Log(exception.Message);
		}
	}
	public void ResolveButtonOnClick()
	{
		this.Resolve();
	}
	
}
