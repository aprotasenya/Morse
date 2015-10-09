using UnityEngine;
using System.Collections;
using System.Collections.Generic;		//Allows us to use Lists. 
using UnityEngine.UI;					//Allows us to use UI.
using DG.Tweening;


public class GameController : MonoBehaviour {

	public enum MovementType {stepTogether, stepInTurns, realTime};
	public MovementType movementType;
	
	public enum EnemyMoveType {allTogether, oneByOne, realTime};
	public EnemyMoveType enemyMoveType;
	
	public float levelStartDelay = 2f;						//Time to wait before starting level, in seconds.
	public float turnDelay = 0.1f;							//Delay between each Player turn.

	public Vector2 stepSize;
	public LayerMask walkableLayer;

	public static GameController instance = null;			//Static instance of GameController which allows it to be accessed by any other script.

	[HideInInspector] public bool playersTurn = true;		//Boolean to check if it's players turn, hidden in inspector but public.
	
	
	private Text levelText;									//Text to display current level number.
	private GameObject levelImage;							//Image to block out level as levels are being set up, background for levelText.
//	private int level = 1;									//Current level number, expressed in game as "Day 1".
	private PlayerController player;
	public List<EnemyController> enemies;					//List of all Enemy units, used to issue them move commands.
	private bool turnMoving = false;
//	private bool playerMoving = false;
//	private bool enemiesMoving = false;
//	private bool doingSetup = true;							//Boolean to check if we're setting up board, prevent Player from moving during setup.
	
	//
	private Sequence turnMoveSequence; 
	private Sequence playerMoveSequence; 
	private Sequence enemiesMoveSequence;
	//

	[HideInInspector] public Text infoText;
	
	void Awake()
	{
		//Reinforce the singleton: set this as the instance or kill this 
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);	
		}

		player = GameObject.FindWithTag ("Player").GetComponent<PlayerController> ();

		
		//Assign enemies to a new List of Enemy objects.
		enemies = new List<EnemyController>();

		foreach (GameObject enemy in GameObject.FindGameObjectsWithTag ("Enemy")) {
			EnemyController control = enemy.GetComponent<EnemyController> ();
			if (control != null) {
				enemies.Add (control);

			}
//			enemy.GetComponent<EnemyController> ().RefreshTargets ();

		}
		
		//Call the InitGame function to initialize the first level 
//		InitGame();

		infoText = GameObject.Find ("InfoText").GetComponent<Text> ();
		infoText.DOText ("", 0f);

		Invoke ("InfoTextInit", 1f);
	}

	void InfoTextInit () {

		string info = GameTexts.instance.level1_ditMessages[0];
		
		infoText.DOText (info, 3f).SetEase (Ease.Linear);


	}

	//This is called each time a scene is loaded.
	void OnLevelWasLoaded (int index)
	{
		//Add one to our level number.
//		level++;
		//Call InitGame to initialize our level.
		ShowLevelImage();
	}
	
	//Initializes the game for each level.
	void ShowLevelImage()
	{
		//While doingSetup is true the player can't move, prevent player from moving while title card is up.
//		doingSetup = true;
		
		//Get a reference to our image LevelImage by finding it by name.
//		levelImage = GameObject.Find("LevelImage");
		
		//Get a reference to our text LevelText's text component by finding it by name and calling GetComponent.
//		levelText = GameObject.Find("LevelText").GetComponent<Text>();
		
		//Set the text of levelText to the string "Day" and append the current level number.
//		levelText.text = "Day " + level;
		
		//Set levelImage to active blocking player's view of the game board during setup.
//		levelImage.SetActive(true);
		
		//Call the HideLevelImage function with a delay in seconds of levelStartDelay.
		Invoke("HideLevelImage", levelStartDelay);

	}
	
	
	//Hides black image used between levels
	void HideLevelImage()
	{
		//Disable the levelImage gameObject.
//		levelImage.SetActive(false);
		
		//Set doingSetup to false allowing player to move again.
//		doingSetup = false;
	}
	
