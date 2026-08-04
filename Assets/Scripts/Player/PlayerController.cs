using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform; // optional camera reference for movement direction
    [SerializeField] private float speed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private bool shouldfaceCameraDirection = false; // whether the player should face the camera direction when moving

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;

    private bool isSprinting = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // lock the mouse cursor to the game window
        Cursor.visible = false; // hide the cursor during play
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    // Create a Sprint action in your Input Actions and bind it to Left Shift
    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValueAsButton();
    }

    void Update()
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f; // keep movement horizontal
        right.y = 0f; // keep movement horizontal
        forward.Normalize();
        right.Normalize();
        Vector3 movedirection = forward * moveInput.y + right * moveInput.x;

        // Choose walking or sprinting speed
        float currentSpeed = isSprinting ? sprintSpeed : speed;

        controller.Move(movedirection * currentSpeed * Time.deltaTime);

        if (shouldfaceCameraDirection && movedirection.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(movedirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 10f);
        }

        // Prevent gravity from building up while grounded
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Apply vertical movement
        controller.Move(velocity * Time.deltaTime);
    }
}