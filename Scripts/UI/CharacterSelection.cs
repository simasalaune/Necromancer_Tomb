using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    public GameObject[] characters;
    public GameObject[] stats;
    public int selectedCharacter = 0;
    public int selectedStat = 0;

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    void Start()
    {
        DisableControls();
    }

    private void DisableControls()
    {
        GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }
        GetComponentInChildren<PlayerController>().enabled = false;
        GetComponentInChildren<MovementController>().enabled = false;
    }

    public void NextCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        selectedCharacter = (selectedCharacter + 1) % characters.Length;
        characters[selectedCharacter].SetActive(true);

        
        stats[selectedStat].SetActive(false);
        selectedStat = (selectedStat + 1) % stats.Length;
        stats[selectedStat].SetActive(true);

        DisableControls();
    }

    public void PreviousCharacter()
    {
        characters[selectedCharacter].SetActive(false);
        selectedCharacter--;
        if (selectedCharacter < 0)
        {
            selectedCharacter += characters.Length;
        }    
        characters[selectedCharacter].SetActive(true);
        
        stats[selectedStat].SetActive(false);
        selectedStat--;
        if (selectedStat < 0)
        {
            selectedStat += stats.Length;
        }
        stats[selectedStat].SetActive(true);

        DisableControls();
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
