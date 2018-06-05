using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeBehavior : MonoBehaviour {

	[SerializeField] PlayerScript myPlayer;
	[SerializeField] Transform myPistol;

	SpriteRenderer sRend;

	bool init = true;


	bool follow = true;

	Animator anim;

	void Awake(){
		sRend = GetComponentInChildren<SpriteRenderer>();
		sRend.enabled = false;
		anim = GetComponent<Animator>();
	}

	void OnEnable(){
		myPlayer.playerAnimationFinished += GunFireSmoke;
		GameState.Instance.restart += Restart;
	}

	void OnDisable(){
		myPlayer.playerAnimationFinished -= GunFireSmoke;
		GameState.Instance.restart += Restart;
	}

	void Restart(){
		sRend.enabled = false;
		anim.Play("Default");
	}

	void Update(){
		if(follow){
			transform.position = myPistol.position;
			transform.eulerAngles = myPistol.eulerAngles;
		}
	}

	void GunFireSmoke(PlayerScript player){
		follow = false;
		sRend.enabled = true;
		anim.Play("Grow");
	}
}
