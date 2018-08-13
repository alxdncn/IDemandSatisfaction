using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour {

    Animator anim;

    PlayerScript winningPlayer;

    void Awake(){
        anim = GetComponent<Animator>();
    }

    void OnEnable(){
        GameState.Instance.endGame += OnEndGame;
        GameState.Instance.neutralEnd += OnNeutralEnd;
        GameState.Instance.restart += Restart;
    }

    void OnDisable(){
        GameState.Instance.endGame -= OnEndGame;
        GameState.Instance.neutralEnd -= OnNeutralEnd;
        GameState.Instance.restart -= Restart;
    }

    void Restart(){
        anim.Play("EnterScene");
        winningPlayer = null;
    }

    void KillInstantly(){
        winningPlayer.animator.Play(SharedPlayerState.Instance.instantKillAnimation.name);
    }

    public void OnEndGame(PlayerScript playerThatWon, bool instant){
        if(instant){
            winningPlayer = playerThatWon;
            if(winningPlayer.transform.parent.gameObject.name.Contains("Left")){
                anim.Play("DramaticEndRight");
            } else{
                anim.Play("DramaticEndLeft");
            }
        } else{
            winningPlayer = playerThatWon;
            if(winningPlayer.transform.parent.gameObject.name.Contains("Left")){
                anim.Play("EndRight");
            } else{
                anim.Play("EndLeft");
            }
        }
    }

    void OnNeutralEnd(){
        anim.Play("CamNeutralEnd");
    }

    void FireStart(){
        GameState.Instance.StartGame();
    }


    public void FireRestart(){
        GameState.Instance.FireRestart();
    }

    public void ShowText(){
        winningPlayer.GetPoint();
    }
}
