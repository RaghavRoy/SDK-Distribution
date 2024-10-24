using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace JioXSDK
{
    public class Popup : MonoBehaviour
    {

        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private float duration = 1f;

        [SerializeField] private TextMeshProUGUI _message;

        [SerializeField] private bool _useButtons = true;
        [SerializeField] private InputActionProperty _positiveAction, _negativeAction;
        [SerializeField] private Button _positiveBtn, _negativeBtn;

        Action _positiveCallback, _negativeCallback;

        private void Awake()
        {
            if (!_useButtons)
            {
                _positiveAction.action.performed += OnPositiveButtonClicked;
                _negativeAction.action.performed += OnNegativeButtonClicked;
            }
            else
            {
                _positiveBtn.onClick.AddListener(OnPositiveButtonClicked);
                _negativeBtn.onClick.AddListener(OnNegativeButtonClicked);
            }
        }

        private void OnEnable()
        {
            StartAnimation();
        }

        public void Open(string message, Action positiveCallback, Action negativeCallback = null)
        {
            gameObject.SetActive(true);


            _message.text = message;
            _positiveCallback = positiveCallback;
            _negativeCallback = negativeCallback;
        }

        [ContextMenu("Animate")]
        private void StartAnimation() => StartCoroutine(AnimateScale(duration, _curve));

        private void OnNegativeButtonClicked(InputAction.CallbackContext context)
        {
            OnNegativeButtonClicked();
        }

        private void OnPositiveButtonClicked(InputAction.CallbackContext context)
        {
            OnPositiveButtonClicked();
        }

        private void OnPositiveButtonClicked()
        {
            HideAndReset();
            _positiveCallback?.Invoke();
        }

        private void OnNegativeButtonClicked()
        {
            HideAndReset();
            _negativeCallback?.Invoke();
        }

        private void HideAndReset()
        {
            gameObject.SetActive(false);
            _message.text = "";
        }
        private IEnumerator AnimateScale(float duration, AnimationCurve curve)
        {
            Vector3 startScale = Vector3.zero;
            Vector3 endScale = transform.localScale;

            Transform target = transform;
            var wait = new WaitForEndOfFrame();
            float time = 0;

            while (time < duration)
            {
                float t = time / duration;
                target.localScale = Vector3.LerpUnclamped(startScale, endScale, curve.Evaluate(t));
                time += Time.deltaTime;
                yield return wait;
            }
            target.localScale = Vector3.Lerp(startScale, endScale, 1);
        }
    }
}
