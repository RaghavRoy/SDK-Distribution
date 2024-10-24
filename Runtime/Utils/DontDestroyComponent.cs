using UnityEngine;

namespace JioXSDK.Utils
{
    public class DontDestroyComponent : MonoBehaviour
    {
        private static DontDestroyComponent Instance;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DestroyHandler.Instance?.AddDontDestroyInList(this.gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }
}
