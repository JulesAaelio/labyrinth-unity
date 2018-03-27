using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
 	private GameObject _target;
	private Vector3 _offset;
	
	// Use this for initialization
	void Start () {}

	public void Init(GameObject target)
	{
		this._target = target;
		this._offset = Vector3.forward;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (this._target != null)
		{
			transform.position = this._target.transform.position - this._offset + new Vector3(0,2,0);
			transform.rotation = new Quaternion(transform.rotation.x,
				this._target.transform.rotation.y, transform.rotation.z,1);
			transform.LookAt(this._target.transform);
		}
	}
}
