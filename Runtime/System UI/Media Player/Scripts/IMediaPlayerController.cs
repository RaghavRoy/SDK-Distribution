using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace JioXSDK
{
    public interface IMediaPlayerController
    {
        void SetMediaClip(VideoClip clip);

        void PlayPauseJioMedia();
        void SkipForward();

        void SkipBackward();

        void MediaPanelActivate();

        void ChangeSprite();

        void closeVideo();

        JioXMediaPlayerUI jioXUI { get; set; }
    }
}
