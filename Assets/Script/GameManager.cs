using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	//----------------------------------
	//Scores of 2 players
	int playerScore1;
	int playerScore2;
	
	bool player1Turn;	//Which player is playing
	bool throwing;
	bool turnStart;
	
	Vector3 originalCameraPos;
	//Quaternion originalCameraRot;
	Vector3 tempCameraPos;
	
	GameObject currentStone;	//The moving one
	GameObject stoneObject;
	
	List<GameObject>	finishedStones;
	
	//----------------------------------
	//Score zone define
	public GameObject scoreCenter;
	//public Vector3 scoreCenter = new Vector3(-36.4f,0.0f,4.29f);
	public float scoreZone1 = 0.3f;
	public int score1 = 40;
	public float scoreZone2 = 1.22f;
	public int score2 = 30;
	public float scoreZone3 = 2.44f;
	public int score3 = 20;
	public float scoreZone4 = 3.66f;
	public int score4 = 10;

	// Use this for initialization
	void Start () {
		playerScore1 = 0;
		playerScore2 = 0;
		player1Turn = true;
		throwing = false;
		originalCameraPos = Camera.mainCamera.transform.position;
		tempCameraPos = originalCameraPos;
		//originalCameraRot = Camera.mainCamera.transform.rotation;
		//load resource
		stoneObject = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Prefab/Pref_Stone.prefab",typeof(GameObject));
		finishedStones = new List<GameObject>();
	}
	
	void OnGUI() {
        GUI.enabled = true;
		
    	GUILayout.BeginArea(new Rect(10, 10, 150,150));
    	string text = "Player 1 : " + playerScore1;
    	GUILayout.Box(text);
		text = "Player 2 : " + playerScore2;
		GUILayout.Box(text);
		if(turnStart==false)
		{
			if(GUILayout.Button("Start to throw"))
				StartTurn();
		}
		else
		{
			if(player1Turn)
				text = "Player 1's turn";
			else
				text = "Player 2's turn";
			GUILayout.Box(text);
		}
    	GUILayout.EndArea();		
    }
	
	void StartTurn()
	{
		turnStart = true;
		currentStone = (GameObject)Object.Instantiate(stoneObject);
		if(!player1Turn)
		{
			currentStone.renderer.materials[1].SetColor("_Color",Color.green);
			currentStone.GetComponent<ThrowCurling>().SetPlayer(2);
		}
		else
		{
			currentStone.renderer.materials[1].SetColor("_Color",Color.red);
			currentStone.GetComponent<ThrowCurling>().SetPlayer(1);
		}
	}
	
	void FinishTurn()
	{
		//Finish a trun
		finishedStones.Add(currentStone);
		CalculateScore();
		throwing = false;
		turnStart = false;
		Camera.mainCamera.transform.position = originalCameraPos;
		Debug.Log(""+finishedStones.Count);
		player1Turn = !player1Turn;
	}
	
	void CalculateScore()
	{
		playerScore1 = 0;
		playerScore2 = 0;
		foreach(GameObject obj in finishedStones)
		{
			float distance = Vector3.Distance(obj.transform.position,scoreCenter.transform.position);
			int score = 0;
			if(distance<scoreZone1)
				score = score1;
			else if(distance<scoreZone2)
				score = score2;
			else if(distance<scoreZone3)
				score = score3;
			else if(distance<scoreZone4)
				score = score4;
			else
				score = 0;
			
			if(obj.GetComponent<ThrowCurling>().GetPlayer() == 1)
				playerScore1 += score;
			else
				playerScore2 += score;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!throwing)
		{
			if (Input.GetKey("left"))
			{
				Vector3 pos = currentStone.transform.position;
				currentStone.transform.position = new Vector3(pos.x,pos.y,pos.z+Time.deltaTime*2.0f);
			}
			if (Input.GetKey("right"))
			{
				Vector3 pos = currentStone.transform.position;
				currentStone.transform.position = new Vector3(pos.x,pos.y,pos.z-Time.deltaTime*2.0f);
			}
			if (Input.GetKeyDown("space"))
			{
				currentStone.GetComponent<ThrowCurling>().Throw();
				throwing = true;
			}
		}
		else
		{
			if(currentStone.rigidbody.velocity.magnitude<0.1)
				FinishTurn();
			else
			{
				tempCameraPos.x = currentStone.transform.position.x-8;
				tempCameraPos.y = currentStone.transform.position.y+7;
				//tempCameraPos.z = Camera.mainCamera.transform.position.z;
				Camera.mainCamera.transform.position = tempCameraPos;
			}
		}
	}
}
