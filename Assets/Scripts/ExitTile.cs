using UnityEngine;
using System.Collections;

public class ExitTile : MonoBehaviour {

	private void OnTriggerEnter2D (Collider2D other) {

		if (other.gameObject.tag == "Player") {

			GetComponent<AudioSource> ().Play ();
			GameController.instance.gameObject.GetComponent<UIController_level> ().ExitLevel ("win");

		}
	
	}

}
