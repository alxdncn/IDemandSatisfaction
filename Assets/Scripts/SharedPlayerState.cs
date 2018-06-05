using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedPlayerState : MonoBehaviour {

	private static SharedPlayerState instance;
	public static SharedPlayerState Instance { get { return instance; } }

	public float holdToStartTime = 0.5f;
	public AnimationClip fireAnimation;
	public AnimationClip instantKillAnimation;

	public AudioClip fireSound;
	public AudioClip dieSound;

	public float minFirePitch = 0.3f;
	
	public float dieSoundPitch = 0.2f;

	// Use this for initialization
	void Awake () {
		instance = this;
	}
}
