using RenderHeads.Media.AVProVideo;
using RenderHeads.Media.AVProVideo.Demos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Video;

namespace JioXSDK
{

    public class AVProMediaPlayerController : MonoBehaviour, IMediaPlayerController
    {
        [SerializeField] MediaPlayer _mediaPlayer = null;
       // public Slider _sliderTime = null;

        [SerializeField] RectTransform _canvasTransform = null;

   //     [SerializeField] MediaPlayerUI mediaPlayerUI;

      //  [SerializeField] JioXMediaPlayerUI jioXUI;

        [SerializeField] MediaReference mediaToPlay;

        //Slider _TimelineSlider;

        Slider _sliderTime = null;

        TimeRange timelineRange;

        private bool _wasPlayingBeforeTimelineDrag;

        [SerializeField] bool _useAudioFading = true;
        private bool _isAudioFadingUpToPlay = true;

        private float _audioFadeTime = 0f;
        //      bool finishedPlaying;

        public JioXMediaPlayerUI jioXUI { get; set; }

        public void SetMediaClip(VideoClip clip)
        {
            
        }
        public void SetMediaClip(MediaReference clip)
        {
            _mediaPlayer.OpenMedia(clip);// = clip;
                                         //     _mediaPlayer.Control.Play();
            PlayPauseJioMedia();
            InitMediaPlayer();
        }

        private void InitMediaPlayer()
        {
            jioXUI.finishedPlaying = true;

            jioXUI._Timeline.SetActive(true);
       //     _TimelineSlider = jioXUI._Timeline.GetComponent<Slider>();

            _sliderTime = jioXUI._Timeline.GetComponent<Slider>();

            //string mediaPath = _mediaPlayer.MediaPath.ToString();
            //string videoFileName = System.IO.Path.GetFileName(mediaPath);

      //      jioXUI.videoName.text = videoFileName;

            jioXUI.videoName.text = _mediaPlayer.MediaReference.ToString();

            //      mediaPlayerUI._textTimeDuration = jioXUI._TimeDuration;
            //      mediaPlayerUI._segmentsSeek = jioXUI._fillSeek;
            //      mediaPlayerUI._segmentsBuffered = jioXUI._fillBuffered;
            //      mediaPlayerUI._segmentsProgress = jioXUI._fillProgress;
            //       mediaPlayerUI._sliderTime = _TimelineSlider;

            if (_mediaPlayer)
            {
            //    _audioVolume = _mediaPlayer.AudioVolume;
#if UNITY_ANDROID
                // Disable screen sleep timeout if the video is set to auto-start
                if (_mediaPlayer.AutoStart)
                {
                    Screen.sleepTimeout = SleepTimeout.NeverSleep;
                }
#endif
            }
        }

        public void PlayPauseJioMedia()
        {
            Debug.Log("Play Av pro");
           // mediaPlayerUI.TogglePlayPause();

            if(jioXUI.finishedPlaying == false)
            {
                _mediaPlayer.OpenMedia(mediaToPlay);
            }
            
            TogglePlayPause();

            ChangeSprite();

            jioXUI.finishedPlaying = true;

            //MediaPanelActivate();
        }

        public void TogglePlayPause()
        {
         
            if (_mediaPlayer && _mediaPlayer.Control != null)
            {
               
                if (_useAudioFading && _mediaPlayer.Info.HasAudio())
                {
                    Debug.Log("Toggle play pause");
                    if (_mediaPlayer.Control.IsPlaying())
                    {
                        //if (_overlayManager)
                        //{
                        //	_overlayManager.TriggerFeedback(OverlayManager.Feedback.Pause);
                        //}
                        _isAudioFadingUpToPlay = false;
                        Pause();
                    }
                    else
                    {
                        _isAudioFadingUpToPlay = true;
                        Debug.Log("yes inside play1");
                        Play();
                    }
                    _audioFadeTime = 0f;
                }
                else
                {
                    if (_mediaPlayer.Control.IsPlaying())
                    {
                        Debug.Log("yes inside pause");
                        Pause();
                    }
                    else
                    {
                        Debug.Log("yes inside play");
                        Play();
                    }
                }
            }
        }

