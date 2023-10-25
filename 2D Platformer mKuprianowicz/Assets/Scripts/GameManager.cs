using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
	GS_PAUSEMENU,
	GS_GAME,
	GS_GAME_OVER,
	GS_LEVELCOMPLETED
}

public class GameManager : MonoBehaviour
{
	public GameState currentGameState;
	public Canvas inGameCanvas;
	public Canvas pauseMenuCanvas;
	public static GameManager instance;

	public Text coinsText;
	public Text timeText;
	public Text enemiesCounter;
	public Image[] keysTab;
	public Image[] playerLivesTab;
	public Color[] colorsTab;

	private int coins = 0;
	private int keys = 0;
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

	}

	// Start is called before the first frame update
	void Start()
	{
		InGame();

	}

	// Update is called once per frame
	void Update()
	{

		if (Input.GetKeyDown(KeyCode.Escape) && currentGameState == GameState.GS_PAUSEMENU)
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
		if (newGameState == GameState.GS_GAME)
		{
			inGameCanvas.enabled = true;
		}
		else
		{
			inGameCanvas.enabled = false;
		}
		currentGameState = newGameState;
		new WaitForSeconds(1);
	}

	public void InGame()
	{
		SetGameState(GameState.GS_GAME);
	}

	public void GameOver()
	{
		SetGameState(GameState.GS_GAME_OVER);
	}

	public void PauseMenu()
	{
		SetGameState(GameState.GS_PAUSEMENU);

	}

	public void LevelCompleted()
	{
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
		if (keys == 3)
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
		playerLivesTab[livesLeft].enabled = false;
	}

	public void FoundLife(int newLifes)
	{

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

	public void OnExitButtonClicked()
	{
		inGameCanvas.enabled = false;
		pauseMenuCanvas.enabled = false;
		SceneManager.LoadScene("MainMenu");
	}

}
