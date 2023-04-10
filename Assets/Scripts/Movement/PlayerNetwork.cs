using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : MonoBehaviour
{   
    //Inspector exposed
    [SerializeField] float moveSpeed = 3.0f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] private float jumpHeight = 3.0f;
    [SerializeField] Animator gunAnimator = null;
    [SerializeField] float crouchSpeed = 0.5f;
    [SerializeField] float walkSpeed = 1.75f;
    [SerializeField] float runSpeed = 4.34f;

    //private
    private CharacterController characterController = null;
    private Animator bodyAnimator = null;
    private Vector3 velocity = Vector3.zero;
    private float verticalSpeed = 1.75f;
    private float horizontalSpeed = 1.75f;

    private bool isGrounded = true;
    private bool isCrouching = false;
    private bool isRunning = false;

    //public
    public bool IsGrounded { get { return isGrounded; } }
    public bool IsCrouching { get { return isCrouching; } }
    public bool IsRunning { get { return isRunning; } }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        bodyAnimator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }

   
    void Update()
    {     

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayerMask);      

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float xAxis = Input.GetAxis("Horizontal") ;
        float zAxis = Input.GetAxis("Vertical");
        
        ManageMovement(xAxis, zAxis);   
    }

    

    private void ManageMovement(float xAxis, float zAxis)
    { 
        Vector3 moveDir = (transform.right * xAxis + transform.forward * zAxis).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            bodyAnimator.SetTrigger("Jump");
        }       

        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded)
        {
            isCrouching = !isCrouching;
            if (isCrouching)
            {
                verticalSpeed = crouchSpeed;
            }
            else
            {
                verticalSpeed = walkSpeed ;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            isRunning = !isRunning;
           
            if (isRunning)
            {                
                verticalSpeed = runSpeed;
                horizontalSpeed = runSpeed;
            }
            else
            {
                verticalSpeed = walkSpeed;
                horizontalSpeed = walkSpeed;
            }
        }

        if (gunAnimator.GetBool("aiming"))
        {
            bodyAnimator.SetFloat("Vertical",0);
            bodyAnimator.SetFloat("Horizontal", 0);
            characterController.Move(moveDir * Time.deltaTime);
        }
        else
        {
            bodyAnimator.SetFloat("Vertical", zAxis * verticalSpeed, 0.1f, Time.deltaTime);
            bodyAnimator.SetFloat("Horizontal", xAxis * horizontalSpeed, 0.1f, Time.deltaTime);
            gunAnimator.SetFloat("speed", zAxis * verticalSpeed);
            characterController.Move(moveDir * verticalSpeed * Time.deltaTime * moveSpeed);
        } 
        
        velocity.y += gravity * Time.deltaTime;       
        characterController.Move(velocity * Time.deltaTime);
    }
}
