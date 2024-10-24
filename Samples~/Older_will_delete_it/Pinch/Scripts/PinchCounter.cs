using JioXSDK.Interactions;
using JioXSDK.Utils;
using System;
using TMPro;
using UnityEngine;

namespace Samples.PinchOnUI
{
    public class PinchCounter : MonoBehaviour
    {
        private TextMeshProUGUI counterText;
        [SerializeField] private GameObject textPrefab;
        [SerializeField] private Transform logParent;

        int counter = 0;
        float holdTime = 0f;
        bool isPinchHold = false;

        private void Start()
        {
            CSVHandler.SetHeaderData("Counter", "Pinch Hold Duration");
        }

        public void OnPinchRelease()
        {
            CSVHandler.AddLineData(counter, holdTime.ToString());
            counterText.SetText(+counter + "                " + holdTime.ToString("F2") + "s");
            holdTime = 0f;
            isPinchHold = false;
        }

        public void SaveToCSV()
        {
            CSVHandler.SaveCSV("PinchCounter");
        }

        public void OnPinch()
        {
            var textObject = Instantiate(textPrefab, logParent);
            counterText = textObject.GetComponent<TextMeshProUGUI>();
            isPinchHold = true;
            counter++;
        }

        void Update()
        {
            if (isPinchHold)
            {
                holdTime += Time.deltaTime;
                counterText.SetText(+ counter + "                " + holdTime.ToString("F2") + "s");
            }
        }

        public void ResetLogs()
        {
            var logs = logParent.GetComponentsInChildren<MonoBehaviour>();
            counter = 0;
            
            for (int i = 1; i < logs.Length - 1; i++)
            {
                Destroy(logs[i].gameObject);
            }
        }
    }
}
