using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Hands;

namespace JioXSDK.Interactions
{
    public class HandInteractionEventsQCHT : HandInteractionEventsBase
    {
        protected override void MapInputActions()
        {
            var rightHandActionMap = inputActionAsset.FindActionMap("RightHand");
            var leftHandActionMap = inputActionAsset.FindActionMap("LeftHand");
            var rightHandInputActionMap = inputActionAsset.FindActionMap("RightHand Input");
            var leftHandInputActionMap = inputActionAsset.FindActionMap("LeftHand Input");

            // Get the actions
            onPinchRightHand = rightHandInputActionMap.FindAction("Select");
            pointerPositionRightHand = rightHandActionMap.FindAction("Device Position");
            pointerRotationRightHand = rightHandActionMap.FindAction("Device Rotation");
            onPinchLeftHand = leftHandInputActionMap.FindAction("Select");
            pointerPositionLeftHand = leftHandActionMap.FindAction("Device Position");
            pointerRotationLeftHand = leftHandActionMap.FindAction("Device Rotation");
        }
    }
}
