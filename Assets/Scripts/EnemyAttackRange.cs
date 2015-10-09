using UnityEngine;
using System.Collections;

public class EnemyAttackRange : MonoBehaviour {

	EnemyController parentEnemy;

	void Awake () {
	
		parentEnemy = GetComponentInParent<EnemyController> ();

	}


	private void OnTriggerEnter2D (Collider2D other) {
		
		if (other.gameObject.tag == "Player") {
			Debug.Log ("~~~~~ I sense the Player! ~~~~~");
			parentEnemy.playerIsInRange = true;
		}
	}
	
	private void OnTriggerExit2D (Collider2D other) {
		
		if (other.gameObject.tag == "Player") {
			Debug.Log ("~~~~~ I lost the Player! ~~~~~");
			parentEnemy.playerIsInRange = false;
		}
	}
}
