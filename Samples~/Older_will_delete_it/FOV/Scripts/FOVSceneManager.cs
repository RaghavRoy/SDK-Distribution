using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JioXSDK.FOVSample
{
    public class FOVSceneManager : MonoBehaviour
    {
        [SerializeField] private Canvas _uiCanvas;
        [SerializeField] private HandTracker _handTracker;

        private void Awake()
        {
            Camera cam = Camera.main;
            _uiCanvas.worldCamera = cam;

            Transform pointer = new GameObject("handPointer").transform;
            pointer.SetParent(cam.transform);
            pointer.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            pointer.localScale = Vector3.one;

            _handTracker.SetHandPointer = pointer;
        }
    }
}
