using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JioXSDK
{
    public class LongPinch : Selectable
    {
        [SerializeField] private bool continuous = false;
        [SerializeField] private Color longClickColor = Color.white;
        [SerializeField] private float longClickTimer = 1.5f;

        public UnityEvent OnEnter;
        public UnityEvent OnExit;
        public UnityEvent OnSingleClick;
        public UnityEvent OnPotentialClickStarted;
        public UnityEvent<float> OnClickHeld;
        public UnityEvent OnLongClick;

        private Coroutine longClickRoutine;
        private bool isLongClickPerformed;
        private bool canPerformSingleClick = true;
        private bool isClicking;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (!IsActive() || !IsInteractable())
                return;

            isClicking = true;
            canPerformSingleClick = true;
            OnPotentialClickStarted?.Invoke();
            StartLongClickTimer();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (!IsActive() || !IsInteractable())
                return;

            isClicking = false;
            StopLongClickTimer();

            if (!isLongClickPerformed && canPerformSingleClick)
                OnSingleClick?.Invoke();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            if (!IsInteractable())
                return;
           
            if (!continuous)
                StopLongClickTimer();
            else
                if(isClicking) StartLongClickTimer();
            
            OnEnter?.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            if (!IsInteractable())
                return;

            canPerformSingleClick = false;
            StopLongClickTimer();

            OnExit?.Invoke();
        }

        private void StartLongClickTimer()
        {
            if (longClickRoutine != null) StopCoroutine(longClickRoutine);
            if (longClickRoutine == null)
                longClickRoutine = StartCoroutine(LongClickCoroutine());
        }

        private void StopLongClickTimer()
        {
            if (longClickRoutine != null)
            {
                StopCoroutine(longClickRoutine);
                longClickRoutine = null;
            }
        }

        private IEnumerator LongClickCoroutine()
        {
            float elapsedTime = 0f;
            isLongClickPerformed = false;

            while (elapsedTime < longClickTimer)
            {
                OnClickHeld?.Invoke(elapsedTime);
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            OnLongClick?.Invoke();
            isLongClickPerformed = true;
            ApplyLongClickColorEffect();
        }

        private void ApplyLongClickColorEffect()
        {
            if (targetGraphic != null)
                targetGraphic.CrossFadeColor(longClickColor, ColorBlock.defaultColorBlock.fadeDuration, true, true);
        }
    }
}