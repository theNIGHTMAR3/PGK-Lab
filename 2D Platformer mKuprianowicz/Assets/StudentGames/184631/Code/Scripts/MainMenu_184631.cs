using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

	public Text highscoreLevel1Number;
	public Text highscoreLevel2Number;
	
	private void Awake()
	{
		if(!PlayerPrefs.HasKey("Highscore184631"))
		{
			PlayerPrefs.SetInt("Highscore184631", 0);
		}
		highscoreLevel1Number.text= PlayerPrefs.GetInt("Highscore184631").ToString();

		if (!PlayerPrefs.HasKey("HighscoreLevel2"))
		{
			PlayerPrefs.SetInt("HighscoreLevel2", 0);
		}
		highscoreLevel2Number.text = PlayerPrefs.GetInt("HighscoreLevel2").ToString();
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		
	}

    public IEnumerator StartGame(string levelName)
    {

        yield return new WaitForSeconds(.2f);
		SceneManager.LoadScene(levelName);
	}

	public void OnLevel1ButtonPressed()
	{
		StartCoroutine(StartGame("184631"));
	}

	public void OnLevel2ButtonPressed()
	{
		StartCoroutine(StartGame("Level2"));
	}

	public void OnExitButtonPressed()
	{

#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif

		Application.Quit();
	}

}
