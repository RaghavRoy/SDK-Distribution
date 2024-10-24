using System.Collections;
using JioXSDK.Interactions;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.Mathematics;
using System;
using UnityEngine.Events;

namespace JioXSDK
{
    [System.Flags]
    public enum AxisConstraints
    {
        x = 1 << 0,  // 1
        y = 1 << 1,  // 2
        z = 1 << 2,  // 4
    }
    public class Draggable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Drag Settings")]
        [SerializeField] AxisConstraints axisConstraints = AxisConstraints.x | AxisConstraints.y | AxisConstraints.z;
        [SerializeField] private float sensitivityFactor = 0.1f;
        [SerializeField] private TMP_Dropdown dropDown;
        [SerializeField] private Transform screenToMove;
        [SerializeField] private bool enableLookRotation = true;
        [SerializeField] private bool showHoverEffect = true;
        [SerializeField] private float limit = 0.3f;
        [SerializeField] bool screenhoverflag;
        [SerializeField] private float minDragDistance = 0.5f;

        [Space(10)]
        [Header("Adaptive Scale Settings")]
        [SerializeField] private bool enableAdaptiveScale = false;
        [SerializeField] private float maxScale = 10f;
        [SerializeField] private float minScale = 1f;
        [SerializeField] private float scaleAcceleration = 10f;
        [SerializeField] private float maxDistance = 15f;
        private Vector3 initialPosition;
        private float initialDistance;
        private Vector3 initialScale;

        //private int hand = 0; // 0 is none, 1 is left, 2 is right

        private Hand hand;
        private enum Hand
        {
            none,
            left,
            right
        }

        private Image handleImage;
        private Image screenImage;
        private float screenImageAlphaOnMove = 0.8f;
        private float handleImagedefaultAlpha = 0.6f;


        private int frameCount = 0;
        private float distanceFromCamera;
        private Quaternion _rotation;

        
        public static Action<bool> IsDragStarted;
        
        private Vector3 leftHandPinchStartPosition = Vector3.zero;
        private Vector3 rightHandPinchStartPosition = Vector3.zero;

        private Vector3 pinchStartPosition = Vector3.zero;

       

        private Camera cam;

        private bool isHovered = false;

        private bool isDragging = false;

        private Vector3 movementVector;

        private float sensitivity = 300;

        private float acceleration = 10f;   // Acceleration factor
        private float deacceleration = 5f; // Deceleration factor

        #region Debug

        public float SensitivityFactor
        {
            set
            {
                sensitivityFactor = value;
            }
            get { return sensitivityFactor; }
        }

        public float Acceleration
        {
            set
            {
                acceleration = value;
            }
            get { return acceleration; }
        }

        public float Deacceleration
        {
            set
            {
                deacceleration = value;
            }
            get { return deacceleration; }
        }

        public float Limit
        {
            set
            {
                limit = value;
            }
            get { return limit; }
        }

        public float DistanceFromCamera
        {
            get { return distanceFromCamera; }
        }

        #endregion


        private void Start()
        {
            HandInteractionEvents.OnLeftHandPinch += OnLeftHandDragStart;
            HandInteractionEvents.OnRightHandPinch += OnRightHandDragStart;
            HandInteractionEvents.OnLeftHandPinchRelease += OnLeftHandDragEnd;
            HandInteractionEvents.OnRightHandPinchRelease += OnRightHandDragEnd;
            
            cam = Camera.main;
            movementVector = screenToMove.position;
            
            handleImage = GetComponent<Image>();
            
            if(screenhoverflag)

                screenImage = screenToMove.GetComponent<Image>();

            initialPosition = screenToMove.localPosition;
            initialDistance = Vector3.Distance(cam.transform.position, screenToMove.transform.position);
            initialScale = screenToMove.localScale;
            if(enableAdaptiveScale) ApplyCanvasScale();


        }
        private void Update()
        {
            if (isDragging)
            {
              distanceFromCamera = Vector3.Distance(cam.transform.position, screenToMove.position);
              CalculateMovement(pinchStartPosition);

              if (enableAdaptiveScale) ApplyCanvasScale();

              MoveScreen();
            }            
        }
        private void OnDestroy() {
            

            HandInteractionEvents.OnLeftHandPinch -= OnLeftHandDragStart;
            HandInteractionEvents.OnRightHandPinch -= OnRightHandDragStart;
            HandInteractionEvents.OnLeftHandPinchRelease -= OnLeftHandDragEnd;
            HandInteractionEvents.OnRightHandPinchRelease -= OnRightHandDragEnd;
        }

        #region Dragging Events
        private void OnLeftHandDragStart()
        {
            if (isDragging || !isHovered)
                return;
            pinchStartPosition = HandInteractionEvents.LeftHandPinchPosition;
            StartDragging(Hand.left);
        }

