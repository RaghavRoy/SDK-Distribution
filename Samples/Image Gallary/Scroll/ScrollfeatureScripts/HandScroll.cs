using System.Collections;
using System.Collections.Generic;
using JioXSDK.Interactions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JioXSDK
{
    public class HandScroll : MonoBehaviour
    {
        public ScrollRect scrollView; // Reference to the ScrollRect component
        private Vector3 handPosition; // Public variable to receive the hand position

        private float scrollSensitivity = 130; // Adjust sensitivity for smoother scrolling
        private bool isScrolling;
        private Vector3 handLastPos;

        public TextMeshProUGUI normalizedHandPosText;

        public TMP_Text pinchin;

        public TMP_Text pinchout;

        private void OnEnable()
        {
            HandInteractionEvents.OnRightHandPinch += OnPinch;
            HandInteractionEvents.OnRightHandPinchRelease += OnPinchRelease;
        }
        private void OnDisable()
        {
            HandInteractionEvents.OnRightHandPinch -= OnPinch;
            HandInteractionEvents.OnRightHandPinchRelease -= OnPinchRelease;
        }
        public void OnPinch()
        {
            handLastPos = HandInteractionEvents.RightHandPosition;
            pinchin.text = "pinch in called " + handLastPos;
            isScrolling = true;
        }
        public void OnPinchRelease()
        {
            pinchout.text = "pinch out " + handLastPos;
            isScrolling = false;
        }


        void Update()
        {
            // Calculate normalized hand position within the ScrollRect's viewport
            if (!isScrolling)
            {
                return;
            }

            handPosition = HandInteractionEvents.RightHandPosition;
            var delta = handLastPos - handPosition;
            if (delta.y > 0f)
            {

                Vector2 normalizedHandPosition = scrollView.normalizedPosition - (Vector2)handPosition / scrollView.viewport.rect.size;
                normalizedHandPosText.SetText(normalizedHandPosition.ToString());
                // Clamp normalized position to avoid going beyond scroll bounds
                normalizedHandPosition.x = Mathf.Clamp01(normalizedHandPosition.x);
                normalizedHandPosition.y = Mathf.Clamp01(normalizedHandPosition.y);

                // Smoothly lerp (linearly interpolate) towards the calculated position
                scrollView.normalizedPosition = Vector2.Lerp(scrollView.normalizedPosition, normalizedHandPosition, scrollSensitivity * Time.deltaTime);
            }
            else if (delta.y < 0f)
            {
                Vector2 normalizedHandPosition = scrollView.normalizedPosition + (Vector2)handPosition / scrollView.viewport.rect.size;
                normalizedHandPosText.SetText(normalizedHandPosition.ToString());
                // Clamp normalized position to avoid going beyond scroll bounds
                normalizedHandPosition.x = Mathf.Clamp01(normalizedHandPosition.x);
                normalizedHandPosition.y = Mathf.Clamp01(normalizedHandPosition.y);

                // Smoothly lerp (linearly interpolate) towards the calculated position
                scrollView.normalizedPosition = Vector2.Lerp(scrollView.normalizedPosition, normalizedHandPosition, scrollSensitivity * Time.deltaTime);
            }
            handLastPos = handPosition;
        }
    }
}
