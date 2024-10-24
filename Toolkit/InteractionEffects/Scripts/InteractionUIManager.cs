using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace JioXSDK
{
    public class InteractionUIManager : MonoBehaviour
    {
        [SerializeField]private List<InteractionUIState> interactionUIObjs;
        [SerializeField]private TextMeshProUGUI buttonText;

        private bool isPoke = false;

        public void ToggleInteraction()
        {
            if(isPoke)
            {
                isPoke = false;
                buttonText.text = "Gaze";
            }
            else
            {
                isPoke = true;
                buttonText.text = "Poke";
            }
            foreach(InteractionUIState obj in interactionUIObjs)
            {
                obj.isPokeEnabled = isPoke;
            }
        }
    }
}
