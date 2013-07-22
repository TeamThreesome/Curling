using UnityEngine;
using System.Collections;

public class ThrowCurling : MonoBehaviour {
	
	public float mForce = 250;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("space"))
		{
			this.rigidbody.AddForce(mForce,0,0,ForceMode.Acceleration);
		}
	}
}
