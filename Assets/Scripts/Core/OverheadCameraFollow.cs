using UnityEngine;

namespace Core
{
    public class OverheadCameraFollow : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform target; // The player or object to follow
        [SerializeField] private Vector3 offset; // Offset from the target position

        [Header("Overhead Camera Follow Settings")]
        [SerializeField] private float smoothSpeed = 0.125f; // Smoothing speed

        private void LateUpdate()
        {
            if (target == null) return;
            // Calculate the desired position based on the target's position and the offset
            var desiredPosition = target.position + offset;

            // Smoothly interpolate between the current position and the desired position
            var smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Update the camera's position
            transform.position = smoothedPosition;
        }
    }
}