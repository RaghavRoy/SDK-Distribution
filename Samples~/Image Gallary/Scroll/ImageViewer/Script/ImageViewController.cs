using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.LegacyInputHelpers;
namespace JioXSDK
{
    public class ImageViewController : MonoBehaviour
    {
        public ScrollRect scrollrect;
        public RectTransform scrollViewContent;
        public GameObject[] images;
        public float imageWidth;
        public int centerIndex = 0;
        private const int totalImages = 5;

        private float centerofImageWidth;

        [SerializeField]
        private Vector3[] positions;

        private int changeIndexVariable = 0;

        private int currentindex;

        // [SerializeField]
        // private ImageContainer imageContainer;

        public Sprite[] allImages;
        public Sprite[] b;

        // private void OnEnable()
        // {
        //     Debug.Log("all Images =========" + allImages.Count());
        // }
        void Start()
        {
            Debug.Log("b " + b[0]);
            Debug.Log("all Images =========" + allImages.Length);
            // imageWidth = scrollViewContent.rect.width * images.Length;
            // Debug.Log(scrollViewContent.rect.width)
            imageWidth = scrollViewContent.rect.width / images.Length;
            centerofImageWidth = imageWidth / 2f;
            SelectTheImage(0);

            // images[0].GetComponent<Image>().sprite = imageContainer.GettheSprite(imageContainer.ImageCount - 2);
            // images[1].GetComponent<Image>().sprite = imageContainer.GettheSprite(imageContainer.ImageCount - 1);
            // images[2].GetComponent<Image>().sprite = imageContainer.GettheSprite(0);
            // images[3].GetComponent<Image>().sprite = imageContainer.GettheSprite(1);
            // images[4].GetComponent<Image>().sprite = imageContainer.GettheSprite(2);
            // SelectTheImage(currentIndex);
        }
        private void FixedUpdate()
        {
            if (scrollViewContent.anchoredPosition.x > imageWidth)
            {
                centerIndex = (centerIndex + 1) % allImages.Length;
                //  UpdateImages();
                MoveImagesLeft();
            }
            if (scrollViewContent.localPosition.x < -imageWidth)
            {
                centerIndex = (centerIndex - 1 + allImages.Length) % allImages.Length;
                //  UpdateImages();
                MoveImagesRight();
            }
        }
        public void UpdateImages()
        {
            Debug.Log("images.Length" + images.Length);
            for (int i = 0; i < images.Length; i++)
            {
                // Get the Image component from each GameObject
                Image imgComponent = images[i].GetComponent<Image>();
                if (imgComponent != null)
                {
                    // Calculate the correct index based on clockwise/anticlockwise rotation
                    int imageIndex = (centerIndex + i - 2 + allImages.Length) % allImages.Length;
                    imgComponent.sprite = allImages[imageIndex];
                }
            }
            // Force the canvas/UI to refresh and update the images in real-time
            Canvas.ForceUpdateCanvases();
        }
        public void SelectTheImage(int index)
        {

            Debug.Log("I N D E X =========" + index);
            centerIndex = index;
            // currentindex = index;
            if (index == 0)
            {
                Debug.Log("Length " + allImages[0]);
                Debug.Log("images[0].GetComponent<Image>().sprite" + images[0].GetComponent<Image>().sprite);
                images[0].GetComponent<Image>().sprite = allImages[22];

                images[1].GetComponent<Image>().sprite = allImages[23];
                images[2].GetComponent<Image>().sprite = allImages[index];
                images[3].GetComponent<Image>().sprite = allImages[index + 1];
                images[4].GetComponent<Image>().sprite = allImages[index + 2];
            }
            // else if (index == 24)
            // {
            //     images[0].GetComponent<Image>().sprite = imageContainer.GettheSprite(index - 2);
            //     images[1].GetComponent<Image>().sprite = imageContainer.GettheSprite(index - 1);
            //     images[2].GetComponent<Image>().sprite = imageContainer.GettheSprite(index);
            //     images[3].GetComponent<Image>().sprite = imageContainer.GettheSprite(0);
            //     images[4].GetComponent<Image>().sprite = imageContainer.GettheSprite(1);
            // }
            // else if (index == 1)
            // {
            //     images[0].GetComponent<Image>().sprite = imageContainer.GettheSprite(imageContainer.ImageCount - 1);
            //     images[1].GetComponent<Image>().sprite = imageContainer.GettheSprite(index - 1);
            //     images[2].GetComponent<Image>().sprite = imageContainer.GettheSprite(index);
            //     images[3].GetComponent<Image>().sprite = imageContainer.GettheSprite(index + 1);
            //     images[4].GetComponent<Image>().sprite = imageContainer.GettheSprite(index + 2);
            // }
            // else if (index == imageContainer.ImageCount - 1)
            // {
            //     images[0].GetComponent<Image>().sprite = imageContainer.GettheSprite(imageContainer.ImageCount - 2);
            //     images[1].GetComponent<Image>().sprite = imageContainer.GettheSprite(imageContainer.ImageCount - 1);
            //     images[2].GetComponent<Image>().sprite = imageContainer.GettheSprite(index);
            //     images[3].GetComponent<Image>().sprite = imageContainer.GettheSprite(index + 1);
            //     images[4].GetComponent<Image>().sprite = imageContainer.GettheSprite(0);
            // }
            // else
            // {
            //     images[0].GetComponent<Image>().sprite = imageContainer.GettheSprite(index - 2);
            //     images[1].GetComponent<Image>().sprite = imageContainer.GettheSprite(index - 1);
            //     images[2].GetComponent<Image>().sprite = imageContainer.GettheSprite(index);
            //     images[3].GetComponent<Image>().sprite = imageContainer.GettheSprite(index + 1);
            //     images[4].GetComponent<Image>().sprite = imageContainer.GettheSprite(index + 2);
            // }
        }

        private void LeftAtCenterChangePosition(Vector3 to)
        {
            //  images[0].transform.localPosition = Vector3.Lerp(images[0].transform.localPosition, positions[1], 0.2f);

        }

        // private void SwapIndex()
        // {
        //     images[2].transform.SetSiblingIndex(0);
        //     scrollViewContent.transform.localPosition = images[1].transform.localPosition;
        //     SwapIndexofImagesLeftSide();
        // }

        // private void SwapIndexofImagesLeftSide()
        // {
        //     GameObject temp = images[0];//left
        //     images[0] = images[2];
        //     images[2] = images[1];
        //     images[1] = temp;
        // }

        private void MoveImagesLeft()
        {
            scrollrect.enabled = false;
            images[4].transform.SetSiblingIndex(0);
            scrollViewContent.transform.localPosition = images[2].transform.localPosition;
            SwapIndexofImagesLeftSide();
            scrollrect.enabled = true;

        }
        private void SwapIndexofImagesLeftSide()
        {
            GameObject temp = images[0];
            images[0] = images[4];
            images[4] = images[3];
            images[3] = images[2];
            images[2] = images[1];
            images[1] = temp;
        }

        private void MoveImagesRight()
        {
            scrollrect.enabled = false;
            images[0].transform.SetSiblingIndex(4);
            scrollViewContent.transform.localPosition = images[2].transform.localPosition;
            SwapIndexofImagesRightSide();
            scrollrect.enabled = true;
        }
        private void SwapIndexofImagesRightSide()
        {
            GameObject temp = images[4];
            images[4] = images[0];
            images[0] = images[1];
            images[1] = images[2];
            images[2] = images[3];
            images[3] = temp;
        }
    }

}

