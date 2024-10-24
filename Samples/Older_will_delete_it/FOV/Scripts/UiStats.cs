using JioXSDK.Interactions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JioXSDK.FOVSample
{
    public class UiStats : MonoBehaviour
    {

        [SerializeField] private bool _showHandPos;
        [SerializeField] private TextMeshProUGUI _rightHandRotation;
        [SerializeField] private TextMeshProUGUI _leftHandRotation;

        [SerializeField] private bool _showHandIndicator;
        [SerializeField] private Image _leftHandIndicator;
        [SerializeField] private Image _rightHandIndicator;

        private HandTrackingState leftTrackingState = HandTrackingState.Lost;
        private HandTrackingState rightTrackingState = HandTrackingState.Lost;

        private void Awake()
        {
            HandInteractionEvents.LeftHandTrackingAcquired += () => leftTrackingState = HandTrackingState.Acquired;
            HandInteractionEvents.LeftHandTrackingLost += () => leftTrackingState = HandTrackingState.Lost;

            HandInteractionEvents.RightHandTrackingAcquired += () => rightTrackingState = HandTrackingState.Acquired;
            HandInteractionEvents.RightHandTrackingLost += () => rightTrackingState = HandTrackingState.Lost;

            _rightHandRotation.gameObject.SetActive(_showHandPos);
            _leftHandRotation.gameObject.SetActive(_showHandPos);

            _leftHandIndicator.gameObject.SetActive(_showHandIndicator);
            _rightHandIndicator.gameObject.SetActive(_showHandIndicator);
        }

        private void Update()
        {
            DisplayHandPos();
            DisplayHandTrackingState();
        }

        private void DisplayHandTrackingState()
        {
            if (!_showHandIndicator)
                return;

            _leftHandIndicator.color = HandTrackingState.Lost == leftTrackingState ? Color.red : Color.green;
            _rightHandIndicator.color = HandTrackingState.Lost == rightTrackingState ? Color.red : Color.green;
        }

        private void DisplayHandPos()
        {
            if (!_showHandPos)
                return;

            _rightHandRotation.text = HandInteractionEvents.RightHandPosition.ToString();
            _leftHandRotation.text = HandInteractionEvents.LeftHandPosition.ToString();

        }
    }
}
