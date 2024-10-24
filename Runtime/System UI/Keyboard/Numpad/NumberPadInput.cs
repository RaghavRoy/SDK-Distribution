using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UI;
namespace JioXSDK
{
    [System.Serializable]
    public struct ButtonWthText
    {
        public KeyStringcharbutton button; // Reference to the Button
        public TMP_Text buttonText; // Reference to the Text component

        // Method to set button text
        public void SetText(string text)
        {
            buttonText.text = text;
        }
    }
    public class NumberPadInput : MonoBehaviour
    {
        [SerializeField]
        private List<ButtonWithText> numpadButtons;

        //  [SerializeField]
        public TMP_InputField tMP_NumpadInputField;

        [SerializeField]
        private TMP_InputField tMP_PreviewNumpadInputField;

        private StringBuilder str = new StringBuilder();


        private void Start()
        {
            tMP_NumpadInputField = GameObject.Find("NumpadInputField").GetComponent<TMP_InputField>();
        }
        public void PreviewNumpadInput(int i)
        {
            //   string input = numpadButtons[i].button.GetComponent<KeyStringcharbutton>().StrInput; // Adjust this to your button component
            string input = numpadButtons[i].buttonText.text;//.GetComponent<KeyStringcharbutton>().StrInput;
            tMP_NumpadInputField.text += input;
            tMP_PreviewNumpadInputField.text = tMP_NumpadInputField.text;
            str.Append(input); // Add the input to the StringBuilder

        }

        public void NumpadDeleteKey()
        {
            Debug.Log(str.Length);
            if (str.Length > 0)
            {
                str.Remove(str.Length - 1, 1); // Remove last character
            }
            tMP_NumpadInputField.text = str.ToString();
            tMP_PreviewNumpadInputField.text = tMP_NumpadInputField.text;
            // numpadManager.tMP_previewInputField.text = tMP_NumpadInputField.text;
        }

        public void CloseNumPad()
        {
            // if (numpadManager.currentNumpad != null)
            // {
            //     Destroy(numpadManager.currentNumpad);
            //     numpadManager.currentNumpad = null;
            //     numpadManager.tMP_previewInputField.text = "";
            // }
        }
    }
}
