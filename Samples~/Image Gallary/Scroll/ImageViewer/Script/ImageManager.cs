using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace JioXSDK
{
    public class ImageManager : MonoBehaviour
    {

        private List<Sprite> imageList;
        private Image leftImage, centerImage, rightImage;
        private int centerIndex;

        public ImageManager(List<Sprite> imageList, Image leftImage, Image centerImage, Image rightImage)
        {
            this.imageList = imageList;
            this.leftImage = leftImage;
            this.centerImage = centerImage;
            this.rightImage = rightImage;
            this.centerIndex = 0; // Start at the first image by default
        }

        // Move to the left image in the circular list
        public void MoveLeft()
        {
            centerIndex = (centerIndex - 1 + imageList.Count) % imageList.Count;
            UpdateImages(centerIndex);
        }

        // Move to the right image in the circular list
        public void MoveRight()
        {
            centerIndex = (centerIndex + 1) % imageList.Count;
            UpdateImages(centerIndex);
        }

        // Update the displayed images based on the center index
        public void UpdateImages(int centerIndex)
        {
            this.centerIndex = centerIndex;
            int leftIndex = (centerIndex - 1 + imageList.Count) % imageList.Count;
            int rightIndex = (centerIndex + 1) % imageList.Count;

            leftImage.sprite = imageList[leftIndex];
            centerImage.sprite = imageList[centerIndex];
            rightImage.sprite = imageList[rightIndex];
        }
    }
}
