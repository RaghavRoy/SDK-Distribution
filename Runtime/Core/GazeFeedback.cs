using System.Collections;
using JioXSDK.Interactions;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace JioXSDK
{
    public class GazeFeedback : MonoBehaviour
    {
        [SerializeField] private Transform maskCircle;
        [SerializeField] private Transform innerCircle;
        [SerializeField] private Transform outerCircle;
        private bool isHovering = false;
        private XRGazeInteractor gazeInteractor;
        private Camera mainCamera;

        private Vector3 maskCircleInitScale;
        private Vector3 maskCirclePressedScale = new Vector3(1.1f, 1.1f, 1.1f);

        private Vector3 innerCircleInitScale;
        private Vector3 innerCirclePressedScale = new Vector3(0.3f, 0.3f, 0.3f);
        private float pressedAnimationDurataion = 0.1f;

        private bool innerCirclePressed;
        private bool innerCircleReleased;
        private bool maskCirclePressed;
        private bool maskCircleReleased;

        private float initialSize = 2.5f;
        private float initialDistance = 1f;

        private void Awake()
        {
            gazeInteractor = FindObjectOfType<XRGazeInteractor>();
            mainCamera = Camera.main;
        }

        private void Start()
        {
            maskCircleInitScale = maskCircle.localScale;
            innerCircleInitScale = innerCircle.localScale;

            initialDistance = Vector3.Distance(mainCamera.transform.position, transform.position);
        }
        private void Update()
        {
            if (!isHovering)
            {
                transform.rotation = Quaternion.LookRotation(mainCamera.transform.position - transform.position);
                //positionDelta = transform.position - mainCamera.transform.position;
                // if(positionDelta.magnitude > initialDistance)
                // {
                //     transform.position = mainCamera.transform.position + (positionDelta.normalized * initialDistance);
                // }


            }

            // Calculate the current distance from the camera
            //float currentDistance = Vector3.Distance(transform.position, mainCamera.transform.position);

            // Calculate the scale based on the dynamic size formula
            //float scaleFactor = initialDistance / currentDistance;
            //float scale = initialSize * (currentDistance / initialDistance);

            // Clamp scale to avoid scaling down too much
            //scale = Mathf.Max(scale, minSize);

            // Apply the scale to the GameObject
            //transform.localScale = new Vector3(scale, scale, scale);


            // if(frameCount > 5)
            // {
            //     currentDistance = Vector3.Distance(mainCamera.transform.position, transform.position);
            //     //currentDistance = (mainCamera.transform.position - transform.position).magnitude;
            //     Debug.Log("initialDistance " + initialDistance);
            //     scale = transform.localScale / initialDistance * currentDistance;
            //     transform.localScale = scale;
            //     initialDistance = currentDistance;
            //     frameCount = 0;
            // }
            //transform.localScale = Vector3.Lerp(transform.localScale , scale, 0.2f);
            //frameCount++;
            if (innerCirclePressed)
            {
                innerCircle.localScale = Vector3.Lerp(innerCircle.localScale, innerCirclePressedScale, Time.deltaTime / pressedAnimationDurataion);
                if (Vector3.Distance(innerCircle.localScale, innerCirclePressedScale) < 0.01)
                {
                    innerCirclePressed = false;
                }
            }

            if (innerCircleReleased)
            {
                innerCircle.localScale = Vector3.Lerp(innerCircle.localScale, innerCircleInitScale, Time.deltaTime / pressedAnimationDurataion);
                if (Vector3.Distance(innerCircle.localScale, innerCircleInitScale) < 0.01)
                {
                    innerCircleReleased = false;
                }
            }

            if (maskCirclePressed)
            {
                maskCircle.localScale = Vector3.Lerp(maskCircle.localScale, maskCirclePressedScale, Time.deltaTime / pressedAnimationDurataion);
                if (Vector3.Distance(maskCircle.localScale, maskCirclePressedScale) < 0.01)
                {
                    maskCirclePressed = false;
                }
            }

            if (maskCircleReleased)
            {
                maskCircle.localScale = Vector3.Lerp(maskCircle.localScale, maskCircleInitScale, Time.deltaTime / pressedAnimationDurataion);
                if (Vector3.Distance(maskCircle.localScale, maskCircleInitScale) < 0.01)
                {
                    maskCircleReleased = false;
                }
            }
        }

        private void OnEnable()
        {
            HandInteractionEvents.OnRightHandPinch += OnPinch;
            HandInteractionEvents.OnLeftHandPinch += OnPinch;
            HandInteractionEvents.OnRightHandPinchRelease += OnPinchRelease;
            HandInteractionEvents.OnLeftHandPinchRelease += OnPinchRelease;
            gazeInteractor.hoverEntered.AddListener(OnHoverEnter);
            gazeInteractor.hoverExited.AddListener(OnHoverExit);
            gazeInteractor.uiHoverEntered.AddListener(OnUIHoverEnter);
            gazeInteractor.uiHoverExited.AddListener(OnUIHoverExit);
        }

        private void OnDisable()
        {
            HandInteractionEvents.OnRightHandPinch -= OnPinch;
            HandInteractionEvents.OnLeftHandPinch -= OnPinch;
            HandInteractionEvents.OnRightHandPinchRelease -= OnPinchRelease;
            HandInteractionEvents.OnLeftHandPinchRelease -= OnPinchRelease;
            gazeInteractor.hoverEntered.RemoveListener(OnHoverEnter);
            gazeInteractor.hoverExited.RemoveListener(OnHoverExit);
            gazeInteractor.uiHoverEntered.RemoveListener(OnUIHoverEnter);
            gazeInteractor.uiHoverExited.RemoveListener(OnUIHoverExit);
        }

        private void OnPinch()
        {
            innerCircleReleased = false;
            maskCircleReleased = false;
            maskCirclePressed = true;
            innerCirclePressed = true;
        }

        private void OnPinchRelease()
        {
            maskCirclePressed = false;
            innerCirclePressed = false;
            innerCircleReleased = true;
            maskCircleReleased = true;
        }

        public void OnHoverEnter(HoverEnterEventArgs args)
        {
            isHovering = true;
            maskCircle.transform.localEulerAngles = new Vector3(90, 0, 0);
            innerCircle.transform.localEulerAngles = new Vector3(90, 0, 0);
            outerCircle.transform.localEulerAngles = new Vector3(90, 0, 0);
        }

        public void OnHoverExit(HoverExitEventArgs args)
        {
            isHovering = false;
            maskCircle.transform.localEulerAngles = new Vector3(0, 180, 0);
            innerCircle.transform.localEulerAngles = new Vector3(0, 180, 0);
            outerCircle.transform.localEulerAngles = new Vector3(0, 180, 0);
        }

        public void OnUIHoverEnter(UIHoverEventArgs args)
        {
            isHovering = true;
            maskCircle.transform.localEulerAngles = new Vector3(90, 0, 0);
            innerCircle.transform.localEulerAngles = new Vector3(90, 0, 0);
            outerCircle.transform.localEulerAngles = new Vector3(90, 0, 0);
        }

        public void OnUIHoverExit(UIHoverEventArgs args)
        {
            isHovering = false;
            maskCircle.transform.localEulerAngles = new Vector3(0, 180, 0);
            innerCircle.transform.localEulerAngles = new Vector3(0, 180, 0);
            outerCircle.transform.localEulerAngles = new Vector3(0, 180, 0);
        }
    }
}
