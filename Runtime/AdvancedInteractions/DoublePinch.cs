using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JioXSDK
{
    public class DoublePinch : Selectable, IPointerClickHandler
    {
        [SerializeField] private Color doubleClickColor = Color.white;
        [SerializeField] private float secondClickTimer = .5f;
        [SerializeField] private UnityEvent OnClick;
        [SerializeField] private UnityEvent OnDoubleClick;

        private bool canDoubleClick;
        private Coroutine timerRoutine;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
                return;

            if (!canDoubleClick)
            {
                timerRoutine = StartCoroutine(OnFirstClickPerformed());
                OnClick?.Invoke();
            }
            else
            {
                //targetGraphic.color = DoubleClickColor;
                StopCoroutine(timerRoutine);
                canDoubleClick = false;
                OnDoubleClick?.Invoke();
                StartColorTween(doubleClickColor, false);
            }
        }

        private IEnumerator OnFirstClickPerformed()
        {
            var elapsedTime = 0f;
            
            canDoubleClick = true;
            while (elapsedTime < secondClickTimer)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            canDoubleClick = false;
        }

        void StartColorTween(Color targetColor, bool instant)
        {
            if (targetGraphic == null)
                return;

            targetGraphic.CrossFadeColor(targetColor, instant ? 0f : ColorBlock.defaultColorBlock.fadeDuration, true, true);
        }
    }
}
