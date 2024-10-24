using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MainController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hover;
    [SerializeField] private TextMeshProUGUI threshold;
    private XRPokeInteractor[] pokeInteractors;

    [SerializeField] private TextMeshProUGUI buttonClicked;

    private int buttonClickedCount = 0;

    private void Awake() {
        pokeInteractors = FindObjectsOfType<XRPokeInteractor>();
    }

    private void OnDestroy() {
        
    }

    private void Start() {
        hover.text = pokeInteractors[0].pokeHoverRadius.ToString();
        threshold.text = pokeInteractors[0].pokeSelectWidth.ToString();
    }

    public void UpdateHoverValue(float num)
    {
        float total = float.Parse(hover.text) + num;

        for(int i = 0; i < pokeInteractors.Length; i++)
        {
            pokeInteractors[i].pokeHoverRadius = total;
        }

        hover.text = total.ToString();
    }

    public void UpdateThresholdValues(float num)
    {
        float total = float.Parse(threshold.text) + num;

        for(int i = 0; i < pokeInteractors.Length; i++)
        {
            pokeInteractors[i].pokeSelectWidth = total;
        }

        threshold.text = total.ToString();
    }

    public void UpdateButtonClicked()
    {
        buttonClickedCount++;
        buttonClicked.text = buttonClickedCount.ToString();
    }

    public void ResetButtonClicked()
    {
        buttonClickedCount = 0;
        buttonClicked.text = buttonClickedCount.ToString();
    }
}
