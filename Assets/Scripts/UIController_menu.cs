using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class UIController_menu : MonoBehaviour {

	public Image pressAny;
	public Image cover;

	public Text infoText, twitterAB, twitterAP;

	public float fadeTime = 2f;

	bool awaitAnyKey = false;

	public string twAB = GameTexts.instance.winMenu_twAB;
	public string twAP = GameTexts.instance.winMenu_twAP;

	Sequence glitchTw;
	float twGlitchTime = 0.5f;
	float twGlitchDelay = 2f;


	void Awake () {

		if (Application.loadedLevelName.Contains ("win")) {
			string info = GameTexts.instance.winMenu_mainText;

			infoText.text = "";
			twitterAB.text = "";
			twitterAP.text = "";
			pressAny.enabled = false;


			Sequence winMenu = DOTween.Sequence ();
			TweenCallback onComplete = WinMenuComplete;
			winMenu.Pause ();


			winMenu.AppendInterval (1f)
				.Append(infoText.DOText (info, 4f).SetEase (Ease.Linear))
				.Append (twitterAB.DOText (twAB, 2f).SetEase (Ease.Linear))
					.Append (twitterAP.DOText (twAP, 2f).SetEase (Ease.Linear))
					.AppendCallback (onComplete);

			winMenu.Play ();

		} else {
			TweenCallback onComplete = MainMenuComplete;
			cover.DOFade (0f, (fadeTime)).SetEase(Ease.Linear).OnComplete (onComplete);

		}

	}

	void MainMenuComplete () {
		awaitAnyKey = true;
		HidePressAny ();

	}

	void WinMenuComplete () {
		awaitAnyKey = true;
		ShowPressAny ();

		Invoke ("GlitchTwitters", twGlitchDelay);
	}

	void HidePressAny () {
		pressAny.enabled = false;
		Invoke ("ShowPressAny", 1f);

	}

	void ShowPressAny () {
		pressAny.enabled = true;
		Invoke ("HidePressAny", 1f);

	}

	void GlitchTwitters () {

		glitchTw = DOTween.Sequence ();
		TweenCallback onComplete = RestartGlitch;

		glitchTw.Append (twitterAB.DOText (twAB, twGlitchTime, true, ScrambleMode.Lowercase).SetEase (Ease.Linear))
			.Append (twitterAP.DOText (twAP, twGlitchTime, true, ScrambleMode.Lowercase).SetEase (Ease.Linear))
				.AppendInterval (twGlitchDelay)
				.AppendCallback (onComplete);

	}

	void RestartGlitch () {
		twGlitchTime = Random.Range (0.3f, 0.7f);
		twGlitchDelay = Random.Range (2f, 6f);

		GlitchTwitters ();
	
	}


	void Update () {
	
		if (awaitAnyKey && Input.anyKeyDown) {
		
			if (Application.loadedLevelName.Contains ("win")) {
				TweenCallback onComplete;

				if (Application.isWebPlayer) {
					onComplete = LoadStartMenu;

				} else {
					onComplete = ExitApp;

				}

				cover.DOFade (1f, fadeTime).SetEase (Ease.Linear).OnComplete (onComplete);

			} else {
				TweenCallback loadNext = LoadNext;
				cover.DOFade (1f, fadeTime).SetEase (Ease.Linear).OnComplete (loadNext);

			}
		}

	}

	void LoadNext () {
		Application.LoadLevel (Application.loadedLevel + 1);
		
	}
	void LoadStartMenu () {
		Application.LoadLevel (0);
		
	}
	void ExitApp () {
		Application.Quit ();
		
	}
}
