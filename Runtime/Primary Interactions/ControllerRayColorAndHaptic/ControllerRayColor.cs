using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;


namespace JioXSDK
{
    public class ControllerRayColor : MonoBehaviour
    {
        [SerializeField] private ActionBasedController controllerAction;
        [SerializeField] private XRInteractorLineVisual interactorLineVisual;

        [SerializeField] private Gradient defaultColor;
        [SerializeField] private Gradient selectedColor;

        private void OnEnable()
        {
            controllerAction.activateActionValue.action.started += Started;
            //controllerAction.activateActionValue.action.performed += Performed;
            controllerAction.activateActionValue.action.canceled += Canceled;
        }

        private void OnDisable()
        {
            controllerAction.activateActionValue.action.started -= Started;
            //controllerAction.activateActionValue.action.performed -= Performed;
            controllerAction.activateActionValue.action.canceled -= Canceled;
        }

        private void Canceled(InputAction.CallbackContext context)
        {
            interactorLineVisual.validColorGradient = defaultColor;
        }

        private void Started(InputAction.CallbackContext context)
        {
            //Debug.Log("JJ Started:");
            //Debug.Log("JJ" + context.ToString());
            interactorLineVisual.validColorGradient = selectedColor;
            controllerAction.SendHapticImpulse(0.1f, 0.4f);
        }

        private void Performed(InputAction.CallbackContext context)
        {
            Debug.Log("JJ Performed:");
            Debug.Log("JJ" + context.ToString());
        }
    }
}
