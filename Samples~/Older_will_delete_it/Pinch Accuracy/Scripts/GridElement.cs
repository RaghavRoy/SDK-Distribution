using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridElement : MonoBehaviour
{
    [SerializeField] Gradient colorGradient;
    [SerializeField] TextMeshPro counterTxt;
    [SerializeField] private Renderer mainRenderer;
    private int clickedCount = 0;
    private Vector3 highLightedScale = new Vector3(1.5f, 1.5f, 1.5f);
    private IEnumerator ScaleUpCoroutine;
    private IEnumerator ScaleDownCoroutine;
    private float scaleSpeed = 10;

    public int ClickedCount
    {
        get{ return clickedCount; }
    }

    public void OnSelect()
    {
        clickedCount++;
        mainRenderer.material.color = colorGradient.Evaluate(clickedCount * .1f);
        // counterTxt.text = ((int)(clickedCount * 10)).ToString();
        counterTxt.text = clickedCount.ToString();
    }

    public void OnHover()
    {
        // if(ScaleDownCoroutine != null)
        // {
        //     StopCoroutine(ScaleDownCoroutine);
        // }
        // ScaleUpCoroutine = ScaleUP();
        // StartCoroutine(ScaleUpCoroutine);
        transform.localScale = Vector3.Lerp(transform.localScale, highLightedScale, Time.deltaTime * 10);
    }

    public void OnHoverExit()
    {
        // if(ScaleUpCoroutine != null)
        // {
        //     StopCoroutine(ScaleUpCoroutine);
        // }
        // ScaleDownCoroutine = ScaleDown();
        // StartCoroutine(ScaleDownCoroutine);
         transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 10);
    }

    public void Reset()
    {
        clickedCount = 0;
        mainRenderer.material.color = Color.red;
        counterTxt.text = "0";
    }

    private IEnumerator ScaleUP()
    {
        while(transform.localScale != highLightedScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, highLightedScale, Time.deltaTime * 10);
            yield return null;
        }
    }

    private IEnumerator ScaleDown()
    {
        while(transform.localScale != Vector3.one)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 10);
            yield return null;
        }
    }
}
