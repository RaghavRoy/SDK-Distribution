using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

namespace JioXSDK
{
    public class PokeCustomization : MonoBehaviour
    {
        private XRRayInteractor xRRayInteractor;

        [SerializeField]
        public TMP_Text[] hover; 

        [SerializeField]
        public TMP_Text[] distance; 

         [SerializeField]
        public TMP_Text pokeselect ; 

         [SerializeField]
        public TMP_Text pokesecondselect ; 

         [SerializeField]
         public TMP_Text onclickButtonOne;
           [SerializeField]
         public TMP_Text onclickButtonTwo;


        public void IncreaseRay()
        {
            if(xRRayInteractor.maxRaycastDistance >= 0.1f && xRRayInteractor.maxRaycastDistance <= 0.9f){
                xRRayInteractor.maxRaycastDistance += 0.1f;
            }
        }
        public void DecreaseRay()
        {
            if(xRRayInteractor.maxRaycastDistance >= 0.9f && xRRayInteractor.maxRaycastDistance <= 0.1f){
                xRRayInteractor.maxRaycastDistance -= 0.1f;
            }
        }
        public void OnHoverEnter(){
            hover[0].text = "poke hover";
            
        }
        public void OnHowerExit(){
            hover[0].text = "poke hover exit..";
             
        }
         public void PointerDown(){
            pokeselect.text = "poke start";
        }

        public void PointerUp(){
           pokeselect.text = "poke Release..";
        }
         public void howerSecondEnter(){
            hover[1].text = "poke hover enter";
            
           
        }
        public void howerSecondEnterExit(){
            hover[1].text = "poke hover exit..";
           
        }
         public void PointerSecondDown(){
            pokesecondselect.text = "poke start..";
        }
        public void PointerSecondUp(){
            pokesecondselect.text = "poke Release..";
        }
        public void OnClickButtonPlus(){
                onclickButtonOne.text = "Button one Click Register";
        }
          public void OnClickButtonminus(){
                onclickButtonTwo.text = "Button Two Click Register";
        }

    }
}