/*	//Update is called every frame.
	void Update()
	{
		//Check that playersTurn or enemiesMoving or doingSetup are not currently true.
		if (playersTurn || enemiesMoving || doingSetup)
			
			//If any of these are true, return and do not start MoveEnemies.
			return;
		
		//Start moving enemies.
		MoveEnemies ();
	}
*/	

	void SetSequences () {

		TweenCallback playerTweenStart = PlayerMoveStart;
		TweenCallback playerTweenEnd = PlayerMoveEnd;

		playerMoveSequence = DOTween.Sequence ();
		playerMoveSequence.Pause ();

		playerMoveSequence
			.AppendCallback (playerTweenStart)
				.Append (player.transform.DOMove (player.stepTarget, player.moveTime, false))
				.SetEase (player.moveEase)
				.AppendCallback (playerTweenEnd);

		TweenCallback enemiesTweenStart = EnemiesMoveStart;
		TweenCallback enemiesTweenEnd = EnemiesMoveEnd;

		enemiesMoveSequence = DOTween.Sequence ();
		enemiesMoveSequence.Pause ();

		enemiesMoveSequence.AppendCallback (enemiesTweenStart);

		for (int i = 0; i < enemies.Count; i++) {

			if (enemyMoveType == EnemyMoveType.oneByOne) {
				enemiesMoveSequence.Append (enemies[i].EnemySequence ());

			} else if (enemyMoveType == EnemyMoveType.allTogether) {
				enemiesMoveSequence.Insert (0f, enemies[i].EnemySequence ());

			}

			if (i + 1 < enemies.Count && enemyMoveType == EnemyMoveType.oneByOne) {
				enemiesMoveSequence.AppendInterval (turnDelay);
			
			}
		}

		enemiesMoveSequence.AppendCallback (enemiesTweenEnd);


	}

	public void MovePlayer<T> (float xDir, float yDir) 
		where T : Component
	{
		if (turnMoving) return;
		if (!player.canGoThere ()) return;

		SetSequences ();

		TweenCallback moveStart = MoveTurnStart;
		TweenCallback moveEnd = MoveTurnEnd;

		turnMoveSequence = DOTween.Sequence ();
		turnMoveSequence.Pause ();

		turnMoveSequence.AppendCallback (moveStart);
		turnMoveSequence.Append (playerMoveSequence);

		if (movementType == MovementType.stepInTurns) {
			turnMoveSequence.AppendInterval (turnDelay);
			turnMoveSequence.Append (enemiesMoveSequence);

		} else if (movementType == MovementType.stepTogether) {
			turnMoveSequence.Insert (0f, enemiesMoveSequence);

		}

		turnMoveSequence.AppendInterval (turnDelay);
		turnMoveSequence.AppendCallback (moveEnd);
		turnMoveSequence.Play ();

	}


	// A TweenCallback for the PLAYER movement START
	void PlayerMoveStart () {
//		Debug.Log ("PLAYER GO (" + Time.time +")");
//		playerMoving = true;

		player.PlayEffects ("PlayerDash");

	}

	// A TweenCallback for the PLAYER movement FINISH
	void PlayerMoveEnd () {
//		Debug.Log ("PLAYER END (" + Time.time +")");
//		playerMoving = false;

	}

	// A TweenCallback for the END movement START
	void EnemiesMoveStart () {
//		Debug.Log ("ENEMIES GO (" + Time.time +")");

		//While enemiesMoving is true player is unable to move.
//		enemiesMoving = true;



	}

	// A TweenCallback for the END movement FINISH
	void EnemiesMoveEnd () {
//		Debug.Log ("ENEMIES END (" + Time.time +")");

		//Enemies are done moving, set enemiesMoving to false.
//		enemiesMoving = false;

//		//Once Enemies are done moving, set playersTurn to true so player can move.
//		playersTurn = true;

	}

	void MoveTurnStart () {
//		Debug.Log ("EVERYONE GO (" + Time.time +")");
		turnMoving = true;
	}

	void MoveTurnEnd () {
//		Debug.Log ("EVERYONE END (" + Time.time +")");
		turnMoving = false;
	}

	public void PlayerIsDead () {

		GetComponent<UIController_level> ().ExitLevel ("reload");

	}


}
