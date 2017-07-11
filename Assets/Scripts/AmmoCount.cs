using UnityEngine;
using System.Collections;

public class AmmoCount : MonoBehaviour {

    public TextMesh number;
    public Gun gun;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        number.text = gun.ammo.ToString();

	}
}
