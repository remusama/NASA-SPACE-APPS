using UnityEngine;
using UnityEngine.InputSystem;

namespace SpaceStation5
{
    /// <summary>
    /// Robust First Person Controller with collision, head bobbing, flashlight, and New Input System support.
    /// Scaled to standard 1.8m human height.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class SpaceStationPlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float walkSpeed = 5.0f;
        public float sprintSpeed = 9.0f;
        public float jumpHeight = 1.3f;
        public float gravity = -15.0f;

        [Header("Look Settings")]
        public float mouseSensitivity = 0.15f;
        public float lookUpLimit = 85.0f;
        public float lookDownLimit = -85.0f;

        [Header("Head Bobbing")]
        public bool enableHeadBob = true;
        public float bobFrequency = 8.0f;
        public float bobHorizontalAmplitude = 0.04f;
        public float bobVerticalAmplitude = 0.05f;

        [Header("Safety & Spawn")]
        public Vector3 spawnPosition = new Vector3(0, 1.2f, 6.0f);

        [Header("References")]
        public Camera playerCamera;
        public Light flashlight;

        private CharacterController controller;
        private Vector3 velocity;
        private float verticalRotation = 0f;
        private Vector3 defaultCameraLocalPos;
        private float bobTimer = 0f;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.4f;
            controller.center = new Vector3(0, 0.9f, 0);

            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>();
            }

            if (playerCamera != null)
            {
                defaultCameraLocalPos = playerCamera.transform.localPosition;
            }

            if (spawnPosition == Vector3.zero)
            {
                spawnPosition = transform.position;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            // Fall/Void protection: immediately recover if pushed or clipped out of bounds
            if (transform.position.y < -15.0f)
            {
                velocity = Vector3.zero;
                controller.enabled = false;
                transform.position = spawnPosition;
                controller.enabled = true;
            }

            HandleRotation();
            HandleMovement();
            HandleHeadBob();
            HandleFlashlight();
            HandleCursorToggle();
        }

        private void HandleRotation()
        {
            // In VR with an active XR display, head tracking is handled physically by TrackedPoseDriver
            if (UnityEngine.XR.XRSettings.isDeviceActive)
            {
                var gp = Gamepad.current;
                if (gp != null)
                {
                    float stickX = gp.rightStick.ReadValue().x;
                    if (Mathf.Abs(stickX) > 0.2f)
                    {
                        transform.Rotate(Vector3.up * stickX * 90f * Time.deltaTime);
                    }
                }
                return;
            }

            if (Cursor.lockState != CursorLockMode.Locked) return;

            var mouse = Mouse.current;
            if (mouse == null) return;

            Vector2 mouseDelta = mouse.delta.ReadValue();
            float mouseX = mouseDelta.x * mouseSensitivity;
            float mouseY = mouseDelta.y * mouseSensitivity;

            // Horizontal rotation (turns body)
            transform.Rotate(Vector3.up * mouseX);

            // Vertical rotation (pitches camera)
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, lookDownLimit, lookUpLimit);

            if (playerCamera != null)
            {
                playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
            }
        }

        private void HandleMovement()
        {
            bool isGrounded = controller.isGrounded;
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            var kb = Keyboard.current;
            float horizontal = 0f;
            float vertical = 0f;
            bool isSprinting = false;
            bool jumpPressed = false;

            if (kb != null)
            {
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed) vertical += 1f;
                if (kb.sKey.isPressed || kb.downArrowKey.isPressed) vertical -= 1f;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) horizontal += 1f;
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) horizontal -= 1f;

                isSprinting = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;
                jumpPressed = kb.spaceKey.wasPressedThisFrame;
            }

            // Also support Gamepad / VR Controller Thumbsticks
            var gp = Gamepad.current;
            if (gp != null)
            {
                Vector2 stick = gp.leftStick.ReadValue();
                horizontal += stick.x;
                vertical += stick.y;
                if (gp.leftStickButton.isPressed) isSprinting = true;
                if (gp.buttonSouth.wasPressedThisFrame) jumpPressed = true;
            }

            float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

            Vector3 move = transform.right * horizontal + transform.forward * vertical;
            if (move.sqrMagnitude > 1f) move.Normalize();

            if (jumpPressed && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;

            // Unified single-pass movement to prevent discrete physics jitter and ground tunneling
            Vector3 totalMovement = (move * currentSpeed + velocity) * Time.deltaTime;
            controller.Move(totalMovement);
        }

        private void HandleHeadBob()
        {
            if (UnityEngine.XR.XRSettings.isDeviceActive) return; // Prevent motion sickness in VR
            if (!enableHeadBob || playerCamera == null) return;

            var kb = Keyboard.current;
            bool isMoving = false;
            bool isSprinting = false;

            if (kb != null)
            {
                isMoving = kb.wKey.isPressed || kb.sKey.isPressed || kb.aKey.isPressed || kb.dKey.isPressed;
                isSprinting = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;
            }

            if (controller.isGrounded && isMoving)
            {
                float speedMultiplier = isSprinting ? 1.4f : 1.0f;
                bobTimer += Time.deltaTime * bobFrequency * speedMultiplier;
                float bobX = Mathf.Cos(bobTimer * 0.5f) * bobHorizontalAmplitude;
                float bobY = Mathf.Sin(bobTimer) * bobVerticalAmplitude;

                playerCamera.transform.localPosition = defaultCameraLocalPos + new Vector3(bobX, bobY, 0);
            }
            else
            {
                bobTimer = 0f;
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, defaultCameraLocalPos, Time.deltaTime * 6f);
            }
        }

        private void HandleFlashlight()
        {
            var kb = Keyboard.current;
            if (kb != null && flashlight != null && kb.fKey.wasPressedThisFrame)
            {
                flashlight.enabled = !flashlight.enabled;
            }
        }

        private void HandleCursorToggle()
        {
            var kb = Keyboard.current;
            if (kb != null && kb.escapeKey.wasPressedThisFrame)
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }
    }
}
