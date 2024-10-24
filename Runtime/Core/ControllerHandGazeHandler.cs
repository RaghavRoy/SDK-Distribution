using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.Hands;

namespace JioXSDK
{
    public class ControllerHandGazeHandler : MonoBehaviour
    {

        private const int HIDE_GAZE = 1;
        private const int SHOW_GAZE = -1;
        [SerializeField] private PokeGestureDetector[] _pokeDetectors;

        [SerializeField] private InputActionProperty _leftControllerTracking;
        [SerializeField] private InputActionProperty _rightControllerTracking;

        [Space]
        [SerializeField] private ActionBasedController _leftABController;
        [SerializeField] private ActionBasedController _rightABController;
        [SerializeField] private XRGazeInteractor _gazeInteractor;

        public Action<bool> gazeEnabled;
        public Action<bool> controllersEnabled;

        private int _gazeDisableRequest = 0;

        private void Awake()
        {
            SubscribeAllGazeHandler();

            _leftControllerTracking.action.Enable();
            _rightControllerTracking.action.Enable();

            _leftControllerTracking.action.performed += LeftControllerTrackingPerformed;
            _rightControllerTracking.action.performed += RightControllerTrackingPerformed;

            _leftControllerTracking.action.canceled += LeftControllerTrackingLost;
            _rightControllerTracking.action.canceled += RightControllerTrackingLost;


        }

        private void Start()
        {
            gazeEnabled?.Invoke(_gazeInteractor.gameObject.activeInHierarchy);

        }
        private void OnDestroy()
        {
            _leftControllerTracking.action.Disable();
            _rightControllerTracking.action.Disable();

            _leftControllerTracking.action.performed -= LeftControllerTrackingPerformed;
            _rightControllerTracking.action.performed -= RightControllerTrackingPerformed;

            _leftControllerTracking.action.canceled -= LeftControllerTrackingLost;
            _rightControllerTracking.action.canceled -= RightControllerTrackingLost;
        }

        private void RightControllerTrackingPerformed(InputAction.CallbackContext context)
        {
            _rightABController.gameObject.SetActive(true);
            controllersEnabled?.Invoke(true);
            EnableGazeController(false);
        }

        private void LeftControllerTrackingPerformed(InputAction.CallbackContext context)
        {
            _leftABController.gameObject.SetActive(true);
            EnableGazeController(false);
        }

        private void RightControllerTrackingLost(InputAction.CallbackContext context)
        {
            _rightABController.gameObject.SetActive(false);
            controllersEnabled?.Invoke(false);
            EnableGazeController(true);
        }

        private void LeftControllerTrackingLost(InputAction.CallbackContext context)
        {
            _leftABController.gameObject.SetActive(false);
            EnableGazeController(true);
        }

        private void EnableGazeController(bool state)
        {
            Debug.Log($"Hiding Request for Gaze with state: {state}");
            try
            {
                gazeEnabled?.Invoke(state);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            } 
            if (!state)
            {
                Debug.Log($"Hiding the Gaze controller.");
                _gazeInteractor.gameObject.SetActive(state);
                return;
            }

            bool controllerActive = _rightABController.gameObject.activeInHierarchy || _leftABController.gameObject.activeInHierarchy;
            Debug.Log($"Right Hand controller: {_rightABController.gameObject.activeInHierarchy} | Left Controller: {_leftABController.gameObject.activeInHierarchy} | Requested State: {state}");
            _gazeInteractor.gameObject.SetActive(!controllerActive && state);
        }

        private void SubscribeAllGazeHandler()
        {
            foreach (PokeGestureDetector pokeDetector in _pokeDetectors)
            {
                pokeDetector.PokeGestureStarted.AddListener(() => IncreaseGazeDisableRequest(HIDE_GAZE));
                pokeDetector.PokeGestureEnded.AddListener(() => IncreaseGazeDisableRequest(SHOW_GAZE));
            }
        }

        private void IncreaseGazeDisableRequest(int incrementAmount = HIDE_GAZE)
        {
            _gazeDisableRequest += incrementAmount;

            _gazeInteractor.gameObject.SetActive(_gazeDisableRequest <= 0);
        }
    }
}
