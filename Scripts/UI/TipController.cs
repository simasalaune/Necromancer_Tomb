using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TipController : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Image image;
    private Vector2 move;
    private int currentTip;
    private Coroutine coroutine;

    void Start()
    {
        currentTip = 0;
        image = transform.GetComponent<Image>();
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (currentTip == 0)
        {
            move.x = Input.GetAxisRaw("Horizontal");
            move.y = Input.GetAxisRaw("Vertical");

            if (move.x != 0 || move.y != 0)
            {
                text.text = "To attack press left mouse button";
                currentTip = 1;
            }
        }
        else if (currentTip == 1)
        {
            if (Input.GetMouseButtonDown(0))
            {
                text.text = "Good luck!";
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
                coroutine = StartCoroutine(FadeOut());
            }
        }
        else if (currentTip == 2)
        {
            if (Input.GetMouseButtonDown(1))
            {
                text.text = "Fight safely!";
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
                coroutine = StartCoroutine(FadeOut());
            }
        }
    }

    public void SetTip(string tip, int index)
    {
        Color textColor = text.color;
        Color panelColor = image.color;
        textColor.a = 1;
        panelColor.a = 0.8f;
        text.color = textColor;
        image.color = panelColor;

        currentTip = index;
        text.text = tip;

        if (index == 10)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        currentTip = 10;
        yield return new WaitForSeconds(1.0f);

        while (text.color.a > image.color.a)
        {
            Color color = text.color;
            color.a -= 0.05f;
            text.color = color;
            yield return new WaitForSeconds(0.1f);
        }

        while (text.color.a > 0)
        {
            Color textColor = text.color;
            Color panelColor = image.color;
            textColor.a -= 0.05f;
            panelColor.a -= 0.05f;
            text.color = textColor;
            image.color = panelColor;
            yield return new WaitForSeconds(0.1f);
        }
        gameObject.SetActive(false);
    }
}
