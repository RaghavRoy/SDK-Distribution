using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Hands;

namespace JioXSDK.Interactions
{
    public class HandInteractionEventsOpenXR : HandInteractionEventsBase
    {
        protected override void MapInputActions()
        {
            // Get the action maps
            var rightHandActionMap = inputActionAsset.FindActionMap("XRI RightHand");
            var leftHandActionMap = inputActionAsset.FindActionMap("XRI LeftHand");
            var rightHandInputActionMap = inputActionAsset.FindActionMap("XRI RightHand Interaction");
            var leftHandInputActionMap = inputActionAsset.FindActionMap("XRI LeftHand Interaction");

            // Get the actions
            onPinchRightHand = rightHandInputActionMap.FindAction("Select");
            pointerPositionRightHand = rightHandActionMap.FindAction("Position");
            pointerRotationRightHand = rightHandActionMap.FindAction("Rotation");
            onPinchLeftHand = leftHandInputActionMap.FindAction("Select");
            pointerPositionLeftHand = leftHandActionMap.FindAction("Position");
            pointerRotationLeftHand = leftHandActionMap.FindAction("Rotation");
        }
    }
}
