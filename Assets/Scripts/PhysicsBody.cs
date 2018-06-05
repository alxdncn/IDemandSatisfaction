using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBody : MonoBehaviour {

	[SerializeField] GameObject myPlayer;
	[SerializeField] PlayerScript opposingPlayer;

	[SerializeField] float minForce = 40f;
	[SerializeField] float maxForce = 50f;

	void Awake(){
		opposingPlayer.playerAnimationFinished += Explode;
		// gameObject.SetActive(false);
	}

	public void Explode(PlayerScript playerThatWon){
		gameObject.SetActive(true);
		myPlayer.SetActive(false);

		Transform[] children = GetComponentsInChildren<Transform>();

		for(int i = 0; i < children.Length; i++){
			children[i].parent = transform;
		}

		PolygonCollider2D[] allColliders = gameObject.GetComponentsInChildren<PolygonCollider2D>();

		for(int i = 0; i < allColliders.Length; i++){
			allColliders[i].enabled = true;
		}
		
		Rigidbody2D[] allRigs = gameObject.GetComponentsInChildren<Rigidbody2D>();

		for(int i = 0; i < allRigs.Length; i++){
			float xForce = Random.Range(minForce, maxForce);
			float yForce = Random.Range(minForce, maxForce);

			int xDir = Random.Range(0, 2);
			int yDir = Random.Range(0, 2);

			if(xDir == 0)
				xForce = -xForce;

			if(yDir == 0)
				yForce = -yForce;

			allRigs[i].AddForce(new Vector2(xForce, yForce));
		}
	}
}