        private void OnRightHandDragStart()
        {
            if (isDragging || !isHovered)
                return;
            pinchStartPosition = HandInteractionEvents.RightHandPinchPosition;
            StartDragging(Hand.right);
        }

        private void OnLeftHandDragEnd()
        {
            Debug.Log("##Dragging: OnLeftHandDragEnd");
            isDragging = false;

            hand = Hand.none;
            ShowHoverEffect();
        }

        private void OnRightHandDragEnd()
        {
            isDragging = false;
            Debug.Log("##Dragging: OnRightHandDragEnd");
            hand = Hand.none;
            ShowHoverEffect();
        }
        #endregion

        #region Movement Logic
        private void StartDragging(Hand hand)
        {
            
                isDragging = true;
                this.hand = hand;
                if (screenhoverflag)
                {
                    Color _color = screenImage.color;
                    _color.a = screenImageAlphaOnMove;
                    screenImage.color = _color;
                }
             
        }

        private void CalculateMovement(Vector3 pinchStartPosition)
        {
            Vector3 pinchCurrentPosition = hand == Hand.left ? HandInteractionEvents.LeftHandPinchPosition : HandInteractionEvents.RightHandPinchPosition;
            Debug.Log($"##Dragging position {pinchCurrentPosition}");
            movementVector = pinchCurrentPosition - pinchStartPosition;
            Debug.Log($"##Dragging movementVector {pinchCurrentPosition}");

            //movementVector = AddAxisConstraint(movementVector);
            // float theta = Mathf.Atan((pinchCurrentPosition.z - cam.transform.position.z)/(pinchCurrentPosition.x - cam.transform.position.x));
            // float zVal = cam.transform.position.z - pinchCurrentPosition.z;
            // float xVal = cam.transform.position.x - pinchCurrentPosition.x;
            // if(zVal > 0 && xVal < 0)
            // {
            //     theta = 180 - theta;
            // }
            // else if(zVal < 0 && xVal < 0)
            // {
            //     theta = 180 - Mathf.Abs(theta);
            // }
            // else if(zVal < 0 && xVal > 0)
            // {
            //     theta = 360 - Mathf.Abs(theta);
            // }

            //sensitivity calculated so that its relative to distance(values provided by designer works well)
            sensitivity = Mathf.Sqrt((distanceFromCamera + 0.5f) / 0.0000035f) - 400;
            sensitivity = sensitivity * sensitivityFactor * 0.25f;
            movementVector = movementVector * sensitivity;
            movementVector = screenToMove.position + movementVector;


            

            float newDistance = Vector3.Distance(cam.transform.position, movementVector);
            if (newDistance < minDragDistance) movementVector = cam.transform.position + (movementVector - cam.transform.position).normalized * minDragDistance;

            this.pinchStartPosition = pinchCurrentPosition;
        }

        private void MoveScreen()
        {
            // Calculate the distance between the camera and the target position
            float currentDistance = Vector3.Distance(cam.transform.position, movementVector);
            float angle = Vector3.Angle(cam.transform.position - screenToMove.position, cam.transform.position - new Vector3(screenToMove.position.x, cam.transform.position.y, screenToMove.position.z));

            // Dynamically adjust limit based on angle and position relative to the camera
            float calculatedLimit = screenToMove.position.y > cam.transform.position.y ? 0.0078f * angle + limit : limit;

            // Direction from the camera to the current object position
            Vector3 directionFromCamera = (screenToMove.position - cam.transform.position).normalized;

            // If the object is closer than the minimum drag distance, clamp the movement
            if (currentDistance < minDragDistance)
            {
                // Gently move the object back to maintain the minimum distance
                float clampedDistance = Mathf.Lerp(currentDistance, minDragDistance, Time.deltaTime * deacceleration);
                ApplyLookRotation();
                // Calculate the new position while preserving the movement direction toward/away from the camera
                movementVector = cam.transform.position + directionFromCamera * clampedDistance;
                screenToMove.position = Vector3.Lerp(screenToMove.position, movementVector, Time.deltaTime * deacceleration);
            }
            else
            {
                // Calculate movement delta
                Vector3 movementDelta = movementVector - screenToMove.position;

                // Cap the movement speed to prevent overly fast movement
                float maxMovementSpeed = 0.5f;  // Adjust this for desired speed
                if (movementDelta.magnitude > maxMovementSpeed)
                {
                    movementDelta = movementDelta.normalized * maxMovementSpeed;
                }
                ApplyLookRotation();
                // Apply the movement while keeping it smooth
                screenToMove.position = Vector3.Lerp(screenToMove.position, screenToMove.position + movementDelta, Time.deltaTime * deacceleration);
            }
        }

