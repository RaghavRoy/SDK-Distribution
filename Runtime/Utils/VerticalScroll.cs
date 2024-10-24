using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JioXSDK
{

    public class VerticalScroll : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private float verticalScrollSpeed = 20000f;

        [SerializeField] private bool resetPositionOnDisable = false;

        private bool isScrollingDown = false;

        public static Action LoadMoreData;

        private void OnEnable()
        {
            //Debug.Log("moving scroll...");

            ScrollController.verticalScrollEvent += VerticalScrollEvent;
        }

        private void OnDisable()
        {
            if (resetPositionOnDisable) { scrollRect.verticalNormalizedPosition = 1.0f; }
            ScrollController.verticalScrollEvent -= VerticalScrollEvent;
        }

        private void VerticalScrollEvent(Vector3 diff)
        {
            MoveScroll(diff);
        }

        void Update()
        {
            if (scrollRect.normalizedPosition.y < 0.3f) LoadMoreData?.Invoke();
        }

        private void MoveScroll(Vector3 diff)
        {

            Debug.Log($"Scrolling");
            if (diff.y < 0) isScrollingDown = true;
            else if (diff.y > 0) isScrollingDown = false;

            if (isScrollingDown) scrollRect.velocity = new Vector2(0f, diff.y * verticalScrollSpeed * Time.deltaTime);
            else scrollRect.velocity = new Vector2(0f, diff.y * verticalScrollSpeed * Time.deltaTime);

            if (scrollRect.normalizedPosition.y < 0.3f) LoadMoreData?.Invoke();
        }
    }
}
