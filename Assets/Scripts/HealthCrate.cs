using UnityEngine;
using System.Collections;

public class HealthCrate : MonoBehaviour {

    public float healthValue;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.name == "Bounce Player")
        {
            col.transform.GetComponent<PlayerStatus1>().health += healthValue;
            Destroy(gameObject);
        }
    }
}
