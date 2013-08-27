using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	//----------------------------------
	//Scores of 2 players
	int playerScore1 = 0;
	int playerScore2 = 0;
	
	public float leapMotionPower = 3;
	public float leapMotionSidePower = 20;

	public int roundsOfGame = 10;
	public int curlsOfRound = 8;
	int rounded = 1;
	int curled = 1;

	bool player1Turn = true;	//Which player is first throwing in this turn
	bool player1Throw = true;	//Which player is throwing
	bool secondThrow = false;
	bool throwing = false;		//If the stone is flying
	bool turnStart = false;		//If the player is ready to throwing

	//Control related
	bool readyToReleaseCurl = false;	//true when left button hold down
	float releaseDeltaTime = 0;

	bool gameFinished = false;
	int gameResult = -1;
	
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

	int width = Screen.width;
	int height = Screen.height;

	// Use this for initialization
	void Start () {
		originalCameraPos = Camera.mainCamera.transform.position;
		tempCameraPos = originalCameraPos;
		//originalCameraRot = Camera.mainCamera.transform.rotation;
		//load resource
		stoneObject = (GameObject)Resources.Load("Pref_Stone",typeof(GameObject));
		startPoint = stoneObject.transform.position;
		finishedStones = new List<GameObject>();
	}
	
	void OnGUI() {
        GUI.enabled = true;

    	string text;
        if(gameFinished)
        {
        	GUILayout.BeginArea(new Rect(width/2 - 75, height/2-25, 150, 50));
        	switch(gameResult)
        	{
        	case 0:
        		text = "Game Draw";
        		break;
        	case 1:
        		text = "Player 1 Wins";
        		break;
        	case 2:
        		text = "Player 2 Wins";
        		break;
    		default:
    			text = "Bug happens";
    			break;
        	}
        	GUILayout.Box(text);
        	if(GUILayout.Button("Start a New Game"))
        		newGame();
        	GUILayout.EndArea();
        	return;
        }
		
    	GUILayout.BeginArea(new Rect(10, 10, 200, 150));
    	if(player1Turn)
    		text = "Round : " + rounded + " of " + roundsOfGame + " - Player1";
    	else
    		text = "Round : " + rounded + " of " + roundsOfGame + " - Player2";
    	GUILayout.Box(text);

    	if(!secondThrow)
    		text = "Turn : " + curled + " of "+ curlsOfRound + " - 1st pitch";
    	else
    		text = "Turn : " + curled + " of "+ curlsOfRound + " - 2nd pitch";
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
    		FinishGame();

    	//Change the throwing order
    	player1Turn = !player1Turn;
    	player1Throw = player1Turn;
    	rounded++;	//next round
    	curled=1;
    }

	void FinishTurn()
	{
		if(curled==curlsOfRound)
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
		{
			FinishTurn();
			secondThrow = false;
		}
		else
			secondThrow = true;
	}

	void FinishGame()
	{
		gameFinished = true;
		if(playerScore1>playerScore2)
			gameResult = 1;
		else if(playerScore2>playerScore1)
			gameResult = 2;
		else
			gameResult = 0;	//0 means draw
	}

	void newGame()
	{
		gameFinished = false;
		rounded = 1;
		curled = 1;
		player1Turn = true;	//Which player is first throwing in this turn
		player1Throw = true;	//Which player is throwing
		secondThrow = false;
		throwing = false;		//If the stone is flying
		turnStart = false;		//If the player is ready to throwing
		readyToReleaseCurl = false;	//true when left button hold down
		gameResult = -1;
		playerScore1 = 0;
		playerScore2 = 0;
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
			{
				if(distance<minPlayer1)
					minPlayer1 = distance;
			}
			else
			{
				if(distance<minPlayer2)
					minPlayer2 = distance;
			}
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
		if(gameFinished)
			return;
		if(!throwing)
		{
			if(!turnStart)
			{
				if (Input.GetKeyDown("space") || Input.GetMouseButtonDown(0) )
					StartThrow();
				return;
			}
			if(LeapMotionInput.GetHandAxis("Depth")<-0.9)
				readyToReleaseCurl = true;
			if(Input.GetMouseButtonDown(0))
			{
				mousePoint = Input.mousePosition;
				readyToReleaseCurl = true;
				return;
			}
			if(readyToReleaseCurl)
			{
				releaseDeltaTime += Time.deltaTime;
				if(Input.GetMouseButtonUp(0))
				{
					//float forceDistance = Vector3.Distance(Input.mousePosition, mousePoint);
					float forceDistance = Input.mousePosition.y - mousePoint.y;
					if (forceDistance<0)
						forceDistance = -forceDistance;
					float speed = forceDistance / releaseDeltaTime;
					//Debug.Log(speed);
					if(speed>1000)
						speed = 1000; //Threshold here
					float force = speed/66 + 5; //Narrow the range to 0-20.0f

					//Torque here
					float xPosMoved = mousePoint.x - Input.mousePosition.x ;
					//Debug.Log(xPosMoved);
					if(xPosMoved>1000)
						xPosMoved = 1000; //Threshold here
					float sideforce = xPosMoved/20;
					//Debug.Log(sideforce);

					currentStone.GetComponent<ThrowCurling>().sideForce = sideforce;
					//Debug.Log("Side Force : "+sideforce);
					currentStone.GetComponent<ThrowCurling>().mForce = force;
					//Debug.Log("Force : "+force);
					currentStone.GetComponent<ThrowCurling>().mRange = 0;
					currentStone.GetComponent<ThrowCurling>().Throw();
					readyToReleaseCurl = false;
					releaseDeltaTime = 0;
					throwing = true;
				}
				//Detect Leap motion release
				if(LeapMotionInput.GetHandAxis("Depth")>0.9)
				{
					float sideforce = LeapMotionInput.GetHandAxis("Horizontal")*leapMotionSidePower;
					currentStone.GetComponent<ThrowCurling>().sideForce = -sideforce;//oppsite
					//Debug.Log("Side Force : "+sideforce);
					float force = leapMotionPower/releaseDeltaTime;
					currentStone.GetComponent<ThrowCurling>().mForce = force;
					//Debug.Log("Force : "+force);
					currentStone.GetComponent<ThrowCurling>().mRange = 0;
					currentStone.GetComponent<ThrowCurling>().Throw();
					readyToReleaseCurl = false;
					releaseDeltaTime = 0;
					throwing = true;
				}
				return;
			}
			if (Input.GetKey("left") || Input.GetKey(KeyCode.A))
			{
				Vector3 pos = currentStone.transform.position;
				if(currentStone.transform.position.z<3.5)
					currentStone.transform.position = new Vector3(pos.x,pos.y,pos.z+Time.deltaTime*2.0f);
			}
			if (Input.GetKey("right") || Input.GetKey(KeyCode.D))
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
			//Check other stones
			for(int i=0;i<finishedStones.Count;i++)
			{
				if(finishedStones[i].transform.position.z>4.13 || finishedStones[i].transform.position.z<-4.13 || finishedStones[i].transform.position.x>43)
				{
					Object.Destroy(finishedStones[i]);
					finishedStones.RemoveAt(i);
				}
			}

			//Update the camera
			if(currentStone!=null && currentStone.rigidbody.velocity.magnitude>0.1 && currentStone.transform.position.z<4.13 && currentStone.transform.position.z>-4.13 && currentStone.transform.position.x<43) 
			{
				if(currentStone.transform.position.x - originalCameraPos.x > 7)
				{
					tempCameraPos.x = currentStone.transform.position.x - 7;
					//tempCameraPos.z = Camera.mainCamera.transform.position.z;
					Camera.mainCamera.transform.position = tempCameraPos;
				}
			}
			else// if(currentStone!=null && Vector3.Distance(currentStone.transform.position,startPoint)>3 )
			{
				//To check if the throwing finished
				if(currentStone!=null && (currentStone.transform.position.z>4.13 || currentStone.transform.position.z<-4.13 || currentStone.transform.position.x>43) )
				{
					Object.Destroy(currentStone);
					currentStone = null;
				}
				for(int i=0; i<finishedStones.Count;i++)
				{
					if(finishedStones[i].rigidbody.velocity.magnitude>0.1)
						return;
				}
				//For 0 speed at beginning problem 
				if(currentStone!=null && Vector3.Distance(currentStone.transform.position,startPoint)<3)
					return;
				FinishThrow();
			}
		}
	}
}