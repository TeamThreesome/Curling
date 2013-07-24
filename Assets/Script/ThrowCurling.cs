using UnityEngine;
using System.Collections;

public class ThrowCurling : MonoBehaviour {
	
	public float mForce = 250;
	public float mRange = 50;
	int playerID;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public void SetPlayer(int id){
		playerID = id;
	}
	
	public int GetPlayer(){
		return playerID;
	}
	
	public void Throw () {
		mForce = Random.Range(mForce-mRange,mForce+mRange);
		this.rigidbody.AddForce(mForce,0,0,ForceMode.VelocityChange);
	}

	void OnCollisionEnter(Collision collision) {
		if(collision.rigidbody != null)
		{
			collision.rigidbody.useGravity = true;
			this.rigidbody.useGravity = true;
		}
	}
}
