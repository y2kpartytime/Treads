using UnityEngine;

public class Engineer : MonoBehaviour
{
    Animator animator;

    // Setup 2 float for vertical/horizontal input
    float verticalInput;
    float horizontalInput;
    public float turnSpeed = 150.0f;
    
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

        transform.Rotate(0f, horizontalInput * turnSpeed * Time.deltaTime, 0f);
    }

    void FixedUpdate()
    {
        // Now set the animator float values (vAxisInput/hAxisInput)
        animator.SetFloat ("vAxisInput", verticalInput);
        animator.SetFloat ("hAxisInput", horizontalInput);

        // Detect Run
        if (Input.GetKey(KeyCode.Z)) 
        {
            animator.SetBool ("isRunning", true);
            Debug.Log ("Run");
        } 
        else 
        {
            animator.SetBool ("isRunning", false);
            Debug.Log ("No Run");
        }

        // Detect Jump
        if (Input.GetKey (KeyCode.Space)) 
        {
            animator.SetTrigger("jumpTrigger");
            Debug.Log ("Jumped");
        } 
        else 
        {
            animator.ResetTrigger("jumpTrigger");
            animator.SetBool ("isJumping", false);
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            animator.SetBool ("isMoving", true);
        }
        else
        {
            animator.SetBool ("isMoving", false);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
        {
            animator.SetBool ("isTurning", true);
        }
        else
        {
            animator.SetBool ("isTurning", false);
        }

        // Detect Crouch layer swap
        if (Input.GetKey (KeyCode.C)) 
        {
            // Set the Crouch Layer Weight to 0.5, this
            // activtes the masked couch animation
            animator.SetLayerWeight (1, 0.5f);
        } 
        else 
        {
            // Set the Couch Layer Weight back to 0.0
            // This deactivated the crouch animation
            animator.SetLayerWeight (1, 0.0f);
        }
    }
}