        private void ApplyLookRotation()
        {
            if (enableLookRotation)
            {
                if (Vector3.Distance(screenToMove.position, movementVector) > 0.1f)
                {
                    _rotation = Quaternion.LookRotation(screenToMove.position - cam.transform.position);
                }
            }
            if (Quaternion.Angle(screenToMove.localRotation, _rotation) > 0.1f)
            {
                screenToMove.localRotation = Quaternion.Lerp(screenToMove.localRotation, _rotation, Time.deltaTime * acceleration);
            }
        }
        #endregion

        #region Axis Constraints
       

        private Vector3 AddAxisConstraint(Vector3 target)
        {
            if ((axisConstraints & AxisConstraints.x) == 0)
            {
                target.x = 0;
            }
            if ((axisConstraints & AxisConstraints.y) == 0)
            {
                target.y = 0;
            }
            if ((axisConstraints & AxisConstraints.z) == 0)
            {
                target.z = 0;
            }
            return target;
        }

        public void OnDropDownValueChanged()
        {
            switch (dropDown.value)
            {
                case 0:
                    axisConstraints = AxisConstraints.x | AxisConstraints.y | AxisConstraints.z;
                    break;

                case 1:
                    axisConstraints = AxisConstraints.x;
                    break;

                case 2:
                    axisConstraints = AxisConstraints.y;
                    break;

                case 3:
                    axisConstraints = AxisConstraints.z;
                    break;

                case 4:
                    axisConstraints = AxisConstraints.x | AxisConstraints.y;
                    break;

                case 5:
                    axisConstraints = AxisConstraints.x | AxisConstraints.z;
                    break;

                case 6:
                    axisConstraints = AxisConstraints.y | AxisConstraints.z;
                    break;
            }
        }


        public void AddAxisConstraint(AxisConstraints _axisConstraint)
        {
            axisConstraints |= _axisConstraint;
        }

        public void RemoveAxisConstraint(AxisConstraints _axisConstraint)
        {
            axisConstraints &= ~_axisConstraint;
        } 
        #endregion

        #region UI Effects
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerEnter == gameObject)
            {
                isHovered = true;
                Color _color = handleImage.color;
                _color.a = 1;
                handleImage.color = _color;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerEnter == gameObject)
            {
                Debug.Log($"");
                isHovered = false;
                if (!isDragging)
                {
                    Color _color = handleImage.color;
                    _color.a = handleImagedefaultAlpha;
                    handleImage.color = _color;
                }
            }
        }


        private void ShowHoverEffect()
        {
            if (showHoverEffect)
            {
                if (screenhoverflag)
                {
                    Color _color = screenImage.color;
                    _color.a = 1;
                    screenImage.color = _color;
                }
                if (!isHovered)
                {
                    Color _handleColor = handleImage.color;
                    _handleColor.a = handleImagedefaultAlpha;
                    handleImage.color = _handleColor;
                }
            }
        }
        #endregion

        #region Adaptive Scaling Logic
        private void ApplyCanvasScale()
        {
            // Calculate current distance
            float currentDistance = Vector3.Distance(cam.transform.position, screenToMove.position);
            if (currentDistance == initialDistance)
            {
                screenToMove.localScale = Vector3.Lerp(screenToMove.localScale, initialScale, Time.deltaTime * acceleration);
                return;
            }
            // Adjust the position of the canvas based on min/max distance limits
            //AdjustCanvasPosition(currentDistance);

            // Calculate scale factor based on distance
            float scaleFactor = currentDistance / initialDistance;
            Vector3 targetScale = initialScale * scaleFactor;

            // Clamp the scale values between minScale and maxScale
            Vector3 clampedScale = new Vector3(
                Mathf.Clamp(targetScale.x, minScale, maxScale),
                Mathf.Clamp(targetScale.y, minScale, maxScale),
                Mathf.Clamp(targetScale.z, minScale, maxScale)
            );

            // Smoothly transition the scale of the canvas
            screenToMove.localScale = Vector3.Lerp(screenToMove.localScale, clampedScale, Time.deltaTime * acceleration);
        }

        private void AdjustCanvasPosition(float currentDistance)
        {
            // Adjust canvas position if it's beyond the max or min distance
            if (currentDistance > maxDistance)
            {
                Vector3 direction = (initialPosition - screenToMove.position).normalized;
                float distanceOffset = currentDistance - maxDistance;
                screenToMove.position += direction * distanceOffset;
            }
            /*else if (currentDistance < MinDistance)
            {
                Vector3 direction = (screenToMove.position - initialPosition).normalized;
                float distanceOffset = MinDistance - currentDistance;
                screenToMove.position -= direction * distanceOffset;
            }*/
        }

        #endregion
    }
}
