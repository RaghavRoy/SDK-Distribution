using TMPro;
using UnityEngine;

namespace JioXSDK
{
    public class LongPinchSeekbutton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI seekButtonText;
        [SerializeField] private float currentValue = 0;
        [SerializeField] private float length = 0;

        public void MoveForward()
        {
            currentValue += Time.deltaTime;
            currentValue = Mathf.Clamp(currentValue, 0, length);
            seekButtonText.SetText($"{currentValue.ToString("0.0")}s");
        }

        public void MoveBackward()
        {
            currentValue -= Time.deltaTime;
            currentValue = Mathf.Clamp(currentValue, 0, length);
            seekButtonText.SetText($"{currentValue.ToString("0.0")}s");
        }
    }
}
