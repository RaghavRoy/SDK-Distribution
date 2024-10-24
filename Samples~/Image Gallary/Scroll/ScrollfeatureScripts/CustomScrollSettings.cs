using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace JioXSDK
{
    public class CustomScrollSettings : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scrollSensitivity;
        [SerializeField] private TextMeshProUGUI scrollBarSensitivity;
        [SerializeField] private CustomScroll[] customScrolls;
        [SerializeField] private CustomScrollbar[] customScrollbars;

        private void Start()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            Debug.Log(customScrolls[0].Sensitivity);
            scrollSensitivity.text = customScrolls[0].Sensitivity.ToString();
            scrollBarSensitivity.text = customScrollbars[0].Sensitivity.ToString();
        }

        public void SetScrollSensitivity(int amount)
        {
            foreach (CustomScroll scroll in customScrolls)
            {
                scroll.Sensitivity += amount;
            }
            UpdateText();
        }

        public void SetScrollbarSensitivity(int amount)
        {
            foreach (CustomScrollbar scrollbar in customScrollbars)
            {
                scrollbar.Sensitivity += amount;
            }
            UpdateText();
        }
    }
}
