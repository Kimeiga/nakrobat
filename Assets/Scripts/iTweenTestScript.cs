using UnityEngine;
using System.Collections;

public class iTweenTestScript : MonoBehaviour {

    public GameObject gun;

	// Use this for initialization
	void Start () {

        iTween.MoveTo(gameObject, gun.transform.localPosition, 1);

	}
	
	// Update is called once per frame
	void Update () {
	
        

	}
}
