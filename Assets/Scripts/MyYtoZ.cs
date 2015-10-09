using UnityEngine;
using System.Collections;

public class MyYtoZ : MonoBehaviour {

	//Set proper Z on start, based on starting Y
	void Start () {
		float zStep = transform.position.y - transform.position.z;
		transform.Translate (0, 0, zStep, Space.World);


	}
	
}
