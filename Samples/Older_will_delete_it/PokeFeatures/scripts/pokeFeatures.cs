using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.XR.OpenXR.NativeTypes;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System;
using JetBrains.Annotations;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.XR.Interaction.Toolkit.UI;
using JioXSDK.Interactions;
using System.ComponentModel;
namespace JioXSDK
{
   [RequireComponent(typeof(XRRayInteractor))]
   [RequireComponent(typeof(XRInteractorLineVisual))]
   public class pokeFeatures : MonoBehaviour
    {
        private XRRayInteractor xRRayInteractor;
        private XRInteractorLineVisual xRInteractorLineVisual;
        private LineRenderer lineRenderer;    
        private float raydistance = 0.1f;
        private float minraydistance = 0.1f;
        private float maxraydistance = 0.9f;

        [SerializeField]
        private Material raymaterial;    

         [SerializeField]
        private Transform rayorigin;    

        [SerializeField]
         private Sprite endrectile;
         float reticleScaleValue = 0.009f;    

        //  [SerializeField]
        //  TMP_Text distance;

         EventTrigger buttonincrement;
         EventTrigger buttondecrement;

         bool hower = false;

        void Start(){

            xRRayInteractor = GetComponent<XRRayInteractor>();

            xRInteractorLineVisual =  GetComponent<XRInteractorLineVisual>();

            lineRenderer = GetComponent<LineRenderer>();

           
            rayorigin = this.gameObject.transform;

            lineRenderer.startWidth = 0.001f;

            lineRenderer.endWidth = 0.001f;

            lineRenderer.material = raymaterial;

            xRRayInteractor.rayOriginTransform = rayorigin;

            xRRayInteractor.maxRaycastDistance = Mathf.Clamp(raydistance, minraydistance, maxraydistance);

            GameObject childObject = new GameObject("ChildObject");

            childObject.transform.parent = transform;
            StartCoroutine(EnableGameobject(childObject));      

            SpriteRenderer spriteRenderer = childObject.AddComponent<SpriteRenderer>();
            
            childObject.transform.localPosition = Vector3.zero;

            childObject.transform.localScale = new Vector3(reticleScaleValue, reticleScaleValue, reticleScaleValue);

            spriteRenderer.sprite =  endrectile;

            xRInteractorLineVisual.reticle = childObject ;
          
        }
       
        IEnumerator EnableGameobject(GameObject gameObject){
          
            yield return new WaitForEndOfFrame();
            gameObject.SetActive(true);
           // distance = GameObject.FindGameObjectWithTag("distance").GetComponent<TMP_Text>();
          //  Debug.Log(distance.name);
        //    buttonincrement = GameObject.FindGameObjectWithTag("increment").GetComponent<EventTrigger>();
        //    buttondecrement =  GameObject.FindGameObjectWithTag("decrement").GetComponent<EventTrigger>();
            // if (buttonincrement != null)
            // {
            //     // Add event listeners or interact with the EventTrigger as needed
            //     buttonincrement.triggers.Add(CreateTriggerEntry(EventTriggerType.PointerClick, OnIncrementButtonClick));
            // }
            // if (buttondecrement != null)
            // {
            //     // Add event listeners or interact with the EventTrigger as needed
            //     buttondecrement.triggers.Add(CreateTriggerEntry(EventTriggerType.PointerClick, OnDecementButtonClick));
            // }
          
            
          //  buttondecrement.OnPointerDown.AddBinding()
            yield return null;
        }

        private EventTrigger.Entry CreateTriggerEntry(EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> callback)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventType;
            entry.callback.AddListener(callback);
            return entry;
        }

        //  private EventTrigger.Entry CreateTriggerEnter(EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> callback)
        // {
        //     EventTrigger.Entry entry = new EventTrigger.Entry();
        //     entry.eventID = eventType;
        //     entry.callback.AddListener(callback);
        //     return entry;
        // }
        private void OnDecementButtonClick(BaseEventData eventData)
        {
            xRRayInteractor.maxRaycastDistance -= 0.1f;
              xRInteractorLineVisual.lineLength -= 0.1f;
            // Implement your logic for incrementing here
        }
        private void OnIncrementButtonClick(BaseEventData eventData)
        {
            xRRayInteractor.maxRaycastDistance += 0.1f;
            xRInteractorLineVisual.lineLength += 0.1f;
            // Implement your logic for incrementing here
        }
         void Update(){
          // if(xRRayInteractor.rayEndTransform != null && distance != null ){
          //   if(Vector3.Distance(this.transform.position, xRRayInteractor.rayEndTransform.position) <= xRRayInteractor.maxRaycastDistance)
          //   {  
          //       distance.text = Vector3.Distance(this.transform.position, xRRayInteractor.rayEndTransform.position).ToString();
          //   }
          //}

        }


        // public void OnHoverEntered(HoverEnterEventArgs args){
        //     Debug.Log(args..transform.name);
        // }

    }

    
}
