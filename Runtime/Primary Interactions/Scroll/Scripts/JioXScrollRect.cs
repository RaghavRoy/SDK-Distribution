using JioXSDK.Interactions;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace JioXSDK
{
    public class JioXScrollRect : ScrollRect, IPointerEnterHandler, IPointerExitHandler
    {

        #region Serialized Properties
        [SerializeField] float handScrollSensitivity = 1f; //Scrolling sensitivity
        [SerializeField] private JioXScrollBar JioXScrollBar; // Scroll bar if needed
        [SerializeField] private float velocityDecayRate = 5f; // Controls how fast the velocity decays (like friction)
        [SerializeField] private float minVelocityThreshold = 0.01f;  // Minimum velocity to stop scrolling
        [SerializeField] private float scrollMultiplier = 100f;  // Multiplier to amplify the hand movement

        [SerializeField] private InputActionProperty _leftControllerTracking; //Left hand tracking event
        [SerializeField] private InputActionProperty _rightControllerTracking; //Right hand tracking event 

        [SerializeField] private float movementThreshold = 1f;
        [SerializeField] private Vector2 previousScrollPosition;
        #endregion

        #region Private properties
        private bool isGazing = false; //To check if gazing (in pointerEnter and pointerExit)
        private bool isGazeControllerEnabled = false; //if gazeEnabled
        private bool areControllersEnabled = false; //if controllers enabled
        private bool isPinching = false; //if pinching with hands
        private bool isDragging = false;

        private Hand currentHand; //Current hand which is pinching
        private enum Hand
        {
            none,
            left,
            right
        }

        private Vector3 lastHandPosition; //Stores hand position when initially started pinching
        private Vector3 scrollVelocity;    // Stores the current scrolling velocity
        Vector3 movementDelta; // Store movement direction between lastHandPosition and currentHandPosition 
        public Action threshHoldReached;
        #endregion

        #region Monobehaviour functions
        protected override void Awake()
        {
            //Debug.Log($"##Scroll start");

            _leftControllerTracking.action.Enable();
            _rightControllerTracking.action.Enable();

            _leftControllerTracking.action.performed += LeftControllerTrackingPerformed;
            _rightControllerTracking.action.performed += RightControllerTrackingPerformed;

            _leftControllerTracking.action.canceled += LeftControllerTrackingLost;
            _rightControllerTracking.action.canceled += RightControllerTrackingLost;

            HandInteractionEvents.OnLeftHandPinch += OnLeftPinchStart;
            HandInteractionEvents.OnRightHandPinch += OnRightPinchStart;
            HandInteractionEvents.OnLeftHandPinchRelease += OnLeftPinchRelease;
            HandInteractionEvents.OnRightHandPinchRelease += OnRightPinchRelease;

#if UNITY_EDITOR
            ChangeGazeState(false);
#else
            ChangeGazeState(true); //By default gaze is enabled. 

#endif

            if (JioXScrollBar)
            {
                if (vertical)
                    verticalScrollbar = JioXScrollBar;
                else
                    horizontalScrollbar = JioXScrollBar;
            }

            base.Awake();



        }

        // Override Update to handle velocity-based scrolling
        private void Update()
        {

            if (isPinching)
            {
                //Debug.Log($"##Scroll pinching");
                Vector3 currentHandPosition = currentHand == Hand.left ? HandInteractionEvents.LeftHandPosition : HandInteractionEvents.RightHandPosition; //Get current hand's position
                movementDelta = currentHandPosition - lastHandPosition; //Calculate movement direction
                //Debug.Log($"##Scroll Movement delta {movementDelta}, current hadn pos {currentHandPosition}, last hand pos {lastHandPosition}");
                // Apply the multiplier to amplify the movement delta
                ScrollBasedOnHandMovement(movementDelta * scrollMultiplier); //Using scroll multiplier here otherwise hand position values are negligible

                // Update the last hand position for the next frame
                lastHandPosition = currentHandPosition;

                // Apply the same multiplier to the velocity calculation
                scrollVelocity = (movementDelta * 5f) / Time.deltaTime;  // Calculates amplified velocity
            }
            else if (!isPinching && isGazing && scrollVelocity.magnitude > minVelocityThreshold)
            {
                // If not pinching, apply the velocity-based scrolling with decay (momentum)
                //Debug.Log($"##Scroll applying velocity");
                ScrollBasedOnVelocity();
                scrollVelocity = Vector3.Lerp(scrollVelocity, Vector3.zero, velocityDecayRate * Time.deltaTime);  // Decay the velocity
            }

            SendThreshHoldCallback();
        }

        private void SendThreshHoldCallback()
        {
            Vector2 currentScrollPosition = this.normalizedPosition;

            // Calculate the magnitude of the scroll movement
            float movementMagnitude = Vector2.Distance(previousScrollPosition, currentScrollPosition);

            //Debug.Log($"Movement magnitutde {movementMagnitude}");
            // Check if the movement is significant
            if (movementMagnitude > movementThreshold)
            {
                // Deactivate the object if movement is significant
                //Debug.Log($"Movement magnitutde invoking");
                threshHoldReached?.Invoke();

                // Optionally, update the previous scroll position only if significant movement occurred
                previousScrollPosition = currentScrollPosition;
            }
            else
            {
                // Update the previous scroll position to the current one if no significant movement occurred
                previousScrollPosition = currentScrollPosition;
            }
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

        // Override Update to prevent velocity-based scrolling after drag
        protected override void LateUpdate()
        {
            // If you want to stop all default scrolling including momentum after dragging
            if (isPinching) // if gazing return, we'll scroll using our logic
            {
                // Stop any velocity-based movement
                return;
            }

            base.LateUpdate();
        }
        #endregion

        #region Pointer Events
        // Override OnScroll to block any base scrolling input (mouse wheel, etc.)
        public override void OnScroll(PointerEventData data)
        {
            if (isPinching) // if gazing return, we'll scroll using our logic
            {
                //Debug.Log($"##Scroll Doing Onscroll isGazing");
                // Optionally, call the base if you want default scrolling in some cases.
                // base.OnScroll(data);
                return;
            }
            //Debug.Log($"##Scroll doing OnScroll not isGazing");
            base.OnScroll(data);
        }

        // Override to block drag scrolling
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (isPinching) // if gazing return, we'll scroll using our logic
            {
                isDragging = true;
                //Debug.Log($"##Scroll doing onBeginDrag isGazing");
                return; // Prevent base drag behavior
            }
            //Debug.Log($"##Scroll doing onBeginDrag not isGazing");
            base.OnBeginDrag(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (isPinching) // if gazing return, we'll scroll using our logic
            {
                //Debug.Log($"##Scroll doin onDrag isGazing");
                isDragging = true;
                return; // Prevent base drag behavior
            }
            //Debug.Log($"##Scroll doing onDrag not isGazing");
            base.OnDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (isPinching) // if gazing return, we'll scroll using our logic
            {
                //Debug.Log($"##Scroll doing onEndDrag isGazing");
                isDragging = false;
                return; // Prevent base drag behavior
            }
            Debug.Log($"##Scroll doing onEndDrag not isGazing");

            base.OnEndDrag(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Debug.Log($"##Scroll onPointerEnter");
            if (IsGazeEnabled())
            {
                isGazing = true;
            }
            else
            {
                isGazing = false;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log($"##Scroll onPointerExit");
            isGazing = false;
        }
        #endregion

        #region Hand Scrolling Logic
        /// <summary>
        /// Takes vector3 value calculated by using  <see cref="lastHandPosition"/> and current hand position
        /// </summary>
        /// <param name="movementDelta"></param>
        private void ScrollBasedOnHandMovement(Vector3 movementDelta)
        {
            float scrollVertical = -movementDelta.y * handScrollSensitivity; // calculate vertical movement
            float scrollHorizontal = -movementDelta.x * handScrollSensitivity; //calculate horizontal movement

            //Debug.Log($"##Scroll scrollHorizontal {scrollHorizontal}");

            if (movementDelta.y != 0 && vertical)
            {
                this.verticalNormalizedPosition = Mathf.Clamp01(verticalNormalizedPosition + scrollVertical * Time.deltaTime);
                if (JioXScrollBar) JioXScrollBar.SyncWithScrollRect(verticalNormalizedPosition); //Update scroll bar if available
                onValueChanged?.Invoke(new Vector2(0f, scrollVertical));
            }

            if (movementDelta.x != 0 && horizontal)
            {
                this.horizontalNormalizedPosition = Mathf.Clamp01(horizontalNormalizedPosition + scrollHorizontal * Time.deltaTime);
                if (JioXScrollBar) JioXScrollBar.SyncWithScrollRect(horizontalNormalizedPosition); //Update scroll bar if available
                onValueChanged?.Invoke(new Vector2(scrollHorizontal, 0f));
            }

        }

        // Apply the scrolling based on the velocity
        private void ScrollBasedOnVelocity()
        {
            float scrollVertical = -scrollVelocity.y * handScrollSensitivity;
            float scrollHorizontal = -scrollVelocity.x * handScrollSensitivity;

            if (vertical && Mathf.Abs(scrollVertical) > minVelocityThreshold)
            {
                this.verticalNormalizedPosition = Mathf.Clamp01(verticalNormalizedPosition + scrollVertical * Time.deltaTime);
                if (JioXScrollBar) JioXScrollBar.SyncWithScrollRect(verticalNormalizedPosition);
            }

            if (horizontal && Mathf.Abs(scrollHorizontal) > minVelocityThreshold)
            {
                this.horizontalNormalizedPosition = Mathf.Clamp01(horizontalNormalizedPosition + scrollHorizontal * Time.deltaTime);
                if (JioXScrollBar) JioXScrollBar.SyncWithScrollRect(horizontalNormalizedPosition);
            }
        }

        private void OnLeftPinchStart()
        {
            if (!isGazing) return; //Register pinch only if gazing
            isPinching = true;
            currentHand = Hand.left;
            lastHandPosition = HandInteractionEvents.LeftHandPinchPosition;
            scrollVelocity = Vector3.zero;  // Reset velocity when starting a pinch
        }

        private void OnRightPinchStart()
        {
            if (!isGazing) return; //Register pinch only if gazing
            isPinching = true;
            currentHand = Hand.right;
            lastHandPosition = HandInteractionEvents.RightHandPinchPosition;
            scrollVelocity = Vector3.zero;  // Reset velocity when starting a pinch
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

        #region Controller and Gaze state logic
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

        public void OnScrollbarValueChanged(float value)
        {
            if (vertical)
                verticalNormalizedPosition = value;
            else if (horizontal)
                horizontalNormalizedPosition = value;
        }

        private void RightControllerTrackingPerformed(InputAction.CallbackContext context)
        {
            //Debug.Log($"##Scroll right Controller tracking performed");
            ChangeGazeState(false);
            ChangeControllerState(true);
        }

        private void LeftControllerTrackingPerformed(InputAction.CallbackContext context)
        {
            //Debug.Log($"##Scroll left Controller tracking performed");
            ChangeGazeState(false);
            ChangeControllerState(true);
        }

        private void RightControllerTrackingLost(InputAction.CallbackContext context)
        {


            //Debug.Log($"##Scroll right Controller tracking Lost");
            ChangeGazeState(true);
            ChangeControllerState(false);
        }

        private void LeftControllerTrackingLost(InputAction.CallbackContext context)
        {
            //Debug.Log($"##Scroll left Controller tracking Lost");
            ChangeGazeState(true);
            ChangeControllerState(false);
        }
        #endregion
    }
}

