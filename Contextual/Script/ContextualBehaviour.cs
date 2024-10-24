using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace JioXSDK
{
    public class ContextualBehaviour : MonoBehaviour
    {
        [SerializeField]
        private float activeContextualMenuTimer = 3f;

        [SerializeField]
        private GameObject contextualPrefab;

        private XRGazeInteractor xRGaze;
        private Coroutine closeCoroutine;

        [SerializeField] private JioXScrollRect scrollRect;

        private void Start()
        {
            xRGaze = FindObjectOfType<XRGazeInteractor>();
            if (scrollRect != null) scrollRect.threshHoldReached += CloseMenu;
        }

        public void ShowContextualMenu(Transform clickedTransform)
        {
            // Position the contextual menu at the position of the clicked object
            contextualPrefab.transform.position = clickedTransform.position;
            contextualPrefab.SetActive(true);

            if (closeCoroutine != null)
            {
                StopCoroutine(closeCoroutine);
            }
            closeCoroutine = StartCoroutine(CloseContextualMenu());
        }

        private IEnumerator CloseContextualMenu()
        {
            yield return new WaitForSeconds(activeContextualMenuTimer);
            contextualPrefab.SetActive(false);
        }
        public void CloseMenu()
        {
            /*var deltaMagnitude = delta.magnitude;
            Debug.Log($"ContextualMenu --> Delta {delta}, Delta Magnitude {delta.magnitude}");
            if (deltaMagnitude > 1000)
            {
                Debug.Log($"ContextualMenu --> Delta magnitutde very large");
                if (deltaMagnitude > deltaThreshold * deltaThresholdMultiplier)
                {
                    contextualPrefab.SetActive(false);
                }
            }
            else if(deltaMagnitude > deltaThreshold)
            {
                Debug.Log($"ContextualMenu --> Delta low");*/
            contextualPrefab.SetActive(false);

            //}

        }

        private void OnDestroy()
        {
            if (scrollRect != null) scrollRect.threshHoldReached -= CloseMenu;
        }
    }
}
