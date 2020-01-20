using UnityEngine;
using System.Collections;

public class MoveMe : MonoBehaviour {
	private bool forwarding = false;
	private float delta = 0.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (forwarding) {
			if (gameObject.transform.position.z > 0.0f) {
				forwarding = false;
			}
		} else {
			if (gameObject.transform.position.z < -260.0f) {
				forwarding = true;
			}
		}
		gameObject.transform.Translate(0, 0, forwarding?delta:(-delta));
	}
}
