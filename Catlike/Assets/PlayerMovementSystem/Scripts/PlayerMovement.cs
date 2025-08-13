using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    private float verticalVelocity; // handles Y-axis movement
    private bool isGrounded;
    
    [Header("Crouch Settings")]
    public float crouchHeight = 0.5f;      // height when crouched
    public float standHeight = 2f;       // normal height
    public float crouchSpeed = 2.5f;     // slower speed
    private bool isCrouching;


    [Header("References")]
    private CharacterController controller;
    private Animator animator;
    
    private Vector2 moveInput; // from Input System
    private bool isRunning;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // These will be called by PlayerInput events
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed) isRunning = true;
        if (context.canceled) isRunning = false;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }
    }
    
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isCrouching = !isCrouching; // toggle
            controller.height = isCrouching ? crouchHeight : standHeight;
        }
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // small downward force to stick to ground
        }
        
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        if (move.magnitude >= 0.1f)
        {
            //rotate player to where the movement direction is
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            
            float speed;
            if (isCrouching)
            {
                speed = crouchSpeed;
            }
            else if (isRunning)
            {
                speed = runSpeed;
            }
            else
            {
                speed = walkSpeed;
            }
            controller.Move(move * speed * Time.deltaTime);
        }
        float targetHeight = isCrouching ? crouchHeight : standHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * runSpeed);

        //gravity applied
        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
        
        //animator parameters updated
        float animationSpeed = new Vector3(moveInput.x, 0, moveInput.y).magnitude;
        animator.SetFloat("Speed", animationSpeed * (isRunning ? 1f : 0.5f)); //for blend tree
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetFloat("Crouch", animationSpeed * (isCrouching ? 1f : 0.5f));
        animator.SetBool("IsGrounded", isGrounded);
    }
}