using UnityEngine;
using System.Collections;

public class ButtonAnimationContoller : MonoBehaviour {
	private Animator buttonAnimator;
	
	void OnEnable () {
		buttonAnimator = gameObject.GetComponent <Animator> ();
	}

	public void OnButtonPressed () {
		buttonAnimator.SetTrigger ("Pressed");
	}
	
	public void OnButtonRelease () {
		buttonAnimator.SetTrigger ("Release");
	}
}
