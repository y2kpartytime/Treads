using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;

[DisallowMultipleComponent]
public class Scene1_Script3 : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
