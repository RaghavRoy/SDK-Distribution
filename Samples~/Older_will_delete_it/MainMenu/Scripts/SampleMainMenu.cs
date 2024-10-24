using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


namespace JioXSDK
{
    public class SampleMainMenu : MonoBehaviour
    {
        [SerializeField] private List<Button> buttons;
        private void OnEnable()
        {
            foreach (var button in buttons)
            {
                button.onClick.AddListener(() => ButtonClicked(button.name));
            }
        }

        private void OnDisable()
        {
            foreach (var button in buttons)
            {
                button.onClick.RemoveListener(() => ButtonClicked(button.name));
            }
        }
        private void ButtonClicked(string sceneName)
        {
            Debug.Log("Loading Scenename: " + sceneName);
            SceneManager.LoadSceneAsync(sceneName);
        }
    }
}