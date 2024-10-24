using System.Collections;
using System.Collections.Generic;
using JioXSDK.Interactions;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JioXSDK
{
    public class CustomScroll : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IDropHandler, IPointerExitHandler, IPointerEnterHandler
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private CustomScrollbar customScrollbar;
        [SerializeField] private bool isHorizontal;
        [SerializeField] private bool isVertical;
        [SerializeField] private float yMaxPosition;
        [SerializeField] private float xMinPosition;
        private Vector3 previousPinchPos;
        private float interactionOffsetY;
        private float interactionOffsetX;
        private bool isScrolling = false;
        private float sensitivity = 1000;
        private float currentHand = -1;
        public Vector3 updatedPosition = Vector3.zero;
        private Vector3 inertia;
        private bool isHovered = false;
        private float inertiaFactor = 5;
        private float pinchMovementFactor = 20;

        public bool zoneController;

        public float Sensitivity
        {
            get
            {
                return sensitivity;
            }
            set
            {
                sensitivity = value;
            }
        }

        private void Awake()
        {
            HandInteractionEvents.OnRightHandPinch += OnRightPinch;
            HandInteractionEvents.OnRightHandPinchRelease += OnRightPinchRelease;
            HandInteractionEvents.OnLeftHandPinch += OnLeftPinch;
            HandInteractionEvents.OnLeftHandPinchRelease += OnLeftPinchRelease;
            if (customScrollbar != null)
            {
                customScrollbar.OnScroll += OnScroll;
            }
        }

        private void Start()
        {
            if (isHorizontal)
            {
                if (customScrollbar != null)
                {
                    customScrollbar.UpdateScrollbarPosition((float)Mathf.Abs(content.anchoredPosition3D.x / xMinPosition));
                }
            }
            if (isVertical)
            {
                if (customScrollbar != null)
                {
                    customScrollbar.UpdateScrollbarPosition((float)(content.anchoredPosition3D.y / yMaxPosition));
                }
            }
        }

        private void OnDestroy()
        {
            HandInteractionEvents.OnRightHandPinch -= OnRightPinch;
            HandInteractionEvents.OnRightHandPinchRelease -= OnRightPinchRelease;
            HandInteractionEvents.OnLeftHandPinch -= OnLeftPinch;
            HandInteractionEvents.OnLeftHandPinchRelease -= OnLeftPinchRelease;
            if (customScrollbar != null)
            {
                customScrollbar.OnScroll -= OnScroll;
            }
        }

        private void OnScroll(float percent)
        {
            float inversePercent = 1 - percent;
            Vector3 updatedPosition = content.anchoredPosition3D;
            if (isHorizontal)
            {
                updatedPosition.x = xMinPosition * Mathf.Abs(percent);
            }
            if (isVertical)
            {
                updatedPosition.y = yMaxPosition * inversePercent;
            }
            inertia = updatedPosition;
            content.anchoredPosition3D = updatedPosition;
        }

        private void Update()
        {
            if (currentHand >= 0)
            {
                Vector3 currentPinchPosition = currentHand == 0 ?
                HandInteractionEvents.LeftHandPinchPosition : HandInteractionEvents.RightHandPinchPosition;
                if (isScrolling)
                {
                    MoveContent(currentPinchPosition);
                }
            }
            if (zoneController)
            {
                if (!isScrolling)
                {
                    updatedPosition = content.anchoredPosition3D;
                    if (isHorizontal)
                    {
                        if (updatedPosition.x < xMinPosition)
                        {
                            updatedPosition.x = Mathf.Lerp(updatedPosition.x, xMinPosition, Time.deltaTime * pinchMovementFactor);
                        }
                        else if (updatedPosition.x > 0)
                        {
                            updatedPosition.x = Mathf.Lerp(updatedPosition.x, 0, Time.deltaTime * pinchMovementFactor);
                        }
                        else
                        {
                            updatedPosition.x = Mathf.Lerp(updatedPosition.x, inertia.x, Time.deltaTime * inertiaFactor);
                        }
                    }
                    if (isVertical)
                    {
                        if (updatedPosition.y > yMaxPosition)
                        {
                            updatedPosition.y = Mathf.Lerp(updatedPosition.y, yMaxPosition, Time.deltaTime * pinchMovementFactor);
                        }
                        else if (updatedPosition.y < 0)
                        {
                            updatedPosition.y = Mathf.Lerp(updatedPosition.y, 0, Time.deltaTime * pinchMovementFactor);
                        }
                        else
                        {
                            updatedPosition.y = Mathf.Lerp(updatedPosition.y, inertia.y, Time.deltaTime * inertiaFactor);
                        }
                    }
                    content.anchoredPosition3D = updatedPosition;
                }
            }
            if (isHorizontal)
            {
                if (customScrollbar != null)
                {
                    customScrollbar.UpdateScrollbarPosition((float)(content.anchoredPosition3D.x > 0 ? 0 : content.anchoredPosition3D.x / xMinPosition));
                }
            }
            if (isVertical)
            {
                if (customScrollbar != null)
                {
                    customScrollbar.UpdateScrollbarPosition((float)(content.anchoredPosition3D.y / yMaxPosition));
                }
            }
        }

        private void OnRightPinch()
        {
            if (currentHand < 0 && isHovered)
            {
                previousPinchPos = HandInteractionEvents.RightHandPinchPosition;
                currentHand = 1;
            }
        }

        private void OnRightPinchRelease()
        {
            if (currentHand == 1)
            {
                Vector3 movementVector = previousPinchPos - HandInteractionEvents.RightHandPinchPosition;
                inertia = content.anchoredPosition3D;
                if (isHorizontal)
                {
                    inertia.x -= movementVector.x * sensitivity;
                }
                if (isVertical)
                {
                    inertia.y -= movementVector.y * sensitivity;
                }
                isScrolling = false;
                currentHand = -1;
            }
        }

        private void OnLeftPinch()
        {
            if (currentHand < 0 && isHovered)
            {
                previousPinchPos = HandInteractionEvents.LeftHandPinchPosition;
                currentHand = 0;
            }
        }

        private void OnLeftPinchRelease()
        {
            if (currentHand == 0)
            {
                Vector3 movementVector = previousPinchPos - HandInteractionEvents.LeftHandPinchPosition;
                inertia = content.anchoredPosition3D;
                if (isHorizontal)
                {
                    inertia.x -= movementVector.x * sensitivity;
                }
                if (isVertical)
                {
                    inertia.y -= movementVector.y * sensitivity;
                }
                isScrolling = false;
                currentHand = -1;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("OnSCrollPointerDown");
            isScrolling = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("OnSCrollBeginDrag");
            isScrolling = true;
            if (currentHand < 0)
            {
                previousPinchPos = eventData.position;
                previousPinchPos.z = Mathf.Abs(transform.position.z - Camera.main.transform.position.z);
                previousPinchPos = Camera.main.ScreenToWorldPoint(previousPinchPos);
                previousPinchPos = transform.InverseTransformPoint(previousPinchPos);
                interactionOffsetY = previousPinchPos.y - content.anchoredPosition3D.y;
                interactionOffsetX = previousPinchPos.x - content.anchoredPosition3D.x;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("OnScrollDrag ");
            if (currentHand < 0 && isScrolling)
            {
                Vector3 currentPinchPosition = eventData.position;
                currentPinchPosition.z = Mathf.Abs(transform.position.z - Camera.main.transform.position.z);
                currentPinchPosition = Camera.main.ScreenToWorldPoint(currentPinchPosition);
                MoveContent(currentPinchPosition);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("OnScrollDrop");
            if (currentHand < 0 && isScrolling)
            {
                Vector3 currentPinchPosition = eventData.position;
                currentPinchPosition.z = Mathf.Abs(transform.position.z - Camera.main.transform.position.z);
                currentPinchPosition = Camera.main.ScreenToWorldPoint(currentPinchPosition);
                inertia = updatedPosition;
                if (isHorizontal)
                {
                    inertia.x = transform.InverseTransformPoint(currentPinchPosition).x - interactionOffsetX;
                }
                if (isVertical)
                {
                    inertia.y = transform.InverseTransformPoint(currentPinchPosition).y - interactionOffsetY;
                }
                isScrolling = false;
            }
        }

        private void MoveContent(Vector3 currentPinchPosition)
        {
            if (currentHand >= 0)
            {
                Vector3 movementVector = previousPinchPos - currentPinchPosition;
                updatedPosition = content.anchoredPosition3D;
                if (isHorizontal)
                {
                    updatedPosition.x -= movementVector.x * sensitivity;
                }
                if (isVertical)
                {
                    updatedPosition.y -= movementVector.y * sensitivity;
                }
                inertia = updatedPosition;
                content.anchoredPosition3D = updatedPosition;
                previousPinchPos = currentPinchPosition;
            }
            else
            {
                if (isHorizontal)
                {
                    updatedPosition.x = transform.InverseTransformPoint(currentPinchPosition).x - interactionOffsetX;
                    inertia = updatedPosition;
                    content.anchoredPosition3D = updatedPosition;
                }
                if (isVertical)
                {
                    updatedPosition.y = transform.InverseTransformPoint(currentPinchPosition).y - interactionOffsetY;
                    inertia = updatedPosition;
                    content.anchoredPosition3D = updatedPosition;
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("OnSCrollExit");
            if (currentHand < 0 && isScrolling)
            {
                Vector3 currentPinchPosition = eventData.position;
                currentPinchPosition.z = Mathf.Abs(transform.position.z - Camera.main.transform.position.z);
                currentPinchPosition = Camera.main.ScreenToWorldPoint(currentPinchPosition);
                inertia = updatedPosition;
                if (isHorizontal)
                {
                    inertia.x = transform.InverseTransformPoint(currentPinchPosition).x - interactionOffsetX;
                }
                if (isVertical)
                {
                    inertia.y = transform.InverseTransformPoint(currentPinchPosition).y - interactionOffsetY;
                }
                isScrolling = false;
            }
            isHovered = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("OnSCrollEnter");
            isHovered = true;
        }
    }
}
