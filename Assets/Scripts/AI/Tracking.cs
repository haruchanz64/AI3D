using UnityEngine;

namespace AI
{
    public class Tracking : MonoBehaviour
    {
        // Reference to the player's Transform component
        private Transform _player;

        // Constant speed at which the AI will move towards the player
        [SerializeField] private float speed = 8f;

        // Detection range within which the AI will start tracking the player
        [SerializeField] private float detectionRange = 10f;

        // Field of view angle for line of sight
        [SerializeField] private float fieldOfViewAngle = 45f;

        // Rotation speed for the AI to face the player
        [SerializeField] private float rotationSpeed = 5f;

        private void Start()
        {
            // Find the player GameObject by tag and get its Transform component
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            // Calculate the distance between the AI and the player
            var distanceToPlayer = Vector3.Distance(transform.position, _player.position);

            // Check if the player is within the detection range and line of sight
            if (distanceToPlayer < detectionRange)
            {
                Debug.Log("Player is detected within radius.");
                if (IsPlayerInLineOfSight())
                {
                    // If the player is detected, call the method to track the player
                    OnTrackPlayer();
                }
            }
        }

        private void OnTrackPlayer()
        {
            // Calculate the direction from the AI to the player
            var direction = (_player.position - transform.position).normalized;

            // Smoothly rotate towards the player's position
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            // Move the AI towards the player's position
            transform.position += direction * (speed * Time.deltaTime);
        }

        private bool IsPlayerInLineOfSight()
        {
            // Calculate the direction to the player
            Vector3 directionToPlayer = (_player.position - transform.position).normalized;

            // Check if the angle between the AI's forward direction and the direction to the player is within the field of view
            float angle = Vector3.Angle(transform.forward, directionToPlayer);
            if (angle < fieldOfViewAngle / 2)
            {
                // Perform a raycast to check for obstacles between the AI and the player
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
                {
                    // If the raycast hits the player, return true
                    if (hit.transform == _player)
                    {
                        return true;
                    }
                }
            }
            return false; // Player is either out of sight or obstructed
        }

        private void OnDrawGizmos()
        {
            // Draw the detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Draw the field of view
            Gizmos.color = Color.green;
            Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfViewAngle / 2, 0) * transform.forward * detectionRange;
            Vector3 rightBoundary = Quaternion.Euler(0, fieldOfViewAngle / 2, 0) * transform.forward * detectionRange;
            Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

            // Draw the line of sight (optional)
            Gizmos.color = Color.red;

            // Check if _player is not null before drawing the line
            if (_player != null)
            {
                Gizmos.DrawLine(transform.position, _player.position);
            }
        }
    }
}