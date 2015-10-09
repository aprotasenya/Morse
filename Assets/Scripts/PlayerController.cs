using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using DG.Tweening;

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class PlayerController : MovingObject
{
//	public float restartLevelDelay = 1f;		//Delay time in seconds to restart level.
//	public Text foodText;						//UI Text to display current player food total.
	public Vector3 stepTarget;
	public Ease moveEase = Ease.Linear;

//	public AudioClip moveSound1;				//1 of 2 Audio clips to play when player moves.
//	public AudioClip moveSound2;				//2 of 2 Audio clips to play when player moves.
//	public AudioClip gameOverSound;				//Audio clip to play when player dies.
	
//		private Animator animator;					//Used to store a reference to the Player's animator component.
//		private int food;							//Used to store player food points total during level.

	public Animator animator;
	[HideInInspector] public Vector3 playerSize;

	Image playerMorseLabel;
	Text playerMorseText;

	BoxCollider2D playerCollider;

	public MorseField morseField;

	//Start overrides the Start function of MovingObject
	protected override void Start ()
	{
		//Call the Start function of the MovingObject base class.
		base.Start ();

		playerSize = gameObject.GetComponent<SpriteRenderer> ().bounds.size;

		//Get a component reference to the Player's animator component
		animator = GetComponent<Animator>();
		morseField = GetComponentInChildren<MorseField> ();

		//Establish a label for writing Morse;
		GameController.instance.gameObject.GetComponent<UIController_level> ().InitPlayerUI ();

		playerCollider = gameObject.GetComponent<BoxCollider2D> ();

	}

	public void PlayEffects (string actionName) {

		if (animator == null) {
			return;
		}

		switch (actionName) {
		
		case "PlayerDash" : 
			animator.SetTrigger (actionName);
			break;

		default : 
			Debug.Log ("effect!");
			break;
		
		}
	
	}


	void Update () {

	}


	//This function is called when the behaviour becomes disabled or inactive.
	private void OnDisable ()
	{
		//When Player object is disabled, store the current local food total in the GameController so it can be re-loaded in next level.
//			GameController.instance.playerFoodPoints = food;
	}


	public bool canGoThere () {

		Vector2 linecastStart = transform.position;
		Vector2 linecastEnd = stepTarget;
		
		RaycastHit2D hit;
		
		//Disable the boxCollider so that linecast doesn't hit this object's own collider.
		playerCollider.enabled = false;
		//...also it's tile's one
		BoxCollider2D tileBox = myTileCollider ();
		tileBox.enabled = false;

		//Cast a line from start point to end point checking collision on walkableLayer.
		hit = Physics2D.Linecast (linecastStart, linecastEnd, GameController.instance.walkableLayer);

		//Re-enable boxCollider after linecast
		playerCollider.enabled = true;
		//...also it's tile's one
		tileBox.enabled = true;


		//is there walkable floor there?
		if (hit.transform == null) {
			Debug.Log ("Nowhere to run, Irvy-boy!");
			return false;

		} else {
			Debug.Log ("There's floor there!");
			return true;
		
		}


	}

	private BoxCollider2D myTileCollider () {

		RaycastHit2D hit;
		
		//Cast a line from start point to end point checking collision on walkableLayer.
		hit = Physics2D.Linecast (transform.position, transform.position, GameController.instance.walkableLayer);

		BoxCollider2D value = hit.transform.GetComponent<BoxCollider2D> ();

		return value;

	}


	public void MovePlayer<T> (float xDir, float yDir) 
		where T : Component
	{
		AttemptMove <T> (xDir, yDir);
	
	}

	//AttemptMove overrides the AttemptMove function in the base class MovingObject
	//AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
	protected override void AttemptMove <T> (float xDir, float yDir)
	{
		//Every time player moves, subtract from food points total.
//			food--;
		
		//Update food text display to reflect current score.
//			foodText.text = "Food: " + food;
		
		//Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
		base.AttemptMove <T> (xDir, yDir);
		
		//Hit allows us to reference the result of the Linecast done in Move.
		RaycastHit2D moveHit;
		
		//If Move returns true, meaning Player was able to move into an empty space.
		if (Move (xDir, yDir, out moveHit)) 
		{
			//Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
//				SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
		}
		
		//Since the player has moved and lost food points, check if the game has ended.
		CheckIfGameOver ();
		
		//Set the playersTurn boolean of GameController to false now that players turn is over.
		GameController.instance.playersTurn = false;
	}
	
	
	//OnCantMove overrides the abstract function OnCantMove in MovingObject.
	//It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
	protected override void OnCantMove <T> (T component)
	{
		//Set hitWall to equal the component passed in as a parameter.
//			Wall hitWall = component as Wall;
		
		//Call the DamageWall function of the Wall we are hitting.
//			hitWall.DamageWall (wallDamage);
		
		//Set the attack trigger of the player's animation controller in order to play the player's attack animation.
//			animator.SetTrigger ("playerChop");
	}
	
	
	//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
	private void OnTriggerEnter2D (Collider2D other)
	{
		//Check if the tag of the trigger collided with is Exit.
		if(other.tag == "Exit")
		{
			//Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
			//Invoke ("Restart", restartLevelDelay);
			
			//Disable the player object since level is over.
			enabled = false;
		}
		
		//Check if the tag of the trigger collided with is Food.
		else if(other.tag == "Food")
		{
			//Add pointsPerFood to the players current food total.
//				food += pointsPerFood;
			
			//Update foodText to represent current total and notify player that they gained points
//				foodText.text = "+" + pointsPerFood + " Food: " + food;
			
			//Call the RandomizeSfx function of SoundManager and pass in two eating sounds to choose between to play the eating sound effect.
//				SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
			
			//Disable the food object the player collided with.
			other.gameObject.SetActive (false);
		}
		
		//Check if the tag of the trigger collided with is Soda.
		else if(other.tag == "Soda")
		{
			//Add pointsPerSoda to players food points total
//				food += pointsPerSoda;
			
			//Update foodText to represent current total and notify player that they gained points
//				foodText.text = "+" + pointsPerSoda + " Food: " + food;
			
			//Call the RandomizeSfx function of SoundManager and pass in two drinking sounds to choose between to play the drinking sound effect.
//				SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
			
			//Disable the soda object the player collided with.
			other.gameObject.SetActive (false);
		}
	}
	
	
	//Restart reloads the scene when called.
	private void Restart ()
	{
		//Load the last scene loaded, in this case Main, the only scene in the game.
		Application.LoadLevel (Application.loadedLevel);
	}
	
	
	//LoseFood is called when an enemy attacks the player.
	//It takes a parameter loss which specifies how many points to lose.
	public void LoseFood (int loss)
	{
		//Set the trigger for the player animator to transition to the playerHit animation.
//			animator.SetTrigger ("playerHit");
		
		//Subtract lost food points from the players total.
//			food -= loss;
		
		//Update the food display with the new total.
//			foodText.text = "-"+ loss + " Food: " + food;
		
		//Check to see if game has ended.
		CheckIfGameOver ();
	}

	public void Die () {
		animator.SetTrigger ("PlayerDeath");
		Invoke ("TellGameControlImDead", 0.5f);
	}

	void TellGameControlImDead () {
		gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		GameController.instance.PlayerIsDead ();

	}
	
	//CheckIfGameOver checks if the player is out of food points and if so, ends the game.
	private void CheckIfGameOver ()
	{
		//Check if food point total is less than or equal to zero.
/*			if (food <= 0) 
		{
			//Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
			SoundManager.instance.PlaySingle (gameOverSound);
			
			//Stop the background music.
			SoundManager.instance.musicSource.Stop();
			
			//Call the GameOver function of GameController.
			GameController.instance.GameOver ();
		}
*/		}
}


