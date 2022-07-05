using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static AudioManager instance = null;

    public enum Root {F, G, Bb, C};
    public enum Scale {F, Db};
    [HideInInspector] public Root currentRoot;
    [HideInInspector] public Scale currentScale;

    public AudioSource musicIntro;
    public AudioSource musicLoop;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance == null) {
			instance = this;
        } else if (instance != this) {
			Destroy (gameObject);
        }
        musicIntro.Play();
        musicLoop.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
