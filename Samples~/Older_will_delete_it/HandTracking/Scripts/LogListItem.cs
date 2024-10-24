using JioXSDK.HandTracking;
using TMPro;
using UnityEngine;

namespace JioXSDK
{
    public class LogListItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _txtHand;
        [SerializeField] private TextMeshProUGUI _txtState;
        [SerializeField] private TextMeshProUGUI _txtTime;

        public void SetData(LogData data)
        {
            _txtHand.text = data.hand.ToString();
            _txtState.text = data.handState;
            _txtTime.text = data.timeElapsed;
        }

    }
}
