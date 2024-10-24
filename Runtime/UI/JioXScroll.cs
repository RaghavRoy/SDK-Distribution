using JioXSDK.Interactions;
using UnityEngine;
using UnityEngine.UI;

namespace JioXSDK
{
    public class JioXScroll : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollView;
        private Vector3 handPosition;

        [SerializeField] private float scrollSensitivity = 130;
        private bool isScrolling;
        private Vector3 handLastPos;

        public void OnPinch()
        {
            handLastPos = HandInteractionEvents.RightHandPosition;
            isScrolling = true;
        }
        public void OnPinchRelease()
        {
            isScrolling = false;
        }


        void Update()
        {
            if (!isScrolling) return;

            handPosition = HandInteractionEvents.RightHandPosition;
            var delta = handLastPos - handPosition;
            if (delta.y < 0f)
            {
                Vector2 normalizedHandPosition = scrollView.normalizedPosition - (Vector2)handPosition / scrollView.viewport.rect.size;
                normalizedHandPosition.x = Mathf.Clamp01(normalizedHandPosition.x);
                normalizedHandPosition.y = Mathf.Clamp01(normalizedHandPosition.y);

                scrollView.normalizedPosition = Vector2.Lerp(scrollView.normalizedPosition, normalizedHandPosition, scrollSensitivity * Time.deltaTime);
            }
            else if (delta.y > 0f)
            {
                Vector2 normalizedHandPosition = scrollView.normalizedPosition + (Vector2)handPosition / scrollView.viewport.rect.size;
                normalizedHandPosition.x = Mathf.Clamp01(normalizedHandPosition.x);
                normalizedHandPosition.y = Mathf.Clamp01(normalizedHandPosition.y);

                scrollView.normalizedPosition = Vector2.Lerp(scrollView.normalizedPosition, normalizedHandPosition, scrollSensitivity * Time.deltaTime);
            }
            handLastPos = handPosition;
        }
    }
}
