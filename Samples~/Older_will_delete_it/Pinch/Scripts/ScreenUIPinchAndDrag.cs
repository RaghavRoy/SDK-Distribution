using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace JioXSDK
{
    public class ScreenUIPinchAndDrag : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI sensitivity;
        [SerializeField] private TextMeshProUGUI accelerationText;
        [SerializeField] private TextMeshProUGUI deaccelerationText;
        [SerializeField] private TextMeshProUGUI limitText;
        [SerializeField] private TextMeshProUGUI distanceText;
        [SerializeField] private Draggable draggable;

        private void Start()
        {
            sensitivity.text = draggable.SensitivityFactor.ToString();
            deaccelerationText.text = draggable.Deacceleration.ToString();
            accelerationText.text = draggable.Acceleration.ToString();
            limitText.text = draggable.Limit.ToString();
        }

        private void Update()
        {
            distanceText.text = draggable.DistanceFromCamera.ToString();
        }

        public void UpdateSensitivityText(float amount)
        {
            draggable.SensitivityFactor += amount;
            sensitivity.text = draggable.SensitivityFactor.ToString();
        }

        public void UpdateDeaccelerationText(float amount)
        {
            draggable.Deacceleration += amount;
            deaccelerationText.text = draggable.Deacceleration.ToString();
        }

        public void UpdateAccelerationText(float amount)
        {
            draggable.Acceleration += amount;
            accelerationText.text = draggable.Acceleration.ToString();
        }

        public void UpdateLimitText(float amount)
        {
            draggable.Limit += amount;
            limitText.text = draggable.Limit.ToString();
        }
    }
}
