using UnityEngine;
using System.Collections;

public class Scene3_Script2 : MonoBehaviour {

	// Declare Animator Object
	Animator animator;

	// Use this for initialization
	void Start () {

		// Instantiate Animator Object 
		animator = GetComponent<Animator> ();
	}
		
	void OnTriggerEnter(Collider other){

		// Ensure door only Opens for Teddy
		if (other.gameObject.name == "Player" ) {

			// Set doorOpen bool to true
			// This triggers DoorSlideUp animation 
			animator.SetBool ("doorOpen", true);
			Debug.Log ("Door Open:" + other.gameObject.name);
		}
	}

	// This fucntion is triggered each time the collider exits interaction with Teddy
	void OnTriggerExit(Collider other){
		// Ensure door only Closes for Teddy
		if (other.gameObject.name == "Player") {

			// Set doorOpen bool to true
			// This triggers DoorSlideDown animation 
			animator.SetBool ("doorOpen", false);
			Debug.Log ("Door Close:" + other.gameObject.name);
		} 
	}
}
