using UnityEngine;

namespace SpaceStation5
{
    /// <summary>
    /// Proximity sliding door for airlocks and room entrances.
    /// </summary>
    public class StationDoor : MonoBehaviour
    {
        [Header("Door Panels")]
        public Transform leftPanel;
        public Transform rightPanel;
        public MeshRenderer statusLight;

        [Header("Settings")]
        public float slideDistance = 1.8f;
        public float openSpeed = 3.5f;
        public float detectionRadius = 4.0f;
        public bool isOpen = false;

        private Vector3 leftClosedPos;
        private Vector3 rightClosedPos;
        private Vector3 leftOpenPos;
        private Vector3 rightOpenPos;
        private Transform playerTransform;

        private void Start()
        {
            if (leftPanel != null)
            {
                leftClosedPos = leftPanel.localPosition;
                leftOpenPos = leftClosedPos + Vector3.left * slideDistance;
                // Ensure colliders don't block the player like solid walls if physics gets stuck
                var col = leftPanel.GetComponent<Collider>();
                if (col != null) col.isTrigger = false;
            }
            if (rightPanel != null)
            {
                rightClosedPos = rightPanel.localPosition;
                rightOpenPos = rightClosedPos + Vector3.right * slideDistance;
                var col = rightPanel.GetComponent<Collider>();
                if (col != null) col.isTrigger = false;
            }

            // Find player
            var player = GameObject.FindWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }

        private void Update()
        {
            if (playerTransform == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null) playerTransform = player.transform;
            }

            if (playerTransform != null)
            {
                float dist = Vector3.Distance(transform.position, playerTransform.position);
                isOpen = dist < detectionRadius;
            }
            else
            {
                // If player is not yet found or in editor, keep open so nothing is blocked
                isOpen = true;
            }

            Vector3 targetLeft = isOpen ? leftOpenPos : leftClosedPos;
            Vector3 targetRight = isOpen ? rightOpenPos : rightClosedPos;

            if (leftPanel != null)
                leftPanel.localPosition = Vector3.Lerp(leftPanel.localPosition, targetLeft, Time.deltaTime * openSpeed);

            if (rightPanel != null)
                rightPanel.localPosition = Vector3.Lerp(rightPanel.localPosition, targetRight, Time.deltaTime * openSpeed);
        }
    }
}
