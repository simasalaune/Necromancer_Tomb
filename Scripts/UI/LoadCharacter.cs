using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCharacter : MonoBehaviour
{
    public GameObject[] characterPrefabs;
    public GameObject healthBar;
    public GameObject manaBar;
    public GameObject coins;
    public GameObject backpack;

    public Transform spawnPoint;

    void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter");
        GameObject prefab = characterPrefabs[selectedCharacter];
        GameObject clone = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        // Set variables depending on character choice
        PlayerController pc = clone.GetComponent<PlayerController>();
        MovementController mc = clone.GetComponent<MovementController>();

        if (selectedCharacter == 0)
        {
            // Wizard
            pc.Role = "Wizard";
            pc.health = 20.0f;
            pc.maxHealth = 20.0f;
            pc.mana = 0.0f;
            pc.maxMana = 20.0f;
            pc.damage = 5.0f;
            pc.knockback = 1.0f;
            pc.attackDuration = 0.75f;
            pc.specialDuration = 0.75f;
            mc.speed = 3.0f;
            pc.attackOffset = new Vector3(0.65f, 0.15f, 0);
        }
        else if (selectedCharacter == 1)
        {
            // Knight
            pc.Role = "Knight";
            pc.health = 30.0f;
            pc.maxHealth = 30.0f;
            pc.mana = 0.0f;
            pc.maxMana = 30.0f;
            pc.damage = 3.0f;
            pc.knockback = 3.0f;
            pc.attackDuration = 0.25f;
            pc.specialDuration = 0.5f;
            mc.speed = 3.0f;
            pc.attackOffset = new Vector3(0, 0, 0);
        }
        else if (selectedCharacter == 2)
        {
            // Archer
            pc.Role = "Archer";
            pc.health = 25.0f;
            pc.maxHealth = 25.0f;
            pc.mana = 0.0f;
            pc.maxMana = 30.0f;
            pc.damage = 4.0f;
            pc.knockback = 1.0f;
            pc.attackDuration = 0.5f;
            pc.specialDuration = 0.5f;
            mc.speed = 4.0f;
            pc.attackOffset = new Vector3(0.5f, 0.25f, 0);
        }
        pc.attackSpeed = 10.0f;

        // Setting up health bar
        HealthController hc = healthBar.GetComponent<HealthController>();
        pc.healthController = hc;
        hc.SetMaxHealth(pc.health);

        // Setting up mana bar
        ManaController manac = manaBar.GetComponent<ManaController>();
        pc.manaController = manac;
        manac.SetMaxMana(pc.mana, pc.maxMana);

        // Setting coins
        CoinsController cc = coins.GetComponent<CoinsController>();
        pc.coinsController = cc;

        // Setting backpack
        BackpackController bc = backpack.GetComponent<BackpackController>();
        pc.backpackController = bc;
    }
}
