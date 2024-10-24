using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Samples.GestureSample;
using JioXSDK.Utils;
using UnityEngine.UI;

namespace JioXSDK.HandTracking
{
    public class HandStateLogDisplay : MonoBehaviour
    {
        [SerializeField] private Transform _logScrollContent;
        [SerializeField] private LogListItem _scrollListItem;
        [SerializeField] private GesturePair[] _gestures;

        [SerializeField] private Button _btnSave;
        [SerializeField] private SkinSwitchingHandler _skinSwitcher;

        private string _selectedHands = "NA";

        private void Awake()
        {
            foreach (var pair in _gestures)
            {
                pair.Subscribe(AddLogOfHandState);
            }

            _btnSave.onClick.AddListener(() => CSVHandler.SaveCSV("HandTracking"));
            CSVHandler.SetHeaderData("Selected Hand", "Hand State", "Time Elapsed");
            _skinSwitcher.HandSkinSwitched += OnHandSkinSwitched;
        }

        private void OnHandSkinSwitched(string handSkinName)
        {
            _selectedHands = handSkinName;
        }

        private void AddLogOfHandState(Handedness hand, string state, string time)
        {
            LogListItem listItem = Instantiate(_scrollListItem, _logScrollContent);
            listItem.transform.localScale = Vector3.one;
            listItem.transform.localPosition = Vector3.zero;
            listItem.transform.SetAsFirstSibling();

            LogData data = new()
            {
                hand = _selectedHands,
                handState = state,
                timeElapsed = time
            };

            listItem.SetData(data);
            CSVHandler.AddLineData(data.hand, data.handState, data.timeElapsed);
        }
    }

    [Serializable]
    internal class GesturePair
    {
        [SerializeField] private string _name;
        [SerializeField] private StaticHandGesture _leftHand;
        [SerializeField] private StaticHandGesture _rightHand;


        private Action<Handedness, string, string> _detectionEvent;
        private Stack<DateTime> _rightStateStack;
        private Stack<DateTime> _leftStateStack;

        private bool _init;

        public void Subscribe(Action<Handedness, string, string> detectionEvent)
        {
            if (!_init)
                Init();

            _detectionEvent = detectionEvent;
        }

        private void Init()
        {

            _rightStateStack = new Stack<DateTime>();
            _leftStateStack = new Stack<DateTime>();

            _leftHand.gesturePerformed.AddListener(() => OnGestureDetected(Handedness.Left));
            _rightHand.gesturePerformed.AddListener(() => OnGestureDetected(Handedness.Right));

            _leftHand.gestureEnded.AddListener(() => OnGestureLost(Handedness.Left));
            _rightHand.gestureEnded.AddListener(() => OnGestureLost(Handedness.Right));
        }

        private void OnGestureDetected(Handedness hand)
        {
            DateTime now = DateTime.Now;
            (hand == Handedness.Left ? _leftStateStack : _rightStateStack).Push(now);
            _detectionEvent?.Invoke(hand, $"{hand} Start: {_name}", "00:00.000");
        }

        private void OnGestureLost(Handedness hand)
        {
            DateTime now = DateTime.Now;
            Stack<DateTime> stack = hand == Handedness.Left ? _leftStateStack : _rightStateStack;

            if (stack.Count < 0)
            {
                Debug.LogError($"There are not start method for the gesture {hand} hand {_name}");
                return;
            }

            var startTime = stack.Pop();
            TimeSpan span = now - startTime;
            string time = $"{span.Minutes}:{span.Seconds}.{span.Milliseconds}";

            _detectionEvent?.Invoke(hand, $"{hand} End: {_name}", time);
        }
    }
}
