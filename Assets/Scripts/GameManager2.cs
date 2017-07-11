using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager2 : MonoBehaviour {


	// s_Instance is used to cache the instance found in the scene so we don't have to look it up every time.
	private static GameManager2 s_Instance = null;

	// This defines a static instance property that attempts to find the manager object in the scene and
	// returns it to the caller.
	public static GameManager2 instance
	{
		get
		{
			//so Jamie King said that this is lazy initialization because we aren't storing a reference to s_Instance as soon as the code runs


			if (s_Instance == null)
			{
				// This is where the magic happens.
				//  FindObjectOfType(...) returns the first GameManager2 object in the scene.
				s_Instance = FindObjectOfType(typeof(GameManager2)) as GameManager2;
			}

			// If it is still null, create a new instance
			if (s_Instance == null)
			{
				GameObject obj = new GameObject("GameManager2");
				s_Instance = obj.AddComponent(typeof(GameManager2)) as GameManager2;
				Debug.Log("Could not locate an GameManager2 object. GameManager2 was generated Automatically.");
			}

			return s_Instance;
		}
	}

	// Ensure that the instance is destroyed when the game is stopped in the editor.
	void OnApplicationQuit()
	{
		s_Instance = null;
	}
	

	[Header("Player Variables")]
	public bool singlePlayer = true;

	public static GameObject player;

	public static MouseRotate playerBodyRotateScript;
	public static MouseRotate playerHeadRotateScript;
	public static PlayerStatus1 playerStatus1Script;


	[Space(10)]


	[Header("Fall off the World Variables")]
	public float fallThreshold;
	public bool dieWhenFallOffWorld = false;
	private Vector3 respawnPosition;
	public float respawnOffset = 1;


	[Header("Item Sorting Variables")]

	public List<string> itemSlotOrder = new List<string>();

	


	// Use this for initialization
	void Start()
	{

		itemSlotOrder.Insert(0, "Hands");
		itemSlotOrder.Insert(1, "Semi");
		itemSlotOrder.Insert(2, "Hato");


		if (singlePlayer)
		{
			player = GameObject.Find("Player");
			playerStatus1Script = player.GetComponent<PlayerStatus1>();

			respawnPosition = player.transform.position;
			respawnPosition.y += respawnOffset;

			playerBodyRotateScript = player.GetComponent<MouseRotate>();


		}

	}

	// Update is called once per frame
	void Update()
	{

		if (player.transform.position.y <= fallThreshold)
		{
			if (!dieWhenFallOffWorld)
			{
				playerBodyRotateScript.Reset();
				playerHeadRotateScript.Reset();

				player.transform.position = respawnPosition;
			}
			else
			{
				playerStatus1Script.health = 0;
			}
		}

	}





}
