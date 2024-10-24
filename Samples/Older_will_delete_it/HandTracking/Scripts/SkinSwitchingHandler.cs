using System;
using QCHT.Interactions.Hands;
using QCHT.Samples.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace JioXSDK
{
    public class SkinSwitchingHandler : MonoBehaviour
    {
        public Action<string> HandSkinSwitched;

        [SerializeField] private Button _btnArlequinHand;
        [SerializeField] private Button _btnGhostHand;
        [SerializeField] private Button _btnNoHand;

        [SerializeField] private Outline[] _btnOutLines;

        [Space]
        [SerializeField] private SkinInformation _arlequinSkin;
        [SerializeField] private SkinInformation _ghostSkin;
        [SerializeField] private SkinInformation _noHandsSkin;

        [SerializeField] private SkinSwitcher _handSkinSwitcher;

        private void Awake()
        {
            _btnArlequinHand.onClick.AddListener(() => ChangeHandSkin(_arlequinSkin, _btnArlequinHand.GetComponent<Outline>()));
            _btnGhostHand.onClick.AddListener(() => ChangeHandSkin(_ghostSkin, _btnGhostHand.GetComponent<Outline>()));
            _btnNoHand.onClick.AddListener(() => ChangeHandSkin(_noHandsSkin, _btnNoHand.GetComponent<Outline>()));
        }


        private void Start()
        {
            _btnGhostHand.onClick.Invoke();
        }

        private void ChangeHandSkin(SkinInformation skinInfo, Outline outline)
        {
            _handSkinSwitcher.SetLeftSkin(skinInfo.LeftHand);
            _handSkinSwitcher.SetRightSkin(skinInfo.RightHand);

            HandSkinSwitched?.Invoke(skinInfo.skinName.ToString());

            foreach (var ol in _btnOutLines)
                ol.enabled = ol == outline;
        }
    }


    [Serializable]
    internal class SkinInformation
    {
        public HandSkinName skinName;
        public HandSkin LeftHand;
        public HandSkin RightHand;
    }

    internal enum HandSkinName
    {
        Arlequin,
        GhostBlue,
        NoHands
    }
}
