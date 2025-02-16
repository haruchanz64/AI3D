using UnityEngine;

namespace AI
{
    public class AreaOfSurveillance : MonoBehaviour
    {
        // Reference to the player's Transform component
        private Transform player;

        // Radius within which the AI can detect the player
        [SerializeField] private float detectionRadius = 30f;

        // Distance at which the AI will alert upon detecting the player
        [SerializeField] private float alertDistance = 15f;

        // Flag to indicate whether the player has been detected
        private bool _playerDetected = false;

        // Store the initial position of the AI
        private Vector3 _initialPosition;

        // Speed at which the AI moves
        [SerializeField] private float followSpeed = 5f;

        private void Start()
        {
            // Find the player GameObject by tag and get its Transform component
            player = GameObject.FindGameObjectWithTag("Player").transform;

            // Store the initial position of the AI
            _initialPosition = transform.position;
        }

        private void Update()
        {
            // Check if the player is within the detection area
            if (IsPlayerInDetectionArea())
            {
                if (!_playerDetected)
                {
                    _playerDetected = true;
                    OnPlayerDetected();
                }

                // Face the player
                FacePlayer();

                // Follow the player if within alert distance
                if (IsPlayerInAlertArea())
                {
                    FollowPlayer();
                }
            }
            else
            {
                // If the player is not in the detection area, reset the detection flag
                _playerDetected = false;

                // Move back to the initial position
                MoveToInitialPosition();
            }
        }

        // Checks if the player is within the detection radius
        private bool IsPlayerInDetectionArea()
        {
            // Calculate the distance between the AI and the player
            return Vector3.Distance(transform.position, player.position) < detectionRadius;
        }

        // Checks if the player is within the alert radius
        private bool IsPlayerInAlertArea()
        {
            // Calculate the distance between the AI and the player
            return Vector3.Distance(transform.position, player.position) < alertDistance;
        }

        // Method called when the player is detected
        private void OnPlayerDetected()
        {
            // Log a message indicating that the player has been detected
            Debug.Log("Player detected!");
        }

        // Face the player
        private void FacePlayer()
        {
            // Calculate the direction to the player
            Vector3 direction = (player.position - transform.position).normalized;

            // Optionally, make the AI look at the player
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * followSpeed);
        }

        // Follow the player
        private void FollowPlayer()
        {
            // Move the AI towards the player's position
            transform.position = Vector3.MoveTowards(transform.position, player.position, followSpeed * Time.deltaTime);
        }

        // Move the AI back to its initial position
        private void MoveToInitialPosition()
        {
            // Face the initial position while moving
            Vector3 directionToInitial = (_initialPosition - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToInitial);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * followSpeed);

            // Move the AI towards the initial position
            transform.position = Vector3.MoveTowards(transform.position, _initialPosition, Time.deltaTime * followSpeed);

            // Check if the AI has reached the initial position
            if (Vector3.Distance(transform.position, _initialPosition) < 0.1f)
            {
                // Set the AI to face forward (you can define what "forward" means in your context)
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0); // Keep the y rotation, reset x and z
            }
        }

        private void OnDrawGizmos()
        {
            // Draw the detection radius
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            // Draw the alert distance
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, alertDistance);
        }
    }
}