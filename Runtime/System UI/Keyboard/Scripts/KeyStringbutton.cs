using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JioXSDK
{
    public class KeyStringbutton : Keybutton
    {
        [SerializeField] private string strInput = string.Empty;
       public string StrInput => strInput;
    }
}
