using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using QCHT.Samples.XRKeyboard;
using UnityEngine.InputSystem;
using System;
namespace JioXSDK
{
  public class KeyboardManager : MonoBehaviour
  {
    public GameObject keyboardPrefab;

    [SerializeField]
    private GameObject numpadPrefab;
    public TMP_InputField alphanumericInput;
    [SerializeField]
    public TMP_InputField tMP_numericInputField;

    [SerializeField] private List<GameObject> keyboardVariants;

    [SerializeField] private List<GameObject> keyboardCanvas;
    // [SerializeField] private List<TMP_InputField> inputFields;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private Vector3 rotationOffset;
    private GameObject instantiatedKeyboard;
    private Vector3 spawnPosition;
    private TMP_InputField activeInputField;
    public static TMP_Text minAdaptiveScale;

    [SerializeField]
    private GameObject jioXOrigin;

    private TMP_InputField searchTextField;

    [HideInInspector]
    public GameObject keyboard;

    [HideInInspector]
    public GameObject currentNumpad;
    private enum KeyboardType
    {
      Alphanumeric,
      NumbersAndPunctuation,
      Special

    };
    private void Update()
    {
      // Check if the input field is selected and reposition the numpad
      // if (alphanumericInput.isFocused && keyboard != null)
      // {
      //   RepositionKeyboard();
      // }
    }
    public void OnSelectKeyboard()
    {
      //asdabdajbdja
      HideNumpad();
      alphanumericInput.ActivateInputField();
      if (alphanumericInput.contentType == TMP_InputField.ContentType.Alphanumeric)
      {
        if (keyboard == null)
        {
          KeyboardSelection(alphanumericInput);
          //  alphanumericInput.DeactivateInputField();
        }
        else
        {
          RepositionKeyboard();
        }
        alphanumericInput.DeactivateInputField();
      }

    }
    public void onSelectNumpadInput()
    {
      HideKeyboard();
      tMP_numericInputField.ActivateInputField();
      if (tMP_numericInputField.contentType == TMP_InputField.ContentType.IntegerNumber)
      {
        if (currentNumpad == null)
        {
          KeyboardSelection(tMP_numericInputField);
          // tMP_numericInputField.DeactivateInputField();
        }
        else
        {
          RepositionKeyboard();
        }
        tMP_numericInputField.DeactivateInputField();
      }
    }
    public void KeyboardSelection(TMP_InputField tMP_InputField)
    {
      if (tMP_InputField.contentType == TMP_InputField.ContentType.Alphanumeric)
      {
        // instantite keyboardprefab
        HideNumpad();
        keyboard = KeyboardSpawn(keyboardPrefab);
        KeepTheKeyboardRequiredObject(keyboard);
        keyboardCanvas[0].SetActive(true);
        keyboardCanvas[1].SetActive(false);
      }

      if (tMP_InputField.contentType == TMP_InputField.ContentType.IntegerNumber)
      {
        // instantite keyboardprefab

        HideKeyboard();
        currentNumpad = KeyboardSpawn(keyboardPrefab);
        KeepTheKeyboardRequiredObject(currentNumpad);
        keyboardCanvas[0].SetActive(false);
        keyboardCanvas[1].SetActive(true);
      }

    }
    private void KeepTheKeyboardRequiredObject(GameObject game)
    {
      var children = game.transform.gameObject.GetComponentsInChildren<Transform>();

      foreach (var child in children)
      {

        if (child.gameObject.name == "AlphaNumericKeyboard" ||
            child.gameObject.name == "NumericKeyboard" ||
            child.gameObject.name == "SpecialKeyboard")
        {
          keyboardVariants.Add(child.gameObject);
        }

        if (child.gameObject.name == "KeyboardCanvas" || child.gameObject.name == "NumpadCanvas")
        {
          keyboardCanvas.Add(child.gameObject);
        }
      }

    }
    private GameObject KeyboardSpawn(GameObject gameObject)
    {
      Vector3 spawnPosition = Camera.main.transform.position +
                              (Camera.main.transform.right * cameraOffset.x) +
                              (Camera.main.transform.up * cameraOffset.y) +
                              (Camera.main.transform.forward * cameraOffset.z);

      GameObject keyboard = Instantiate(gameObject, spawnPosition, Quaternion.Euler(rotationOffset.x, Camera.main.transform.rotation.eulerAngles.y, rotationOffset.z));
      return keyboard;
    }
    public void RepositionKeyboard()
    {
      if (keyboard != null)
      {
        Vector3 spawnPosition = Camera.main.transform.position +
                            (Camera.main.transform.right * cameraOffset.x) +
                            (Camera.main.transform.up * cameraOffset.y) +
                            (Camera.main.transform.forward * cameraOffset.z);

        keyboard.transform.position = spawnPosition;
        keyboard.transform.rotation = Quaternion.Euler(rotationOffset.x, Camera.main.transform.rotation.eulerAngles.y, rotationOffset.z);
      }
    }
    public void HideKeyboard()
    {
      if (keyboard != null)
      {
        keyboardVariants.Clear();
        keyboardCanvas.Clear();
        Destroy(keyboard);
        keyboard = null; // Reset the reference
      }
    }
    public void HideNumpad()
    {
      if (currentNumpad != null)
      {
        keyboardVariants.Clear();
        keyboardCanvas.Clear();
        Destroy(currentNumpad);
        currentNumpad = null; // Reset the reference
      }
    }




