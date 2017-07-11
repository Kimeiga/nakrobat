using UnityEngine;
using System.Collections;

public class SampleNavigation : MonoBehaviour {

    public Transform goal;
    private NavMeshAgent agent;

    // Use this for initialization
    void Start () {
         agent = GetComponent<NavMeshAgent>();
    }
	
	// Update is called once per frame
	void Update () {
        agent.destination = goal.position;
    }
}
