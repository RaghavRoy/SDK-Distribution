using TMPro;
using UnityEngine;

namespace JioXSDK
{
    public class LongPinchTest : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI longPinchStateText;

        public void OnPotentialLongPinchStarted()
        {
            longPinchStateText.SetText("Potential long pinch started");
        }

        public void OnLongPinchEnter()
        {
            longPinchStateText.SetText("Long pinch Entered");
        }

        public void OnLongPinchExit()
        {
            longPinchStateText.SetText("Long pinch Exit");
        }

        public void OnSingleClickPerformed()
        {
            longPinchStateText.SetText("Single pinch performed");
        }

        public void OnLongPinchPerformed()
        {
            longPinchStateText.SetText("Long pinch performed");
        }

        public void OnLongPinchHeld(float elapsedTime)
        {
            longPinchStateText.SetText($"Potential long pinch started {elapsedTime.ToString("0.00")}s");
        }
    }
}
