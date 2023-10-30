using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
	GS_PAUSEMENU,
	GS_GAME,
	GS_GAME_OVER,
	GS_LEVELCOMPLETED,
	GS_OPTIONS
}

public class GameManager : MonoBehaviour
{
	public GameState currentGameState;
	public Canvas inGameCanvas;
	public Canvas pauseMenuCanvas;
	public Canvas levelCompletedCanvas;
	public Canvas gameOverCanvas;
	public Canvas optionsCanvas;
	public static GameManager instance;

	public Text coinsText;
	public Text timeText;
	public Text finalTimeText;
	public Text finalLivesNumber;
	public Text enemiesCounter;
	public Text finalScoreNumber;
	public Text highscoreNumber;

	public Image[] keysTab;
	public Image[] playerLivesTab;
	public Color[] colorsTab;

	private int coins = 0;
	private int keys = 0;
	private int lives = 3;
	private int maxKeyNumber=3;
	private int enemiesKilled = 0;
	private bool hasFoundAllKeys = false;
	private float timer = 0f;



	private void Awake()
	{
		instance = this;

		foreach (Image key in keysTab)
		{
			key.color = Color.gray;
		}

		playerLivesTab[playerLivesTab.Length - 1].enabled = false;

		if(!PlayerPrefs.HasKey("HighscoreLevel1"))
		{
			PlayerPrefs.SetInt("HighscoreLevel1", 0);
		}

		if (!PlayerPrefs.HasKey("HighscoreLevel2"))
		{
			PlayerPrefs.SetInt("HighscoreLevel2", 0);
		}

	}

	// Start is called before the first frame update
	void Start()
	{
		InGame();

	}

	// Update is called once per frame
	void Update()
	{

		if (Input.GetKeyDown(KeyCode.Escape) && (currentGameState == GameState.GS_PAUSEMENU || currentGameState == GameState.GS_OPTIONS))
		{

			InGame();

		}
		else if (Input.GetKeyDown(KeyCode.Escape) && currentGameState == GameState.GS_GAME)
		{

			PauseMenu();
		}


		if (currentGameState == GameState.GS_GAME)
		{
			timer += Time.deltaTime;

			float minutes = Mathf.FloorToInt(timer / 60);
			float seconds = Mathf.FloorToInt(timer % 60);

			timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
		}


	}

	void SetGameState(GameState newGameState)
	{
		pauseMenuCanvas.enabled = (newGameState == GameState.GS_PAUSEMENU);
		levelCompletedCanvas.enabled = (newGameState == GameState.GS_LEVELCOMPLETED);
		gameOverCanvas.enabled = (newGameState == GameState.GS_GAME_OVER);
		inGameCanvas.enabled = (newGameState == GameState.GS_GAME);
		optionsCanvas.enabled = (newGameState == GameState.GS_OPTIONS);
		currentGameState = newGameState;

		if(newGameState == GameState.GS_LEVELCOMPLETED) 
		{
			Scene currentScene = SceneManager.GetActiveScene();

		}
		if(newGameState != GameState.GS_OPTIONS || newGameState !=GameState.GS_PAUSEMENU)
		{
			Time.timeScale = 1.0f;
		}

		new WaitForSeconds(1);
	}

	public void InGame()
	{
		SetGameState(GameState.GS_GAME);
	}

	public void GameOver()
	{
		CalculateFinalScore();
		SetGameState(GameState.GS_GAME_OVER);
	}

	public void PauseMenu()
	{
		SetGameState(GameState.GS_PAUSEMENU);
		Time.timeScale = 0.0f;

	}

	public void Options()
	{
		SetGameState(GameState.GS_OPTIONS);
		Time.timeScale = 0.0f;
	}

	public void LevelCompleted()
	{
		CalculateFinalScore();

		float minutes = Mathf.FloorToInt(timer / 60);
		float seconds = Mathf.FloorToInt(timer % 60);

		finalTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
		finalLivesNumber.text = lives.ToString();

		SetGameState(GameState.GS_LEVELCOMPLETED);
	}

	public void AddCoins()
	{
		coins++;
		coinsText.text = coins.ToString();
	}

	public void AddKeys()
	{
		keys++;
		keysTab[keys - 1].color = colorsTab[keys - 1];
		if (keys == maxKeyNumber)
		{
			hasFoundAllKeys = true;
		}
	}

	public bool HasFoundAllKeys { get { return hasFoundAllKeys; } }

	public void EnemyKilled()
	{
		enemiesKilled++;
		enemiesCounter.text = enemiesKilled.ToString();
	}

	public void PlayerKilled(int livesLeft)
	{
		lives--;
		if(livesLeft>=0) 
		{ 
			playerLivesTab[livesLeft].enabled = false; 
		}
		
	}

	public void FoundLife(int newLifes)
	{
		lives++;
		playerLivesTab[newLifes - 1].enabled = true;

	}

	public void OnResumeButtonClicked()
	{
		InGame();
	}

	public void OnRestartButtonClicked()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void OnNextLevelButtonClicked()
	{
		SceneManager.LoadScene("Level2");
	}

	public void OnExitButtonClicked()
	{
		//inGameCanvas.enabled = false;
		//pauseMenuCanvas.enabled = false;
		SceneManager.LoadScene("MainMenu");
	}


	private void CalculateFinalScore()
	{
		// 50 for each life
		// 20 for each enemy killed
		// 10 for each point found
		// finished under 120s: 120 points
		// over 120s: (220 - time)points, min 0 score

		int seconds = Mathf.FloorToInt(timer % 60);
		int timeScore;
		if(seconds <120)
		{
			timeScore = 120;
		}
		else
		{
			timeScore = Mathf.Max(0, 220 - seconds);
		}

		int finalScore = 50 * lives + 20 * enemiesKilled + 10 * coins + timeScore;
		finalScoreNumber.text = finalScore.ToString();

		string highscoreLevelName = "Highscore" + SceneManager.GetActiveScene().name;

		//Debug.Log(highscoreLevelName);

		if (finalScore > PlayerPrefs.GetInt(highscoreLevelName) )
		{
			PlayerPrefs.SetInt(highscoreLevelName, finalScore);
		}
		highscoreNumber.text = PlayerPrefs.GetInt(highscoreLevelName).ToString();
	}

	public void IncreaseGraphicsQuality()
	{
		
		QualitySettings.IncreaseLevel();
		
	}

	public void DecreaseGraphicsQuality()
	{
		
		QualitySettings.DecreaseLevel();
	}

	
	public void SetVolume(float vol)
	{
		AudioListener.volume = vol;
	}
}
