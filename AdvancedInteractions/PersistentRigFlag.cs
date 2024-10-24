using UnityEngine;
using UnityEngine.UI;

namespace JioXSDK
{
    public class PersistentRigFlag : MonoBehaviour
    {
        public bool needPersistentRig = true;

        private void OnEnable()
        {
            GetComponent<Button>().onClick.AddListener(HandleRig);
        }

        public void HandleRig()
        {
            if (!needPersistentRig)
            {
                DestroyHandler.Instance.CleanUp();
            }
        }
    }
}
