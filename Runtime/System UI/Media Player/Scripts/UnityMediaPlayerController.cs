using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace JioXSDK
{
    public class UnityMediaPlayerController : MonoBehaviour, IMediaPlayerController
    {
        [SerializeField] VideoPlayer videoPlayer;

        Slider videoSlider;

        double totalTime;

        private bool isDragging = false;

        double currentTime;

        string formattedTime;

        public JioXMediaPlayerUI jioXUI { get; set; }

        public void SetMediaClip(VideoClip clip)
        {
            videoPlayer.clip = clip;
            InitMediaPlayer();
        }

        private void InitMediaPlayer()
        {
            jioXUI._slider.SetActive(true);
            videoSlider = jioXUI._slider.GetComponent<Slider>();

            jioXUI.videoName.text = videoPlayer.clip.ToString();

            if (videoPlayer != null)
            {
                // Make sure the video is prepared
                videoPlayer.prepareCompleted += VideoPlayer_prepareCompleted;
                videoPlayer.Prepare();

                videoPlayer.loopPointReached += OnVideoEnd; // Subscribe to the event
            }

            //if (videoPlayer.clip != null)
            //{
            //    videoSlider.maxValue = (float)videoPlayer.length;
            //}
            videoSlider.onValueChanged.AddListener(OnSliderValueChanged);

        }

        private void OnVideoEnd(VideoPlayer vp)
        {
            Debug.Log("Video has ended.");
            jioXUI.MediaPanelUI.SetActive(true);
            ChangeSprite();
        }

        void OnDestroy()
        {
            // Unsubscribe from the event when the object is destroyed
            if (videoPlayer != null)
            {
                videoPlayer.loopPointReached -= OnVideoEnd;
            }
        }

        private void VideoPlayer_prepareCompleted(VideoPlayer source)
        {
            // Access the total length of the video in seconds
            totalTime = source.length;

            videoSlider.maxValue = (float)source.length;
            Debug.Log("Total Video Length: " + totalTime + " seconds");
            Debug.Log("Total Video Length: " + FormatTime(totalTime) + " seconds");
        }

        void OnSliderValueChanged(float value)
        {
            // Update the video time based on slider value
        //    videoPlayer.time = value;

            if (videoPlayer != null)
            {
                videoPlayer.time = value * videoPlayer.length;
            }
        }

        private void Update()
        {
            //double time = videoPlayer.time;
            //jioXUI._TimeDuration.text = time.ToString();


            Debug.Log("value of drag=====" + isDragging);

            if (videoPlayer.isPlaying || videoPlayer.isPaused)
            {
                // Get the current time in seconds
                currentTime = videoPlayer.time;

                // Format the time to a readable string (MM:SS)
                formattedTime = FormatTime(currentTime);
                jioXUI._TimeDuration.text = formattedTime+"/"+ FormatTime(totalTime);
            }
            // for Slider
            //if (videoPlayer.isPlaying)
            //{
            //    videoSlider.value = (float)videoPlayer.time;
            //}

            // videoSlider.value = (int)videoPlayer.time;

            if (videoPlayer.isPlaying || videoPlayer.isPaused || videoPlayer.isPlaying && !isDragging || videoPlayer.isPaused && !isDragging)
            {
                //   videoSlider.value = (int)videoPlayer.time;
                // videoSlider.value = (int)currentTime;
                Debug.Log("Slider checked"+formattedTime + videoSlider);
                Debug.Log("Slider checked 2" + (int)videoPlayer.time);
            //    videoSlider.value = int.Parse(formattedTime);
            videoSlider.SetValueWithoutNotify((int)currentTime);
            }
        }

        private string FormatTime(double time)
        {
            int minutes = Mathf.FloorToInt((float)(time / 60));
            int seconds = Mathf.FloorToInt((float)(time % 60));
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
       
        public void PlayPauseJioMedia()
        {
            //Video Media play/pause functionality
            if(videoPlayer != null && videoPlayer.isPaused)
            {
                videoPlayer.Play();
            }
            else if (videoPlayer != null && videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
            }

            Debug.Log("Time of video======" + videoPlayer.length);

            Debug.Log("Time of video======" + videoPlayer.time);

            ChangeSprite();
    //        MediaPanelActivate();
        }

        //JioX UI Media Panel to Activate/DeActivate
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

        public void ChangeSprite()
        {
            //Changing the sprite of play/pause Button
            if (jioXUI.playPauseBtn.GetComponent<Image>().sprite == jioXUI.playSprite)
            {
                jioXUI.playPauseBtn.GetComponent<Image>().sprite = jioXUI.pauseSprite;
            }
            else if (jioXUI.playPauseBtn.GetComponent<Image>().sprite == jioXUI.pauseSprite)
            {
                jioXUI.playPauseBtn.GetComponent<Image>().sprite = jioXUI.playSprite;
            }
        }

        public void SkipForward()
        {
            double newTime = videoPlayer.time + jioXUI._jumpDeltaTime;
            videoPlayer.time = Mathf.Clamp((float)newTime, 0, (float)videoPlayer.length);
        }

       public void SkipBackward()
        {
            double newTime = videoPlayer.time - jioXUI._jumpDeltaTime;
            videoPlayer.time = Mathf.Clamp((float)newTime, 0, (float)videoPlayer.length);
        }

        public void onPointerDownAndClick()
        {
            isDragging = true;
        }

        public void onPointerUp()
        {
            isDragging = false;
        }

        public void closeVideo()
        {
           videoPlayer.Pause();
        }



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
    }
}
