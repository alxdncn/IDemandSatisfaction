using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHolderBehavior : MonoBehaviour {

	Vector3 startScale;

	[SerializeField] Vector3 endScale;

	bool zoom = false;

	void Start(){
		startScale = transform.localScale;
	}
	
	void Reset(){
		zoom = false;
		transform.localScale = startScale;
	}
	
	void OnEnable(){
		GameState.Instance.restart += Reset;
		GameState.Instance.startDuel += StartZoomOut;
		GameState.Instance.endGame += EndZoomOut;
		GameState.Instance.neutralEnd += EndZoomOut;
	}

	void OnDisable(){
		GameState.Instance.restart -= Reset;
		GameState.Instance.startDuel -= StartZoomOut;
		GameState.Instance.endGame -= EndZoomOut;
		GameState.Instance.neutralEnd -= EndZoomOut;
	}

	void Update(){
		if(zoom){
			float t = 1 - Mathf.Clamp01(GameState.Instance.roundTimer/ GameState.Instance.roundTime);
			transform.localScale = Vector3.Lerp(startScale, endScale, t);
		}
	}

	void StartZoomOut(){
		zoom = true;
	}

	void EndZoomOut(PlayerScript player, bool instant){
		zoom = false;
	}

	void EndZoomOut(){
		zoom = false;
	}
}
