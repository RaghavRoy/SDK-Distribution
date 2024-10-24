using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JioXSDK
{
    public class KeyboardInputController : MonoBehaviour
    {


        [SerializeField] KeyboardManager keyboardManager;
        [SerializeField] private TMP_InputField _keyboardInputField;

        [SerializeField] private TMP_InputField _previewkeyboardInputField;

        public TMP_InputField KeyboardInputsField
        {
            get => _keyboardInputField;
            set => _keyboardInputField = value;
        }

        [SerializeField] List<GameObject> capsbuttonImage;
        //    [SerializeField] List<Image> capsenableview;

        private bool _isCaps = true;

        public bool IsCaps => _isCaps;

        public UnityEvent<Keybutton> OnKeyButtonPressed = new UnityEvent<Keybutton>();

        private List<Keybutton> _keyButtons;

        private void Start()
        {
            _previewkeyboardInputField = GameObject.Find("AlphaNumericInputField").GetComponent<TMP_InputField>();
            keyboardManager = GameObject.FindAnyObjectByType<KeyboardManager>();
            _keyButtons = GetComponentsInChildren<Keybutton>(true).ToList();
            foreach (Keybutton button in _keyButtons)
            {
                button.inputEvent.AddListener(RegisterInput);
            }
            foreach (GameObject im in capsbuttonImage)
            {
                im.SetActive(true);
            }
        }


        private void RegisterInput(Keybutton keyButton)
        {
            ProcessText(keyButton);
            OnKeyButtonPressed?.Invoke(keyButton);
        }

        private void ProcessText(Keybutton keyButton)
        {
            if (_keyboardInputField == null)
                return;

            var keyStringCharButton = keyButton as KeyStringcharbutton;
            if (keyStringCharButton != null && keyStringCharButton.StrInput.Length > 0)
            {
                _keyboardInputField.text += _isCaps ? keyStringCharButton.StrInput.ToUpper() : keyStringCharButton.StrInput.ToLower();
                _previewkeyboardInputField.text += _isCaps ? keyStringCharButton.StrInput.ToUpper() : keyStringCharButton.StrInput.ToLower();

                return;
            }

            var keyStringButton = keyButton as KeyStringbutton;
            if (keyStringButton != null && keyStringButton.StrInput.Length > 0)
            {
                _keyboardInputField.text += keyStringButton.StrInput;
                _previewkeyboardInputField.text += keyStringButton.StrInput;
                return;
            }

            var keySpecialButton = keyButton as KeySpecialbutton;
            if (keySpecialButton != null)
            {
                switch (keySpecialButton.KeySpecial)
                {
                    case KeySpecial.Delete:
                        if (_keyboardInputField.text.Length > 3 && _keyboardInputField.text.Substring(_keyboardInputField.text.Length - 2, 2) == "\n")
                        {
                            _keyboardInputField.text = _keyboardInputField.text.Substring(0, _keyboardInputField.text.Length - 2);
                            _previewkeyboardInputField.text = _previewkeyboardInputField.text.Substring(0, _previewkeyboardInputField.text.Length - 2);
                        }
                        else if (_keyboardInputField.text.Length > 0)
                        {
                            _keyboardInputField.text = _keyboardInputField.text.Substring(0, _keyboardInputField.text.Length - 1);
                            _previewkeyboardInputField.text = _previewkeyboardInputField.text.Substring(0, _previewkeyboardInputField.text.Length - 1);
                        }
                        break;
                    case KeySpecial.Enter:
                        _keyboardInputField.text += "\n";
                        _previewkeyboardInputField.text += "\n";
                        break;
                    case KeySpecial.Shift:
                        SetCaps(!_isCaps);
                        break;
                    case KeySpecial.DeleteAll:
                        _keyboardInputField.text = string.Empty;
                        _previewkeyboardInputField.text = string.Empty;
                        break;
                    case KeySpecial.SwitchObject:
                        keySpecialButton.SwitchObject();
                        break;
                    case KeySpecial.Hide:
                        Debug.Log($"Hiding Keyboard");
                        break;
                }
            }
        }

        private void SetCaps(bool isCap)
        {
            if (isCap)
            {
                foreach (GameObject im in capsbuttonImage)
                {
                    im.SetActive(true);
                }
            }
            else
            {
                foreach (GameObject im in capsbuttonImage)
                {
                    im.SetActive(false);
                }
            }
            foreach (var key in _keyButtons)
            {
                var keyChar = key as KeyStringcharbutton;
                if (keyChar != null)
                {
                    keyChar.SetMajor(isCap);
                }
            }
            _isCaps = isCap;
        }
        public void SetInputField(TMP_InputField tMP_InputField)
        {
            _previewkeyboardInputField = tMP_InputField;

        }


        public void OnSubmitButton()
        {
            keyboardManager.HideKeyboard();
        }
        public void OnSubmitNumpadButton()
        {
            keyboardManager.HideNumpad();
        }

    }
}
