using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JioXSDK.Interactions;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace JioXSDK
{
    public class JioXScrollBar : Scrollbar
    {
        #region Serialized properties
        [SerializeField] private JioXScrollRect scrollRect;  // Reference to the linked ScrollRect cannot be null

        [SerializeField] float handScrollSensitivity = 1f;
        [SerializeField] private float scrollMultiplier = 100f;  // Multiplier to amplify the hand movement

        [SerializeField] private InputActionProperty _leftControllerTracking;
        [SerializeField] private InputActionProperty _rightControllerTracking;

        [SerializeField] private float regularScrollMultiplier = 2f;   
        #endregion

        #region Private properties
        private bool vertical;
        private bool horizontal;
        private Hand currentHand;

        private enum Hand
        {
            none,
            left,
            right
        }
        private bool isDragging = false;
        private bool isGazing = false;
        private bool isGazeControllerEnabled = false;
        private bool areControllersEnabled = false;
        private bool isPinching = false;
        private Vector3 lastHandPosition;
        Vector3 movementDelta; 
        #endregion

        #region Mono behaviour functions
        protected override void Start()
        {
            HandInteractionEvents.OnLeftHandPinch += OnLeftPinchStart;
            HandInteractionEvents.OnRightHandPinch += OnRightPinchStart;
            HandInteractionEvents.OnLeftHandPinchRelease += OnLeftPinchRelease;
            HandInteractionEvents.OnRightHandPinchRelease += OnRightPinchRelease;

            _leftControllerTracking.action.Enable();
            _rightControllerTracking.action.Enable();

            _leftControllerTracking.action.performed += LeftControllerTrackingPerformed;
            _rightControllerTracking.action.performed += RightControllerTrackingPerformed;

            _leftControllerTracking.action.canceled += LeftControllerTrackingLost;
            _rightControllerTracking.action.canceled += RightControllerTrackingLost;

            ChangeGazeState(true);
            base.Start();

#if UNITY_EDITOR
            ChangeGazeState(false);
#else
            ChangeGazeState(true); //By default gaze is enabled. 

#endif
        }
        protected override void OnDestroy()
        {
            HandInteractionEvents.OnLeftHandPinch -= OnLeftPinchStart;
            HandInteractionEvents.OnRightHandPinch -= OnRightPinchStart;
            HandInteractionEvents.OnLeftHandPinchRelease -= OnLeftPinchRelease;
            HandInteractionEvents.OnRightHandPinchRelease -= OnRightPinchRelease;

            _leftControllerTracking.action.Disable();
            _rightControllerTracking.action.Disable();

            _leftControllerTracking.action.performed -= LeftControllerTrackingPerformed;
            _rightControllerTracking.action.performed -= RightControllerTrackingPerformed;

            _leftControllerTracking.action.canceled -= LeftControllerTrackingLost;
            _rightControllerTracking.action.canceled -= RightControllerTrackingLost;
        }
        private void Update()
        {
            if (isPinching)
            {
                //Debug.Log($"##Scrollbar moving");
                Vector3 currentHandPosition = currentHand == Hand.left ? HandInteractionEvents.LeftHandPosition : HandInteractionEvents.RightHandPosition;
                movementDelta = currentHandPosition - lastHandPosition;

                // Apply the multiplier to amplify the movement delta
                ScrollBasedOnHandMovement(movementDelta * scrollMultiplier);

                // Update the last hand position for the next frame
                lastHandPosition = currentHandPosition;
            }
        } 
        #endregion

        #region Pointer Events
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (isPinching)
            {
                //Debug.Log($"##Scroll doing onBeginDrag isGazing");
                return; // Prevent base drag behavior
            }
            //Debug.Log($"##Scroll doing onBeginDrag not isGazing");
            base.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (isPinching)
            {
                // Prevent normal drag when using hand pinching
                return;
            }

            // Increase scroll speed when dragging without hand tracking
            float dragDelta = (eventData.delta.y != 0) ? eventData.delta.y * regularScrollMultiplier : 0f;

            if (scrollRect.vertical)
            {
                var normalisedVal = Mathf.Clamp01(value + dragDelta * Time.deltaTime);
                value = normalisedVal;
            }

            base.OnDrag(eventData);  // Continue base drag behavior
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log($"Scroll bar --> pointerEnter");
            if (IsGazeEnabled())
            {
                isGazing = true;
                return;
            }
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log($"Scroll bar --> pointerExit");
            isGazing = false;
            base.OnPointerExit(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if(isPinching)
            {
                return;
            }
            base.OnPointerDown(eventData);
        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            if(isPinching)
            {
                return;
            }
            base.OnPointerUp(eventData);
        }
        #endregion

        #region Hand movement logic
        private void ScrollBasedOnHandMovement(Vector3 movementDelta)
        {
            float scrollVertical = movementDelta.y * handScrollSensitivity;
            float scrollHorizontal = movementDelta.x * handScrollSensitivity;
            //Debug.Log($"##ScrollBar -> {scrollVertical}, {scrollHorizontal}");
            if (movementDelta.y != 0 && scrollRect.vertical)
            {
                var normalisedVal = Mathf.Clamp01(value + scrollVertical * Time.deltaTime);
                Debug.Log($"##ScrollBar -->  normalisedVal = {normalisedVal}");
                value = normalisedVal;
                //if (scrollRect) scrollRect.OnScrollbarValueChanged(value);
            }

            if (movementDelta.x != 0 && scrollRect.horizontal)
            {
                var normalisedVal = Mathf.Clamp01(value + scrollHorizontal * Time.deltaTime);
                value = normalisedVal;//SyncWithScrollRect(normalisedVal);
                //if (scrollRect) scrollRect.OnScrollbarValueChanged(value);
            }

        }

        // Sync Scrollbar based on ScrollRect value
        public void SyncWithScrollRect(float scrollRectNormalizedPosition)
        {
            if (scrollRect == null) return;
            if (scrollRect.vertical)
                value = scrollRectNormalizedPosition;
            else if (scrollRect.horizontal)
                value = scrollRectNormalizedPosition;
        }

        private void OnLeftPinchStart()
        {
            if (!isGazing) return;
            isPinching = true;
            currentHand = Hand.left;
            lastHandPosition = HandInteractionEvents.LeftHandPinchPosition;
        }

        private void OnRightPinchStart()
        {
            if (!isGazing) return;
            isPinching = true;
            currentHand = Hand.right;
            lastHandPosition = HandInteractionEvents.RightHandPinchPosition;
        }

        private void OnLeftPinchRelease()
        {
            isPinching = false;
            currentHand = Hand.none;
        }

        private void OnRightPinchRelease()
        {
            isPinching = false;
            currentHand = Hand.none;
        } 
        #endregion

        #region Gaze and Controller states
        private void ChangeGazeState(bool state)
        {
            Debug.Log($"##Scroll gazeEnabled {state}");
            isGazeControllerEnabled = state;
        }

        private void ChangeControllerState(bool state)
        {
            Debug.Log($"##Scroll ControllersEnabled {state}");
            areControllersEnabled = state;

        }

        private bool IsGazeEnabled()
        {
            return isGazeControllerEnabled && !areControllersEnabled;
        }

        private void RightControllerTrackingPerformed(InputAction.CallbackContext context)
        {
            ChangeGazeState(false);
            ChangeControllerState(true);
        }

        private void LeftControllerTrackingPerformed(InputAction.CallbackContext context)
        {
            ChangeGazeState(false);
            ChangeControllerState(true);
        }

        private void RightControllerTrackingLost(InputAction.CallbackContext context)
        {

            ChangeGazeState(true);
            ChangeControllerState(false);
        }

        private void LeftControllerTrackingLost(InputAction.CallbackContext context)
        {
            ChangeGazeState(true);
            ChangeControllerState(false);
        } 
        #endregion

        
    }
}
