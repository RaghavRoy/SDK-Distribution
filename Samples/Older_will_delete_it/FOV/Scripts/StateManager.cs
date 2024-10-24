using System;
using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;

namespace JioXSDK.FOVSample
{
    public class StateManager : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _txtTimerText;
        public Action OnProcessStart;
        public Action OnProcessStop;

        private Stopwatch _stopwatch;

        private void Start()
        {
            StartCoroutine(TimerTick("Process Start In: ", 10, () =>
            {
                OnProcessStart?.Invoke();
            }));
        }

        public void StartProcessStopTimer(float duration, string message, Action completeCallback)
        {
            StartCoroutine(TimerTick(message, duration, () =>
            {
                completeCallback?.Invoke();
                // OnProcessStop?.Invoke();
            }));
        }

        private IEnumerator TimerTick(string text, float duration, Action callback)
        {
            _stopwatch ??= new Stopwatch();


            var wait = new WaitForEndOfFrame();
            _stopwatch.Reset();
            _stopwatch.Start();

            _txtTimerText.text = "";
            while (_stopwatch.Elapsed.TotalSeconds <= duration)
            {
                _txtTimerText.text = $"{text}{(duration - _stopwatch.Elapsed.TotalSeconds).ToString("00.0")}s";
                yield return wait;
            }

            callback?.Invoke();
            _txtTimerText.text = "";
        }
    }
}
