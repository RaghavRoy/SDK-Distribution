using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JioXSDK.Interactions;
using JioXSDK.Utils;

namespace JioXSDK
{
    public class ScrollDetails : MonoBehaviour
    {

        [SerializeField]
        private List<TMP_Text> values = new List<TMP_Text>();
        [SerializeField]
        private int count = 0;
        float timer = 0;
        bool startpichtime = false;

        private Vector3 handPosition, handLastPos = Vector3.zero;

        private void Start()
        {
            CSVHandler.SetHeaderData("pinch direction", "pinch position", "Pinch duration");
        }

        private void OnEnable()
        {
            HandInteractionEvents.OnRightHandPinch += HandPinch;
            HandInteractionEvents.OnRightHandPinchRelease += HandPinchRelease;
        }
        void OnDisable()
        {
            HandInteractionEvents.OnRightHandPinch -= HandPinch;
            HandInteractionEvents.OnRightHandPinchRelease -= HandPinchRelease;
        }
        public void HandPinch()
        {
            values[count].text = FindDirection() + " " + HandInteractionEvents.RightHandPosition.ToString() + " " + timer.ToString();
        }
        public void HandPinchRelease()
        {
            values[count].text = FindDirection() + " " + HandInteractionEvents.RightHandPosition.ToString() + " " + timer.ToString();
            if (count < 5)
            {
                count++;
            }
            CSVHandler.AddLineData(FindDirection(), HandInteractionEvents.RightHandPosition, timer.ToString());
            timer = 0;
        }
        private string FindDirection()
        {
            handPosition = HandInteractionEvents.RightHandPosition;
            var delta = handLastPos - handPosition;
            if (delta.y > 0)
            {
                return "Up";
            }
            else if (delta.y < 0)
            {
                return "Down";
            }
            else
            {
                return "";
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (startpichtime)
            {
                timer += Time.deltaTime;
            }
        }

        public void SaveCSV()
        {
            CSVHandler.SaveCSV("ScrollSceneData");
        }
    }
}
