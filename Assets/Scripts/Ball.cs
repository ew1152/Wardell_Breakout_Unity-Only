using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	public float ballInitialVelocity = 600f;
	private Rigidbody rb;
	private bool ballInPlay;



    [SerializeField]private float minPitch, maxPitch, minVol, maxVol;


    public AudioSource ballSounds;
    public AudioClip wallClip;
    public AudioClip paddleSound;
    public AudioClip brickSound;

    

    void Awake () {

        rb = GetComponent<Rigidbody>();
        ballSounds = GetComponent<AudioSource>();
			
	}


	// Update is called once per frame
	void Update () {
	
		if (Input.GetButtonDown ("Fire1") && ballInPlay == false) {
		
			transform.parent = null;
			ballInPlay = true;
			rb.isKinematic = false;
			rb.AddForce(new Vector3(ballInitialVelocity, ballInitialVelocity, 0));
            SoundManager.instance.throwSound();
		}

	}


    void OnCollisionEnter(Collision marcel) {

        if (marcel.gameObject.tag == "Walls") {
            SoundManager.instance.wallSound();
        }

        if (marcel.gameObject.tag == "Bricks") {
            SoundManager.instance.brickSound();
        }
        
        if (marcel.gameObject.tag == "Paddle") {
            SoundManager.instance.paddleSound();
        }

    }

    void BallDeath() {
		Destroy(gameObject);
	}



}
