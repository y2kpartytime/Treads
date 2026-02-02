using UnityEngine;

public class Engineer : MonoBehaviour
{
    Animator animator;

    // Setup 2 float for vertical/horizontal input
    float verticalInput;
    float horizontalInput;
    void Start () 
    {
        //get the Animator Controller Component from the character component hierarchy
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () 
    {
        // Get the input from vertical/horizontal axis
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
    }
    void FixedUpdate()
    {
        // Now set the animator float values (vAxisInput/hAxisInput)
        animator.SetFloat ("vAxisInput", verticalInput);
        animator.SetFloat ("hAxisInput", horizontalInput);

        // Detect W Key press
        if (Input.GetKey (KeyCode.Z)) 
        {
            // Set runBool to true if pressed
            animator.SetBool ("isRunning", true);
            Debug.Log ("Run");
        } 
        else 
        {
            // Set runBool to false if not pressed
            animator.SetBool ("isRunning", false);
            Debug.Log ("No Run");
        }
        

        // Detect C Key press
        if (Input.GetKey (KeyCode.C)) 
        {
            // Set the Crouch Layer Weight to 0.5, this
            // activtes the masked couch animation
            animator.SetLayerWeight (0, 0.5f);
        } 
        else 
        {
            // Set the Couch Layer Weight back to 0.0
            // This deactivated the crouch animation
            animator.SetLayerWeight (0, 0.0f);
        }

    }
}
