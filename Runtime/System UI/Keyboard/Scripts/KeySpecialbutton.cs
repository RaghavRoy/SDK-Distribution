using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JioXSDK
{
    public class KeySpecialbutton : Keybutton
    {
        [SerializeField] private KeySpecial keySpecial;
        [SerializeField] private GameObject _objectToDeactivate, _objectToActivate;

        private void Start()
        {
            if (keySpecial == KeySpecial.None)
                this.enabled = false;
        }

        public void SwitchObject()
        {
            Debug.Log("Switch object");
            if (!_objectToActivate || !_objectToDeactivate)
                return;
            _objectToActivate.SetActive(true);
            _objectToDeactivate.SetActive(false);
        }

        public KeySpecial KeySpecial => keySpecial;
    
    }
}
