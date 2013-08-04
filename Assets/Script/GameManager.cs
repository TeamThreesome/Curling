using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	//----------------------------------
	//Scores of 2 players
	int playerScore1 = 0;
	int playerScore2 = 0;

	public int roundsOfGame = 10;
	public int curlingOfRound = 8;
	int rounded = 1;
	int curled = 1;

	bool player1Turn = false;	//Which player is first throwing in this turn
	bool player1Throw = false;	//Which player is throwing
	bool throwing = false;		//If the stone is flying
	bool turnStart = false;		//If the player is ready to throwing
	bool readyToReleaseCurl = false;	//true when left button hold down
	
	Vector3 originalCameraPos;
	//Quaternion originalCameraRot;
	Vector3 tempCameraPos;
	Vector3 startPoint;
	Vector3 mousePoint;
	
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
		originalCameraPos = Camera.mainCamera.transform.position;
		tempCameraPos = originalCameraPos;
		//originalCameraRot = Camera.mainCamera.transform.rotation;
		//load resource
		stoneObject = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Prefab/Pref_Stone.prefab",typeof(GameObject));
		startPoint = stoneObject.transform.position;
		finishedStones = new List<GameObject>();
	}
	
	void OnGUI() {
        GUI.enabled = true;
		
    	GUILayout.BeginArea(new Rect(10, 10, 150,150));
    	string text = "Round : " + rounded;
    	GUILayout.Box(text);
    	text = "Turn : " + curled;
    	GUILayout.Box(text);
    	text = "Player 1 : " + playerScore1;
    	GUILayout.Box(text);
		text = "Player 2 : " + playerScore2;
		GUILayout.Box(text);
		if(turnStart==false)
		{
			if(GUILayout.Button("Start to throw"))
				StartThrow();
		}
		else
		{
			if(player1Throw)
				text = "Player 1's turn";
			else
				text = "Player 2's turn";
			GUILayout.Box(text);
		}
    	GUILayout.EndArea();		
    }

    void FinishRound()
    {
    	CalculateScore();
    	//Clear all the stone in this round
    	foreach(GameObject stone in finishedStones)
    	{
	    	Object.Destroy(stone);
    	}
		finishedStones.Clear();
	    //Check if game is finished
    	if(rounded==roundsOfGame)
    		Debug.Log("Game is finished");

    	//Change the throwing order
    	player1Turn = !player1Turn;
    	player1Throw = player1Turn;
    	rounded++;	//next round
    	curled=1;
    }

	void FinishTurn()
	{
		if(curled==curlingOfRound)
			FinishRound();
		curled++;
	}

	void StartThrow()
	{
		turnStart = true;
		currentStone = (GameObject)Object.Instantiate(stoneObject);
		if(!player1Throw)
		{
			currentStone.renderer.materials[1].SetColor("_Color",Color.yellow);
			currentStone.GetComponent<ThrowCurling>().SetPlayer(2);
		}
		else
		{
			currentStone.renderer.materials[1].SetColor("_Color",Color.red);
			currentStone.GetComponent<ThrowCurling>().SetPlayer(1);
		}
	}
	
	void FinishThrow()
	{
		//Finish a trun
		if(currentStone!=null)
			finishedStones.Add(currentStone);
		//CalculateScore();
		throwing = false;
		turnStart = false;
		Camera.mainCamera.transform.position = originalCameraPos;
		//Debug.Log(""+finishedStones.Count);
		player1Throw = !player1Throw;
		//if back to player1 then that means new turn
		if(player1Throw==player1Turn)
			FinishTurn();
	}
	
	void CalculateScore()
	{
		int whoScored;
		float minPlayer1 = 100.0f;
		float minPlayer2 = 100.0f;
		foreach(GameObject obj in finishedStones)
		{
			float distance = Vector3.Distance(obj.transform.position,scoreCenter.transform.position);

			if(obj.GetComponent<ThrowCurling>().GetPlayer() == 1)
				if(distance<minPlayer1)
					minPlayer1 = distance;
			else
				if(distance<minPlayer2)
					minPlayer2 = distance;
		}

		if(minPlayer1<minPlayer2)
			whoScored = 1;
		else if(minPlayer2<minPlayer1)
			whoScored = 2;
		else
			return;	//Draw

		foreach(GameObject obj in finishedStones)
		{
			float distance = Vector3.Distance(obj.transform.position,scoreCenter.transform.position);
			if(obj.GetComponent<ThrowCurling>().GetPlayer() == whoScored)
			{
				if(whoScored == 1)
					if(distance<minPlayer2 && distance<scoreZone4)
						playerScore1++;
				if(whoScored == 2)
					if(distance<minPlayer1 && distance<scoreZone4)
						playerScore2++;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(!throwing)
		{
			if(!turnStart)
			{
				if (Input.GetKeyDown("space"))
					StartThrow();
				return;
			}
			if(Input.GetMouseButtonDown(0))
			{
				mousePoint = Input.mousePosition;
				readyToReleaseCurl = true;
				return;
			}
			if(readyToReleaseCurl)
			{
				if(Input.GetMouseButtonUp(0))
				{
					float forceDistance = Vector3.Distance(Input.mousePosition, mousePoint);
					float range = forceDistance/1000.0f;
					range = range*15 + 5.0f;
					currentStone.GetComponent<ThrowCurling>().mForce = range;
					currentStone.GetComponent<ThrowCurling>().mRange = 0;
					currentStone.GetComponent<ThrowCurling>().Throw();
					readyToReleaseCurl = false;
					throwing = true;
				}
				return;
			}
			if (Input.GetKey("left"))
			{
				Vector3 pos = currentStone.transform.position;
				if(currentStone.transform.position.z<3.5)
					currentStone.transform.position = new Vector3(pos.x,pos.y,pos.z+Time.deltaTime*2.0f);
			}
			if (Input.GetKey("right"))
			{
				Vector3 pos = currentStone.transform.position;
				if(currentStone.transform.position.z>-3.5)
					currentStone.transform.position = new Vector3(pos.x,pos.y,pos.z-Time.deltaTime*2.0f);
			}
			if (Input.GetKeyDown("space"))
			{
				currentStone.GetComponent<ThrowCurling>().Throw();
				throwing = true;
			}
		}
		else // Curling is moving!
		{
			if(currentStone.rigidbody.velocity.magnitude>0.1) 
			{
				tempCameraPos.x = currentStone.transform.position.x-8;
				tempCameraPos.y = currentStone.transform.position.y+7;
				//tempCameraPos.z = Camera.mainCamera.transform.position.z;
				Camera.mainCamera.transform.position = tempCameraPos;
			}
			else if(Vector3.Distance(currentStone.transform.position,startPoint)>3 )
				FinishThrow();

			foreach(GameObject obj in finishedStones)
			{
				if(obj.transform.position.z>4.13 || obj.transform.position.z<-4.13 || obj.transform.position.x>43)
				{
					finishedStones.Remove(obj);
					Object.Destroy(obj);
				}
			}
			if(currentStone.transform.position.z>4.13 || currentStone.transform.position.z<-4.13 || currentStone.transform.position.x>43)
			{
				Object.Destroy(currentStone);
				currentStone = null;
				FinishThrow();
			}
		}
	}
}