        public void Play()
        {
            Debug.Log($"##MediaPlayer --> playing mediaplayerui1");
            if (_mediaPlayer && _mediaPlayer.Control != null)
            {
                Debug.Log($"##MediaPlayer --> playing mediaplayerui");
                //if (_overlayManager)
                //{
                //	_overlayManager.TriggerFeedback(OverlayManager.Feedback.Play);
                //}
            //    _buttonPlayPause.GetComponent<Image>().sprite = pauseSprite;
                _mediaPlayer.Play();
#if UNITY_ANDROID
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
            }
        }

        private void Pause(bool skipFeedback = false)
        {
            if (_mediaPlayer && _mediaPlayer.Control != null)
            {
                if (!skipFeedback)
                {
                    //if (_overlayManager)
                    //{
                    //	_overlayManager.TriggerFeedback(OverlayManager.Feedback.Pause);
                    //}
                }
                _mediaPlayer.Pause();
           //     _buttonPlayPause.GetComponent<Image>().sprite = playSprite;
#if UNITY_ANDROID
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
#endif
            }
        }


        public void ChangeSprite()
        {
            if (jioXUI.playPauseBtn.GetComponent<Image>().sprite == jioXUI.playSprite)
            {
                jioXUI.playPauseBtn.GetComponent<Image>().sprite = jioXUI.pauseSprite;
            }
            else if (jioXUI.playPauseBtn.GetComponent<Image>().sprite == jioXUI.pauseSprite)
            {
                jioXUI.playPauseBtn.GetComponent<Image>().sprite = jioXUI.playSprite;
            }
        }

        public void MediaPanelActivate()
        {
            if (jioXUI.MediaPanelUI.activeInHierarchy)
            {
                jioXUI.MediaPanelUI.SetActive(false);
            }
            else
            {
                jioXUI.MediaPanelUI.SetActive(true);
            }
        }

        private void Update()
        {
            if (_mediaPlayer.Info != null)
            {
                timelineRange = GetTimelineRange();
                double _time = _mediaPlayer.Control.GetCurrentTime();
                //		Debug.Log("Current Time" + _time);
                // Update timeline hover popup
#if (!ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER)
                //         if (_timelineTip != null)
                //       {
                if (_isHoveringOverTimeline)
                {
                    Vector2 canvasPos;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasTransform, Input.mousePosition, null, out canvasPos);

                    jioXUI._fillSeek.gameObject.SetActive(true);
                    //           _timelineTip.gameObject.SetActive(true);
                    Vector3 mousePos = _canvasTransform.TransformPoint(canvasPos);

                    //         _timelineTip.position = new Vector2(mousePos.x, _timelineTip.position.y);

                    if (UserInteraction.IsUserInputThisFrame())
                    {
                        // Work out position on the timeline
                        Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(this._sliderTime.GetComponent<RectTransform>());
                        float x = Mathf.Clamp01((canvasPos.x - bounds.min.x) / bounds.size.x);

                        double time = (double)x * timelineRange.Duration;

                        // Seek to the new position
                        //if (_thumbnailMediaPlayer != null && _thumbnailMediaPlayer.Control != null)
                        //{
                        //    _thumbnailMediaPlayer.Control.SeekFast(time);
                        //}

                        // Update time text
                        //Text hoverText = _timelineTip.GetComponentInChildren<Text>();

                        //if (hoverText != null)
                        //{
                        //    time -= timelineRange.startTime;
                        //    time = System.Math.Max(time, 0.0);
                        //    time = System.Math.Min(time, timelineRange.Duration);
                        //    hoverText.text = Helper.GetTimeString(time, false);
                        //}

                        {
                        // Update seek segment when hovering over timeline
                        if (jioXUI._fillSeek != null)
                        {


                            float[] ranges = new float[2];
                            if (timelineRange.Duration > 0.0)
                            {

                                double t = ((_mediaPlayer.Control.GetCurrentTime() - timelineRange.startTime) / timelineRange.duration);
                                ranges[1] = x;
                                ranges[0] = (float)t;
                            }
                            else
                            {
                                Debug.Log("End" + time);
                            }
                            jioXUI._fillSeek.Segments = ranges;
                        }
                    }
                }
            }
            else
            {
                //  _timelineTip.gameObject.SetActive(false);
                jioXUI._fillSeek.gameObject.SetActive(false);
            }
        }
#endif

