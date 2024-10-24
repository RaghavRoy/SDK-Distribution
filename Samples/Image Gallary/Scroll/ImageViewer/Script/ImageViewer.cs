using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using JetBrains.Annotations;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine.InputSystem.Utilities;
namespace JioXSDK
{
    public class ImageViewer : MonoBehaviour
    {
        // [SerializeField]
        // private List<Image> virtualInterfaceImages = new List<Image>();

        [SerializeField]
        private float spacing = 30f;

        [SerializeField]
        private float itemwWidth = 2200f;

        // [SerializeField]
        // private float horizontalScrollSpacing = 30f;

        // [SerializeField]
        // private float horizontalScrollitemwWidth = 440f;

        [SerializeField]
        private GameObject virtualInterface;

        [SerializeField]
        private GameObject imageViewer;
        private bool isVisble = true;

        [SerializeField]
        private ScrollRect scrollRect;

        private float detailsFadeOutTime = 3f;

        // [SerializeField]
        // private ScrollRect horizontalScrollRect;

        private bool zoomInandOut = false;

        private float zoomIn = 3;

        private float zoomOut = 1;
        Vector2 newSize;

        // [SerializeField]
        // private ExtendedClick extendedClick;

        // [SerializeField]
        // private CustomScroll customScroll;

        // [SerializeField]
        // private Image leftImage, centerImage, rightImage;

        // [SerializeField]
        // private RectTransform left, center, right;

        // // private RectTransform temp;

        // [SerializeField]
        // private Vector3[] cirularPosition = new Vector3[3];

        // [SerializeField]
        // private ImageViewController imageViewController;

        [SerializeField]
        private ImageContainer imageContainer;

        private int siblingIndex;

        [SerializeField]
        private ImageViewController imageViewController;

        [SerializeField]
        private Transform contentParent;

        [SerializeField]
        private GameObject closeButton;



        private void Start()
        {
            virtualInterface.SetActive(isVisble);
            imageViewer.SetActive(!isVisble);
            closeButton.SetActive(!isVisble);
            //temp.transform.localPosition = center.transform.localPosition;
            //  newSize = targetRectTransform.sizeDelta;

        }
        // Enable and Disable the ImageViewer
        public void DisableImageViewer()
        {
            virtualInterface.SetActive(isVisble);
            imageViewer.SetActive(!isVisble);
            closeButton.SetActive(!isVisble);
        }
        public void EnableImageViewer(Image image)
        {
            virtualInterface.SetActive(!isVisble);
            imageViewer.SetActive(isVisble);
            siblingIndex = image.transform.GetSiblingIndex();
            imageViewController.centerIndex = siblingIndex;
            imageViewController.SelectTheImage(siblingIndex);
            // imageViewController.SelectTheImage(siblingIndex);
            Debug.Log("sibling index " + siblingIndex);
            //Dynamic Scroll
            // imageViewController.currentIndex = siblingIndex;
            // imageViewController.UpdateImageSlots();
            //AdjustImage(siblingIndex, contentParent);
            closeButton.SetActive(isVisble);

        }

        // private void ImagePlacementWithIndex()
        // {

        //     // centerImage.sprite = imageContainer.GettheSprite(siblingIndex);
        //     // leftImage.sprite = imageContainer.GettheSprite(siblingIndex - 1);
        //     // rightImage.sprite = imageContainer.GettheSprite(siblingIndex + 1);

        // }

        // private void ShowImages(int siblingImage, List<Image> virtualInterface)
        // {
        //     imageViewController.currentIndex = siblingImage;
        //     imageViewController.UpdateImageSlots();
        // }


        private void AdjustImage(int siblingIndex, Transform parent)
        {
            float totalWidth = parent.childCount * itemwWidth + (parent.childCount - 1) * spacing;
            float targetPosition = siblingIndex * (itemwWidth + spacing);
            float viewportWidth = scrollRect.viewport.rect.width;
            float normalizedPosition = targetPosition / (totalWidth - viewportWidth);
            scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(normalizedPosition);
        }

        public void OnZoomInAndOut(Image image)
        {
            if (zoomInandOut)
            {
                image.rectTransform.localScale = Vector3.one * zoomIn;
            }
            else
            {
                image.rectTransform.localScale = Vector3.one;
            }
            zoomInandOut = !zoomInandOut;
        }
        public void EnableDetails(GameObject detailImage)
        {
            StartCoroutine(WaitForThreeSeconds(detailImage));
        }
        private IEnumerator WaitForThreeSeconds(GameObject detailImage)
        {
            detailImage.SetActive(true);
            yield return new WaitForSeconds(detailsFadeOutTime);
            detailImage.SetActive(false);
        }
    }
}
