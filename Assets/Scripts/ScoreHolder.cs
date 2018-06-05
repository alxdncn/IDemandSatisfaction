using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreHolder : MonoBehaviour {

	private static ScoreHolder instance;
	public static ScoreHolder GetInstance(){
		return instance;
	}

	static int leftScore = 0;
	static int rightScore = 0;
	Text leftScoreText;
	Text rightScoreText;



	// Use this for initialization
	void Awake () {
		instance = this;

		leftScoreText = GameObject.Find ("LeftScore").GetComponent<Text> ();
		rightScoreText = GameObject.Find ("RightScore").GetComponent<Text> ();

		leftScoreText.text = leftScore.ToString();
		rightScoreText.text = rightScore.ToString ();
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.R)) {
			leftScore = 0;
			rightScore = 0;
			leftScoreText.text = leftScore.ToString();
			rightScoreText.text = rightScore.ToString ();
		}
	}

	public void AddScore(bool leftSamurai){
		if (leftSamurai) {
			leftScore++;
			leftScoreText.text = leftScore.ToString();
		} else {
			rightScore++;
			rightScoreText.text = rightScore.ToString ();
		}
	}

	void OnLevelWasLoaded(int level) {
		if (level == 0){
			leftScoreText.text = leftScore.ToString();
			rightScoreText.text = rightScore.ToString ();
		}

	}
}
