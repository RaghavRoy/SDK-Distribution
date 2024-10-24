using System;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace JioXSDK.FOVSample
{
    [Serializable]
    public class HandTrackingProcessor
    {
        [SerializeField] private bool _isDebug;
        [SerializeField] private Handedness _hand;
        [SerializeField] private HandTrackingState _trackingStates = HandTrackingState.Lost;
        [SerializeField] private Vector3 _currentPosition = Vector3.zero;
        [SerializeField] private Vector3 _prevPosition = Vector3.zero;

        public Action HandTrackingLost;
        public Action HandTrackingAcquired;

        public HandTrackingProcessor(Handedness hand, bool isDebug)
        {
            _hand = hand;
            _isDebug = isDebug;
        }

        public void ProcessHandPositions(Vector3 positions)
        {
            if (_trackingStates == HandTrackingState.Lost && positions.magnitude > 0)
            {
                _prevPosition = _currentPosition;
                _currentPosition = positions;
                _trackingStates = HandTrackingState.Acquired;
                HandTrackingAcquired?.Invoke();
                if (_isDebug)
                    Debug.Log($"{_hand} Tracking Acquired.");

                return;
            }

            if (_trackingStates == HandTrackingState.Acquired && positions.magnitude > 0)
            {
                _prevPosition = _currentPosition;
                _currentPosition = positions;
                _trackingStates = HandTrackingState.Tracking;
                if (_isDebug)
                    Debug.Log($"{_hand} Start Tracking.");

                return;
            }

            if (_trackingStates == HandTrackingState.Tracking && positions.magnitude == 0)
            {
                _prevPosition = _currentPosition;
                _currentPosition = positions;
                _trackingStates = HandTrackingState.Lost;
                HandTrackingLost?.Invoke();

                if (_isDebug)
                    Debug.Log($"{_hand} Tracking Lost.");

                return;
            }

            else if (_trackingStates == HandTrackingState.Tracking)
            {
                _prevPosition = _currentPosition;
                _currentPosition = positions;

                if (_isDebug)
                    Debug.Log($"{_hand} is being Tracked");
            }
        }

        public Vector3 CurrentPosition => _currentPosition;
        public Vector3 PreviousPosition => _prevPosition;
        public HandTrackingState TrackingState => _trackingStates;
    }
}