            // Update time/duration text display
            if (jioXUI._TimeDuration)
            {
                string t1 = Helper.GetTimeString((_mediaPlayer.Control.GetCurrentTime() - timelineRange.startTime), false);
                string d1 = Helper.GetTimeString(timelineRange.duration, false);
                jioXUI._TimeDuration.text = string.Format("{0} / {1}", t1, d1);
            }

            // Update buffered segments
            if (jioXUI._fillBuffered)
            {
                TimeRanges times = _mediaPlayer.Control.GetBufferedTimes();
                float[] ranges = null;
                if (times.Count > 0 && timelineRange.duration > 0.0)
                {
                    ranges = new float[times.Count * 2];
                    for (int i = 0; i < times.Count; i++)
                    {
                        ranges[i * 2 + 0] = Mathf.Max(0f, (float)((times[i].StartTime - timelineRange.startTime) / timelineRange.duration));
                        ranges[i * 2 + 1] = Mathf.Min(1f, (float)((times[i].EndTime - timelineRange.startTime) / timelineRange.duration));
                    }
                }
                jioXUI._fillBuffered.Segments = ranges;
            }

            // Update progress segment
            if (jioXUI._fillProgress)
            {
                TimeRanges times = _mediaPlayer.Control.GetBufferedTimes();
                float[] ranges = null;
                if (times.Count > 0 && timelineRange.Duration > 0.0)
                {
                    ranges = new float[2];
                    double x1 = (times.MinTime - timelineRange.startTime) / timelineRange.duration;
                    double x2 = ((_mediaPlayer.Control.GetCurrentTime() - timelineRange.startTime) / timelineRange.duration);
                    ranges[0] = Mathf.Max(0f, (float)x1);
                    ranges[1] = Mathf.Min(1f, (float)x2);
                }
                jioXUI._fillProgress.Segments = ranges;
            }

            // Update time slider position
            if (_sliderTime && !_isHoveringOverTimeline)
            {
                double t = 0.0;
                if (timelineRange.duration > 0.0)
                {
                    t = ((_mediaPlayer.Control.GetCurrentTime() - timelineRange.startTime) / timelineRange.duration);
                }
                _sliderTime.value = Mathf.Clamp01((float)t);
            }

            if (/*mediaPlayerUI.*/_mediaPlayer.GetComponent<MediaPlayer>().Control.IsFinished() && jioXUI.finishedPlaying == true)
            {
                jioXUI.MediaPanelUI.SetActive(true);
       //         ChangeSprite();
                jioXUI.playPauseBtn.GetComponent<Image>().sprite = jioXUI.playSprite;

                jioXUI.finishedPlaying = false;
            }
        }

        private TimeRange GetTimelineRange()
        {
            if (_mediaPlayer.Info != null)
            {
                return Helper.GetTimelineRange(_mediaPlayer.Info.GetDuration(), _mediaPlayer.Control.GetSeekableTimes());
            }
            return new TimeRange();
        }

        private bool _isHoveringOverTimeline;

  

