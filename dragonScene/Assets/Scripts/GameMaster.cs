using UnityEngine;
using System.Collections;
using System.Collections.Generic;        //so we can use lists
/*
public class GameMaster : MonoBehaviour {


	//www.youtube.com/watch?=ScWH75U6U4

	public int score;
	public int [] highScore = new int[10]; 
	public Text pointsText;
	public Text inputText; 

	// Use this for initialization
	public void Start () {
		if (PlayerPrefs.HasKey ("Score")) {
			if (Application.loadedLevel == 0) {
				PlayerPrefs.DeleteKey ("Score");
				score = 0;
			} else 
			{
				score = PlayerPrefs.GetInt ("Score");
			}
		}
	}

	void addToHighScores (){
		bool newHigh = false;
		int[] tempHighs = new int[10]; 
		//go through top 10 highest scores
		for (int x = 0; x < 10; x++) {
			//check if current score is higher than current high score or a new high score has been added yet
			if (score > highScore [x] && newHigh == false) {
				tempHighs [x + 1] = highScore [x];
				tempHighs [x] = score;
				newHigh = true;
				//if new high added move each score down one
			} else if (newHigh == true) {
				tempHighs [x + 1] = highScore [x];
			}
			else{
				tempHighs[x] = highScore[x];
			}
		}
		//move scores from temp array to high scores array
		for (int i = 0; i < 10; i++){
			highScore[i] = tempHighs[i];
		}
	}

	// Update is called once per frame
	void Update () 
	{
		pointsText.text = ("Points: " + score);
	}


}
*/