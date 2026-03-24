using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 700f;
    public float jumpForce = 5f;
    public float fallThreshold = -10f; // Y position when falling (void check)
    
    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isGrounded;
    private bool isRagdolling = false;

    // To track last checkpoint position
    private Vector3 lastCheckpointPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false; // Ensure the player has physics applied
        lastCheckpointPosition = transform.position; // Start with current position as last checkpoint
    }

    void Update()
    {
        if (!isRagdolling)
        {
            HandleMovement();
            HandleJump();
        }

        // If the player falls below the threshold, reset to the last checkpoint
        if (transform.position.y < fallThreshold)
        {
            RespawnAtCheckpoint();
        }
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        if (moveDirection != Vector3.zero)
        {
            // Rotate player to face direction of movement
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        // Use MovePosition to apply movement based on physics
        rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Stop downward velocity before applying the jump
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // Reset vertical velocity
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply jump force
        }
    }

    void OnCollisionStay()
    {
        isGrounded = true;
    }

    void OnCollisionExit()
    {
        isGrounded = false;
    }

    public void ActivateRagdoll()
    {
        if (!isRagdolling)
        {
            isRagdolling = true;
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
        }
    }

    public void DeactivateRagdoll()
    {
        if (isRagdolling)
        {
            isRagdolling = false;
            rb.isKinematic = true;
        }
    }

    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        lastCheckpointPosition = checkpointPosition;
    }

    public void RespawnAtCheckpoint()
    {
        transform.position = lastCheckpointPosition;
        rb.linearVelocity = Vector3.zero;
        }
}