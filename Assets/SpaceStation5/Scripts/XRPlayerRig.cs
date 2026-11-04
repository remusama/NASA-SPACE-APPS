using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace SpaceStation5
{
    /// <summary>
    /// Builds a complete runtime VR rig for the existing Player_Astronaut_VR hierarchy.
    /// Adds head tracking, tracked controllers, visible controller models and near/far
    /// interactors without replacing the scene's player object.
    ///
    /// Desktop mode is untouched: the rig is only built once an XR display is active.
    /// </summary>
    [DefaultExecutionOrder(-500)]
    [DisallowMultipleComponent]
    public class XRPlayerRig : MonoBehaviour
    {
        [Header("Input (XRI Default Input Actions)")]
        [SerializeField] private InputActionAsset xriInputActions;

        [Header("Controller visuals (XR Interaction Toolkit Starter Assets)")]
        [SerializeField] private GameObject leftControllerModelPrefab;
        [SerializeField] private GameObject rightControllerModelPrefab;

        [Header("Interactors (XR Interaction Toolkit Starter Assets)")]
        [SerializeField] private GameObject leftInteractorPrefab;
        [SerializeField] private GameObject rightInteractorPrefab;

        [Header("Options")]
        [SerializeField] private bool createInteractors = true;
        [SerializeField] private bool logDeviceInfo = true;
        [SerializeField] private float deviceModeCameraHeight = 1.6f;
        [SerializeField] private float floorRaycastDistance = 5f;

        private const string HeadPositionAction = "XRI Head/Position";
        private const string HeadRotationAction = "XRI Head/Rotation";
        private const string HeadTrackingAction = "XRI Head/Tracking State";

        private const string LeftPositionAction = "XRI Left/Position";
        private const string LeftRotationAction = "XRI Left/Rotation";
        private const string LeftTrackingAction = "XRI Left/Tracking State";

        private const string RightPositionAction = "XRI Right/Position";
        private const string RightRotationAction = "XRI Right/Rotation";
        private const string RightTrackingAction = "XRI Right/Tracking State";

        private Camera headCamera;
        private TrackedPoseDriver headPoseDriver;
        private CharacterController characterController;
        private InputActionManager inputActionManager;
        private XRInputSubsystem inputSubsystem;
        private XROrigin xrOrigin;
        private Transform cameraOffset;
        private GameObject leftHand;
        private GameObject rightHand;

        private const float FloorTrackingRetryInterval = 1f;

        private bool rigReady;
        private bool floorTrackingRequested;
        private bool floorTrackingConfirmed;
        private bool floorTrackingUnsupported;
        private bool warnedFloorFailed;
        private bool warnedNoInputSubsystem;
        private float nextFloorTrackingAttempt;

        private void Awake()
        {
            headCamera = GetComponentInChildren<Camera>(true);
            if (headCamera == null)
            {
                Debug.LogError("[XRPlayerRig] No se encontro una Camera hija. El rig VR no puede inicializarse.", this);
                enabled = false;
                return;
            }

            headPoseDriver = headCamera.GetComponent<TrackedPoseDriver>();
            characterController = GetComponent<CharacterController>();

            if (XRSettings.isDeviceActive)
            {
                BuildRig();
            }
        }

        private void Update()
        {
            if (!rigReady)
            {
                if (XRSettings.isDeviceActive)
                {
                    BuildRig();
                }

                return;
            }

            // If the runtime was still bringing the session up when the rig was built, keep
            // trying to switch to Floor tracking. This can otherwise leave the camera at the
            // fallback height even though the device supports room-scale tracking.
            if (!floorTrackingConfirmed && !floorTrackingUnsupported)
            {
                RequestFloorTrackingOrigin();
            }

            UpdateCameraOffset();
        }

        private void BuildRig()
        {
            if (rigReady || !XRSettings.isDeviceActive)
            {
                return;
            }

            rigReady = true;

            EnableXriActions();
            EnsureInteractionManager();
            RequestFloorTrackingOrigin();
            SnapPlayerToFloor();
            CreateCameraOffset();
            ConfigureHeadTracking();
            EnsureXrOrigin();
            CreateHands();
            UpdateCameraOffset();

            if (logDeviceInfo)
            {
                LogDevices();
            }

            Debug.Log("[XRPlayerRig] Rig VR listo: cabeza, mandos y controladores conectados.", this);
        }

        private void EnableXriActions()
        {
            if (xriInputActions == null)
            {
                Debug.LogWarning("[XRPlayerRig] No hay 'XRI Default Input Actions' asignado. Se usaran acciones directas.", this);
                return;
            }

            inputActionManager = GetComponent<InputActionManager>();
            if (inputActionManager == null)
            {
                inputActionManager = gameObject.AddComponent<InputActionManager>();
            }

            var assets = inputActionManager.actionAssets;
            if (assets == null)
            {
                assets = new List<InputActionAsset>();
            }

            if (!assets.Contains(xriInputActions))
            {
                assets.Add(xriInputActions);
            }

            inputActionManager.actionAssets = assets;
            inputActionManager.EnableInput();
        }

        private void EnsureInteractionManager()
        {
            if (FindAnyObjectByType<XRInteractionManager>() != null)
            {
                return;
            }

            var managerObject = new GameObject("XR Interaction Manager");
            var parent = transform.parent;
            managerObject.transform.SetParent(parent != null ? parent : transform, false);
            managerObject.AddComponent<XRInteractionManager>();
        }

        private void RequestFloorTrackingOrigin()
        {
            if (floorTrackingConfirmed || floorTrackingUnsupported)
            {
                return;
            }

            // The XR session may not be fully initialized during Awake. Retry at a low rate
            // instead of giving up on the first attempt.
            if (floorTrackingRequested && Time.unscaledTime < nextFloorTrackingAttempt)
            {
                return;
            }

            floorTrackingRequested = true;
            nextFloorTrackingAttempt = Time.unscaledTime + FloorTrackingRetryInterval;

            var subsystems = new List<XRInputSubsystem>();
            SubsystemManager.GetSubsystems(subsystems);

            inputSubsystem = null;
            var anySubsystemSupportsFloor = false;

            foreach (var subsystem in subsystems)
            {
                if (subsystem == null)
                {
                    continue;
                }

                inputSubsystem = subsystem;

                // The runtime can destroy the subsystem (headset disconnected, session
                // ending) between discovery and these native calls. Do not let that break
                // the update loop; the retry logic will look for a live subsystem again.
                try
                {
                    var supportedModes = subsystem.GetSupportedTrackingOriginModes();
                    if ((supportedModes & TrackingOriginModeFlags.Floor) == 0)
                    {
                        continue;
                    }

                    anySubsystemSupportsFloor = true;

                    if (subsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor))
                    {
                        floorTrackingConfirmed = true;
                        return;
                    }
                }
                catch (System.Exception)
                {
                    inputSubsystem = null;
                    continue;
                }

                if (!warnedFloorFailed)
                {
                    warnedFloorFailed = true;
                    Debug.LogWarning("[XRPlayerRig] El runtime aun no pudo activar el origen Floor. " +
                                     "Se reintentara automaticamente mientras se usa la altura de respaldo.", this);
                }
            }

            if (inputSubsystem != null && !anySubsystemSupportsFloor)
            {
                floorTrackingUnsupported = true;
                Debug.LogWarning("[XRPlayerRig] El runtime no soporta el origen Floor. " +
                                 $"Se usara una altura de camara de respaldo de {deviceModeCameraHeight} m.", this);
            }

            if (inputSubsystem == null && !warnedNoInputSubsystem)
            {
                warnedNoInputSubsystem = true;
                Debug.LogWarning("[XRPlayerRig] No se encontro ningun XRInputSubsystem activo. " +
                                 "Se reintentara automaticamente.", this);
            }
        }

        /// <summary>
        /// The scene spawns the player slightly above the floor. In VR the head height is
        /// driven by the headset, so the root must be exactly on the floor to avoid floating
        /// or clipping through the station.
        /// </summary>
        private void SnapPlayerToFloor()
        {
            var origin = transform.position + Vector3.up * 0.5f;
            var hits = Physics.RaycastAll(origin, Vector3.down, floorRaycastDistance, ~0, QueryTriggerInteraction.Ignore);

            var closestDistance = float.MaxValue;
            foreach (var hit in hits)
            {
                if (hit.collider == null)
                {
                    continue;
                }

                if (characterController != null && hit.collider == characterController)
                {
                    continue;
                }

                if (hit.collider.transform.IsChildOf(transform))
                {
                    continue;
                }

                if (hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                }
            }

            if (closestDistance == float.MaxValue)
            {
                return;
            }

            var position = transform.position;
            position.y = origin.y - closestDistance;

            if (characterController != null)
            {
                characterController.enabled = false;
                transform.position = position;
                characterController.enabled = true;
            }
            else
            {
                transform.position = position;
            }
        }

        private void CreateCameraOffset()
        {
            if (cameraOffset != null)
            {
                return;
            }

            var offsetObject = new GameObject("Camera Offset");
            cameraOffset = offsetObject.transform;
            cameraOffset.SetParent(transform, false);
            cameraOffset.localPosition = Vector3.zero;
            cameraOffset.localRotation = Quaternion.identity;

            // Move the head camera under the offset. Its local pose is driven by the
            // TrackedPoseDriver; the offset carries the fallback eye height when the runtime
            // only offers device-relative tracking. Do not set a height here as well or the
            // camera would start at twice the intended height.
            headCamera.transform.SetParent(cameraOffset, false);
            headCamera.transform.localPosition = Vector3.zero;
            headCamera.transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// XRI requires an XROrigin in the scene for near/far interactor stabilisation and for
        /// distance-based attach velocity scaling. The existing player hierarchy already matches
        /// the expected structure (origin root -> camera offset -> camera), so the component is
        /// only added to describe it. It is created on an inactive child so Awake sees the camera
        /// and offset object already assigned instead of logging fallback warnings.
        /// </summary>
        private void EnsureXrOrigin()
        {
            if (xrOrigin != null)
            {
                return;
            }

            var originObject = new GameObject("XR Origin");
            originObject.SetActive(false);
            originObject.transform.SetParent(transform, false);

            xrOrigin = originObject.AddComponent<XROrigin>();
            xrOrigin.Origin = gameObject;
            xrOrigin.Camera = headCamera;
            xrOrigin.CameraFloorOffsetObject = cameraOffset != null ? cameraOffset.gameObject : null;
            xrOrigin.RequestedTrackingOriginMode = XROrigin.TrackingOriginMode.NotSpecified;
            xrOrigin.CameraYOffset = deviceModeCameraHeight;

            originObject.SetActive(true);
        }

        private void ConfigureHeadTracking()
        {
            if (headPoseDriver == null)
            {
                headPoseDriver = headCamera.gameObject.AddComponent<TrackedPoseDriver>();
            }

            ConfigureTracking(
                headPoseDriver,
                HeadPositionAction, "<XRHMD>/centerEyePosition",
                HeadRotationAction, "<XRHMD>/centerEyeRotation",
                HeadTrackingAction, "<XRHMD>/trackingState");

            headPoseDriver.enabled = true;
        }

        private void CreateHands()
        {
            var handParent = cameraOffset != null ? cameraOffset : transform;

            leftHand = CreateHand(
                "Left Hand",
                handParent,
                LeftPositionAction, "<XRController>{LeftHand}/devicePosition",
                LeftRotationAction, "<XRController>{LeftHand}/deviceRotation",
                LeftTrackingAction, "<XRController>{LeftHand}/trackingState",
                leftControllerModelPrefab,
                leftInteractorPrefab);

            rightHand = CreateHand(
                "Right Hand",
                handParent,
                RightPositionAction, "<XRController>{RightHand}/devicePosition",
                RightRotationAction, "<XRController>{RightHand}/deviceRotation",
                RightTrackingAction, "<XRController>{RightHand}/trackingState",
                rightControllerModelPrefab,
                rightInteractorPrefab);
        }

        private GameObject CreateHand(
            string handName,
            Transform parent,
            string positionAction,
            string positionFallback,
            string rotationAction,
            string rotationFallback,
            string trackingAction,
            string trackingFallback,
            GameObject modelPrefab,
            GameObject interactorPrefab)
        {
            var hand = new GameObject(handName);
            hand.transform.SetParent(parent, false);
            hand.transform.localPosition = Vector3.zero;
            hand.transform.localRotation = Quaternion.identity;

            var poseDriver = hand.AddComponent<TrackedPoseDriver>();
            ConfigureTracking(
                poseDriver,
                positionAction, positionFallback,
                rotationAction, rotationFallback,
                trackingAction, trackingFallback);

            if (modelPrefab != null)
            {
                // Keep the prefab's authored local offset/rotation (mirrored for the right hand).
                var model = Instantiate(modelPrefab, hand.transform, false);
                model.name = handName + " Visual";
            }

            if (createInteractors && interactorPrefab != null)
            {
                var interactor = Instantiate(interactorPrefab, hand.transform, false);
                interactor.name = handName + " Interactor";
                interactor.transform.localPosition = Vector3.zero;
                interactor.transform.localRotation = Quaternion.identity;
            }

            return hand;
        }

        private void ConfigureTracking(
            TrackedPoseDriver driver,
            string positionAction,
            string positionFallback,
            string rotationAction,
            string rotationFallback,
            string trackingAction,
            string trackingFallback)
        {
            driver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
            driver.updateType = TrackedPoseDriver.UpdateType.UpdateAndBeforeRender;
            driver.ignoreTrackingState = false;

            driver.positionInput = CreateActionProperty(positionAction, positionFallback, "Vector3");
            driver.rotationInput = CreateActionProperty(rotationAction, rotationFallback, "Quaternion");
            driver.trackingStateInput = CreateActionProperty(trackingAction, trackingFallback, "Integer");
        }

        private InputActionProperty CreateActionProperty(string xriActionPath, string fallbackBinding, string expectedControlType)
        {
            if (xriInputActions != null)
            {
                var action = xriInputActions.FindAction(xriActionPath, false);
                if (action != null)
                {
                    // The InputActionManager enables the shared asset, so the driver does not
                    // need to own the action's enabled state.
                    return new InputActionProperty(InputActionReference.Create(action));
                }
            }

            var fallbackAction = new InputAction(xriActionPath, InputActionType.Value, fallbackBinding,
                expectedControlType: expectedControlType);

            // Direct actions are enabled and disabled automatically by TrackedPoseDriver.
            return new InputActionProperty(fallbackAction);
        }

        private void UpdateCameraOffset()
        {
            if (cameraOffset == null)
            {
                return;
            }

            var floorTracking = false;
            if (inputSubsystem != null)
            {
                try
                {
                    var mode = inputSubsystem.GetTrackingOriginMode();
                    floorTracking = (mode & TrackingOriginModeFlags.Floor) != 0;
                }
                catch (System.Exception)
                {
                    // Subsystem torn down mid-session; fall back to the configured height
                    // instead of throwing from Update every frame.
                    inputSubsystem = null;
                }
            }

            var targetY = floorTracking ? 0f : deviceModeCameraHeight;
            var localPosition = cameraOffset.localPosition;
            if (!Mathf.Approximately(localPosition.y, targetY))
            {
                localPosition.y = targetY;
                cameraOffset.localPosition = localPosition;
            }
        }

        private void LogDevices()
        {
            LogDevice("HMD", InputDevices.GetDeviceAtXRNode(XRNode.Head));
            LogDevice("Mando izquierdo", InputDevices.GetDeviceAtXRNode(XRNode.LeftHand));
            LogDevice("Mando derecho", InputDevices.GetDeviceAtXRNode(XRNode.RightHand));

            var originMode = "desconocido";
            if (inputSubsystem != null)
            {
                try
                {
                    originMode = inputSubsystem.GetTrackingOriginMode().ToString();
                }
                catch (System.Exception)
                {
                    inputSubsystem = null;
                }
            }

            Debug.Log($"[XRPlayerRig] Origen de seguimiento: {originMode}");
        }

        private static void LogDevice(string label, UnityEngine.XR.InputDevice device)
        {
            if (!device.isValid)
            {
                Debug.LogWarning($"[XRPlayerRig] {label}: no detectado.");
                return;
            }

            Debug.Log($"[XRPlayerRig] {label}: {device.name} | fabricante: {device.manufacturer} | serie: {device.serialNumber}");
        }
    }
}
