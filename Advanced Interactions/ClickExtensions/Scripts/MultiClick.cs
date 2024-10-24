using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace JioXSDK
{
    public class MultiClick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private InputActionProperty _inputMultipleClick;
        [SerializeField] private UnityEvent _click;
        public UnityEvent OnClick => _click;
        [SerializeField] private UnityEvent _doubleClick;
        public UnityEvent OnDoubleClick => _doubleClick;
        [SerializeField] private UnityEvent _longClick;
        public UnityEvent OnLongClick => _longClick;

        private bool _isHover = false;
        private bool _canClick = true; // Cooldown flag to prevent re-click on hover

        [SerializeField] private float _clickCooldown = 1.0f; // Cooldown period in seconds

        private void Awake()
        {
            if (_inputMultipleClick != null && _inputMultipleClick.action != null)
            {
                _inputMultipleClick.action.Enable();
            }
            else
            {
                Debug.LogError("Input Action not assigned or the action is null.");
            }
        }

        private void OnEnable()
        {
            if (_inputMultipleClick != null && _inputMultipleClick.action != null)
            {
                _inputMultipleClick.action.started += Started;
                _inputMultipleClick.action.performed += Performed;
                _inputMultipleClick.action.canceled += Canceled;
            }
            else
            {
                Debug.LogError("Input Action not assigned or action is null in OnEnable.");
            }
        }

        private void OnDisable()
        {
            if (_inputMultipleClick != null && _inputMultipleClick.action != null)
            {
                _inputMultipleClick.action.started -= Started;
                _inputMultipleClick.action.performed -= Performed;
                _inputMultipleClick.action.canceled -= Canceled;
            }
            _isHover = false;
        }

        private void OnDestroy()
        {
            if (_inputMultipleClick != null && _inputMultipleClick.action != null)
            {
                _inputMultipleClick.action.Disable();
            }
        }

        private void Started(InputAction.CallbackContext context)
        {
            if (context.interaction != null)
            {
                Debug.Log($"Started: {context.interaction.ToString()}");
            }
        }

        private void Canceled(InputAction.CallbackContext context)
        {
            if (context.interaction != null)
            {
                Debug.Log($"Canceled: {context.interaction.ToString()}");
            }
        }

        private void Performed(InputAction.CallbackContext context)
        {
            if (!_isHover || !_canClick)
            {
                Debug.Log("Pointer is not hovering or in cooldown. Ignoring click.");
                return;
            }

            // Handle multiple clicks (double click, triple, etc.)
            if (context.interaction is MultiTapInteraction interaction)
            {
                if (interaction.tapCount == 2)
                {
                    Debug.Log("Double click");
                    _doubleClick.Invoke();
                }
                return;
            }

            // Handle long press
            if (context.interaction is HoldInteraction)
            {
                Debug.Log("Long press");
                _longClick.Invoke();
                return;
            }

            // Handle single click
            if (context.interaction is TapInteraction)
            {
                Debug.Log("Single click");
                _click.Invoke();

                // Trigger the cooldown to prevent unwanted clicks on hover
                StartCooldown();
            }
        }

        private void StartCooldown()
        {
            _canClick = false;
            Invoke(nameof(ResetCooldown), _clickCooldown);
        }

        private void ResetCooldown()
        {
            _canClick = true;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHover = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHover = false;
        }
    }
}