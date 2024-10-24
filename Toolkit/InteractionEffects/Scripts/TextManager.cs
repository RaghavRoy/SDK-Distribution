//using UnityEngine;
//using TMPro;
//using System.Collections;


//public class TextManager : MonoBehaviour
//{
//    public TextMeshProUGUI initialMessage;
//    public TextMeshProUGUI clickMessage;
//    public TextMeshProUGUI imageInteractionMessage;

//    public GameObject imageViewer;
//    private void Start()
//    {
//        StartCoroutine(DisplayMessages());
//    }

//    private IEnumerator DisplayMessages()
//    {
//        // Show initial message
//        yield return StartCoroutine(FadeTextTo(initialMessage, 1, 0.5f));
//        yield return new WaitForSeconds(3);
//        yield return StartCoroutine(FadeTextTo(initialMessage, 0, 0.5f));

//        // Show click message
//        yield return StartCoroutine(FadeTextTo(clickMessage, 1, 0.5f));
//        yield return new WaitForSeconds(3);
//        yield return StartCoroutine(FadeTextTo(clickMessage, 0, 0.5f));
//    }

//    public void OnImageClick()
//    {
//        // Hide any currently visible messages
//        StartCoroutine(HideAllMessages());
//        // Display interaction message
//        StartCoroutine(DisplayImageInteractionMessage());
//    }

//    private IEnumerator DisplayImageInteractionMessage()
//    {
//        // Show interaction message
//        yield return StartCoroutine(FadeTextTo(imageInteractionMessage, 1, 0.5f));
//        yield return new WaitForSeconds(3);
//        yield return StartCoroutine(FadeTextTo(imageInteractionMessage, 0, 0.5f));
//    }

//    private IEnumerator FadeTextTo(TextMeshProUGUI text, float targetAlpha, float duration)
//    {
//        Color color = text.color;
//        float startAlpha = color.a;
//        float time = 0;

//        while (time < duration)
//        {
//            time += Time.deltaTime;
//            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
//            text.color = color;
//            yield return null; // Wait for the next frame
//        }

//        // Ensure the final alpha is set correctly
//        color.a = targetAlpha;
//        text.color = color;
//    }

//    private IEnumerator HideAllMessages()
//    {
//        // Hide all messages before displaying interaction message
//        yield return StartCoroutine(FadeTextTo(initialMessage, 0, 0.5f));
//        yield return StartCoroutine(FadeTextTo(clickMessage, 0, 0.5f));
//        imageViewer.SetActive(false);

//    }
//}