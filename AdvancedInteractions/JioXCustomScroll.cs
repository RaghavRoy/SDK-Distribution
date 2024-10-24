using JioXSDK.Interactions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JioXSDK
{
    public class JioXCustomScroll : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float scrollVelocity = 100f;
        [SerializeField] private ScrollRect scrollRect;

        private Vector2 lastPinchPosition;
        private bool isRightPinchHold;
        private bool isLeftPinchHold;

        private const float DeadzoneThreshold = 0.01f;
        private const float PinchDecelerationRate = 0.0005f;
        private const float DefaultDecelerationRate = 0.135f;

        private void Awake()
        {
            if(scrollRect == null)
                scrollRect = GetComponent<ScrollRect>();
        }

        private void OnEnable()
        {
            HandInteractionEvents.OnLeftHandPinchRelease += OnPinchReleaseLeft;
            HandInteractionEvents.OnRightHandPinchRelease += OnPinchReleaseRight;
        }

        private void OnDisable()
        {
            HandInteractionEvents.OnLeftHandPinchRelease -= OnPinchReleaseLeft;
            HandInteractionEvents.OnRightHandPinchRelease -= OnPinchReleaseRight;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //OnPinchRelease();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnPinch();
        }

        public void OnPinch()
        {
            if (isRightPinchHold || isLeftPinchHold) return;

            isLeftPinchHold = HandInteractionEvents.isLeftPinchHold;
            isRightPinchHold = HandInteractionEvents.isRightPinchHold;

            lastPinchPosition = GetPinchPosition();
            SetScrollRectForPinch();
        }

        public void OnPinchReleaseLeft()
        {
            if (!isLeftPinchHold) return;
            scrollRect.decelerationRate = DefaultDecelerationRate;
            isLeftPinchHold = HandInteractionEvents.isLeftPinchHold;
            isRightPinchHold = HandInteractionEvents.isRightPinchHold;
        }

        public void OnPinchReleaseRight()
        {
            if (!isRightPinchHold) return;
            scrollRect.decelerationRate = DefaultDecelerationRate;
            isLeftPinchHold = HandInteractionEvents.isLeftPinchHold;
            isRightPinchHold = HandInteractionEvents.isRightPinchHold;
        }

        private Vector2 GetPinchPosition()
        {
            Vector2 pos;
            if (isRightPinchHold)
                pos = HandInteractionEvents.RightHandPinchUnitPosition;
            else if (isLeftPinchHold)
                pos = HandInteractionEvents.LeftHandPinchUnitPosition;
            else 
                pos = Vector2.zero;
            return pos;
        }

        private void Update()
        {
            if (!isRightPinchHold && !isLeftPinchHold) return;

            Vector2 delta = GetPinchPosition() - lastPinchPosition;

            if (IsPinchWithinDeadzone(delta)) return;

            UpdateScrollVelocity(delta);
            lastPinchPosition = GetPinchPosition();
        }

        private void SetScrollRectForPinch()
        {
            scrollRect.StopMovement();
            scrollRect.decelerationRate = PinchDecelerationRate;
        }

        private bool IsPinchWithinDeadzone(Vector2 delta)
        {
            return Mathf.Abs(delta.y) <= DeadzoneThreshold && Mathf.Abs(delta.x) <= DeadzoneThreshold;
        }

        private void UpdateScrollVelocity(Vector2 delta)
        {
            Vector2 flickVelocity = delta / Time.deltaTime;
            scrollRect.velocity += flickVelocity * scrollRect.scrollSensitivity * scrollVelocity;
        }       
    }
}