using UnityEngine;
using System.Collections;

public class Scene1_Script2 : MonoBehaviour {

	// Get reference to the player sphere
	public GameObject player;
	// Variable to keep track offset between player and camera
	private Vector3 offset;

	// Use this for initialization
	void Start () {

		offset = transform.position - player.transform.position;

	}
	// Update is called once per frame
	void LateUpdate () {

		transform.position = player.transform.position + offset;
	}
}