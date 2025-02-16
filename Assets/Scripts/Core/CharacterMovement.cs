using UnityEngine;

namespace Core
{
    public class CharacterMovement : MonoBehaviour
    {
        private CharacterController characterController;
        private Vector3 velocity;
        private bool isPlayerGrounded;

        [Header("Movement Settings")]
        [SerializeField] private float movementSpeed = 12.0f;
        [SerializeField] private float jumpHeight = 2.5f;
        [SerializeField] private float gravityValue = -9.81f;

        private void Start()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            // Check if the player is grounded
            isPlayerGrounded = characterController.isGrounded;

            // Reset vertical velocity if grounded
            if (isPlayerGrounded && velocity.y < 0)
            {
                velocity.y = 0;
            }

            // Handle horizontal movement
            HandleMovement();

            // Apply gravity
            velocity.y += gravityValue * Time.deltaTime;

            // Move the character with the updated velocity
            characterController.Move(velocity * Time.deltaTime);
        }

        private void HandleMovement()
        {
            // Get input
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // Create a movement vector based on camera orientation
            Vector3 move = new Vector3(horizontalInput, 0f, verticalInput);
            move.y = 0; // Keep the movement on the ground plane

            // Normalize the movement vector to prevent faster diagonal movement
            if (move.magnitude > 1f)
            {
                move.Normalize();
            }

            // Move the character
            characterController.Move(move * Time.deltaTime * movementSpeed);

            // Rotate the player to face the direction of movement
            if (move != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }

            // Handle jumping
            if (Input.GetButtonDown("Jump") && isPlayerGrounded)
            {
                velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravityValue); // Calculate jump velocity
            }
        }
    }
}