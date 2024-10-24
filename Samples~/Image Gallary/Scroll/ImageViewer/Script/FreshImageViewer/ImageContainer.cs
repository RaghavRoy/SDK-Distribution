using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace JioXSDK
{
    public class ImageContainer : MonoBehaviour
    {
        [SerializeField]
        private List<Sprite> images = new List<Sprite>();
        private void Start()
        {
            Debug.Log("images count" + images.Count);
        }
        public int ImageCount => images.Count;
        public Sprite GettheSprite(int index)
        {
            return images[index];
        }

    }
}
