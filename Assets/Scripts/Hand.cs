using UnityEngine;
using System.Collections;

public class Hand : MonoBehaviour {

    private Vector3 originalPosition;

	// Use this for initialization
	void Start () {

        originalPosition = transform.localPosition;

	}
	
	// Update is called once per frame
	void Update () {
	
        

	}
    /*
    void ReachItem(GameObject item)
    {
        iTween.MoveTo(gameObject, item.transform.position, 1);

    }

    void PullItem(Transform returnLocation)
    {
        print("L");
        iTween.MoveTo(gameObject, returnLocation.position, 1);
        
    }
    */
}
