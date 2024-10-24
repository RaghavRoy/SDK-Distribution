using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JioXSDK
{
    public class FrameRate : MonoBehaviour
    {
        void Start()
        {
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
        }
    }
}
