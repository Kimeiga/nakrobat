using UnityEngine;
using System.Collections;

public class FallOffWorld : MonoBehaviour {

    public GameObject player;
    public float fallThreshold;
    public Transform respawnTransform;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        if(player.transform.position.y <= fallThreshold)
        {
            if(respawnTransform != null)
            {
                player.transform.position = respawnTransform.position;
            }
            else
            {
                PlayerStatus1 playerStatus1Script = player.GetComponent<PlayerStatus1>();
                playerStatus1Script.health = 0;
            }
        }

	}
}
