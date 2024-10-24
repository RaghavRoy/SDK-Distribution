using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using TMPro;
using System.Text;
using UnityEngine.UI;
using UnityEngine.UIElements;
using JioXSDK.Interactions;
namespace JioXSDK
{
    public class ScrollFeature : MonoBehaviour
    {
        // public InputActionProperty _rightHandScroll;
        // Vector2 handscroll;
        // private bool _isScrolling;

        // [SerializeField]
        // public TMP_Text tMP_handText;
        // StringBuilder str;

        // [SerializeField]
        // Scrollbar scrollbar;

        // float timer = 0;

        // public TMP_Text tMP_start;
        // public TMP_Text tMP_end;


        // // Start is called before the first frame update
        // void Start()
        // {
        //     handscroll = _rightHandScroll.action.ReadValue<Vector2>();

        // }
        // // Update is called once per frame
        // void Update()
        // {
        //     handscroll = _rightHandScroll.action.ReadValue<Vector2>();


        //     if (_isScrolling)
        //     {
        //         timer += Time.deltaTime;
        //         tMP_end.text = timer.ToString();
        //         //handscroll = _rightHandScroll.action.ReadValue<Vector2>();
        //         //  tMP_handText.text = "HandScroll Y :" + handscroll.y.ToString() + "HandScroll X :" + handscroll.x.ToString();
        //         // if (handscroll.y > 0)
        //         // {
        //         //     scrollbar.value = handscroll.y;

        //         // }
        //     }
        // }

        // public void OnPointerUp(PointerEventData eventData)
        // {
        //     _isScrolling = true;
        // }

        // public void OnPointerDown(PointerEventData eventData)
        // {
        //     _isScrolling = false;
        // }

        // public void OnSelect()
        // {
        //     // Debug.Log("Hovered");
        //     tMP_handText.text = "Select start";
        //     _isScrolling = true;
        //     tMP_start.text = timer.ToString();
        // }
        // public void OnSelectExit()
        // {
        //     // Debug.Log("Hovered");
        //     tMP_handText.text = "Select End";

        //     _isScrolling = false;
        //     timer = 0f;
        // }

        // public void OnPointerEnter(PointerEventData eventData)
        // {
        //     OnHover();
        // }

        bool isScrollSelect = false;
        [SerializeField]
        private TMP_Text pinchdeltaposition;
        [SerializeField]
        private TMP_Text pinchnormalizedposition;

        [SerializeField]
        private TMP_Text scrollBarvalue;

        private Vector3 initialpinchedpos = Vector3.zero;

        private float scroller = 1;

        private float controlVariable = 1;
        [SerializeField]
        private Scrollbar scrollbar;
        private void OnEnable()
        {
            HandInteractionEvents.OnRightHandPinch += OnPinch;
            HandInteractionEvents.OnRightHandPinchRelease += OnPinchRelease;
        }
        private void OnDisable()
        {
            HandInteractionEvents.OnRightHandPinch -= OnPinch;
            HandInteractionEvents.OnRightHandPinchRelease -= OnPinchRelease;
        }
        public void OnPinch()
        {
            initialpinchedpos = HandInteractionEvents.RightHandPosition;
            isScrollSelect = true;
        }
        public void OnPinchRelease()
        {
            isScrollSelect = false;
            controlVariable = scrollbar.value;
        }
        private void Update()
        {
            if (isScrollSelect)
            {
                pinchdeltaposition.text = "delta " + (HandInteractionEvents.RightHandPosition - initialpinchedpos).ToString();
                pinchnormalizedposition.text = "normalized " + (HandInteractionEvents.RightHandPosition - initialpinchedpos).normalized.ToString();
                if ((HandInteractionEvents.RightHandPosition - initialpinchedpos).normalized.y > 0)
                {
                    scrollbar.value += (HandInteractionEvents.RightHandPosition - initialpinchedpos).y;
                    scrollBarvalue.text = scrollbar.value.ToString();
                }
                else if ((HandInteractionEvents.RightHandPosition - initialpinchedpos).normalized.y < 0)
                {
                    scrollbar.value += (HandInteractionEvents.RightHandPosition - initialpinchedpos).y;
                    scrollBarvalue.text = scrollbar.value.ToString();
                }


                initialpinchedpos = HandInteractionEvents.RightHandPosition;
            }
        }


    }
}
