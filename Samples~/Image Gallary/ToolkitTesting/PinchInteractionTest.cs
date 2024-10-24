using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JioXSDK
{
    public class PinchInteractionTest : MonoBehaviour
    {

        [SerializeField] private Button _btnReset;
        [SerializeField] private bool _enableSinglePinch = true;
        [SerializeField] private TextMeshProUGUI _demoText;
        [SerializeField] private ExtendedClick _pinchInteractions;

        private float dummyCounter = 0;

        private void Awake()
        {
            _btnReset.onClick.AddListener(ResetTest);
            ResetTest();

            if (_enableSinglePinch)
                _pinchInteractions.OnClickPerformed.AddListener(OnPinchPerformed);

            _pinchInteractions.OnDoubleClick.AddListener(OnDoublePinch);
            _pinchInteractions.OnLongClickPerformed.AddListener(OnLongPinchInvoked);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ResetTest();
            }
        }

        public void ResetTest()
        {
            _demoText.text = "---------------";
            dummyCounter = 0;
        }

        private void OnLongPinchInvoked()
        {
            _demoText.text = $"Long Pinch{dummyCounter}";
            dummyCounter += 1f;
        }

        private void OnPinchPerformed()
        {
            _demoText.text = "Pinch performed";
        }

        private void OnDoublePinch()
        {
            _demoText.text = "Double Pinch Performed";
        }
    }
}
