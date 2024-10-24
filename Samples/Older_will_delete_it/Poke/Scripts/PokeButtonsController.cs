using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace JioXSDK
{
    public class PokeButtonsController : MonoBehaviour
    {
        [SerializeField] private Interactor[] interactors;
        [SerializeField] private TextMeshProUGUI longElapsedTimeTxt;
        [SerializeField] private TextMeshProUGUI doubleElapsedTimeTxt;

        [SerializeField] private TextMeshProUGUI interactionStatus;
        [SerializeField] private TextMeshProUGUI interactionStatus2;

        [SerializeField] private TextMeshProUGUI doublePokeTime;
        [SerializeField] private TextMeshProUGUI longPokeTime;

        private void Start()
        {
            for(int i = 0; i < interactors.Length; i++)
            {
                interactors[i].OnLongElapsedTime += OnLongElapsedTime;
                interactors[i].OnDoubleElapsedTime += OnDoubleElapsedTime;
            }

            doublePokeTime.text = interactors[0].DoubleClickTimer.ToString();
            longPokeTime.text = interactors[0].LongClickTimer.ToString();
        }

        private void OnDestroy() 
        {
            for(int i = 0; i < interactors.Length; i++)
            {
                interactors[i].OnLongElapsedTime -= OnLongElapsedTime;
                interactors[i].OnDoubleElapsedTime -= OnDoubleElapsedTime;
            }
        }

        public void SetDoublePokeTimer(float time)
        {
            for(int i = 0; i < interactors.Length; i++)
            {
                interactors[i].DoubleClickTimer += time;
            }
            doublePokeTime.text = interactors[0].DoubleClickTimer.ToString();
        }

        public void SetLongPokeTimer(float time)
        {
            for(int i = 0; i < interactors.Length; i++)
            {
                interactors[i].LongClickTimer += time;
            }
            longPokeTime.text = interactors[0].LongClickTimer.ToString();
        }

        private void OnLongElapsedTime(float time)
        {
            longElapsedTimeTxt.text = time.ToString();
        }

        private void OnDoubleElapsedTime(float time)
        {
            doubleElapsedTimeTxt.text = time.ToString();
        }

        public void OnSingleClick()
        {
            interactionStatus.text = "Single Click";
        }

        public void OnDoubleClick()
        {
            interactionStatus.text = "Double Click";
        }

        public void OnLongClick()
        {
            interactionStatus.text = "Long Click";
        }

        public void OnSingleClick2()
        {
            interactionStatus2.text = "Single Click";
        }

        public void OnDoubleClick2()
        {
            interactionStatus2.text = "Double Click";
        }

        public void OnLongClick2()
        {
            interactionStatus2.text = "Long Click";
        }
    }
}
