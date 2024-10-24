using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace JioXSDK
{
    public class ImageRotate : MonoBehaviour
    {
        public GameObject[] gameObjects; // Array of 5 GameObjects having Image components
        public Sprite[] allImages; // n number of images
        private int currentImageIndex = 0; // To track the current image in the sprite list
        public float imageWidth;
        private float centerofImageWidth;
        public RectTransform scrollViewContent;

        public int centerIndex = 0;

        public ScrollRect scrollrect;

        void Start()
        {
            imageWidth = scrollViewContent.rect.width / gameObjects.Length;
            centerofImageWidth = imageWidth / 2f;
            //  UpdateImages();
        }


        void Update()
        {
            // Clockwise rotation on Space key
            if (scrollViewContent.anchoredPosition.x > imageWidth)
            {

                //RotateAnticlockwise();
                MoveImagesLeft();
            }

            if (scrollViewContent.anchoredPosition.x < -imageWidth)
            {

                //  
                MoveImagesRight();
                //RotateClockwise();

            }

            // // Clockwise rotation on Space key
            // if (Input.GetKeyDown(KeyCode.Space))
            // {
            //     RotateAnticlockwise();

            // }

            // // Anticlockwise rotation on Shift key
            // if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            // {
            //     RotateClockwise();

            // }
        }
        private void MoveImagesLeft()
        {
            scrollrect.enabled = false;
            gameObjects[4].transform.SetSiblingIndex(0);
            scrollViewContent.transform.localPosition = gameObjects[2].transform.localPosition;
            // SwapIndexofImagesLeftSide();
            RotateAnticlockwise();
            scrollrect.enabled = true;

        }
        private void MoveImagesRight()
        {
            scrollrect.enabled = false;
            gameObjects[0].transform.SetSiblingIndex(4);
            scrollViewContent.transform.localPosition = gameObjects[2].transform.localPosition;
            // SwapIndexofImagesRightSide();
            RotateClockwise();
            scrollrect.enabled = true;
        }

        // Rotate the array clockwise and update only the image of the first GameObject
        void RotateClockwise()
        {
            // Update the sprite of the first object (index 0)
            Image imgComponent = gameObjects[0].GetComponent<Image>();
            if (imgComponent != null)
            {
                imgComponent.sprite = allImages[currentImageIndex];
            }

            // Increment the image index, ensuring it loops
            currentImageIndex = (currentImageIndex + 1) % allImages.Length;

            // Move the first object (index 0) to the last position in the array
            ShiftArrayClockwise();

            // Force the canvas/UI to refresh
            Canvas.ForceUpdateCanvases();
        }

        // Rotate the array anticlockwise and update only the image of the last GameObject
        void RotateAnticlockwise()
        {
            // Update the sprite of the last object (index length-1)
            Image imgComponent = gameObjects[gameObjects.Length - 1].GetComponent<Image>();
            if (imgComponent != null)
            {
                imgComponent.sprite = allImages[currentImageIndex];
            }

            // Decrement the image index, ensuring it loops
            currentImageIndex = (currentImageIndex - 1 + allImages.Length) % allImages.Length;

            // Move the last object to the first position in the array
            ShiftArrayAnticlockwise();

            // Force the canvas/UI to refresh
            Canvas.ForceUpdateCanvases();
        }

        // Helper function to move the first object to the last position in the array
        void ShiftArrayClockwise()
        {
            GameObject firstObject = gameObjects[0];
            for (int i = 0; i < gameObjects.Length - 1; i++)
            {
                gameObjects[i] = gameObjects[i + 1];
            }
            gameObjects[gameObjects.Length - 1] = firstObject;
        }

        // Helper function to move the last object to the first position in the array
        void ShiftArrayAnticlockwise()
        {
            GameObject lastObject = gameObjects[gameObjects.Length - 1];
            for (int i = gameObjects.Length - 1; i > 0; i--)
            {
                gameObjects[i] = gameObjects[i - 1];
            }
            gameObjects[0] = lastObject;
        }

        // This method is optional, in case you want to initialize or reset all images at start
        void UpdateImages()
        {
            for (int i = 0; i < gameObjects.Length; i++)
            {
                Image imgComponent = gameObjects[i].GetComponent<Image>();
                if (imgComponent != null)
                {
                    int imageIndex = (currentImageIndex + i) % allImages.Length;
                    imgComponent.sprite = allImages[imageIndex];
                }
            }
        }
    }
}
