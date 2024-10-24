using System;
using System.Collections;
using System.Collections.Generic;
using Qualcomm.Snapdragon.Spaces.Samples;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;
namespace JioXSDK
{
    public class ImageViewerController : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] allImages;

        [SerializeField]
        private GameObject[] imagesGameObject;

        [SerializeField]
        private GameObject[] imagesTickGameObject;

        // [SerializeField]
        // private GameObject imagesContextualMenuGameObject;
        [SerializeField]
        private float spacing = 30f;

        [SerializeField]
        private float itemwWidth = 2400f;

        [SerializeField]
        private GameObject virtualInterface;

        [SerializeField]
        private GameObject imageViewer;
        private bool isVisble = true;

        [SerializeField]
        private GameObject closeButton;

        [SerializeField]
        private RectTransform scrollViewContent;

        [SerializeField]
        private ScrollRect scrollrect;

        [SerializeField]
        private GameObject horizontalLowerImageViewer;

        [SerializeField]
        private GameObject handle;

        private float detailsFadeOutTime = 3f;
        private bool zoomInandOut = false;
        private float zoomIn = 3;
        private float zoomOut = 1;
        private Vector2 newSize;
        private int siblingIndex;
        private int centerIndex = 0;
        private float imageWidth;
        private float centerofImageWidth;

        private bool doubleClickControler = true;

        private bool imageDetails = false;

        private void Start()
        {
            virtualInterface.SetActive(isVisble);
            imageViewer.SetActive(!isVisble);
            closeButton.SetActive(!isVisble);
            horizontalLowerImageViewer.SetActive(!isVisble);
            imageWidth = scrollViewContent.rect.width / imagesGameObject.Length;
            centerofImageWidth = imageWidth / 2f;
            handle.transform.localPosition = new Vector3(0f, -600f, 0f);

            foreach (GameObject tickmark in imagesTickGameObject)
            {
                tickmark.SetActive(false);
            }
        }

        public void DisableImageViewer()
        {
            virtualInterface.SetActive(isVisble);
            imageViewer.SetActive(!isVisble);
            closeButton.SetActive(!isVisble);
            horizontalLowerImageViewer.SetActive(!isVisble);
            handle.transform.localPosition = new Vector3(0f, -600f, 0f);
        }
        public void EnableImageViewer(Image image)
        {
            virtualInterface.SetActive(!isVisble);
            imageViewer.SetActive(isVisble);
            siblingIndex = image.transform.GetSiblingIndex();
            centerIndex = siblingIndex;
            SelectTheImage(siblingIndex);
            ImageTickMarkControl(siblingIndex);
            closeButton.SetActive(isVisble);
            horizontalLowerImageViewer.SetActive(isVisble);
            handle.transform.localPosition = new Vector3(0f, -1100f, 0f);
        }

        public void EnableHorizontalImage()
        {
            closeButton.SetActive(isVisble);
            handle.transform.localPosition = new Vector3(0f, -1100f, 0f);
            horizontalLowerImageViewer.SetActive(isVisble);

        }

        private void ImageTickMarkControl(int siblingIndex)
        {
            foreach (GameObject tickmark in imagesTickGameObject)
            {
                tickmark.SetActive(false);
            }
            imagesTickGameObject[siblingIndex].SetActive(true);
        }

        public void SelectTheImage(int index)
        {
            centerIndex = index;
            int totalImages = allImages.Length;
            int[] imageIndices = new int[5];

            // Calculate the indices for the imagesGameObject array, ensuring they wrap around using modulo
            imageIndices[0] = (index - 2 + totalImages) % totalImages;
            imageIndices[1] = (index - 1 + totalImages) % totalImages;
            imageIndices[2] = index;
            imageIndices[3] = (index + 1) % totalImages;
            imageIndices[4] = (index + 2) % totalImages;

            // Update the imagesGameObject array with the corresponding images
            for (int i = 0; i < imagesGameObject.Length; i++)
            {
                imagesGameObject[i].GetComponent<Image>().sprite = allImages[imageIndices[i]];
            }
        }
        private void MoveImagesLeft()
        {
            scrollrect.enabled = false;
            imagesGameObject[4].transform.SetSiblingIndex(0);
            scrollViewContent.transform.localPosition = imagesGameObject[2].transform.localPosition;
            SwapIndexofImagesLeftSide();
            scrollrect.enabled = true;
            doubleClickControler = true;
        }
        private void MoveImagesRight()
        {
            scrollrect.enabled = false;
            imagesGameObject[0].transform.SetSiblingIndex(4);
            scrollViewContent.transform.localPosition = imagesGameObject[2].transform.localPosition;
            SwapIndexofImagesRightSide();
            scrollrect.enabled = true;
            doubleClickControler = true;
        }
        private void SwapIndexofImagesLeftSide()
        {
            GameObject temp = imagesGameObject[0];
            imagesGameObject[0] = imagesGameObject[4];
            imagesGameObject[4] = imagesGameObject[3];
            imagesGameObject[3] = imagesGameObject[2];
            imagesGameObject[2] = imagesGameObject[1];
            imagesGameObject[1] = temp;
            int newIndex = (centerIndex - 3 + allImages.Length) % allImages.Length;
            imagesGameObject[4].GetComponent<Image>().sprite = allImages[newIndex];
            imagesGameObject[1].transform.localScale = Vector3.one;

            imagesGameObject[1].transform.localScale = Vector3.one * 0.5f;
            imagesGameObject[2].transform.localScale = Vector3.one * 0.5f;
            imagesGameObject[3].transform.localScale = Vector3.one * 0.5f;
        }

        private void SwapIndexofImagesRightSide()
        {
            GameObject temp = imagesGameObject[4];
            imagesGameObject[4] = imagesGameObject[0];
            imagesGameObject[0] = imagesGameObject[1];
            imagesGameObject[1] = imagesGameObject[2];
            imagesGameObject[2] = imagesGameObject[3];
            imagesGameObject[3] = temp;
            int newIndex = (centerIndex + 3) % allImages.Length;
            imagesGameObject[0].GetComponent<Image>().sprite = allImages[newIndex];
            imagesGameObject[1].transform.localScale = Vector3.one * 0.5f;
            imagesGameObject[2].transform.localScale = Vector3.one * 0.5f;
            imagesGameObject[3].transform.localScale = Vector3.one * 0.5f;
        }
        private void FixedUpdate()
        {

            // Debug.Log("scrollViewContent.anchoredPosition.x" + scrollViewContent.anchoredPosition.x);
            // Debug.Log("imageWidth" + imageWidth);
            if (scrollViewContent.anchoredPosition.x > imageWidth)
            {

                //scrollrect.inertia = true;
                centerIndex = (centerIndex - 1 + allImages.Length) % allImages.Length;
                Debug.Log("center Index " + centerIndex);
                MoveImagesLeft();

            }
            if (scrollViewContent.localPosition.x < -imageWidth)
            {
                //scrollrect.inertia = true;
                centerIndex = (centerIndex + 1) % allImages.Length;
                Debug.Log("center Index " + centerIndex);
                MoveImagesRight();
            }


        }
        // private void Update()
        // {
        //     // if (scrollViewContent.anchoredPosition.x > imageWidth / 3)
        //     // {
        //     //     scrollrect.inertia = true;
        //     // }
        //     // if (scrollViewContent.localPosition.x < -imageWidth / 3)
        //     // {
        //     //     scrollrect.inertia = true;
        //     // }


        // }
        public void ImageDetailViewer(GameObject Image)
        {
            Debug.Log("Single Click");
            imageDetails = !imageDetails;
            Image.SetActive(imageDetails);
            if (imageDetails)
            {
                StartCoroutine(ImageDetailDisplay(Image));
            }

        }

        IEnumerator ImageDetailDisplay(GameObject Image)
        {
            Debug.Log("single click");
            //  Image.SetActive(true);
            yield return new WaitForSeconds(2f);
            Image.SetActive(false);
            imageDetails = false;
        }

        public void OnDoubleClick(GameObject Image)
        {
            Debug.Log("double click");
            if (doubleClickControler)
            {
                Image.transform.localScale = Vector3.one;
            }
            else
            {
                Image.transform.localScale = Vector3.one * 0.5f;
            }
            doubleClickControler = !doubleClickControler;
            Debug.Log("after toggle " + doubleClickControler);
        }

        public void EnableCanvasDisableImageViewer()
        {
            virtualInterface.SetActive(true);
            imageViewer.SetActive(false);
        }


    }
}
