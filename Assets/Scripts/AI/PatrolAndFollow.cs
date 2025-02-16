using System.Linq;
using UnityEngine;
// Required for ToArray()

namespace AI
{
    public class PatrolAndFollow : MonoBehaviour
    {
        // Array of waypoints for the AI to patrol between
        [SerializeField] private Transform[] waypoints;

        // Constant speed at which the AI moves
        private const float Speed = 8f;

        // Distance within which the AI will start following the player
        [SerializeField] private float followDistance = 5f;

        // Reference to the player transform
        private Transform _player;

        // Index of the current waypoint the AI is moving towards
        private int _currentWaypointIndex = 0;

        private void Start()
        {
            // Find the player GameObject by tag and get its Transform component
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                _player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player not found! Make sure there is a GameObject with the 'Player' tag.");
            }

            // Find all waypoint GameObjects by tag and assign their Transform components to the waypoints array
            GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("Waypoint");
            waypoints = waypointObjects.Select(waypoint => waypoint.transform).ToArray();

            // Check if waypoints were found
            if (waypoints.Length == 0)
            {
                Debug.LogError("No waypoints found! Make sure there are GameObjects with the 'Waypoint' tag.");
            }
        }

        private void Update()
        {
            // Calculate the distance between the AI and the player
            if (_player != null)
            {
                var distanceToPlayer = Vector3.Distance(transform.position, _player.position);

                // Check if the player is within the follow distance
                if (distanceToPlayer < followDistance)
                {
                    // If the player is close enough, follow the player
                    OnFollowPlayer();
                }
                else
                {
                    // If the player is too far, patrol between waypoints
                    OnPatrolArea();
                }
            }
        }

        private void OnPatrolArea()
        {
            // Get the current target waypoint
            var targetWaypoint = waypoints[_currentWaypointIndex];

            // Move the AI towards the target waypoint
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, Speed * Time.deltaTime);

            // Calculate the direction to the target waypoint
            Vector3 direction = (targetWaypoint.position - transform.position).normalized;

            // Calculate the rotation needed to look at the target waypoint
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Smoothly rotate towards the target waypoint
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * Speed);

            // Check if the AI has reached the target waypoint
            if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
            {
                // Move to the next waypoint, looping back to the start if necessary
                _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
            }
        }

        private void OnFollowPlayer()
        {
            // Calculate the direction to the player
            Vector3 direction = (_player.position - transform.position).normalized;

            // Calculate the rotation needed to look at the player
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Smoothly rotate towards the player
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * Speed);

            // Move the AI towards the player's position
            transform.position = Vector3.MoveTowards(transform.position, _player.position, Speed * Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            // Draw waypoints
            if (waypoints != null)
            {
                Gizmos.color = Color.blue;
                foreach (var waypoint in waypoints)
                {
                    if (waypoint != null)
                    {
                        Gizmos.DrawSphere(waypoint.position, 0.5f); // Draw a sphere at each waypoint
                    }
                }
            }

            // Draw follow distance
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, followDistance); // Draw a wire sphere around the AI
        }
    }
}