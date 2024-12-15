using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;            // Speed of movement
    public float jumpForce = 5f;           // Force applied for jumping
    public float mouseSensitivity = 100f;  // Speed of mouse rotation

    private float xRotation = 0f;          // For vertical camera rotation
    private bool isGrounded = true;        // To check if the player is on the ground

    // Reference to the camera
    public Transform playerCamera;

    // Reference to Rigidbody
    private Rigidbody rb;

    void Start()
    {
        // Lock the cursor to the game window
        Cursor.lockState = CursorLockMode.Locked;

        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    public float rotateSpeed = 100f; // Speed of rotation
    public float joystickOffsetAngle = 45f; // Customizable offset in degrees

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleMouseLook();
    }

    void HandleMovement()
    {
        // Get movement input
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical");     // W/S

        // Calculate direction based on input
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;

        // Apply movement (ignoring Y-axis to prevent unintended vertical movement)
        Vector3 moveVelocity = moveDirection * moveSpeed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }

    void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // If there is controller input, use Right Stick instead
        if (Input.GetAxis("RightStickX") != 0 || Input.GetAxis("RightStickY") != 0)
        {
            // Get right stick input
            float rightStickX = Input.GetAxis("RightStickX");
            float rightStickY = Input.GetAxis("RightStickY");

            // Combine input into a direction vector
            Vector2 stickInput = new Vector2(rightStickX, rightStickY);

            // Apply joystick offset (convert offset angle to radians)
            float offsetRadians = joystickOffsetAngle * Mathf.Deg2Rad;
            float adjustedX = Mathf.Cos(offsetRadians) * stickInput.x - Mathf.Sin(offsetRadians) * stickInput.y;
            float adjustedY = Mathf.Sin(offsetRadians) * stickInput.x + Mathf.Cos(offsetRadians) * stickInput.y;

            // Replace input with adjusted values
            rightStickX = adjustedX;
            rightStickY = adjustedY;

            // Scale the right stick input to match mouse sensitivity
            mouseX = rightStickX * mouseSensitivity * Time.deltaTime;
            mouseY = rightStickY * mouseSensitivity * Time.deltaTime;
        }

        // Rotate player horizontally (around the Y-axis) for yaw
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera vertically (around the X-axis) for pitch
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevent over-rotation (up/down)
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleJump()
    {
        // Check for jump input and if the player is on the ground
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // Prevent double jumps
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Re-enable jumping when the player lands on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
