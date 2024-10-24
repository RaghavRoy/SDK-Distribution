using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using JioXSDK.Interactions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace JioXSDK
{
    public class ApplyCubeMap : MonoBehaviour
    {


        [SerializeField]
        Cubemap cubemap;
        Image image;
        // Start is called before the first frame update
        void Start()
        {
            image = GetComponent<Image>();
        }
        public void ApplyCubemapToMaterial()
        {
            image.material.SetTexture("_Background", cubemap);
        }

    }
}
