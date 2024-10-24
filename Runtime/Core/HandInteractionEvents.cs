using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Hands;

namespace JioXSDK.Interactions
{
    [DefaultExecutionOrder(-1)]
    public class HandInteractionEvents : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActionAsset;

        private XRHandSubsystem m_Subsystem;

        private InputAction onPinchRightHand;
        private InputAction pointerPositionRightHand;
        private InputAction pointerRotationRightHand;
        private InputAction onPinchLeftHand;
        private InputAction pointerPositionLeftHand;
        private InputAction pointerRotationLeftHand;
        private InputAction rightHandPinchPosition;
        private InputAction leftHandPinchPosition;

        public static Action OnRightHandPinch;
        public static Action OnRightHandPinchRelease;
        public static Action OnLeftHandPinch;
        public static Action OnLeftHandPinchRelease;

        public static Action RightHandTrackingAcquired;
        public static Action LeftHandTrackingAcquired;
        public static Action RightHandTrackingLost;
        public static Action LeftHandTrackingLost;

        public static bool isRightPinchHold = false;
        public static bool isLeftPinchHold = false;

        public static Vector3 RightHandPinchPosition { get; private set; }
        public static Vector3 RightHandPinchUnitPosition
        {
            get
            {
                return new Vector3(RightHandPinchPosition.x + .5f, RightHandPinchPosition.y + .5f, RightHandPinchPosition.z + .5f);
            }
            private set { }
        }

        public static Vector3 LeftHandPinchPosition { get; private set; }
        public static Vector3 LeftHandPinchUnitPosition
        {
            get
            {
                return new Vector3(LeftHandPinchPosition.x + .5f, LeftHandPinchPosition.y + .5f, LeftHandPinchPosition.z + .5f);
            }
            private set { }
        }

        public static Vector3 RightHandPosition { get; private set; }
        public static Vector3 RightHandUnitPosition
        {
            get
            {
                return new Vector3(RightHandPosition.x + .5f, RightHandPosition.y + .5f, RightHandPosition.z + .5f);
            }
            private set { }
        }
        public static Vector3 LeftHandPosition { get; private set; }
        public static Vector3 LeftHandUnitPosition
        {
            get
            {
                return new Vector3(LeftHandPosition.x + .5f, RightHandPosition.y + .5f, RightHandPosition.z + .5f);
            }
            private set { }
        }


        private void OnEnable()
        {
            // Get the action maps
            var rightHandActionMap = inputActionAsset.FindActionMap("RightHand");
            var leftHandActionMap = inputActionAsset.FindActionMap("LeftHand");

            // Get the actions
            onPinchRightHand = rightHandActionMap.FindAction("Pinch");
            pointerPositionRightHand = rightHandActionMap.FindAction("Pointer Position");
            pointerRotationRightHand = rightHandActionMap.FindAction("Pointer Rotation");
            onPinchLeftHand = leftHandActionMap.FindAction("Pinch");
            pointerPositionLeftHand = leftHandActionMap.FindAction("Pointer Position");
            pointerRotationLeftHand = leftHandActionMap.FindAction("Pointer Rotation");
            rightHandPinchPosition = rightHandActionMap.FindAction("Pinch Position");
            leftHandPinchPosition = leftHandActionMap.FindAction("Pinch Position");

            // Enable the actions
            onPinchRightHand.Enable();
            rightHandPinchPosition.Enable();
            leftHandPinchPosition.Enable();
            pointerPositionRightHand.Enable();
            pointerRotationRightHand.Enable();
            onPinchLeftHand.Enable();
            pointerPositionLeftHand.Enable();
            pointerRotationLeftHand.Enable();

            onPinchLeftHand.performed += OnLeftHandPinchPerformed;
            onPinchRightHand.performed += OnRightHandPinchPerformed;
        }

        private void OnDisable()
        {
            // Disable the actions
            onPinchRightHand.Disable();
            rightHandPinchPosition.Disable();
            leftHandPinchPosition.Disable();
            pointerPositionRightHand.Disable();
            pointerRotationRightHand.Disable();
            onPinchLeftHand.Disable();
            pointerPositionLeftHand.Disable();
            pointerRotationLeftHand.Disable();
            onPinchLeftHand.performed -= OnLeftHandPinchPerformed;
            onPinchRightHand.performed -= OnRightHandPinchPerformed;
        }

        private void Update()
        {
            RightHandPosition = pointerPositionRightHand.ReadValue<Vector3>();
            LeftHandPosition = pointerPositionLeftHand.ReadValue<Vector3>();
            RightHandPinchPosition = rightHandPinchPosition.ReadValue<Vector3>();
            LeftHandPinchPosition = leftHandPinchPosition.ReadValue<Vector3>();
            HandleSubSystem();

            if (isRightPinchHold)
            {
                if (!onPinchRightHand.IsInProgress())
                {
                    isRightPinchHold = false;
                    OnRightHandPinchRelease?.Invoke();
                }
            }

            if (isLeftPinchHold)
            {
                if (!onPinchLeftHand.IsInProgress())
                {
                    isLeftPinchHold = false;
                    OnLeftHandPinchRelease?.Invoke();
                }
            }
        }

        private void OnRightHandPinchPerformed(InputAction.CallbackContext obj)
        {
            OnRightHandPinch?.Invoke();
            isRightPinchHold = true;
        }

        private void OnLeftHandPinchPerformed(InputAction.CallbackContext obj)
        {
            OnLeftHandPinch?.Invoke();
            isLeftPinchHold = true;
        }

        private void HandleSubSystem()
        {
            List<XRHandSubsystem> subsystems = new();
            SubsystemManager.GetSubsystems(subsystems);

            for (var i = 0; i < subsystems.Count; ++i)
            {
                var handSubsystem = subsystems[i];
                if (handSubsystem.running)
                {
                    SetSubsystem(handSubsystem);
                    break;
                }
            }
        }

        void UnsubscribeFromSubsystem()
        {
            if (m_Subsystem != null)
            {
                m_Subsystem.trackingAcquired -= OnTrackingAcquired;
                m_Subsystem.trackingLost -= OnTrackingLost;
                m_Subsystem.updatedHands -= OnUpdatedHands;
                m_Subsystem = null;
            }
        }


        internal void SetSubsystem(XRHandSubsystem handSubsystem)
        {

            UnsubscribeFromSubsystem();
            m_Subsystem = handSubsystem;

            m_Subsystem.trackingAcquired += OnTrackingAcquired;
            m_Subsystem.trackingLost += OnTrackingLost;
            m_Subsystem.updatedHands += OnUpdatedHands;
        }

        private void OnUpdatedHands(XRHandSubsystem subsystem, XRHandSubsystem.UpdateSuccessFlags flags, XRHandSubsystem.UpdateType type)
        {
            return;
        }

        private void OnTrackingLost(XRHand hand)
        {
            if (hand.handedness == Handedness.Left)
                LeftHandTrackingLost?.Invoke();

            if (hand.handedness == Handedness.Right)
                RightHandTrackingLost?.Invoke();
        }

        private void OnTrackingAcquired(XRHand hand)
        {
            if (hand.handedness == Handedness.Left)
                LeftHandTrackingAcquired?.Invoke();

            if (hand.handedness == Handedness.Right)
                RightHandTrackingAcquired?.Invoke();
        }
    }
}
