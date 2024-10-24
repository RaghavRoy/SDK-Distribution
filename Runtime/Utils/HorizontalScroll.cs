using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JioXSDK
{
    public class HorizontalScroll : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private float scrollSpeed = 15000f;
        
        private bool isScrollingLeft = false;

     
        public void MoveScroll(Vector3 diff)
        {
            if (diff.x < 0) isScrollingLeft = true;
            else if (diff.x > 0) isScrollingLeft = false;
            
            if (isScrollingLeft) scrollRect.velocity = new Vector2(diff.x * scrollSpeed * Time.deltaTime, 0);
            else scrollRect.velocity = new Vector2(diff.x * scrollSpeed * Time.deltaTime, 0f);
        }
    }
}
