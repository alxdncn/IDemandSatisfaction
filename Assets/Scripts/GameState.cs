using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour {

	private static GameState instance;
	public static GameState Instance { get { return instance; } }


	AudioSource audioSource;
	[SerializeField] AudioClip secondTickAudio;
	[SerializeField] AudioClip startAudio;

	int lastSecond = 10;
 
	[SerializeField] Text timerText;

	const float MIN_SPEED = 2f;
	public const float MAX_SPEED = 6f;

	public bool someoneDied = false;

	int playersReady = 0;
	public bool started = false;
	bool onePlayerReleased = false;

	public float roundTime = 10f;
	[System.NonSerialized] public float roundTimer;

	[System.NonSerialized] public bool gameOver = true;

	[SerializeField] PlayerScript[] players;


	public delegate void EndGame(PlayerScript playerThatWon, bool instantKill);
	public event EndGame endGame;

	public delegate void Restart();
	public event Restart restart;

	public delegate void StartDuel();
	public event StartDuel startDuel;

	public delegate void NeutralEnd();
	public event NeutralEnd neutralEnd;

	bool canRestart = false;

	// Use this for initialization
	void Awake () {
		instance = this;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		timerText.gameObject.SetActive (false);

		roundTimer = roundTime;

		audioSource = GetComponent<AudioSource>();

		restart += RestartGameState;
	}

	void RestartGameState(){
		gameOver = true;
		started = false;
		onePlayerReleased = false;
		someoneDied = false;
		canRestart = false;
		roundTimer = roundTime;
		timerText.gameObject.SetActive (false);
		timerText.text = roundTime.ToString();
		playersReady = 0;
		audioSource.pitch = 1f;
	}

	public void StartGame(){
		gameOver = false;
		for(int i = 0; i < players.Length; i++){
			players[i].scoreText.enabled = true;
		}
	}

	void OnEnable(){
		endGame += GameOver;

		for(int i = 0; i < players.Length; i++){
			players[i].playerReleased += PlayerReleased;
			players[i].playerReady += ReadyPlayer;
			players[i].playerAnimationFinished += FireEndGameFromAnimationEnd;
		}
	}

	void OnDisable(){
		endGame -= GameOver;

		for(int i = 0; i < players.Length; i++){
			players[i].playerReleased -= PlayerReleased;
			players[i].playerReady -= ReadyPlayer;
			players[i].playerAnimationFinished -= FireEndGameFromAnimationEnd;
		}
	}

	public void FireRestart(){
		restart();
	}

	void FireEndGameFromAnimationEnd(PlayerScript playerThatWon){
		if(!gameOver)
			endGame(playerThatWon, false);
	}

	void Update(){
		if (started && !gameOver) {
			if (roundTimer < 0) {
				if(neutralEnd != null){
					neutralEnd();
				}
				gameOver = true;
				someoneDied = true;
				roundTimer = roundTime;
				timerText.text = "0";
			} else{
				int second = Mathf.CeilToInt (roundTimer);
				timerText.text = second.ToString ();

				if(second != lastSecond){
					audioSource.pitch = Random.Range(0.95f, 1.05f);
					audioSource.PlayOneShot(secondTickAudio);
					lastSecond = second;
				}

				roundTimer -= Time.deltaTime;
			}
		}

		if(someoneDied && Input.GetKeyDown(KeyCode.Space)){
			restart();
		}
	}

	public float GetSpeed(){
		return (MIN_SPEED + (MAX_SPEED - MIN_SPEED) * (1 - (roundTimer / roundTime)));
	}

	void ReadyPlayer(){
		if(gameOver)
			return;

		playersReady++;

		Debug.Log ("Players ready " + playersReady);

		if (playersReady == 2) {
			timerText.gameObject.SetActive (true);
			started = true;
			audioSource.PlayOneShot(startAudio);
			if(startDuel != null){
				startDuel();
			}
			for(int i = 0; i < players.Length; i++){ //This should be on the players, and they should subscribe to startGame
				players[i].animator.Play("Paces");
			}
		} else if (playersReady > 2) {
			Debug.LogError ("Too many players checked in");
		}
	}

	void PlayerReleased(PlayerScript player){
		if(gameOver || playersReady < 1)
			return;

		if(onePlayerReleased){
			endGame(player, true);
		} else if(started){
			player.animator.speed = GetSpeed();
			player.animator.Play(SharedPlayerState.Instance.fireAnimation.name);
			onePlayerReleased = true;
		} else{
			playersReady--;
		}
	}

	void GameOver(PlayerScript playerThatWon, bool instant){
		someoneDied = true;
		gameOver = true;
	}

}
