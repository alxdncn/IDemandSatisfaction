using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {
	//KeyCode.Joystick1Button6
	//KeyCode.Joystick1Button7
	[SerializeField] PlayerScript otherPlayer;
	[System.NonSerialized] public Animator animator;

	float holdToStartTimer;

	bool ready;
	bool released;

	float animTime;

	[SerializeField] GameObject scoreObject;
	[System.NonSerialized] public Text scoreText;
	Animator scoreAnimator;

	int points = 0;

	float animTimeElapsed = 0f;

	AudioSource audioSource;
	float startPitch;

	Vector3 playerStartPos;

	int moveDirection = 1;

	bool move = false;

	[SerializeField] float moveSpeed;

	InputHandler inputScript;

	// Use this for initialization
	void Awake () {
		playerStartPos = transform.parent.localPosition;
		// Debug.Log(playerStartPos);
		moveDirection *= (int)Mathf.Sign(playerStartPos.x);

		animator = GetComponent<Animator> ();
		if(animator == null){
			animator = GetComponentInChildren<Animator>();
		}
		holdToStartTimer = SharedPlayerState.Instance.holdToStartTime;
		
		inputScript = GetComponent<InputHandler>();

		scoreAnimator = scoreObject.GetComponent<Animator>();
		scoreText = scoreObject.GetComponentInChildren<Text>();
		scoreText.text = inputScript.instructionText;
		audioSource = GetComponentInParent<AudioSource>();
		startPitch = audioSource.pitch;

		// myText.gameObject.SetActive (false);
	}

	void OnEnable(){
		GameState.Instance.endGame += OnGameEnd;
		otherPlayer.playerAnimationFinished += Die;
		GameState.Instance.restart += Restart;
		GameState.Instance.neutralEnd += NeutralEnd;
		GameState.Instance.startDuel += StartDuel;
	}

	void OnDisable(){
		GameState.Instance.endGame -= OnGameEnd;
		otherPlayer.playerAnimationFinished -= Die;
		GameState.Instance.restart -= Restart;
		GameState.Instance.neutralEnd -= NeutralEnd;
		GameState.Instance.startDuel -= StartDuel;
	}

	void Restart(){
		move = false;
		transform.parent.localPosition = playerStartPos;
		Debug.Log("Beginning of restart " + transform.parent.position);
		holdToStartTimer = SharedPlayerState.Instance.holdToStartTime;
		animator.Play("Enter");
		scoreAnimator.Play("Default");
		animator.speed = 1f;
		audioSource.volume = 0f; //In case the sound effect is still playing
		audioSource.pitch = startPitch;
		Debug.Log("After restart " + transform.parent.position);
	}

	void Update(){
		if(move){
			transform.parent.position += new Vector3(moveSpeed * Time.deltaTime * moveDirection, 0, 0);
		}
	}
	
	void LateUpdate () {
		if (!ready && inputScript.playerState == InputState.HELD) {
			holdToStartTimer -= Time.deltaTime;

			if (holdToStartTimer < 0) {
				ready = true;
				holdToStartTimer = SharedPlayerState.Instance.holdToStartTime;
				playerReady();
			}
		} else if (inputScript.playerState == InputState.UP && ready) {
			holdToStartTimer = SharedPlayerState.Instance.holdToStartTime;
			ready = false;
			move = false;
			playerReleased(this);
		}
	}

	void EndedAnimation(){ //called from animation
		released = false;
		playerAnimationFinished(this);
		animator.speed = 1f;
		StartCoroutine(PlayFireSound());
	}

	IEnumerator PlayFireSound(){
		audioSource.volume = 1f;
		audioSource.PlayOneShot(SharedPlayerState.Instance.fireSound);
		yield return new WaitForSeconds(0.2f);

		while(audioSource.pitch > SharedPlayerState.Instance.minFirePitch){
			audioSource.pitch -= 0.15f;
			yield return new WaitForSeconds(0.04f);
		}
	}

	void PlayScreamAudio(){
		audioSource.pitch = SharedPlayerState.Instance.dieSoundPitch;
		audioSource.volume = 1f;
		// audioSource.PlayOneShot(SharedPlayerState.Instance.dieSound, 0.8f);
	}

	public delegate void PlayerReady();
	public event PlayerReady playerReady;

	public delegate void PlayerReleased(PlayerScript player);
	public event PlayerReleased playerReleased;

	public delegate void PlayerAnimationFinished(PlayerScript player);
	public event PlayerAnimationFinished playerAnimationFinished;

	void OnGameEnd(PlayerScript playerThatWon, bool instant){
		move = false;
		if(playerThatWon != this){
			animator.speed = 0;
		} else{
			if(instant){
				animator.Play("None");
				animator.speed = 1f;
				// animator.Play(instantKillAnimation.name); //This animation is called from Camera animation already!
			}
			//Do point text stuff etc.
		}
	}

	void Die(PlayerScript playerThatWon){
		if(playerThatWon != this){
			animator.speed = 1f;
			int dieNumber = Random.Range(1, 4);
			animator.Play("Die" + dieNumber.ToString());
		}
	}

	void StartDuel(){
		scoreText.text = points.ToString();
	}

	public void GetPoint(){
		points++;
		string playerSide = "Left";
		if(transform.parent.gameObject.name.Contains("Right")){
			playerSide = "Right";
		}
		scoreAnimator.Play("MoveToCenter" + playerSide);

		StartCoroutine(ChangeText());
	}

	void StartMove(){
		move = true;
	}

	void EndMove(){
		move = false;
	}

	void NeutralEnd(){
		move = false;
		animator.speed = 1f;
		animator.Play("RunAway");
	}

	IEnumerator ChangeText(){
		yield return new WaitForSeconds(0.1f);

		scoreText.text = points.ToString();
	}

	// void TurnStringIntoArray(){
	// 	//Will want to change this programmatically eventually
	// 	string textRaw = myText.text;
	// 	myText.text = "";
	// 	playText = textRaw.Split (new char[] { ' ' });
	// 	myText.gameObject.SetActive (true);
	// }

	// void ShowText(string[] words){
	// 	int whichOneToPlay = 1 + Mathf.FloorToInt((animTimeElapsed / animTime) * (playText.Length - 1));

	// 	myText.text = "";

	// 	for (int i = 0; i < whichOneToPlay; i++) {
	// 		myText.text += playText[i] + " ";
	// 	}

	// 	if (animTimeElapsed > animTime) {
	// 		released = false;
	// 		animTimeElapsed = animTime;
	// 		ChangeAnimTime.GetInstance ().GameWon (gameObject.name);
	// 	}

	// 	animTimeElapsed += Time.deltaTime;
	// }
}