        private void CreateTimelineDragEvents()
        {
            EventTrigger trigger = _sliderTime.gameObject.GetComponent<EventTrigger>();
            if (trigger != null)
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener((data) => { OnTimeSliderBeginDrag(); });
                trigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.Drag;
                entry.callback.AddListener((data) => { OnTimeSliderDrag(); });
                trigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerUp;
                entry.callback.AddListener((data) => { OnTimeSliderEndDrag(); });
                trigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((data) => { OnTimelineBeginHover((PointerEventData)data); });
                trigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerExit;
                entry.callback.AddListener((data) => { OnTimelineEndHover((PointerEventData)data); });
                trigger.triggers.Add(entry);
            }
        }

        private void OnTimelineBeginHover(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                _isHoveringOverTimeline = true;
                _sliderTime.transform.localScale = new Vector3(1f, 2.5f, 1f);
            }
        }

        private void OnTimelineEndHover(PointerEventData eventData)
        {
            _isHoveringOverTimeline = false;
            _sliderTime.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        private void OnTimeSliderBeginDrag()
        {
            if (_mediaPlayer && _mediaPlayer.Control != null)
            {
                _wasPlayingBeforeTimelineDrag = _mediaPlayer.Control.IsPlaying();
                if (_wasPlayingBeforeTimelineDrag)
                {
                    _mediaPlayer.Pause();
                }
                OnTimeSliderDrag();
            }
        }

        private void OnTimeSliderDrag()
        {
            if (_mediaPlayer && _mediaPlayer.Control != null)
            {
                TimeRange timelineRange = GetTimelineRange();
                double time = timelineRange.startTime + (_sliderTime.value * timelineRange.duration);
                _mediaPlayer.Control.Seek(time);
                _isHoveringOverTimeline = true;
            }
        }

        private void OnTimeSliderEndDrag()
        {
            if (_mediaPlayer && _mediaPlayer.Control != null)
            {
                if (_wasPlayingBeforeTimelineDrag)
                {
                    _mediaPlayer.Play();
                    _wasPlayingBeforeTimelineDrag = false;
                }
            }
        }

        public void SeekRelative(float deltaTime)
        {
            if (_mediaPlayer && _mediaPlayer.Control != null)
            {
                TimeRange timelineRange = GetTimelineRange();
                double time = _mediaPlayer.Control.GetCurrentTime() + deltaTime;
                time = System.Math.Max(time, timelineRange.startTime);
                time = System.Math.Min(time, timelineRange.startTime + timelineRange.duration);
                _mediaPlayer.Control.Seek(time);

                //if (_overlayManager)
                //{
                //	_overlayManager.TriggerFeedback(deltaTime > 0f ? OverlayManager.Feedback.SeekForward : OverlayManager.Feedback.SeekBack);
                //}
            }
        }

        //public void BackButtonPressed()
        //{
        //    mediaPlayerUI.SeekRelative(-jioXUI._jumpDeltaTime);
        //}

        //public void ForwardButtonPressed()
        //{
        //    mediaPlayerUI.SeekRelative(jioXUI._jumpDeltaTime);
        //}

        //private void OnEnable()
        //{
        //    jioXUI.playPauseBtn.onClick.AddListener(PlayPauseJioMedia);
        //    jioXUI.frwdBtn.onClick.AddListener(SkipForward);
        //    jioXUI.backBtn.onClick.AddListener(SkipBackward);
        //}
        //private void OnDisable()
        //{
        //    jioXUI.playPauseBtn.onClick.RemoveAllListeners();
        //    jioXUI.frwdBtn.onClick.RemoveAllListeners();
        //    jioXUI.backBtn.onClick.RemoveAllListeners();
        //}

        //public void PlayPauseJioMedia()
        //{
        //    throw new System.NotImplementedException();
        //}

        public void SkipForward()
        {
            //    mediaPlayerUI.SeekRelative(jioXUI._jumpDeltaTime);
            SeekRelative(jioXUI._jumpDeltaTime);
            jioXUI.finishedPlaying = true;
        }

        public void SkipBackward()
        {
            //     mediaPlayerUI.SeekRelative(-jioXUI._jumpDeltaTime);
            SeekRelative(-jioXUI._jumpDeltaTime);
            jioXUI.finishedPlaying = true;
        }

        public void closeVideo()
        {
           _mediaPlayer.Control.Pause();
          
        }

        private struct UserInteraction
        {
            public static float InactiveTime;
            private static Vector3 _previousMousePos;
            private static int _lastInputFrame;

            public static bool IsUserInputThisFrame()
            {
                if (Time.frameCount == _lastInputFrame)
                {
                    return true;
                }
#if (!ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER)
                bool touchInput = (Input.touchSupported && Input.touchCount > 0);
                bool mouseInput = (Input.mousePresent && (Input.mousePosition != _previousMousePos || Input.mouseScrollDelta != Vector2.zero || Input.GetMouseButton(0)));

                if (touchInput || mouseInput)
                {
                    _previousMousePos = Input.mousePosition;
                    _lastInputFrame = Time.frameCount;
                    return true;
                }

                return false;
#else
				return true;
#endif
            }
        }
    }
}

