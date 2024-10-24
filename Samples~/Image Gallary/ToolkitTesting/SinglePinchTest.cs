using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JioXSDK
{
    public class SinglePinchTest : MonoBehaviour
    {
        [SerializeField] private Button _btnReset;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private ExtendedClick _interaction;

        private int i = 0;

        private void Awake()
        {
            _btnReset.onClick.AddListener(ResetTest);
            ResetTest();

            _interaction.OnClickPerformed.AddListener(AddNumberToTheCount);
            _interaction.OnDoubleClick.AddListener(AddNumberToTheCount);
        }

        private void AddNumberToTheCount()
        {
            i++;
            _text.text = $"Pinch Performed ({i})";
        }

        public void ResetTest()
        {
            i = 0;
            _text.text = $"Pinch Performed ({i})";
        }
    }
}
