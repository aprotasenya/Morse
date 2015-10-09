using UnityEngine;
using DG.Tweening;
using System.Collections;

//The abstract keyword enables you to create classes and class members that are incomplete and must be implemented in a derived class.
public abstract class MovingObject : MonoBehaviour
{
	public enum Direction { right, left, up, down };
	public Direction currentDir = Direction.down;

	public bool mustMove = true;

	public float moveTime = 0.2f;			//Time it will take object to move, in seconds.
	public LayerMask blockingLayer;			//Layer on which collision will be checked.
		
		
	private BoxCollider2D boxCollider; 		//The BoxCollider2D component attached to this object.
//	private Rigidbody2D rb2D;				//The Rigidbody2D component attached to this object.
		
		
	//Protected, virtual functions can be overridden by inheriting classes.
	protected virtual void Start ()
	{
		//Get a component reference to this object's BoxCollider2D
		boxCollider = GetComponent <BoxCollider2D> ();

		float zStep = transform.position.y - transform.position.z;
		transform.Translate (0, 0, zStep, Space.World);

		//Get a component reference to this object's Rigidbody2D
//		rb2D = GetComponent <Rigidbody2D> ();
			
	}
		
		
		
		
	//The virtual keyword means AttemptMove can be overridden by inheriting classes using the override keyword.
	//AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact with if blocked (Player for Enemies, Wall for Player).
	protected virtual void AttemptMove <T> (float xDir, float yDir)
		where T : Component
	{
		//Hit will store whatever our linecast hits when Move is called.
		RaycastHit2D attemptMoveHit;

		//Set canMove to true if Move was successful, false if failed.
		bool canMove = Move (xDir, yDir, out attemptMoveHit);

		//Check if nothing was hit by linecast
		if(attemptMoveHit.transform == null)
			//If nothing was hit, return and don't execute further code.
			return;

		//Get a component reference to the component of type T attached to the object that was hit
		T hitComponent = attemptMoveHit.transform.GetComponent <T> ();

		//If canMove is false and hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
		if(!canMove && hitComponent != null)
				
			//Call the OnCantMove function and pass it hitComponent as a parameter.
			OnCantMove (hitComponent);
	}


	//Move returns true if it is able to move and false if not. 
	//Move takes parameters for x direction, y direction and a RaycastHit2D to check collision.
	protected bool Move (float xDir, float yDir, out RaycastHit2D hit)
	{
		//Store start position to move from, based on objects current transform position.
		Vector2 start = transform.position;
			
		// Calculate end position based on the direction parameters passed in when calling Move.
		Vector2 end = start + new Vector2 (xDir, yDir);
			
		//Disable the boxCollider so that linecast doesn't hit this object's own collider.
		boxCollider.enabled = false;
		//Cast a line from start point to end point checking collision on blockingLayer.
		hit = Physics2D.Linecast (start, end, blockingLayer);
		//Re-enable boxCollider after linecast
		boxCollider.enabled = true;
			
		//Check if anything was hit
		if(hit.transform == null)
		{

			//If nothing was hit, use DoTween to move the object to "end" destination
			transform.DOMove (end, moveTime, false).SetEase (Ease.Linear);

			//Return true to say that Move was successful
			return true;
		}

		//If something was hit, return false, Move was unsuccesful.
		return false;
	}

		
		
	//The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
	//OnCantMove will be overriden by functions in the inheriting classes.
	protected abstract void OnCantMove <T> (T component)
		where T : Component;
}
