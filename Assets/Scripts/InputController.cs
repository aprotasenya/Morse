using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class InputController : MonoBehaviour {

	PlayerController player;
	UIController_level LevelUI;

	float btnHoldTime = 0f;
	string bufferMorseStr = "";
	string morseComboToDo = "";

	public float comboKeysIntervalMax = 0.3f;
	float currentKeysInterval = 0f;
	bool countKeysInterval = false;

	public float dotMaxLength = 0.1f;
	public float dashMinLength = 0.3f;

	public List<string> morseComboList;

	public float morseUIHideDelay = 2f;

	public GameObject sfxMorse;
	private GameObject sfxMorseInst;



	void Awake () {

		player = GameObject.FindWithTag ("Player").GetComponent<PlayerController> ();
		LevelUI = gameObject.GetComponent<UIController_level> ();

	}

	private void Update ()
	{

		//If it's not the player's turn, exit the function.
		if (!GameController.instance.playersTurn) return;

		//If UI Effects are in process, don't accept any input and don't cound intervals
		if (LevelUI.UIEffectsGoing) return;

		//============================================== MOVEMENT INPUT ==============================================

		float horizRound = 0;
		float vertRound = 0;
		float horizontal = 0;  	//Used to store the horizontal move direction.
		float vertical = 0;		//Used to store the vertical move direction.
		
		//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
		horizRound = Mathf.Round (Input.GetAxisRaw ("Horizontal"));
		horizontal = horizRound * GameController.instance.stepSize.x;
		
		//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
		vertRound = Mathf.Round (Input.GetAxisRaw ("Vertical"));
		vertical = vertRound * GameController.instance.stepSize.y;

		//Check if moving horizontally, if so set vertical to zero.
		if(horizontal != 0)
		{
			vertRound = 0;
			vertical = 0;
		}

		//Check if we have a non-zero value for horizontal or vertical
		if (horizontal != 0 || vertical != 0) {

			player.stepTarget = new Vector3(horizontal, vertical, vertical) + player.transform.position;

//			Debug.Log ("hor=" + horizontal + "; ver=" + vertical);
//			Debug.Log ("stepTarget = " + player.stepTarget.ToString ());

			//send rounded directions to animator, so that when trigger hits, it knows where to dash
			player.animator.SetInteger ("DashHor", (int)(horizRound));
			player.animator.SetInteger ("DashVert", (int)(vertRound));

			// A MovePlayer call and some OLD SHIT WHICH IS TO BE CAUTIOUSLY CUT OUT
			GameController.instance.MovePlayer<WallObj>(horizontal, vertical);

		}


		//============================================== MORSE INPUT ==============================================

//		if (Input.GetButtonDown ("morseDot")) {
//			Debug.Log ("DOT start = " + btnHoldTime);
//
//		} else if (Input.GetButtonDown ("morseDash")) {
//			Debug.Log ("DASH start = " + btnHoldTime);
//		}

		if (Input.GetButton ("morseDot") || Input.GetButton ("morseDash")) {

			//The Button is down, so stop counting interval if you are
			if (countKeysInterval) countKeysInterval = false;

			//Add to the time The Button is down
			btnHoldTime += Time.deltaTime;
			
		} else if (Input.GetButtonUp ("morseDot")) {

			if (btnHoldTime <= dotMaxLength) {
				Debug.Log ("DOT = " + btnHoldTime);
				GetMorseInput ("E");

			} else {
				Debug.Log ("too long for DOT! = " + btnHoldTime);

			}
			btnHoldTime = 0f;
			currentKeysInterval = 0f;
			countKeysInterval = true;

		} else if (Input.GetButtonUp ("morseDash")) {

			if (btnHoldTime > dashMinLength) {
				Debug.Log ("DASH = " + btnHoldTime);
				GetMorseInput ("T");

			} else {
				Debug.Log ("too short for DASH! = " + btnHoldTime);

			}
			btnHoldTime = 0f;
			currentKeysInterval = 0f;
			countKeysInterval = true;

		}

		//==================================== MORSE INPUT - Counting Keys Interval ====================================

		if (countKeysInterval) {
			currentKeysInterval += Time.deltaTime;

			//if too much time's gone since the last key hit
			if (currentKeysInterval > comboKeysIntervalMax) {
				Debug.Log ("## KEY INTERVAL TOO LONG");
				Debug.Log ("## Check Combo by Time");

				CheckCombo_byTime (bufferMorseStr);
				countKeysInterval = false;
			}
		}


	}

	void GetMorseInput (string input) {
		//is this the FIRST char in sequence?
		if (bufferMorseStr.Length < 1) StartMorseInput ();

		bufferMorseStr += input;
		LevelUI.SetMorseText (bufferMorseStr);

		Debug.Log ("## Check (partial?) Combo by Button");
		CheckCombo_byButton (bufferMorseStr);
	}

	void StartMorseInput () {
		LevelUI.DoMorseLabelEffect ("morseInput");
		LevelUI.ShowMorseUI ();

		btnHoldTime = 0f;
		currentKeysInterval = 0f;

	}

	void CloseMorseInput () {
		LevelUI.HideMorseUI ();

		bufferMorseStr = "";
		LevelUI.SetMorseText (bufferMorseStr);

		btnHoldTime = 0f;
		currentKeysInterval = 0f;
		countKeysInterval = false;

	}


	void CheckCombo_byButton (string input) {
		//check for PARTIAL inclusivity
		int inclusivities = 0;

		foreach (string combo in morseComboList) {
			if (combo.Length < input.Length) {
				continue;
			}

			if (combo.Substring (0, input.Length) == input) {
				Debug.Log ("There's [" + input + "] in [" + combo + "]");
				inclusivities++;
			}
		}
		Debug.Log ("inclusivities = " + inclusivities);

		switch (inclusivities) {
		case 0 :
			//0 inclusivities, player is doing smth wrong
			ComboFailed ();
			break;

		case 1 :
			//1 inclusivity, check if it's full combo already
			if (morseComboList.Contains (input)) {
				//if so, state the combo's correct and (then) perform it
				morseComboToDo = input;
				ComboCorrect ();
			}

			//if not, we just wait and do nothing
			break;
		}
		// if there are 2+ inclusivities, player is typing right; we wait & do nothing

	}

	void CheckCombo_byTime (string input) {
		//check for the EXACT coincidence
		if (morseComboList.Contains (input)) {
			morseComboToDo = input;
			ComboCorrect ();

		} else {
			ComboFailed ();

		}

	}

	void ComboCorrect () {
		Debug.Log ("* Combo Correct *");
		LevelUI.DoMorseLabelEffect ("morseRight");
		
		LevelUI.UIEffectsGoing = true;
		Invoke ("CloseMorseInput", morseUIHideDelay);
		Invoke ("PerformMorseCombo", morseUIHideDelay);


	}

	void ComboFailed () {
		Debug.Log ("! COMBO FAILED !");
		LevelUI.DoMorseLabelEffect ("morseWrong");

		LevelUI.UIEffectsGoing = true;
		Invoke ("CloseMorseInput", morseUIHideDelay);

	}

	void PerformMorseCombo () {
		Debug.Log ("! Go Combo " + morseComboToDo.ToUpper () + " !");

		CreateSfxMorse ();
		Invoke ("KillSfxMorse", 0.6f);

		switch (morseComboToDo) {
		case "EE" :
			// this is I for INFO
			Debug.Log ("GET INFO");

			if (player.morseField.inMorseField.Count > 0) {
				foreach(EnemyController enemy in player.morseField.inMorseField) {
					enemy.GiveInfo ();
				}

			} else {
				Debug.Log ("noone in field");

			}

			break;

		case "TET" :
			// this is K for KILL
			Debug.Log ("KILL 'EM");

			if (player.morseField.inMorseField.Count > 0) {
				foreach(EnemyController enemy in player.morseField.inMorseField) {
					enemy.Die ();
				}

			} else {
				Debug.Log ("noone in field");
				
			}

			break;

		case "ETEE" :
			// this is L for LOCK
			Debug.Log ("LOCK/UNLOCK");

			if (player.morseField.inMorseField.Count > 0) {
				foreach(EnemyController enemy in player.morseField.inMorseField) {
					enemy.SwitchLock ();
				}

			} else {
				Debug.Log ("noone in field");
				
			}

			break;

		}
		
	}

	void CreateSfxMorse () {
		sfxMorseInst = (GameObject)(GameObject.Instantiate (sfxMorse, player.transform.position, Quaternion.identity));
	}

	void KillSfxMorse () {
		DestroyObject (sfxMorseInst);
		sfxMorseInst = null;
	}
	

}
