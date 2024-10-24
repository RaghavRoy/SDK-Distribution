using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace JioXSDK
{
    public class LongPokeFeature : Selectable
    {
        [SerializeField] private bool continuous = false;
        [SerializeField] private Color longClickColor = Color.white;
        [SerializeField] private float longClickTimer = 1.5f;

        [SerializeField] TMP_Text longpokeTimerText;
        public UnityEvent OnEnter;
        public UnityEvent OnExit;
        public UnityEvent OnSingleClick;
        public UnityEvent OnPotentialClickStarted;
        public UnityEvent<float> OnClickHeld;
        public UnityEvent OnLongClick;
        public UnityEvent OnLongClickContinue;

        public UnityEvent OnLongClickContinueBackward;

        private Coroutine longClickRoutine;
        private Coroutine longClickContinue;
        private Coroutine longClickContinuebackward;
        private bool isLongPokePerformed;
        private bool canPerformSingleClick = true;
        private bool isClicking;
        [SerializeField] TMP_Text elapsedtext;
        [SerializeField] TMP_Text longpokecontinuetext;
        private bool isLongClickForward = false;
        private bool isLongClickBackward = false;
        private static float continueElapsedTime = 0f;

        private void Awake()
        {
            longpokeTimerText.text = longClickTimer.ToString();
            continueElapsedTime = 0;
        }

        public void IncrementLongPokeTimer()
        {
            if (longClickTimer < 5)
            {
                longClickTimer++;
                longpokeTimerText.text = longClickTimer.ToString();
            }
        }
        public void DecrementLongPokeTimer()
        {
            if (longClickTimer > 1)
            {
                longClickTimer--;
                longpokeTimerText.text = longClickTimer.ToString();
            }
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (!IsActive() || !IsInteractable())
                return;

            isClicking = true;
            canPerformSingleClick = true;
            OnLongClickContinue?.Invoke();
            OnLongClickContinueBackward?.Invoke();
            OnPotentialClickStarted?.Invoke();
            StartLongPokeTimer();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (!IsActive() || !IsInteractable())
                return;

            isClicking = false;
            Debug.Log(isClicking);
            StopLongPokeTimer();

            if (!isLongPokePerformed || canPerformSingleClick)
                OnSingleClick?.Invoke();

            if (longClickContinue != null)
            {

                StopCoroutine(longClickContinue);
                longClickContinue = null;
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            if (!IsInteractable())
                return;

            if (!continuous)
                StopLongPokeTimer();
            else
                if (isClicking) StartLongPokeTimer();

            OnEnter?.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            if (!IsInteractable())
                return;

            canPerformSingleClick = false;
            StopLongPokeTimer();

            isLongClickForward = false;
            StopCoroutine(HandleLongClickContinueForward());
            isLongClickBackward = false;
            StopCoroutine(HandleLongClickContinueBackward());

            OnExit?.Invoke();
        }

        private void StartLongPokeTimer()
        {
            if (longClickRoutine != null) StopCoroutine(longClickRoutine);
            if (longClickRoutine == null)
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
                OnClickHeld?.Invoke(elapsedTime);
                elapsedTime += Time.unscaledDeltaTime;
                elapsedtext.text = elapsedTime.ToString();
                yield return null;
            }

            OnLongClick?.Invoke();
            isLongPokePerformed = true;
            ApplyLongPokeColorEffect();
        }

        private void ApplyLongPokeColorEffect()
        {
            if (targetGraphic != null)
            {
                targetGraphic.CrossFadeColor(longClickColor, ColorBlock.defaultColorBlock.fadeDuration, true, true);
            }
        }


        public void StartLongClickContinueForward()
        {
            isLongClickForward = true;
            StartCoroutine(HandleLongClickContinueForward());
        }

        public void StopLongClickContinueForward()
        {
            isLongClickForward = false;
            StopCoroutine(HandleLongClickContinueForward());
        }

        public void StartLongClickContinueBackward()
        {
            isLongClickBackward = true;
            StartCoroutine(HandleLongClickContinueBackward());
        }

        public void StopLongClickContinueBackward()
        {
            isLongClickBackward = false;
            StopCoroutine(HandleLongClickContinueBackward());
        }

        private IEnumerator HandleLongClickContinueBackward()
        {
            while (isLongClickBackward)
            {
                continueElapsedTime -= 1f;
                longpokecontinuetext.text = continueElapsedTime.ToString();
                yield return null;
            }
        }

        private IEnumerator HandleLongClickContinueForward()
        {
            while (isLongClickForward)
            {
                continueElapsedTime += 1;
                longpokecontinuetext.text = continueElapsedTime.ToString();
                yield return null;
            }
        }

    }
}
