using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBehaviour : MonoBehaviour {

	private bool _visited;
	private DIRECTION _direction; 
	public enum DIRECTION
	{
		UP,
		DOWN,
		LEFT,
		RIGHT
	}
	
	// Use this for initialization
	void Start ()
	{
		this._visited = false;
	}

	public void Init(int x,int y)
	{
		this._x = x;
		this._y= y;
		gameObject.transform.position = new Vector3(this._x*(float)1.1,0,this._y*(float)1);
	}

	private int _x;
	private int _y;

	public int X
	{
		get { return _x; }
	}

	public int Y
	{
		get { return _y; }
	}

	public float PosX
	{
		get { return gameObject.transform.position.x; }
	}
	
	public float PoxY
	{
		get { return gameObject.transform.position.z; }
	}

	public void BreakRightWall()
	{
		Destroy(gameObject.transform.Find("RIGHT").gameObject);
	}
	
	public void BreakLeftWall()
	{
		Destroy(gameObject.transform.Find("LEFT").gameObject);
	}
	
	public void BreakBackWall()
	{
		Destroy(gameObject.transform.Find("BACK").gameObject);
	}
	
	public void BreakFrontWall()
	{
		Destroy(gameObject.transform.Find("FRONT").gameObject);
	}

	public void MarkAsStart()
	{
		transform.Find("BASE").GetComponent<Renderer>().material.color = Color.green;
	}
	
	public void MarkAsFinish()
	{
		transform.Find("BASE").GetComponent<Renderer>().material.color = Color.red;
	}
	
	public void MarkAsPathMember()
	{
		transform.Find("BASE").GetComponent<Renderer>().material.color = Color.cyan;
	}
	
	public void MarkAsNeutral()
	{
		transform.Find("BASE").GetComponent<Renderer>().material.color = Color.black;
	}
	
	public bool IsUpperWallRaised()
	{
		return transform.Find("FRONT") != null;
	}
        
	public bool IsLowerWallRaised()
	{
		return transform.Find("BACK") != null;
	}
        
	public bool IsLeftWallRaised()
	{
		return transform.Find("LEFT") != null;
	}
        
	public bool IsRightWallRaised()
	{
		return transform.Find("RIGHT") != null;
	}
	
	public void MarkDirection()
	{
		MarkDirection(this._direction);
	}

	
	/// <summary>
	/// Display arrow that show direction.
	/// </summary>
	/// <param name="direction"></param>
	public void MarkDirection(DIRECTION direction)
	{
		GameObject arrow = (GameObject)Instantiate(Resources.Load("Arrow_"));
		arrow.transform.position = gameObject.transform.position + new Vector3(-0, -0.49f, 0.5f);
		arrow.transform.parent = gameObject.transform;
		switch (direction)
		{
			case DIRECTION.UP : 
				arrow.transform.Rotate(0,0,180);
				break;
			case DIRECTION.LEFT : 
				arrow.transform.Rotate(0,0,270);
				break;
			case DIRECTION.RIGHT : 
				arrow.transform.Rotate(0,0,90);
				break;
		}

	}
	
	// Update is called once per frame
	void Update(){}

	public bool Visited
	{
		get { return _visited; }
		set { _visited = value; }
	}

	public DIRECTION Direction
	{
		get { return _direction; }
		set { _direction = value; }
	}
}
