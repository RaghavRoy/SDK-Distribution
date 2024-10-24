using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
namespace JioXSDK
{
    public class KeyStringcharbutton : KeyStringbutton, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private TMP_Text character;
        public void SetMajor(bool major)
        {
            if (character != null)
            {
                if (major)
                {
                    character.text = character.text.ToUpper();
                }
                else
                {
                    character.text = character.text.ToLower();
                }
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
        }
    }
}
