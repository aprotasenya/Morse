using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

//Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
public class EnemyController : MovingObject
{
	public Transform[] waypoints;
	public int skipXThenMove;

	public Transform target;	//Transform to attempt to move toward each turn.
	public Vector3 stepTarget;
	public int movesLeftToSkip;
	public int movesInOneTurn = 1;
	private int currentWaypointIndex;
	private PlayerController player;

	public BoxCollider2D attackRange;
	public bool playerIsInRange = false;
	private Direction newDir;

	Animator animator;
	
	//Start overrides the virtual Start function of the base class.
	protected override void Start ()
	{
		movesLeftToSkip = skipXThenMove;
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();

		attackRange = transform.FindChild ("attackRange").gameObject.AddComponent<BoxCollider2D> ();
		attackRange.isTrigger = true;
		attackRange.size = new Vector2 (3, 2);
		attackRange.offset = new Vector2 (0, -0.5f);

		animator = gameObject.GetComponent<Animator> ();

		//Get and store a reference to the attached Animator component.
//		animator = GetComponent<Animator> ();

		//Call the start function of our base class MovingObject.
		base.Start ();
	}
	
	public void RefreshTargets (Vector3 startPosition) {

		if (waypoints.Length != 0) {

			if ((Vector2)(waypoints[currentWaypointIndex].position) == (Vector2)(startPosition)) {
				currentWaypointIndex = (currentWaypointIndex < waypoints.Length - 1) ? (currentWaypointIndex + 1) : 0;
			}
			target = waypoints [currentWaypointIndex];

		} else {
			//Find the Player GameObject using it's tag and store a reference to its transform component.
			target = player.transform;

		}


		stepTarget = Vector3.zero;

		//Declare variables for X and Y axis move directions, these range from -1 to 1.
		//These values allow us to choose between the cardinal directions: up, down, left and right.
		float xDir = 0;
		float yDir = 0;

		//If the difference in X-positions is approximately zero (Epsilon) do the following:
		if (Mathf.Abs (target.position.x - startPosition.x) < float.Epsilon) {

			//If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
			yDir = target.position.y > startPosition.y ? GameController.instance.stepSize.y : -GameController.instance.stepSize.y;
		
			//If the difference in positions is not approximately zero (Epsilon) do the following:
		} else {
			//Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
			xDir = target.position.x > startPosition.x ? GameController.instance.stepSize.x : -GameController.instance.stepSize.x;
		}

		stepTarget = new Vector3 (xDir, yDir, yDir) + startPosition;

		if (xDir != 0) {
			newDir = (xDir > 0) ? Direction.right : Direction.left;
		
		} else {
			newDir = (yDir > 0) ? Direction.up : Direction.down;

		}

	}

	public Sequence EnemySequence () {

		TweenCallback onStart = EnemyPreTurn;
		TweenCallback onComplete = EnemyPostTurn;
		Sequence enemySeq = DOTween.Sequence ();

		enemySeq.AppendCallback (onStart);

		if (skipXThenMove != 0) {
			if (movesLeftToSkip > 0) {
				movesLeftToSkip--;

			} else {
				movesLeftToSkip = skipXThenMove;

				if (mustMove) {

					if (!playerIsInRange) {

						for (int k = 0; k < movesInOneTurn; k++) {
							Vector3 refreshPos = (k == 0) ? transform.position : stepTarget;

							RefreshTargets (refreshPos);
							enemySeq.Append (transform.DOMove (stepTarget, moveTime, false));

						}
					}
				}
			}
		} 

		enemySeq.AppendCallback (onComplete);

		return enemySeq;
	
	}

	public void EnemyPreTurn () {

		if (!gameObject.activeSelf)
			return;

		if (playerIsInRange) {
			Debug.Log ("BANG-BANG, Mr. Player!");
			animator.SetTrigger ("Attack");

			player.Die ();

			//play animation, sounds, etc. for the attack instead of move;
		
		} else {
			EnemyChangeDir ();
		
		}

	}

	public void EnemyPostTurn () {
	}

	public void EnemyChangeDir () {

		if (newDir == currentDir) {
			return;
		}

		switch (newDir) {
		
		case Direction.down:
			attackRange.size = new Vector2 (3, 2);
			attackRange.offset = new Vector2 (0, -0.5f);

			animator.SetBool ("FaceDown", true);
			animator.SetBool ("FaceUp", false);
			animator.SetBool ("FaceLeft", false);
			animator.SetBool ("FaceRight", false);

			currentDir = newDir;
			break;

		case Direction.up:
			attackRange.size = new Vector2 (3, 2);
			attackRange.offset = new Vector2 (0, 0.5f);

			animator.SetBool ("FaceDown", false);
			animator.SetBool ("FaceUp", true);
			animator.SetBool ("FaceLeft", false);
			animator.SetBool ("FaceRight", false);

			currentDir = newDir;
			break;

		case Direction.right:
			attackRange.size = new Vector2 (2, 3);
			attackRange.offset = new Vector2 (0.5f, 0);
			
			animator.SetBool ("FaceDown", false);
			animator.SetBool ("FaceUp", false);
			animator.SetBool ("FaceLeft", false);
			animator.SetBool ("FaceRight", true);

			currentDir = newDir;
			break;

		case Direction.left: 
			attackRange.size = new Vector2 (2, 3);
			attackRange.offset = new Vector2 (-0.5f, 0);
			
			animator.SetBool ("FaceDown", false);
			animator.SetBool ("FaceUp", false);
			animator.SetBool ("FaceLeft", true);
			animator.SetBool ("FaceRight", false);

			currentDir = newDir;
			break;

		}
		
	}

//	private void OnTriggerEnter2D (Collider2D other) {
//		
//		if (other.gameObject.tag == "Player") {
//			Debug.Log ("~~~~~ I sense the Player! ~~~~~");
//			playerIsInRange = true;
//		}
//	}
//	
//	private void OnTriggerExit2D (Collider2D other) {
//		
//		if (other.gameObject.tag == "Player") {
//			Debug.Log ("~~~~~ I lost the Player! ~~~~~");
//			playerIsInRange = false;
//		}
//	}
	

	protected override void OnCantMove <T> (T component) {}


	public void GiveInfo () {
		Debug.Log ("I GIVE INFO");
		string info = GameTexts.instance.level1_ditMessages [1];

		GameController.instance.infoText.DOText ("", 0f);
		GameController.instance.infoText.DOText (info, 3f).SetEase (Ease.Linear);

	}


	public void SwitchLock () {
		mustMove = !mustMove;
	
	}


	public void Die() {
		animator.SetTrigger ("Die");
		Invoke ("Vanish", 0.7f);
	}

	void Vanish() {

		gameObject.SetActive (false);

	}


}
