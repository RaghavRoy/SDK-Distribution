using System;
using System.Collections;
using System.Collections.Generic;
using JioXSDK.Interactions;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JioXSDK
{
    public class CustomScrollbar : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IDropHandler, IPointerExitHandler, IPointerEnterHandler
    {
        private enum ScrollbarDirection
        {
            Horizontal,
            Vertical
        }
        public Action<float> OnScroll;
        [SerializeField] private RectTransform handle;
        [SerializeField] private ScrollbarDirection scrollDirection;
        [SerializeField] private float limitY = 0;
        [SerializeField] private float limitX = 0;

        private Vector3 previousPinchPos;
        private float interactionOffsetY;
        private float interactionOffsetX;
        private bool isScrolling = false;
        private float sensitivity = 1000;
        private float currentHand = -1;
        private Vector3 updatedPosition = Vector3.zero;
        private Vector3 inertia;
        private bool isHovered = false;
        private float inertiaFactor = 5;
        public float Sensitivity { 
            get{
                return sensitivity;
            }
            set{
                sensitivity = value;
            }
        }

        private void Awake()
        {
            HandInteractionEvents.OnRightHandPinch += OnRightPinch;
            HandInteractionEvents.OnRightHandPinchRelease += OnRightPinchRelease;
            HandInteractionEvents.OnLeftHandPinch += OnLeftPinch;
            HandInteractionEvents.OnLeftHandPinchRelease += OnLeftPinchRelease;
            handle.anchoredPosition3D = Vector3.zero;
        }

        private void Start()
        {
            handle.anchoredPosition3D = new Vector3(
                scrollDirection == ScrollbarDirection.Horizontal?limitX/2:handle.anchoredPosition3D.x,
                scrollDirection == ScrollbarDirection.Vertical?limitY:handle.anchoredPosition3D.y,
                handle.anchoredPosition3D.z);
            updatedPosition = handle.anchoredPosition3D;
            inertia = updatedPosition;
        }

        private void OnDestroy()
        {
            HandInteractionEvents.OnRightHandPinch -= OnRightPinch;
            HandInteractionEvents.OnRightHandPinchRelease -= OnRightPinchRelease;
            HandInteractionEvents.OnLeftHandPinch -= OnLeftPinch;
            HandInteractionEvents.OnLeftHandPinchRelease -= OnLeftPinchRelease;
        }

        private void Update()
        {
            if(currentHand >= 0)
            {
                Vector3 currentPinchPosition = currentHand == 0? 
                HandInteractionEvents.LeftHandPinchPosition: HandInteractionEvents.RightHandPinchPosition;
                if(isScrolling)
                {
                    MoveContent(currentPinchPosition);
                }
            }
            if(!isScrolling && Vector3.Distance(inertia, updatedPosition) > 0.1f)
            {
                updatedPosition = handle.anchoredPosition3D;
                if(scrollDirection == ScrollbarDirection.Horizontal)
                {
                    updatedPosition.x = Mathf.Clamp(Mathf.Lerp(updatedPosition.x, inertia.x, Time.deltaTime * inertiaFactor),
                    -limitX, limitX);
                }
                if(scrollDirection == ScrollbarDirection.Vertical)
                {
                    updatedPosition.y = Mathf.Clamp(Mathf.Lerp(updatedPosition.y, inertia.y, Time.deltaTime * inertiaFactor),
                    -limitY, limitY);
                }
                handle.anchoredPosition3D = updatedPosition;
                if(scrollDirection == ScrollbarDirection.Horizontal)
                {
                    OnScroll?.Invoke((handle.anchoredPosition3D.x - (-limitX))/(limitX - (-limitX)));
                }
                if(scrollDirection == ScrollbarDirection.Vertical)
                {
                    OnScroll?.Invoke((handle.anchoredPosition3D.y - (-limitY))/(limitY - (-limitY)));
                }
            }
        }

        public void UpdateScrollbarPosition(float percent)
        {
            if(Vector3.Distance(inertia, updatedPosition) < 0.1f)
            {
                percent = Mathf.Clamp(Mathf.Abs(percent), 0, 1);
                float inversePercent = 1 - Mathf.Abs(percent);
                Vector3 newPos = handle.anchoredPosition3D;
                if(scrollDirection == ScrollbarDirection.Horizontal)
                {
                    newPos.x = Mathf.Lerp(-limitX, limitX, percent);
                }
                if(scrollDirection == ScrollbarDirection.Vertical)
                {
                    newPos.y = Mathf.Lerp(-limitY, limitY, inversePercent);
                }
                updatedPosition = newPos;
                inertia = updatedPosition;
                handle.anchoredPosition3D = newPos;
            }
        }

        private void OnRightPinch()
        {
            if(currentHand < 0 && isHovered)
            {
                previousPinchPos = HandInteractionEvents.RightHandPinchPosition;
                currentHand = 1;
            }
        }

        private void OnRightPinchRelease()
        {
            if(currentHand == 1)
            {
                Vector3 movementVector = previousPinchPos - HandInteractionEvents.RightHandPinchPosition;
                inertia = handle.anchoredPosition3D;
                if(scrollDirection == ScrollbarDirection.Horizontal)
                {
                    inertia.x -= movementVector.x * sensitivity;
                }
                if(scrollDirection == ScrollbarDirection.Vertical)
                {
                    inertia.y -= movementVector.y * sensitivity;
                }
                isScrolling = false;
                currentHand = -1;
            }
        }

        private void OnLeftPinch()
        {
            if(currentHand < 0 && isHovered)
            {
                previousPinchPos = HandInteractionEvents.LeftHandPinchPosition;
                isScrolling = true;
                currentHand = 0;
            }
        }

        private void OnLeftPinchRelease()
        {
            if(currentHand == 0)
            {
                Vector3 movementVector = previousPinchPos - HandInteractionEvents.LeftHandPinchPosition;
                inertia = handle.anchoredPosition3D;
                if(scrollDirection == ScrollbarDirection.Horizontal)
                {
                    inertia.x -= movementVector.x * sensitivity;
                }
                if(scrollDirection == ScrollbarDirection.Vertical)
                {
                    inertia.y -= movementVector.y * sensitivity;
                }
                isScrolling = false;
                currentHand = -1;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(eventData.pointerEnter != handle.gameObject && currentHand < 0)
            {
                Vector3 newPosition = handle.position;
                if(scrollDirection == ScrollbarDirection.Horizontal)
                {
                    newPosition.x = transform.InverseTransformPoint(eventData.pointerPressRaycast.worldPosition).x;
                    newPosition.x = Mathf.Clamp(newPosition.x, -limitX, limitX);
                    handle.anchoredPosition3D = newPosition;
                    inertia = newPosition;
                    OnScroll?.Invoke((handle.anchoredPosition3D.x - (-limitX))/(limitX - (-limitX)));
                }
                if(scrollDirection == ScrollbarDirection.Vertical)
                {
                    newPosition.y = transform.InverseTransformPoint(eventData.pointerPressRaycast.worldPosition).y;
                    newPosition.y = Mathf.Clamp(newPosition.y, -limitY, limitY);
                    handle.anchoredPosition3D = newPosition;
                    inertia = newPosition;
                    OnScroll?.Invoke((handle.anchoredPosition3D.y - (-limitY))/(limitY - (-limitY)));
                }
            }
            isScrolling = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(currentHand < 0)
            {
                previousPinchPos = eventData.position;
                previousPinchPos.z = Mathf.Abs(transform.position.z - Camera.main.transform.position.z);
                previousPinchPos = Camera.main.ScreenToWorldPoint(previousPinchPos);
                previousPinchPos = transform.InverseTransformPoint(previousPinchPos);
                if(scrollDirection == ScrollbarDirection.Horizontal)
                {
                    interactionOffsetX = previousPinchPos.x - handle.anchoredPosition3D.x;
                }
                if(scrollDirection == ScrollbarDirection.Vertical)
                {
                    interactionOffsetY = previousPinchPos.y - handle.anchoredPosition3D.y;
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(currentHand < 0)
            {
                Vector3 currentPinchPosition = eventData.position;
                currentPinchPosition.z = Mathf.Abs(transform.position.z - Camera.main.transform.position.z);
                currentPinchPosition = Camera.main.ScreenToWorldPoint(currentPinchPosition);
                MoveContent(currentPinchPosition);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if(currentHand < 0 && isScrolling)
            {
                Vector3 currentPinchPosition = eventData.position;
                currentPinchPosition.z = Mathf.Abs(transform.position.z - Camera.main.transform.position.z);
                currentPinchPosition = Camera.main.ScreenToWorldPoint(currentPinchPosition);
                
                inertia = updatedPosition;
                if(scrollDirection == ScrollbarDirection.Horizontal)
                {
                    inertia.x = transform.InverseTransformPoint(currentPinchPosition).x - interactionOffsetX;
                }
                if(scrollDirection == ScrollbarDirection.Vertical)
                {
                    inertia.y = transform.InverseTransformPoint(currentPinchPosition).y - interactionOffsetY;
                }
                isScrolling = false;
            }
        }

        private void MoveContent(Vector3 currentPinchPosition)
        {
            if(currentHand >= 0)
            {
                Vector3 movementVector = previousPinchPos - currentPinchPosition;
                updatedPosition = handle.anchoredPosition3D;
                if(scrollDirection == ScrollbarDirection.Horizontal)
                {
                    updatedPosition.x -= movementVector.x * sensitivity;
                }
                if(scrollDirection == ScrollbarDirection.Vertical)
                {
                    updatedPosition.y -= movementVector.y * sensitivity;
                }
                inertia = updatedPosition;
                handle.anchoredPosition3D = updatedPosition;
                previousPinchPos = currentPinchPosition;
            }
            else
            {
                updatedPosition = handle.anchoredPosition3D;
                if(scrollDirection == ScrollbarDirection.Horizontal)
                {
                    updatedPosition.x = transform.InverseTransformPoint(currentPinchPosition).x - interactionOffsetX;
                }
                if(scrollDirection == ScrollbarDirection.Vertical)
                {
                    updatedPosition.y = transform.InverseTransformPoint(currentPinchPosition).y - interactionOffsetY;
                }
            }
            if(scrollDirection == ScrollbarDirection.Horizontal)
            {
                updatedPosition.x = Mathf.Clamp(updatedPosition.x, -limitX, limitX);
                inertia = updatedPosition;
                handle.anchoredPosition3D = updatedPosition;
                OnScroll?.Invoke((handle.anchoredPosition3D.x - (-limitX))/(limitX - (-limitX)));
            }
            if(scrollDirection == ScrollbarDirection.Vertical)
            {
                updatedPosition.y = Mathf.Clamp(updatedPosition.y, -limitY, limitY);
                inertia = updatedPosition;
                handle.anchoredPosition3D = updatedPosition;
                OnScroll?.Invoke((handle.anchoredPosition3D.y - (-limitY))/(limitY - (-limitY)));
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(currentHand < 0 && isScrolling)
            {
                Vector3 currentPinchPosition = eventData.position;
                currentPinchPosition.z = Mathf.Abs(transform.position.z - Camera.main.transform.position.z);
                currentPinchPosition = Camera.main.ScreenToWorldPoint(currentPinchPosition);
                
                inertia = updatedPosition;
                if(scrollDirection == ScrollbarDirection.Horizontal)
                {
                    inertia.x = transform.InverseTransformPoint(currentPinchPosition).x - interactionOffsetX;
                }
                if(scrollDirection == ScrollbarDirection.Vertical)
                {
                    inertia.y = transform.InverseTransformPoint(currentPinchPosition).y - interactionOffsetY;
                }
                isScrolling = false;
            }
            isHovered = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;
        }
    }
}
