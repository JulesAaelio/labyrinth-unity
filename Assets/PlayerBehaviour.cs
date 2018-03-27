using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

	// Use this for initialization
	private Rigidbody _rb;
	void Start ()
	{
		_rb = GetComponent<Rigidbody>();
	}

	/// <summary>
	/// Place the player on the right cell. 
	/// </summary>
	/// <param name="cell"></param>
	public void Init(GameObject cell)
	{
		gameObject.transform.position = cell.transform.position + new Vector3(0,0.2f,0);
	}

	/// <summary>
	/// Color the player in green. 
	/// </summary>
	public void MarkAsWinner()
	{
		GetComponent<Renderer>().material.color = Color.green;
	}
	
	// Update is called once per frame
	/// <summary>
	/// Manage user input to move the player object. 
	/// </summary>
	void Update()
	{
		if (Input.GetKey("left"))
			_rb.AddForce(new Vector3(-1, 0, 0) *2 );
		if (Input.GetKey("right"))
			_rb.AddForce(new Vector3(1, 0, 0) *2 );
		if (Input.GetKey("up"))
			_rb.AddForce(new Vector3(0, 0, 1) * 2);
		if (Input.GetKey("down"))
			_rb.AddForce(new Vector3(0, 0, -1) * 2);
	}
}
