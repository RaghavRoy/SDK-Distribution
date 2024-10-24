using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace JioXSDK
{
    public class NumpadManager : MonoBehaviour
    {

        [SerializeField]
        public TMP_InputField tMP_previewInputField;

        [SerializeField]
        private GameObject numpadPrefab;

        [HideInInspector]
        public GameObject currentNumpad;

        [SerializeField] private Vector3 cameraOffset;
        [SerializeField] private Vector3 rotationOffset;

        [SerializeField] private NumpadController numpadController;

        private void Update()
        {
            // Check if the input field is selected and reposition the numpad
            if (tMP_previewInputField.isFocused && currentNumpad != null)
            {
                RepositionNumpad();
            }
        }

        public void OnSelectNumpadInput()
        {
            if (tMP_previewInputField.contentType == TMP_InputField.ContentType.IntegerNumber && tMP_previewInputField.isFocused)
            {
                if (currentNumpad == null)
                {
                    currentNumpad = KeyboardSpawn();
                }
                else
                {
                    RepositionNumpad();
                }
            }
        }

        private GameObject KeyboardSpawn()
        {
            Vector3 spawnPosition = Camera.main.transform.position +
                                    (Camera.main.transform.right * cameraOffset.x) +
                                    (Camera.main.transform.up * cameraOffset.y) +
                                    (Camera.main.transform.forward * cameraOffset.z);

            Debug.Log("spawnPosition: " + spawnPosition);

            GameObject numpad = Instantiate(numpadPrefab, spawnPosition, Quaternion.Euler(rotationOffset.x, Camera.main.transform.rotation.eulerAngles.y, rotationOffset.z));

            return numpad;
        }

        private void RepositionNumpad()
        {
            if (currentNumpad != null)
            {
                currentNumpad.transform.position = Camera.main.transform.position +
                                                    (Camera.main.transform.right * cameraOffset.x) +
                                                    (Camera.main.transform.up * cameraOffset.y) +
                                                    (Camera.main.transform.forward * cameraOffset.z);

                currentNumpad.transform.rotation = Quaternion.Euler(rotationOffset.x, Camera.main.transform.rotation.eulerAngles.y, rotationOffset.z);
            }
        }

        public void UpdatePreviewTextField()
        {
            tMP_previewInputField.text = numpadController.tMP_NumpadInputField.text;
        }

        public void CloseNumpad()
        {
            if (currentNumpad != null)
            {
                Destroy(currentNumpad);

                currentNumpad = null;
            }
        }
    }
}


