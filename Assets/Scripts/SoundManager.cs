using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    [HideInInspector] public enum Root {F, G, Bb, C, Db, Eb};
    [HideInInspector] public enum Scale {F, Db};

    // used to determine which note is played by the paddle when hit by the ball
    public Root currentRoot;

    // used to determine the scale from which this script decides which note to play
    // when a brick is hit by the ball (further determined later in the script by
    // the brickTracker int)
    public Scale currentScale;

    public AudioSource musicIntro;
    public AudioSource musicLoop;
    public AudioSource paddlePitchedSource;
    public AudioSource paddleUnpitchedSource;
    public AudioSource brickPitchedSource;
    public AudioSource brickUnpitchedSource;
    public AudioSource wallSource;
    public AudioSource deathSource;
    public AudioSource throwSource;
    public AudioSource victorySource;
    public AudioSource gameOverSource;
    public AudioSource startSoundSource;

    AudioClip introClip;
    AudioClip loopClip;
    double introDuration;
    double startTime;

    float musicTracker;
    float loopLength;
    
    public List<AudioClip> paddlePitchedClips = new List<AudioClip>();
    public List<AudioClip> paddleUnpitchedClips = new List<AudioClip>();
    public List<AudioClip> pitchedBrickF = new List<AudioClip>();
    public List<AudioClip> pitchedBrickDb = new List<AudioClip>();
    public List<AudioClip> unpitchedBrick = new List<AudioClip>();
    public List<AudioClip> wallClips = new List<AudioClip>();
    public List<AudioClip> throwClips = new List<AudioClip>();
    public List<AudioClip> victoryClips = new List<AudioClip>();
    public List<AudioClip> gameOverClips = new List<AudioClip>();

    List<float> measures = new List<float>();
    float numMeasures;

    float paddlePitchedVol;
    float wallVol;

    // used to determine whether the ball has hit the paddle since the last time
    // it hit a brick; in brickSound(), this is used to determine whether to
    // randomly select a starting pitch from the current scale (if true), or
    // to continue incrementing from the last note (if false)
    bool hitPaddle;

    // used to determine which note within the current scale is played when a brick is hit
    public int brickTracker;

    public int wallIndex;
    public int brickIndex;

    void Start()
    {
        // establishes the singleton, static game object pattern
        DontDestroyOnLoad(this.gameObject);
        if (instance == null) {
			instance = this;
        } else if (instance != this) {
			Destroy (gameObject);
        }
        
        // grabs the audio clip information for the intro and loop sections
        introClip = musicIntro.clip;
        loopClip = musicLoop.clip;
        
        // finds the length of the loop clip, hard-codes the number of measures to be expected,
        // then uses these to calculate the time value of each barline, which are then added to
        // an array to be referenced later.
        loopLength = loopClip.length;
        numMeasures = 23f;
        for (int i = 0; i < numMeasures; i++) {
            measures.Add(i*(loopLength/23f));
        }
        
        // Necessary implementation for sequencing loop segment to play after intro segment;
        // use of double values (rather than float) allows for sample-accurate scheduling
        // so that there is no gap in between the intro and loop segments

        introDuration = (double)introClip.samples / introClip.frequency;
        startTime = AudioSettings.dspTime + 0.1f;
        musicIntro.PlayScheduled(startTime);
        musicLoop.PlayScheduled(startTime + introDuration);

        // Various setup for volume randomization and random clip selection for certain sounds
        // elsewhere in the script.
        paddlePitchedVol = paddlePitchedSource.volume;
        hitPaddle = true;
        wallVol = wallSource.volume;
        wallIndex = wallClips.Count;
        brickIndex = unpitchedBrick.Count;

        startSound();
    }

    
    void Update()
    {
        // Here, the Update function is used only for harmony-tracking. The Root and Scale enums
        // are used to determine the note, or set of notes, played when the ball hits the paddle
        // and bricks, respectively. Essentially, I have stored the time values of each measure
        // into the measures array, and am cycling through them from the end of the loop segment,
        // backwards, determining the current harmony based on the latest harmony-change point
        // which the segment has already passed.
        if (musicIntro.isPlaying) {
            currentRoot = Root.F;
            currentScale = Scale.F;
        } else {
            musicTracker = musicLoop.time;
            if (musicTracker > measures[21]) {
                // brickTracker is used to determine which note within each pentatonic scale
                // is played when a brick is hit; this logic here is implemented so that when
                // the current scale changes, the next note being played is set to the closest
                // available note in the new scale to the last note hit in the previous scale,
                // if applicable.
                if (currentScale == Scale.Db) {
                    if (brickTracker > 1) {
                        brickTracker -= 2;
                    } else {
                        brickTracker = 0;
                    }
                }
                currentRoot = Root.F;
                currentScale = Scale.F;
            } else if (musicTracker > measures[18]) {
                currentRoot = Root.Eb;
                currentScale = Scale.Db;
            } else if (musicTracker > measures[16]) {
                if (currentScale == Scale.F) {
                    if (brickTracker < pitchedBrickDb.Count - 1) {
                        brickTracker += 1;
                    } else {
                        brickTracker = pitchedBrickDb.Count - 5;
                    }
                }
                currentRoot = Root.Db;
                currentScale = Scale.Db;
            } else if (musicTracker > measures[13]) {
                currentRoot = Root.C;
                currentScale = Scale.F;
            } else if (musicTracker > measures[12]) {
                currentRoot = Root.Bb;
                currentScale = Scale.F;
            } else if (musicTracker > measures[11]) {
                currentRoot = Root.F;
                currentScale = Scale.F;
            } else if (musicTracker > measures[9]) {
                currentRoot = Root.G;
                currentScale = Scale.F;
            } else if (musicTracker > measures[8]) {
                currentRoot = Root.Bb;
                currentScale = Scale.F;
            } else if (musicTracker > measures[7]) {
                currentRoot = Root.F;
                currentScale = Scale.F;
            } else if (musicTracker > measures[5]) {
                currentRoot = Root.C;
                currentScale = Scale.F;
            } else if (musicTracker > measures[4]) {
                currentRoot = Root.Bb;
                currentScale = Scale.F;
            } else if (musicTracker > measures[3]) {
                currentRoot = Root.F;
                currentScale = Scale.F;
            } else if (musicTracker > measures[1]) {
                currentRoot = Root.G;
                currentScale = Scale.F;
            } else if (musicTracker > measures[0]) {
                currentRoot = Root.Bb;
                currentScale = Scale.F;
            }
        }

        
    }

    public void paddleSound() {
        paddlePitchedSource.pitch = Random.Range(0.98f,1.02f);
        paddlePitchedSource.volume = Random.Range(0.9f*paddlePitchedVol, paddlePitchedVol);

        if (currentRoot == Root.F) {
            paddlePitchedSource.PlayOneShot(paddlePitchedClips[0]);
        } else if (currentRoot == Root.G) {
            paddlePitchedSource.PlayOneShot(paddlePitchedClips[1]);
        } else if (currentRoot == Root.Bb) {
            paddlePitchedSource.PlayOneShot(paddlePitchedClips[2]);
        } else if (currentRoot == Root.C) {
            paddlePitchedSource.PlayOneShot(paddlePitchedClips[3]);
        } else if (currentRoot == Root.Db) {
            paddlePitchedSource.PlayOneShot(paddlePitchedClips[4]);
        } else if (currentRoot == Root.Eb) {
            paddlePitchedSource.PlayOneShot(paddlePitchedClips[5]);
        }

        paddleUnpitchedSource.pitch = Random.Range(0.9f,1.1f);
        int temp = Random.Range(0,paddleUnpitchedClips.Count);
        paddleUnpitchedSource.PlayOneShot(paddleUnpitchedClips[temp]);

        hitPaddle = true;
    }
    
    public void brickSound() {
        // resets the counter when the paddle is hit
        if (hitPaddle) {
            brickTracker = Random.Range(0,pitchedBrickF.Count - 5);
        } 
        // increments the counter each time a new brick is hit after the first one
        else if (brickTracker < pitchedBrickF.Count - 1) {
            brickTracker +=1;
        }
        // resets the counter to repeat the last 5 notes of the scale after reaching the top note
        else if (brickTracker == pitchedBrickF.Count - 1) {
            brickTracker = pitchedBrickF.Count - 5;
        }
        
        // checks current scale of music, plays the appropriately ordered note from the corresponding array
        if (currentScale == Scale.F) {
            brickPitchedSource.PlayOneShot(pitchedBrickF[brickTracker]);
        } else if (currentScale == Scale.Db) {
            brickPitchedSource.PlayOneShot(pitchedBrickDb[brickTracker]);
        }
        // ensures that this incrementing process now continues until the next time the paddle is hit
        hitPaddle = false;

        // behavior for the unpitched SFX that also play when a brick is hit
        if (!brickUnpitchedSource.isPlaying) {
            brickUnpitchedSource.pitch = Random.Range(0.9f,1.1f);
        }
        int temp = Random.Range(0,unpitchedBrick.Count);
        while (brickIndex == temp) {
            temp = Random.Range(0,unpitchedBrick.Count);
        }
        paddleUnpitchedSource.PlayOneShot(unpitchedBrick[temp]);

        brickIndex = temp;
        
    }

    public void wallSound() {
        // wallIndex is used here as a simple solution to prevent playing the same sample
        // twice in a row - alternatively, could move the sample to the end of the array
        // each time, and restrict the sample selection to (0, wallClips.Count - 1)
        int temp = Random.Range(0,wallClips.Count);
        while (temp == wallIndex) {
            temp = Random.Range(0,wallClips.Count);
        }
        // Only randomizes pitch and volume of the audio source if another clip isn't
        // still playing (as that clip would also be affected, otherwise)
        if (!wallSource.isPlaying) {
            wallSource.pitch = Random.Range(0.9f,1.05f);
            wallSource.volume = Random.Range(0.9f*wallVol, wallVol);
        }
        wallSource.PlayOneShot(wallClips[temp]);
        wallIndex = temp;

        
    }

    public void throwSound() {
        int temp = Random.Range(0,throwClips.Count);
        throwSource.PlayOneShot(throwClips[temp]);
    }

    public void deathSound() {
        deathSource.Play();
    }

    public void victorySound() {
        if (currentScale == Scale.F) {
            //print("Victory F");
            victorySource.clip = victoryClips[0];
            victorySource.Play();
        } else if (currentScale == Scale.Db) {
            //print("Victory Db");
            victorySource.clip = victoryClips[1];
            victorySource.Play();
        }
    }

    public void gameOverSound() {
        if (currentScale == Scale.F) {
            //print("Game Over F");
            gameOverSource.clip = gameOverClips[0];
            gameOverSource.Play();
        } else if (currentScale == Scale.Db) {
            //print("Game Over Db");
            gameOverSource.clip = gameOverClips[1];
            gameOverSource.Play();
        }
    }

    public void startSound() {
        startSoundSource.Play();
    }
}
