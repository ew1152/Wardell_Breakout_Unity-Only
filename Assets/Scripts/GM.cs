using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;


public class GM : MonoBehaviour {
	
	public int lives = 3;
	public int bricks = 20;
	public float resetDelay = 1f;
	public Text livesText;
	public GameObject gameOver;
	public GameObject youWon;
	public GameObject bricksPrefab;
	public GameObject paddle;
	public GameObject deathParticles;
	public static GM instance = null;
	
	private GameObject clonePaddle;
	
	// Use this for initialization
	void Awake () 
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
		
		Setup();
		
	}

	void Update(){

		if (Input.GetKeyDown(KeyCode.Escape)){
		    print ("tata");
		}
	}
	
	public void Setup()
	{
		clonePaddle = Instantiate(paddle, transform.position, Quaternion.identity) as GameObject;
		Instantiate(bricksPrefab, transform.position, Quaternion.identity);
	}
	
	void CheckGameOver()
	{
		if (bricks < 1)
		{
			youWon.SetActive(true);
			SoundManager.instance.victorySound();
			Time.timeScale = .25f;
			Invoke ("Reset", resetDelay);
		}
		
		if (lives < 1)
		{
			gameOver.SetActive(true);
			SoundManager.instance.gameOverSound();
			Time.timeScale = .25f;
			Invoke ("Reset", resetDelay);
		}
		
	}
	
	void Reset()
	{
		Time.timeScale = 1f;
		SoundManager.instance.startSound();
		Application.LoadLevel(Application.loadedLevel);
	}
	
	public void LoseLife()
	{
		lives--;
		
		livesText.text = "Lives: " + lives;
		Instantiate(deathParticles, clonePaddle.transform.position, Quaternion.identity);
		Destroy(clonePaddle);
		Invoke ("SetupPaddle", resetDelay);
		CheckGameOver();
	}
	
	void SetupPaddle()
	{
		clonePaddle = Instantiate(paddle, transform.position, Quaternion.identity) as GameObject;
	}
	
	public void DestroyBrick()
	{
		bricks--;

		CheckGameOver();
	}
}