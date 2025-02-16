using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class NavMeshPatrol : MonoBehaviour
    {
        [SerializeField] private float detectionRange = 10f; // Range to detect the player
        [SerializeField] private Transform player; // Reference to the player
        [SerializeField] private float speed = 3.5f; // Speed of the patrol
        [SerializeField] private float followSpeed = 5f; // Speed when following the player
        [SerializeField] private float wanderRadius = 10f; // Radius within which to wander
        [SerializeField] private float wanderTimer = 5f; // Time between random wander movements

        private NavMeshAgent agent;
        private float timer;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = speed;
            timer = wanderTimer;

            player = GameObject.FindGameObjectWithTag("Player").transform;

            SetRandomDestination();
        }

        private void Update()
        {
            // Check distance to player
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= detectionRange)
            {
                // Follow the player
                agent.speed = followSpeed;
                agent.SetDestination(player.position);
            }
            else
            {
                // Wander randomly
                agent.speed = speed;

                // Update the timer
                timer += Time.deltaTime;
                if (timer >= wanderTimer)
                {
                    SetRandomDestination();
                    timer = 0; // Reset the timer
                }
            }
        }

        private void SetRandomDestination()
        {
            // Generate a random point within a sphere around the AI's current position
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;

            // Find a valid point on the NavMesh
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas);
            agent.SetDestination(hit.position);
        }

        private void OnDrawGizmos()
        {
            // Draw the detection range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Draw the wander radius
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, wanderRadius);

            // Draw a line to the player's position if the player is assigned
            if (player != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, player.position);
            }
        }
    }
}