using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    void Start()
    {
        SoundPlayer.Instance?.PlayBGM();
    }

    
}
