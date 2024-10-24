using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace JioXSDK
{
    public class ScrollController : MonoBehaviour
    {
        private HorizontalScroll horizontalScroll;

        private bool isScrolling;

        private bool isVerticalScrolling;
        private bool isHorizontalScrolling;

        private Vector3 handPosition = Vector3.zero;
        private Vector3 lastPalmPostion = Vector3.zero;

        public static Action<Vector3> verticalScrollEvent;

        public bool IsScrolling => isScrolling;

        private bool allowHorizontalScroll = true;

        public void ToggleAllowHorizontalScroll(bool flag)
        {
            allowHorizontalScroll = flag;
        }
        private void PinchInputPerformed()
        {
            ResetData();
        }

        public void InitializeScroll(Vector3 currentPos, HorizontalScroll hScrollRect = null)
        {
            Debug.Log($"InitializeScroll {currentPos}, {hScrollRect != null}");
            PinchInputPerformed();
            handPosition = currentPos;
            lastPalmPostion = handPosition;
            if (hScrollRect) horizontalScroll = hScrollRect;
        }

        public bool CheckAndScroll(Vector3 currentPos)
        {
            handPosition = currentPos;
            SetScrollDirection();
            return isScrolling;
        }

        private void SetScrollDirection()
        {
            Debug.Log($"SetScrollDirection :: {isScrolling} :: {handPosition.y} : {lastPalmPostion.y}");
            if ((handPosition.y > lastPalmPostion.y + 0.05f || handPosition.y < lastPalmPostion.y - 0.05f) && !isScrolling)
            {
                isScrolling = true;
                isVerticalScrolling = true;
                lastPalmPostion = handPosition;
            }

            if ((handPosition.x > lastPalmPostion.x + 0.05f || handPosition.x < lastPalmPostion.x - 0.05f) && !isScrolling)
            {
                isScrolling = true;
                isHorizontalScrolling = true;
                lastPalmPostion = handPosition;
            }

            if (!isScrolling) return;

            var diff = GetDifference();
            DoScroll(diff);
        }

        private void DoScroll(Vector3 diff)
        {
            Debug.Log("isVerticalScrolling");
            if (isVerticalScrolling) verticalScrollEvent?.Invoke(diff);
            if (!allowHorizontalScroll) { return; }
            if (isHorizontalScrolling && horizontalScroll) horizontalScroll.MoveScroll(diff);
        }

        private Vector3 GetDifference()
        {
            var newPalmPosition = handPosition;
            var diff = newPalmPosition - lastPalmPostion;

            lastPalmPostion = newPalmPosition;
            return diff;
        }


        private void ResetData()
        {
            isScrolling = false;
            isVerticalScrolling = false;
            isHorizontalScrolling = false;
            lastPalmPostion = Vector3.zero;

            horizontalScroll = null;
        }
    }
}
