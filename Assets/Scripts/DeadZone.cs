using UnityEngine;
using System.Collections;

public class DeadZone : MonoBehaviour {

	void OnTriggerEnter(Collider col){
		SoundManager.instance.deathSound();
		col.SendMessage("BallDeath");
		GM.instance.LoseLife ();
	}
	
}
