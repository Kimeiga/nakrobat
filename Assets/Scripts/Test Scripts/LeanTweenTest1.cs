using UnityEngine;
using System.Collections;

public class LeanTweenTest1 : MonoBehaviour {

	public GameObject ball;

	// Use this for initialization
	void Start () {

		LeanTween.moveLocal(ball, Vector3.zero, 3).setDelay(2);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
