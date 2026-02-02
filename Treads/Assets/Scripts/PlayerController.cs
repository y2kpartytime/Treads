using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float rotation;
    Vector2 inputs;
    Vector3 velocity;
    public Camera playerCam;
    CharacterController controller;
    float gravity = -13f, velocityY, terminalVelocity = -20;
    float currentSpeed;
    public float baseSpeed = 8f, runSpeed = 4f, rotateSpeed = 0.5f, rotateMult = 2;
    bool run = true, jump;
    bool jumping, airDirLocked, jumpingWhileStill, isFalling;
    public float jumpSpeed, jumpHeight, jumpSlow;
    Vector3 jumpDirection;
    bool isSliding;
    bool canJump;
    Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        GetInputs();
        Movement();
        Gravity();
        Rotation();
    }

    void Movement()
    {   
        //input direction and speed
        Vector3 inputDir = transform.forward * inputs.y + transform.right * inputs.x;
        currentSpeed = baseSpeed;

        if (controller.isGrounded)
        {
            isFalling = false;
            jumping = false;
            jumpingWhileStill = false;
            airDirLocked = false;

            if (run) currentSpeed *= runSpeed;

            if (!isSliding)
            {
                if (jump) Jump();
            }
        }
        
        //velocity
        if (jumping)
        {
            //jumping logic
            if (!airDirLocked)
            {
                if (inputDir.sqrMagnitude > 0.25f)
                {
                    jumpDirection = inputDir.normalized;
                    airDirLocked = true;
                }
            }

            float slowSpeed = jumpSpeed;

            if (jumpingWhileStill)
            {
                slowSpeed *= jumpSlow;
            }

            velocity = jumpDirection * slowSpeed + Vector3.up * velocityY;
            Debug.Log("Jumping");
        }
        else if (isFalling)
        {
            //if falling off ledge we get no control
            //keeps horizontal velocity
            Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
            velocity = horizontalVelocity + Vector3.up * velocityY;
        }
        else
        {
            //if grounded and not jumping, normal movement, i hate this so much
            jumpDirection = inputDir.normalized;
            velocity = jumpDirection * currentSpeed + Vector3.up * velocityY;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void GetInputs()
    {
        //fowards + backward + strafe controls
        if (Input.GetKey(KeyCode.W))
        {
            animator.SetBool("isRunning", true);
            inputs.y = 1;
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.W))
            {
                inputs.y = 0;
            }
            else
            {
                animator.SetBool("isRunningBackward", true);
                inputs.y = -1;
            }
        }
        
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            inputs.y = 0;
        }

        //strafing
        if (Input.GetKey(KeyCode.A))
        {
            inputs.x = -1;
        }

        //left/right
        if (Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.A))
            {
                inputs.x = 0;
            }
            else
            {
                inputs.x = 1;
            }
        }
        //stand still
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            inputs.x = 0;
        }

        if (Input.GetKey(KeyCode.D))
        {
            rotation = 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            {
                if (Input.GetKey(KeyCode.D))
                {
                    rotation = 0;
                }
                else
                {
                    rotation = -1;
                }
            }
        }

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            rotation = 0;
        }

        //RP walk toggle :3
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            run = !run;
        }

        //jumping
        jump = Input.GetKey(KeyCode.Space);
        }
    
    
    public void Jump()
    {
        if (!jumping)
        {
            jumping = true;
        }

        Vector3 inputDir = transform.forward * inputs.y + transform.right * inputs.x;

        if (inputDir.sqrMagnitude > 0.01f)
        {
            //lock directions if jump-move
            jumpDirection = inputDir.normalized;
            airDirLocked = true;
            jumpingWhileStill = false;
            isFalling = true;
        }
        else
        {
            //slow jump if standing still
            jumpDirection = Vector3.zero;
            airDirLocked = false;
            jumpSpeed = currentSpeed * jumpSlow;
            jumpingWhileStill = true;
        }

        //set upward speed
        jumpSpeed = currentSpeed;
        velocityY = Mathf.Sqrt(-gravity * jumpHeight);
    }

    public void Gravity()
    {
        //gravity and Y velocity
        if (!controller.isGrounded)
        {   
            velocityY += gravity * Time.deltaTime;
            
            if (velocityY < terminalVelocity)
                velocityY = terminalVelocity;

            isFalling = true;
            Debug.Log("Falling");
        }
        else
        {
            //ewhen grounded
            isFalling = false;
            velocityY = -4f;
        }

        Vector3 origin = transform.position + Vector3.up * 0.1f;
        float radius = controller.radius * 0.95f;
        float distance = controller.height * 0.5f + 0.2f;

        if (Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit hitInfo, distance))
        {
            float groundAngle = Vector3.Angle(hitInfo.normal, Vector3.up);

            if (groundAngle > controller.slopeLimit)
            {
                isSliding = true;
                canJump = false;
                Vector3 slideDir = Vector3.ProjectOnPlane(Vector3.down, hitInfo.normal).normalized;

                float slideSpeed = 4f;

                velocityY += gravity * Time.deltaTime;

                Vector3 slideVelocity = slideDir * slideSpeed;
                slideVelocity.y = velocityY;

                controller.Move(slideVelocity * Time.deltaTime);
                Debug.Log("Sliding");
                return;
            }
            else
            {
                isSliding = false;
                canJump = true;
            }
        }
    }

    public void Rotation()
    {
        Vector3 characterRotation = transform.eulerAngles + new Vector3(0, rotation * rotateSpeed, 0);
        transform.eulerAngles = characterRotation;
    }
}

