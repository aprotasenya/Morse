using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MorseField : MonoBehaviour {

	public List<EnemyController> inMorseField;

	// Use this for initialization
	void Start () {
	
		inMorseField = new List<EnemyController> ();

	}
	
	private void OnTriggerEnter2D (Collider2D other) {

		EnemyController control = other.GetComponent<EnemyController> ();

		if (control != null) {
			inMorseField.Add (control);
		
		}

	}

	private void OnTriggerExit2D (Collider2D other) {
		EnemyController control = other.GetComponent<EnemyController> ();

		if (inMorseField.Contains (control)) {
			inMorseField.Remove (control);
		
		}

	}


}
