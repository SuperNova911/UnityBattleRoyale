using UnityEngine;
using System.Collections;

public class RotateMe : MonoBehaviour {
	private Vector3 center_to_ratate;
	// Use this for initialization
	void Start () {
		center_to_ratate = new Vector3(0.0f,0.0f,0.0f);
		Component[] allRenderers = (Component[])(gameObject.GetComponentsInChildren (typeof(Renderer)));
		foreach (Component child in allRenderers) {
			Renderer rend = (Renderer)child;
			Vector3 center = rend.bounds.center;
			center_to_ratate += center;
		}
		center_to_ratate /= allRenderers.Length;
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.RotateAround(center_to_ratate, Vector3.up, 10 * Time.deltaTime);
		gameObject.transform.RotateAround(center_to_ratate, Vector3.forward, 15 * Time.deltaTime);
	}
}
