using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DynamicScrollView : MonoBehaviour
{
    public GameObject imagePrefab; // Assign your image prefab in the inspector
    public RectTransform content;   // Assign the Content RectTransform
    public ScrollRect scrollRect;

    [SerializeField]
    private List<Sprite> allImages; // Your image sprites
    public Queue<GameObject> imagePool = new Queue<GameObject>(); // Object pool
    private int imagesLoaded = 0;   // Track how many images are loaded
    private int imagesToLoad = 5;   // Number of images to load at once
    private float scrollThreshold = 0.9f; // Threshold for loading more images

    void Start()
    {
        LoadImages();
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    private void LoadImages()
    {
        while (imagePool.Count < imagesToLoad && imagesLoaded < allImages.Count)
        {
            GameObject imageObject = GetImageObject();
            Image imageComponent = imageObject.GetComponent<Image>();
            imageComponent.sprite = allImages[imagesLoaded];
            imagesLoaded++;
        }
    }

    private GameObject GetImageObject()
    {
        if (imagePool.Count > 0)
        {
            return imagePool.Dequeue();
        }
        else
        {
            return Instantiate(imagePrefab, content);
        }
    }

    private void ReturnImageObject(GameObject imageObject)
    {
        imageObject.SetActive(false); // Hide it to remove from view
        imagePool.Enqueue(imageObject); // Add it back to the pool
    }

    private void OnScroll(Vector2 position)
    {
        // Check if we need to load more images when scrolling to the right
        if (position.x >= scrollThreshold)
        {
            LoadImages();
        }

        // Check if we need to unload images that are off-screen
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            RectTransform child = content.GetChild(i).GetComponent<RectTransform>();
            if (child.anchoredPosition.x < -content.rect.width / 2 ||
                child.anchoredPosition.x > content.rect.width / 2)
            {
                ReturnImageObject(child.gameObject);
            }
        }
    }
}