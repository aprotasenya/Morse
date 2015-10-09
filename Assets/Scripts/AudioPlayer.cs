using UnityEngine;
using System.Collections;
using DG.Tweening;

public class AudioPlayer : MonoBehaviour {

	public static AudioPlayer instance = null;

	AudioSource audioPlayer;

	// Use this for initialization
	void Awake () {

		//Reinforce the singleton: set this as the instance or kill this 
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);	
		}

		DontDestroyOnLoad (transform.gameObject);

		audioPlayer = GetComponent<AudioSource> ();
		audioPlayer.volume = 0f;

		Invoke ("PlayMusic", 1f);
	}

	void PlayMusic () {
		audioPlayer.Play ();
		audioPlayer.DOFade (0.1f, 2f).SetEase (Ease.Linear);
	
	}

}
