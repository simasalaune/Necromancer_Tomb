using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinsController : MonoBehaviour
{
    private TextMeshProUGUI text;

    void Start()
    {
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();    
    }

    public void SetCoins(int coins)
    {
        text.text = coins.ToString();
    }
}
