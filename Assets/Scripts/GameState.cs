using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour {

	private static GameState instance;
	public static GameState Instance { get { return instance; } }


	[SerializeField] Text timerText;

	const float MIN_SPEED = 1f;
	public const float MAX_SPEED = 5f;

	public bool someoneDied = false;

	int playersReady = 0;
	public bool started = false;
	bool onePlayerReleased = false;

	[SerializeField] float roundTime = 10f;
	float roundTimer;

	[System.NonSerialized] public bool gameOver = true;

	[SerializeField] PlayerScript[] players;


	public delegate void EndGame(PlayerScript playerThatWon, bool instantKill);
	public event EndGame endGame;

	public delegate void Restart();
	public event Restart restart;

	bool canRestart = false;

	// Use this for initialization
	void Awake () {
		instance = this;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		timerText.gameObject.SetActive (false);

		roundTimer = roundTime;

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
			timerText.text = Mathf.Ceil (roundTimer).ToString ();

			roundTimer -= Time.deltaTime;
			if (roundTimer < 0) {
				//Do something??
				roundTimer = roundTime;
				started = false;
//				playersReady = 0;
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
		} else if (playersReady > 2) {
			Debug.LogError ("Too many players checked in");
		}
	}

	void PlayerReleased(PlayerScript player){
		if(gameOver || playersReady < 1)
			return;

		if(onePlayerReleased){
			player.animator.speed = MAX_SPEED;
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
