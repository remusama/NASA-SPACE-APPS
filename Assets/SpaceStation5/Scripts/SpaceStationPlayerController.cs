using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace SpaceStation5
{
    /// <summary>
    /// Robust First Person Controller with collision, head bobbing, flashlight, and New Input System support.
    /// Supports both standard desktop keyboard/mouse and full Meta Quest VR head/controller tracking with snap/continuous turns.
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
        public float vrSnapTurnAngle = 45.0f;

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
        private bool vrSnapTurnReset = true;

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
            bool isVR = UnityEngine.XR.XRSettings.isDeviceActive;

            // In VR with an active XR display, head rotation is driven directly by the XR device tracking
            if (isVR)
            {
                float turnInput = 0f;

                // 1. Check XR Right Hand controller primary2DAxis
                var rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
                if (rightHandDevice.isValid && rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 rightStick))
                {
                    turnInput = rightStick.x;
                }

                // 2. Fallback to Gamepad right stick
                if (Mathf.Abs(turnInput) < 0.1f)
                {
                    var gp = Gamepad.current;
                    if (gp != null)
                    {
                        turnInput = gp.rightStick.ReadValue().x;
                    }
                }

                // Snap Turn handling
                if (Mathf.Abs(turnInput) > 0.5f)
                {
                    if (vrSnapTurnReset)
                    {
                        float angle = Mathf.Sign(turnInput) * vrSnapTurnAngle;
                        transform.Rotate(Vector3.up * angle);
                        vrSnapTurnReset = false;
                    }
                }
                else if (Mathf.Abs(turnInput) < 0.2f)
                {
                    vrSnapTurnReset = true;
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

            // VR Left Controller Thumbstick
            var leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            if (leftHandDevice.isValid)
            {
                if (leftHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 moveStick))
                {
                    horizontal += moveStick.x;
                    vertical += moveStick.y;
                }
                if (leftHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out bool thumbClick) && thumbClick)
                {
                    isSprinting = true;
                }
            }

            // VR Right Controller Button (A button = primaryButton) for Jump
            var rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            if (rightHandDevice.isValid)
            {
                if (rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool aBtn) && aBtn)
                {
                    jumpPressed = true;
                }
            }

            // Also support Gamepad
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

            // Move relative to player transform (or player camera if in VR)
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            if (playerCamera != null && UnityEngine.XR.XRSettings.isDeviceActive)
            {
                forward = Vector3.ProjectOnPlane(playerCamera.transform.forward, Vector3.up).normalized;
                right = Vector3.ProjectOnPlane(playerCamera.transform.right, Vector3.up).normalized;
            }

            Vector3 move = right * horizontal + forward * vertical;
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
            bool toggle = false;

            var kb = Keyboard.current;
            if (kb != null && kb.fKey.wasPressedThisFrame)
            {
                toggle = true;
            }

            // VR Controller Y Button or Secondary Button
            var leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            if (leftHandDevice.isValid && leftHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out bool yBtn) && yBtn)
            {
                toggle = true;
            }

            if (toggle && flashlight != null)
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
