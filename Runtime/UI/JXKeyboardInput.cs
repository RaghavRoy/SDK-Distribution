using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.InputSystem;
using System.Security.Cryptography;
using QCHT.Samples.XRKeyboard;
namespace JioXSDK
{
    public class JXKeyboardInput : MonoBehaviour
    {
        [SerializeField] private List<TMP_InputField> _inputField;
        TMP_InputField inputField;
        KeyboardManager keyboard;

        // public delegate void SelectKeyboard(TMP_InputField val);
        // public SelectKeyboard select;

        // Start is called before the first frame update
        void Start()
        {
            inputField = this.GetComponent<TMP_InputField>();

            keyboard = GameObject.FindObjectOfType<KeyboardManager>();

            //inputField.placeholder.GetComponent<TMP_Text>().text = "";
            // foreach (TMP_InputField tMP_Input in _inputField)
            // {
            //     tMP_Input.onSelect.AddListener(OnSelect);
            // }
        }

        // public void OnSelectAlphaNumericKeyboard()
        // {
        //     if (_inputField[0].contentType == TMP_InputField.ContentType.Alphanumeric && _inputField[0].isFocused)
        //     {
        //         if (keyboard.keyboardPrefab != null)
        //         {
        //             keyboard.KeyboardSelection(_inputField[0]);
        //         }
        //         else
        //         {
        //             keyboard.RepositionKeyboard();
        //         }
        //     }
        // }

        // public void OnSelectNumpadKeyboard()
        // {
        //     if (_inputField[1].isFocused && _inputField[1].contentType == TMP_InputField.ContentType.IntegerNumber)
        //     {

        //     }
        // }





        // public void OnSelect(string str)
        // {
        //     Debug.Log("OnSelect ");
        //     select = keyboard.KeyboardSelection;
        //     // inputField.placeholder.GetComponent<TMP_Text>().text = "";
        //     select(inputField);
        // }

        // public void OnDeSelect()
        // {
        //     select -= keyboard.KeyboardSelection;
        //     inputField.DeactivateInputField();
        // }


        private void OnKeyPressed(KeyStringCharButton key)
        {
            inputField.text += key.StrInput;
            inputField.caretPosition = inputField.text.Length;
        }
    }
}