    // public void KeyboardSelection(TMP_InputField tMP_InputField)
    // {
    //   searchTextField = tMP_InputField;
    //   Debug.Log("check....");
    //   if (tMP_InputField.contentType == TMP_InputField.ContentType.Alphanumeric && keyboardPrefab != null)
    //   {

    //     keyboardPrefab.transform.GetChild(0).transform.localScale = new Vector3(0.00026f, 0.00026f, 0.00026f);
    //     keyboardPrefab.GetComponent<KeyboardInputController>().KeyboardInputsField.text = tMP_InputField.text;

    //     if (keyboardPrefab.name != "JioXkeyboard(Clone)")
    //     {
    //       keyboardPrefab = Instantiate(keyboardPrefab, spawnPosition, Quaternion.identity);
    //       var children = keyboardPrefab.transform.gameObject.GetComponentsInChildren<Transform>();
    //       foreach (var child in children)
    //       {
    //         if (child.gameObject.name == "AlphaNumericKeyboard" ||
    //             child.gameObject.name == "NumericKeyboard" ||
    //             child.gameObject.name == "SpecialKeyboard")
    //         {
    //           keyboardVariants.Add(child.gameObject);
    //         }
    //       }
    //     }
    //   }
    // keyboardPrefab = KeyboardSpawn();
    // SetKeyboardType(tMP_InputField.keyboardType.ToString());
    //   RepositionKeyboard();
    //  }

    // public void KeyboardSelection(TMP_InputField tMP_InputField)
    // {

    // if (keyboardPrefab != null)
    // {
    //   searchTextField = tMP_InputField;
    //   tMP_InputField.ActivateInputField();
    //   //tMP_InputField.text = "";
    //   keyboardPrefab.transform.GetChild(0).transform.localScale = new Vector3(0.00026f, 0.00026f, 0.00026f);

    //   keyboardPrefab.GetComponent<KeyboardInputController>().KeyboardInputsField.text = tMP_InputField.text;
    //   if (keyboardPrefab.name != "JioXkeyboard(Clone)")
    //   {
    //     keyboardPrefab = Instantiate(keyboardPrefab, spawnPosition, Quaternion.identity);
    //     var children = keyboardPrefab.transform.gameObject.GetComponentsInChildren<Transform>();
    //     foreach (var child in children)
    //     {
    //       if (child.gameObject.name == "AlphaNumericKeyboard" ||
    //           child.gameObject.name == "NumericKeyboard" ||
    //           child.gameObject.name == "SpecialKeyboard")
    //       {
    //         keyboardVariants.Add(child.gameObject);
    //       }
    //     }
    //   }
    // }

    //   SetKeyboardType(tMP_InputField.keyboardType.ToString());
    //   keyboardPrefab = KeyboardSpawn();
    //   keyboardPrefab.GetComponent<KeyboardInputController>().SetInputField(tMP_InputField);
    //   keyboardPrefab.SetActive(true);

    // }


    // public void RepositionKeyboard()
    // {
    //   if (keyboardPrefab != null)
    //   {
    //     spawnPosition = Camera.main.transform.position +
    //                              (Camera.main.transform.right * cameraOffset.x) +
    //                              (Camera.main.transform.up * cameraOffset.y) +
    //                              (Camera.main.transform.forward * cameraOffset.z);

    //     keyboardPrefab.transform.position = spawnPosition;

    //     keyboardPrefab.transform.rotation = Quaternion.Euler(rotationOffset.x, Camera.main.transform.rotation.eulerAngles.y, rotationOffset.z);
    //   }

    // return keyboardPrefab;

    // }
    // private void Update()
    // {
    //   // if (searchTextField.isFocused && keyboardPrefab != null)
    //   // {
    //   //   RepositionKeyboard();
    //   // }
    // }

    // private void SetKeyboardType(string str)
    // {
    //   DisableObject();
    //   switch (str)
    //   {
    //     case "Default":
    //       keyboardVariants[0].SetActive(true);
    //       break;
    //     case "NumbersAndPunctuation":
    //       keyboardVariants[1].SetActive(true);
    //       break;
    //     case "Social":
    //       keyboardVariants[2].SetActive(true);
    //       break;
    //     default:
    //       break;
    //   }
    // }

    // public void HideKeyboard()
    // {
    //   keyboardPrefab.SetActive(false);
    // }
    // private void DisableObject()
    // {
    //   if (keyboardPrefab != null && keyboardVariants != null && keyboardVariants.Count > 0)
    //   {
    //     foreach (GameObject keyboard in keyboardVariants)
    //     {
    //       keyboard.SetActive(false);
    //     }
    //   }
    // }
    // public void SubmitButtonClick()
    // {
    //   keyboardPrefab.SetActive(false);
    //   // Destroy(keyboardPrefab);
    //   searchTextField.DeactivateInputField(false);
    //   searchTextField.text = "";

    // }

    private void UpdateInput()
    {
      alphanumericInput.text = keyboard.GetComponent<KeyboardInputController>().KeyboardInputsField.text;
    }

  }
}

