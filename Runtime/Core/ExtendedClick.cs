using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace JioXSDK
{

    public class ExtendedClick : Selectable, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private bool _enableClick = false;
        [Tooltip("Time within which the click action must be release to register the single tap. This also applies for the double click as well. In Seconds.")]
        [SerializeField] private float _pressTime = 0.5f;
        [SerializeField] private UnityEvent _clickPerformed;

        [SerializeField] private DoubleClickType _doubleClickType = DoubleClickType.None;
        [Tooltip("Max wait time for second click to be started, to register it as second click. In Seconds.")]
        [SerializeField] private float _maxSpaceTime = 0.65f;
        [SerializeField] private UnityEvent _doubleClickPerformed;

        [SerializeField] private LongClickType _longClickType = LongClickType.None;
        [Tooltip("Interval time In Seconds after which the continuous event will be Invoked.")]
        [SerializeField] private float _continuosInterval = 0.2f;
        [SerializeField] private UnityEvent _longClickPerformed;

        private Stopwatch _stopwatch, _longClickStopWatch;
        private bool _isLongClickPerformed = false;
        private int _clickCount = 0;
        private bool _clickStarted = false;
        private bool _isSecondClick = false;


        #region Unity Callbacks
        protected override void Awake()
        {
            _stopwatch = new Stopwatch();
            _longClickStopWatch = new Stopwatch();
        }

        private void Update()
        {
            if (_clickCount == 1 && StopwatchElapsedSeconds > _maxSpaceTime && !_clickStarted && !_isLongClickPerformed)
            {
                if (_doubleClickType == DoubleClickType.SingleTrigger)
                    _clickPerformed.Invoke();

                _clickCount = 0;

                _stopwatch?.Stop();
                _stopwatch?.Reset();

            }

            if (!_isLongClickPerformed && _clickStarted && StopwatchElapsedSeconds > _pressTime /* && _clickCount <= 0 */)
            {
                StartLongClick();
            }


            if (_isLongClickPerformed && _longClickType == LongClickType.Continuous && LongClickStopwatchElapsedSeconds > _continuosInterval)
            {
                _longClickPerformed.Invoke();
                _longClickStopWatch?.Restart();
            }
        }

        protected override void OnDisable()
        {
            CancelTheInteraction();
        }

        protected override void OnDestroy()
        {
            CancelTheInteraction();
        }

        #endregion

        #region Public Methods

        public override void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log($"{eventData.position} | {eventData.pressPosition}");
            base.OnPointerDown(eventData);
            OnInteractionStarted();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log($"{eventData.position} | {eventData.pressPosition} | {EventSystem.current.pixelDragThreshold}");
            base.OnPointerUp(eventData);

            if (Vector3.Distance(eventData.pressPosition, eventData.position) > (EventSystem.current.pixelDragThreshold + 20))
            {
                Debug.Log($"Canceling the Interaction");
                CancelTheInteraction();
                return;
            }

            OnInteractionCanceled();
            Debug.Log($"On Pointer Up...");
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            CancelTheInteraction();
            Debug.Log($"On Pointer Exit...");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            CancelTheInteraction();
            Debug.Log($"On Begin Drag...");
        }
        #endregion

        #region  Private Methods

        private void CancelTheInteraction()
        {
            _isSecondClick = false;
            _clickStarted = false;
            _isLongClickPerformed = false;

            _clickCount = 0;

            _stopwatch?.Stop();
            _stopwatch?.Reset();
            _longClickStopWatch?.Stop();
            _longClickStopWatch?.Reset();
        }

        private void OnInteractionStarted()
        {
            _clickStarted = true;
            if (_clickCount == 0)
                _stopwatch?.Start();

            CheckForSecondClickStart();
        }

        private void OnInteractionCanceled()
        {
            _clickStarted = false;
            _stopwatch?.Stop();
            double time = StopwatchElapsedSeconds;
            _stopwatch?.Reset();
            _longClickStopWatch?.Stop();
            _isLongClickPerformed = false;


            if (time > _pressTime)
            {
                _clickCount = 0;
                return;
            }

            _clickCount++;

            if (CheckAndPerformClick())
                return;

            if (CheckAndPerformSecondClick())
                return;

        }

        private bool CheckAndPerformSecondClick()
        {
            if (_isSecondClick)
            {
                _isSecondClick = false;
                _clickCount = 0;
                _clickStarted = false;
                _doubleClickPerformed.Invoke();
                return true;
            }

            return false;
        }

        private bool CheckAndPerformClick()
        {
            if (_doubleClickType == DoubleClickType.None)
            {
                _clickPerformed.Invoke();
                _clickCount = 0;
                _stopwatch?.Stop();
                return true;
            }

            if (_clickCount == 1)
            {
                if (_doubleClickType == DoubleClickType.DoubleTrigger)
                    _clickPerformed.Invoke();

                _stopwatch?.Start();
                return true;
            }

            return false;
        }

        private void CheckForSecondClickStart()
        {
            if (_clickCount == 1 && StopwatchElapsedSeconds <= _maxSpaceTime)
            {
                _stopwatch?.Restart();
                _isSecondClick = true;
            }
        }

        private void StartLongClick()
        {
            if (_longClickType == LongClickType.None)
                return;

            _isLongClickPerformed = true;
            if (_longClickType == LongClickType.Normal)
            {
                _longClickPerformed.Invoke();
            }
            else if (_longClickType == LongClickType.Continuous)
            {
                _longClickStopWatch?.Restart();
            }
        }



        #endregion

        private double StopwatchElapsedSeconds
        {
            get
            {
                if (_stopwatch == null)
                    return 0;
                return _stopwatch.Elapsed.TotalSeconds;
            }
        }


        private double LongClickStopwatchElapsedSeconds
        {
            get
            {
                if (_longClickStopWatch == null)
                    return 0;
                return _longClickStopWatch.Elapsed.TotalSeconds;
            }
        }

        public UnityEvent OnClickPerformed => _clickPerformed;
        public UnityEvent OnDoubleClick => _doubleClickPerformed;
        public UnityEvent OnLongClickPerformed => _longClickPerformed;
    }

    public enum DoubleClickType
    {
        /// <summary>
        /// Disables the double click feature.
        /// </summary>
        None,

        /// <summary>
        /// This interaction means that only double tap/click will be invoked upon completing
        /// the double click interaction.
        /// </summary>
        SingleTrigger,

        /// <summary>
        /// This interaction means that both single and double tap/click events will be invoked
        /// upon double click gesture is performed.
        /// </summary>
        DoubleTrigger,
    }

    public enum LongClickType
    {
        /// <summary>
        /// Disables the long click.
        /// </summary>
        None,

        /// <summary>
        /// Raises an event once the configured time/config is meat.
        /// </summary>
        Normal,

        /// <summary>
        /// Once the configured time is meat the event will be continuously invoked at the interval time. 
        /// </summary>
        Continuous,
    }
}