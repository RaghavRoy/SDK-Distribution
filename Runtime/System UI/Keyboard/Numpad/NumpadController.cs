using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using System.Text;
namespace JioXSDK
{
    [System.Serializable]
    public struct ButtonWithText
    {
        public KeyStringcharbutton button; // Reference to the Button
        public TMP_Text buttonText; // Reference to the Text component

        // Method to set button text
        public void SetText(string text)
        {
            buttonText.text = text;
        }
    }
    public class NumpadController : MonoBehaviour
    {
        [SerializeField]
        private List<ButtonWithText> numpadButtons;

        //  [SerializeField]
        public TMP_InputField tMP_NumpadInputField;

        private StringBuilder str = new StringBuilder();

        // [SerializeField]
        // private TMP_InputField tmp_PreviewInputField;

        [SerializeField]
        private GameObject numpadManagerGameObject;

        [SerializeField]
        private NumpadManager numpadManager;
        private void Start()
        {
            numpadManagerGameObject = GameObject.Find("NumpadManager");
            numpadManager = numpadManagerGameObject.GetComponent<NumpadManager>();
        }
        public void PreviewNumpadInput(int i)
        {
            //   string input = numpadButtons[i].button.GetComponent<KeyStringcharbutton>().StrInput; // Adjust this to your button component
            string input = numpadButtons[i].buttonText.text;//.GetComponent<KeyStringcharbutton>().StrInput;
            tMP_NumpadInputField.text += input;
            str.Append(input); // Add the input to the StringBuilder
            //  numpadManager.tMP_previewInputField.text = tMP_NumpadInputField.text;
        }

        public void NumpadDeleteKey()
        {
            Debug.Log(str.Length);
            if (str.Length > 0)
            {
                str.Remove(str.Length - 1, 1); // Remove last character
            }
            tMP_NumpadInputField.text = str.ToString();
            // numpadManager.tMP_previewInputField.text = tMP_NumpadInputField.text;
        }

        public void CloseNumPad()
        {
            // if (numpadManager.currentNumpad != null)
            // {
            //     Destroy(numpadManager.currentNumpad);
            //     // numpadManager.currentNumpad = null;
            //     // numpadManager.tMP_previewInputField.text = "";
            // }
        }

    }
}
