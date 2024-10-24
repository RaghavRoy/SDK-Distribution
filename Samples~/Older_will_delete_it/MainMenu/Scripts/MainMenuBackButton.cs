using UnityEngine;
using UnityEngine.SceneManagement;

namespace JioXSDK
{
    public class MainMenuBackButton : MonoBehaviour
    {
        private const string MainMenuScene = "JioXMenu"; 
        public void OnMainMenuBack()
        {
            DestroyHandler.Instance.CleanUp();
            SceneManager.LoadSceneAsync(MainMenuScene);
        }
    }
}
