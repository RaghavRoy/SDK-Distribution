using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace JioXSDK
{
    public class LongPokeTest : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI longPokeStateText;
      
        public void OnPotentialLongPokeStarted()
        {
            longPokeStateText.SetText("Potential long Poke started");
        }

        public void OnLongPokeEnter()
        {
            longPokeStateText.SetText("Long Poke Entered");
        }

        public void OnLongPokeExit()
        {
            longPokeStateText.SetText("Long Poke Exit");
        }

        public void OnSingleClickPerformed()
        {
            longPokeStateText.SetText(" Poke Release performed");
        }

        public void OnLongPokePerformed()
        {
            longPokeStateText.SetText("Long Poke performed");
        }

        public void OnLongPokeHeld(float elapsedTime)
        {
            longPokeStateText.SetText($"Potential long Poke started {elapsedTime.ToString("0.00")}s");
        }
          public void OnLongPokeRelease()
        {
            longPokeStateText.SetText("Long Poke Release");
        }

    }
}
