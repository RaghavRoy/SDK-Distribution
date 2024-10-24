using UnityEngine;

namespace JioXSDK.Utils
{
    public class DontDestroyComponent1 : MonoBehaviour
    {
        private static DontDestroyComponent1 Instance;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DestroyHandler.Instance.AddDontDestroyInList(this.gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }
}
