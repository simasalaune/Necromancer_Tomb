using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public bool GameIsPaused = false;
    private GameObject pausemenuUI;
    private GameObject deathmenuUI;

    private void Awake()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    private void Start()
    {
        pausemenuUI = gameObject.transform.GetChild(1).gameObject;
        deathmenuUI = gameObject.transform.GetChild(2).gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pausemenuUI.SetActive(false);
        deathmenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pausemenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Death(int hits, int highscore)
    {
        deathmenuUI.SetActive(true);

        GameObject.FindGameObjectWithTag("Hits").GetComponent<TextMeshProUGUI>().text = "Hits: " + hits.ToString();
        GameObject.FindGameObjectWithTag("Highscore").GetComponent<TextMeshProUGUI>().text = "Highscore: " + highscore.ToString();
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
