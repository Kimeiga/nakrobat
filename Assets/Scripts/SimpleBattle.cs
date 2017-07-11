using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SimpleBattle : Singleton<SimpleBattle> {

    protected SimpleBattle() { } // guarantee this will be always a singleton only - can't use the constructor!

    public List<SimpleEnemy> aliveCapsuleTeam;
    public List<SimpleEnemy> aliveCubeTeam;
    public List<GameObject> guns;


	// Use this for initialization
	void Start () {

        aliveCapsuleTeam = new List<SimpleEnemy>();
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Capsule Team"))
        {
            aliveCapsuleTeam.Add(obj.GetComponent<SimpleEnemy>());
        }

        aliveCubeTeam = new List<SimpleEnemy>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Cube Team"))
        {
            aliveCubeTeam.Add(obj.GetComponent<SimpleEnemy>());
        }

        guns = new List<GameObject>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Gun"))
        {
            guns.Add(obj);
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
