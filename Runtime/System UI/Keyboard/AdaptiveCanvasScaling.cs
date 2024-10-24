using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace JioXSDK
{
    public class AdaptiveCanvasScaling : MonoBehaviour
    {
        private Vector3 initialScale;
        private float initialDistance;

        private bool isScaling;

        private const float MaxDistance = 10f;
        //private const float MinDistance = 0.3f;

        private float maxScale = 0.01f;
        private float minScale = 0.0002f;
        private Vector3 initialPosition;
        private Coroutine scaleCoroutine;

        // [SerializeField] TMP_Text keyboardScale;

        // [SerializeField] TMP_Text scaleRegulatorText;

        // [SerializeField] TMP_Text distanceText;

        private void OnEnable() => Draggable.IsDragStarted += DragStatus;
        private void OnDisable() => Draggable.IsDragStarted -= DragStatus;

        private void Start()
        {
            initialScale = transform.localScale;

            initialDistance = Vector3.Distance(Camera.main.transform.position, transform.position);

            // scaleRegulatorText.text = minScale.ToString();

            // distanceText.text = initialDistance.ToString();

            // keyboardScale.text = initialScale.ToString("F7");

            scaleCoroutine = null;
        }

        private void DragStatus(bool val)
        {
            if (val)
            {
                StartScaling();
            }
            else
            {
                StopScaling();
            }
        }

        private IEnumerator AdjustCanvasScaleCoroutine()
        {
            while (isScaling)
            {
                AdjustCanvasScale();
                yield return null;
            }
        }

        [ContextMenu("StartScaling")]
        public void StartScaling()
        {
            initialPosition = transform.position;
            initialScale = transform.localScale;
            initialDistance = Vector3.Distance(Camera.main.transform.position, transform.position);
            Debug.Log(initialDistance);
            isScaling = true;

            if (scaleCoroutine == null)
            {
                scaleCoroutine = StartCoroutine(AdjustCanvasScaleCoroutine());
            }
        }


        public void StopScaling()
        {
            isScaling = false;
            if (scaleCoroutine != null)
            {
                StopCoroutine(AdjustCanvasScaleCoroutine());
                scaleCoroutine = null;
            }
        }

        public void IncrementScaleLimit()
        {
            minScale += 0.001f;
            //   scaleRegulatorText.text = minScale.ToString();
        }
        public void DecrementScaleLimit()
        {
            minScale -= 0.001f;
            //  scaleRegulatorText.text = minScale.ToString();
        }
        private void AdjustCanvasScale()
        {
            float currentDistance = Vector3.Distance(Camera.main.transform.position, transform.position);
            //  distanceText.text = currentDistance.ToString();
            Debug.Log(currentDistance);
            if (currentDistance >= MaxDistance)
            {
                Vector3 vect = initialPosition - transform.position;
                vect = vect.normalized;
                vect *= (currentDistance - MaxDistance);
                transform.position += vect;
            }
            // else if (currentDistance <= MinDistance)
            // {
            //     Vector3 vect = transform.position - initialPosition;  
            //     vect = vect.normalized;
            //     vect *= (MinDistance - currentDistance);              
            //     transform.position -= vect;
            //}

            // Calculate scale factor based on distance
            float scaleFactor = currentDistance / initialDistance;
            Vector3 currentScale = initialScale * scaleFactor;

            // Clamp the scale and manage the object’s visibility
            if (currentScale.z < minScale)
            {
                transform.localScale = new Vector3(minScale, minScale, minScale);
                Debug.Log(transform.localScale);
            }
            else
            {
                float clampedX = Mathf.Clamp(currentScale.x, minScale, maxScale);
                float clampedY = Mathf.Clamp(currentScale.y, minScale, maxScale);
                float clampedZ = Mathf.Clamp(currentScale.z, minScale, maxScale);

                transform.localScale = new Vector3(clampedX, clampedY, clampedZ);
                Debug.Log(transform.localScale);

            }
            // keyboardScale.text = transform.localScale.ToString("F7");
        }
    }
}