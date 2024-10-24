using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JioXSDK
{
    [DefaultExecutionOrder(-1)]
    public class DestroyHandler :MonoBehaviour
    {
        [SerializeField] private List<GameObject> objectList;
        public static DestroyHandler Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void CleanUp()
        {
            foreach (var item in objectList)
            {
                Destroy(item);
            }
            objectList.Clear();
        }

        public void AddDontDestroyInList(GameObject go)
        {
            objectList.Add(go);
        }
    }
}
