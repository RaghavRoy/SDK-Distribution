using RenderHeads.Media.AVProVideo.Demos.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JioXSDK
{
    public class JioXMediaPlayerUI : MonoBehaviour
    {

        public GameObject MediaPanelUI;
        
        public Button playPauseBtn;
        public Button frwdBtn;
        public Button backBtn;

        public Sprite playSprite;
        public Sprite pauseSprite;

        public Text _TimeDuration;
        public TextMeshProUGUI videoName;

        //AVPro
        public GameObject _Timeline;

        //Unity
        public GameObject _slider;

        public  HorizontalSegmentsPrimitive _fillSeek;
        public  HorizontalSegmentsPrimitive _fillBuffered;
        public  HorizontalSegmentsPrimitive _fillProgress;

        public float _jumpDeltaTime;

        public bool finishedPlaying;
    }
}
