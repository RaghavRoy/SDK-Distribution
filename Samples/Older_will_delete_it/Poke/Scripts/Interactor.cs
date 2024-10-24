using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace JioXSDK
{
    public class Interactor : Selectable
    {
        private enum DoubleEventType
        {
            singleEvent,
            doubleEvent
        }
        [SerializeField] private float longClickTimer = 1.5f;
        [SerializeField] private float doubleClickTimer = 0.5f;
        [SerializeField] TMPro.TextMeshProUGUI statusTxt;
        [SerializeField] private DoubleEventType eventType = DoubleEventType.singleEvent;


        public UnityEvent<Interactor> OnSingleClick;
        public UnityEvent OnDoubleClick;
        public UnityEvent OnLongClick;
        public Action<float> OnLongElapsedTime;
        public Action<float> OnDoubleElapsedTime;

        private Coroutine longClickRoutine;
        private Coroutine doubleClickRoutine;
        private bool isLongPokePerformed;
        private bool isDoublePokeStarted;
        private bool canPerformSingleClick = true;
        private bool isClicking;
        private int clickCount = 0;

        public float DoubleClickTimer {set {doubleClickTimer = value;} get{return doubleClickTimer;}}
        public float LongClickTimer {set {longClickTimer = value;} get{return longClickTimer;}}

        protected override void Start()
        {
            base.Start();
            if(OnDoubleClick == null)
            {
                doubleClickTimer = 0;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (!IsActive() || !IsInteractable())
                return;

            isClicking = true;
            canPerformSingleClick = true;
            statusTxt.text = (clickCount + 1) + " Poke Start";
            StartLongPokeTimer();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (!IsActive() || !IsInteractable())
                return;

            isClicking = false;
            StopLongPokeTimer();
            statusTxt.text = (clickCount + 1) + " Poke Released";
            if (!isLongPokePerformed)
            {
                if(isDoublePokeStarted)
                {
                    OnDoubleClick?.Invoke();
                    clickCount = 0;
                    isDoublePokeStarted = false;
                }
                else
                {
                    clickCount++;

                    if(clickCount > 1)
                    {
                        clickCount = 0;
                    }
                    if(eventType == DoubleEventType.doubleEvent)
                    {
                        OnSingleClick?.Invoke(this);
                    }
                    StartCoroutine(DoublePokeCoroutine());
                }
            }
            else
            {
                clickCount = 0;
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            if (!IsInteractable())
                return;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            if (!IsInteractable())
                return;

            //canPerformSingleClick = false;
            //StopLongPokeTimer();
        }

        private void StartLongPokeTimer()
        {
            if (longClickRoutine != null) StopCoroutine(longClickRoutine);
            if (longClickRoutine == null && OnLongClick != null)
                longClickRoutine = StartCoroutine(LongPokeCoroutine());
        }

        private void StopLongPokeTimer()
        {
            if (longClickRoutine != null)
            {
                StopCoroutine(longClickRoutine);
                longClickRoutine = null;
            }
        }

        private IEnumerator LongPokeCoroutine()
        {
            float elapsedTime = 0f;
            isLongPokePerformed = false;

            while (elapsedTime < longClickTimer)
            {
                elapsedTime += Time.unscaledDeltaTime;
                OnLongElapsedTime?.Invoke(elapsedTime);
                statusTxt.text = (clickCount + 1) + " Poke Hold";
                yield return null;
            }

            OnLongClick?.Invoke();
            isLongPokePerformed = true;
        }

        private IEnumerator DoublePokeCoroutine()
        {
            float elapsedTime = 0f;

            while (elapsedTime < doubleClickTimer)
            {
                if(isClicking)
                {
                    isDoublePokeStarted = true;
                    yield break;
                }
                elapsedTime += Time.unscaledDeltaTime;
                OnDoubleElapsedTime?.Invoke(elapsedTime);
                yield return null;
            }
            isDoublePokeStarted = false;
            if(!isDoublePokeStarted && eventType == DoubleEventType.singleEvent)
            {
                OnSingleClick?.Invoke(this);
                clickCount = 0;
            }
        }
    }
}
