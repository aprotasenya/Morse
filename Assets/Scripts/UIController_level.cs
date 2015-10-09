using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class UIController_level : MonoBehaviour {

	public GameObject playerObject;
	public bool UIEffectsGoing = false;

	PlayerController player;
	Image playerMorseLabel;
	Text playerMorseText;
	public Image cover;
	public float fadeTime = 0.5f;

	Sequence showMorseUISeq;
	Sequence hideMorseUISeq;

	void Awake () {
		GameController.instance.playersTurn = false;

		TweenCallback onComplete = LevelCoverGone;
		cover.DOFade (0f, fadeTime).SetEase(Ease.Linear).OnComplete (onComplete);

	}

	void LevelCoverGone () {
		GameController.instance.playersTurn = true;

	}


	public void InitPlayerUI () {
		player = playerObject.GetComponent<PlayerController> ();
		playerMorseLabel = GameObject.Find ("PlayerMorseLabel").GetComponent<Image> (); //playerObject.transform.FindChild ("PlayerMorseLabel").GetComponent<Image> ();
		playerMorseText = playerMorseLabel.GetComponentInChildren<Text> ();


		//Establish a label for writing Morse; and hide it
		playerMorseLabel = GameObject.Find ("PlayerMorseLabel").GetComponent<Image> ();
		playerMorseText = playerMorseLabel.GetComponentInChildren<Text> ();
		playerMorseText.text = "";

		HideMorseUI ();

	}


	public void ShowMorseUI () {

		//Place the label above the player
		float labelHeight = playerMorseLabel.rectTransform.sizeDelta.y;
		playerMorseLabel.rectTransform.position = Camera.main.WorldToScreenPoint (player.transform.position + new Vector3 (0f, player.playerSize.y, 0f)) 
			+ new Vector3 (0f, Mathf.Round (labelHeight / 2f), 0f);

		//Show label by setting it's alpha to max
		Color LblBuffer = playerMorseLabel.color;
		Color TxtBuffer = playerMorseText.color;

		playerMorseLabel.color = new Color (LblBuffer.r, LblBuffer.g, LblBuffer.b, 255f);
		playerMorseText.color = new Color (TxtBuffer.r, TxtBuffer.g, TxtBuffer.b, 255f);

		UIEffectsGoing = false;

	}

	public void HideMorseUI () {

		//Hide label by setting it's alpha to min
		Color LblBuffer = playerMorseLabel.color;
		Color TxtBuffer = playerMorseText.color;
		
		playerMorseLabel.color = new Color (LblBuffer.r, LblBuffer.g, LblBuffer.b, 0f);
		playerMorseText.color = new Color (TxtBuffer.r, TxtBuffer.g, TxtBuffer.b, 0f);

		UIEffectsGoing = false;

	}


	public void SetMorseText (string value) {
		playerMorseText.text = value;
	}

	public void DoMorseLabelEffect (string effectName) {

		switch (effectName) {
		case "morseRight" :
//			playerMorseLabel.color = Color.green;
			playerMorseText.color = Color.cyan; // new Color (34f, 206f, 255f, 255f);
			break;

		case "morseWrong" :
//			playerMorseLabel.color = Color.red;
			playerMorseText.color = Color.red; // new Color (255f, 19f, 33f);
			break;

		case "morseInput" :
//			playerMorseLabel.color = Color.black;
			playerMorseText.color = Color.white; //new Color(255f, 72f, 77f);
			break;

		default :
//			playerMorseLabel.color = Color.black;
			playerMorseText.color = Color.white; //new Color(255f, 72f, 77f);
			break;

		
		}

	}


	public void ExitLevel (string key) {
		GameController.instance.playersTurn = false;

		TweenCallback loadLevel;

		switch (key) {
		case "reload" :
			loadLevel = ReloadLevel;
			break;
		case "win" :
			loadLevel = LoadWinMenu;
			break;
		default :
			loadLevel = ReloadLevel;
			break;
		}

		cover.DOFade (1f, fadeTime).SetEase (Ease.Linear).OnComplete (loadLevel);

	}

	void LoadWinMenu () {
		Application.LoadLevel (2);
		
	}

	void ReloadLevel () {
		Application.LoadLevel (Application.loadedLevel);
		
	}


}
