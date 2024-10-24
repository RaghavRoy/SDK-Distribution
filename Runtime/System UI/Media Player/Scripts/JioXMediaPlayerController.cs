using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace JioXSDK
{
    public class JioXMediaPlayerController : MonoBehaviour
    {
        public static JioXMediaPlayerController instance;

        [SerializeField] private IMediaPlayerController mediaPlayerController;

        [SerializeField] GameObject UnityMediaPlayer;
        [SerializeField] GameObject AVProMediaPlayer;

        [SerializeField] private UnityMediaPlayerController unityMediaPlayerController;
        [SerializeField] private AVProMediaPlayerController aVProMediaPlayerController;
      
        [SerializeField] JioXMediaPlayerUI jioXUI;
     

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void CloseMediaPlayer()
        {
            UnityMediaPlayer.SetActive(false);
            AVProMediaPlayer.SetActive(false);

            jioXUI.MediaPanelUI.SetActive(false);

            mediaPlayerController.closeVideo();
        }

        public void SetMediaPlayer(VideoClip clip, bool unitySelected = true)
        {
            mediaPlayerController = unitySelected ? unityMediaPlayerController : aVProMediaPlayerController;
            mediaPlayerController.jioXUI = jioXUI;
            mediaPlayerController.SetMediaClip(clip);
            UnityMediaPlayer.SetActive(unitySelected);
            AVProMediaPlayer.SetActive(!unitySelected);
        }
        public void SetMediaPlayer(MediaReference clip, bool unitySelected = true)
        {
            mediaPlayerController = unitySelected ? unityMediaPlayerController : aVProMediaPlayerController;
            mediaPlayerController.jioXUI = jioXUI;
            aVProMediaPlayerController.SetMediaClip(clip);
            UnityMediaPlayer.SetActive(unitySelected);
            AVProMediaPlayer.SetActive(!unitySelected);
        }

        private void OnEnable()
        {
            jioXUI.playPauseBtn.onClick.AddListener(PlayPauseJioMedia);
            jioXUI.frwdBtn.onClick.AddListener(SkipForward);
            jioXUI.backBtn.onClick.AddListener(SkipBackward);
        }
        private void OnDisable()
        {
            jioXUI.playPauseBtn.onClick.RemoveAllListeners();
            jioXUI.frwdBtn.onClick.RemoveAllListeners();
            jioXUI.backBtn.onClick.RemoveAllListeners();
        }

        public void PlayPauseJioMedia()
        {
            Debug.Log("manager");
            mediaPlayerController?.PlayPauseJioMedia();
        }

        public void SkipForward()
        {
            mediaPlayerController?.SkipForward();
        }

        public void SkipBackward()
        {
            mediaPlayerController?.SkipBackward();
        }

        public void ChangeSprite()
        {
            mediaPlayerController?.ChangeSprite();
        }

        public void MediaPanelActivate()
        {
            mediaPlayerController?.MediaPanelActivate();
        }

        public void SetVolume(float volume)
        {
            //mediaPlayerController?.SetVolume(volume);
        }

        public void LoadMedia(string mediaPath)
        {
            // mediaPlayerController?.Load(mediaPath);
        }
    }
}
