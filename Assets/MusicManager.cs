using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance = null;
  
    public AudioSource intro;
    public AudioSource loop;

    double introDuration;
    double startTime;
    
    void Awake()
    {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy (gameObject);
        }
        DontDestroyOnLoad(gameObject);
        introDuration = (double)intro.clip.samples / intro.clip.frequency;
        startTime = AudioSettings.dspTime + 0.3;
        intro.PlayScheduled(startTime);
        loop.PlayScheduled(startTime + introDuration);
    }


    // void playMain() {
    //     double introDuration = (double)intro.clip.samples / intro.clip.frequency;
    //     double startTime = AudioSettings.dspTime + 0.3;
    //     intro.PlayScheduled(startTime);
    //     loop.PlayScheduled(startTime + introDuration);
    // }

}
