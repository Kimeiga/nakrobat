using UnityEngine;
using System.Collections;

public class DisplayHealth : MonoBehaviour {

    public PlayerStatus1 playerStatus1Script;

    public TextMesh text;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        text.text = playerStatus1Script.health.ToString();

	}
}
