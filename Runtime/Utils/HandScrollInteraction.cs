using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using JioXSDK.Interactions;

namespace JioXSDK
{
    public class HandScrollInteraction : MonoBehaviour
    {
        [SerializeField] private Transform mainCamera;
        [SerializeField] private InputActionProperty rightHandInputAction;
        [SerializeField] private InputActionProperty rightHandInputActionPosition;

        [SerializeField] private ScrollController scrollController;

        //[SerializeField] private SliderController sliderController;

        private Vector3 rightHandPosition;

        private bool isPinchHold;
        public bool IsPinchHold => isPinchHold;

        private ScrollRect horizontalScroll;

        private Vector3 lastPalmPostion = Vector3.zero;
        private RaycastHit hitinfo;
        private bool slideHit;

        public static Action pinchClicked;
        public static Action<RaycastHit, Vector3> OnSliderMoved;
        public static Action<Vector3> sliderCurrentPosition;

        public static Action OnUserClick;

        private float pinchTime = 0;

        private void OnEnable()
        {
            rightHandInputAction.EnableDirectAction();
            rightHandInputActionPosition.EnableDirectAction();
            rightHandInputAction.action.performed += PinchActionPerformedByRightHand;
            HandInteractionEvents.OnRightHandPinch += PinchActionPerformedByRightHand;
            HandInteractionEvents.OnRightHandPinchRelease += OnPinchRelease;
            //OnUserClick += HandleKeyboardState;
        }

        private void PinchActionPerformedByRightHand(InputAction.CallbackContext obj)
        {
            Debug.Log($"Pinch performed");
            pinchTime = 0;
            slideHit = false;
            ScrollRaycast();
            isPinchHold = true;
        }

        private void PinchActionPerformedByRightHand()
        {
            Debug.Log($"Pinch performed");
            pinchTime = 0;
            slideHit = false;
            ScrollRaycast();

            isPinchHold = true;
        }

        private void OnDisable()
        {
            rightHandInputAction.DisableDirectAction();
            rightHandInputActionPosition.DisableDirectAction();
            HandInteractionEvents.OnRightHandPinch -= PinchActionPerformedByRightHand;
            HandInteractionEvents.OnRightHandPinchRelease -= OnPinchRelease;
            //OnUserClick += HandleKeyboardState;
        }

        private void ScrollRaycast()
        {
            if (mainCamera == null) return;
            Ray _gazeRay = new Ray(mainCamera.position, mainCamera.forward);

            if (Physics.Raycast(_gazeRay, out hitinfo))
            {
                Debug.Log($"Pinch performed raycast hit {hitinfo.transform.name}");

                if (hitinfo.transform.TryGetComponent(out HorizontalScroll scrollRect))
                {
                    scrollController.InitializeScroll(rightHandInputActionPosition.action.ReadValue<Vector3>(), scrollRect);
                    scrollController.InitializeScroll(HandInteractionEvents.RightHandPosition, scrollRect);
                }
                else
                {
                    scrollController.InitializeScroll(rightHandInputActionPosition.action.ReadValue<Vector3>());
                    scrollController.InitializeScroll(HandInteractionEvents.RightHandPosition);
                }


                //Debug.Log($"Hitinfo slider: {hitinfo.point.x}, {hitinfo.point.y}, {hitinfo.point.z}");

                // if (hitinfo.transform.TryGetComponent(out Slider _slider))
                // {
                //     Debug.Log($"point pos : {_slider.transform.InverseTransformPoint(hitinfo.point)}");
                //     OnSliderMoved?.Invoke(hitinfo, rightHandInputActionPosition.action.ReadValue<Vector3>());

                //     slideHit = true;
                //     //sliderController.InitializeSlider(rightHandInputActionPosition.action.ReadValue<Vector3>(), _slider, hitinfo);
                // }
            }
            else
            {
                scrollController.InitializeScroll(rightHandInputActionPosition.action.ReadValue<Vector3>());
                scrollController.InitializeScroll(HandInteractionEvents.RightHandPosition);
            }
        }


        private void Update()
        {
            rightHandPosition = rightHandInputActionPosition.action.ReadValue<Vector3>();
            if (rightHandPosition != Vector3.zero)
            {
                rightHandPosition = HandInteractionEvents.RightHandPosition;
            }
            if (isPinchHold)
            {
                var isScroll = scrollController.CheckAndScroll(rightHandPosition);

                // if (slideHit)
                // {
                //     sliderCurrentPosition?.Invoke(rightHandPosition);
                // }

                if (!rightHandInputAction.action.IsInProgress())
                {
                    isPinchHold = false;
                    horizontalScroll = null;

                    if ((pinchTime is > 0 and <= 0.500f) && !isScroll)
                    {
                        // #if !APPLE_VISION
                        //                         pinchClicked?.Invoke();
                        // #endif
                        pinchTime = 0;
                    }
                }
                pinchTime += Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ScrollRaycast();
            }
        }

        private void HandleKeyboardState()
        {
#if APPLE_VISION
            // if (!this.gameObject.CompareTag("Keyboard"))
            // {
            //     if (KeyboardManager.Instance && KeyboardManager.Instance.IsKeyboardOpen)
            //         KeyboardManager.Instance.CloseKeyboard();
            // }
#endif
        }

        private void OnPinchRelease()
        {
            isPinchHold = false;
            horizontalScroll = null;
            pinchTime = 0;
        }
    }

}

