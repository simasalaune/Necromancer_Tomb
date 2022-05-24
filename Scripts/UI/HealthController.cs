using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HealthController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject textObject;
    public Slider slider;
    public TextMeshProUGUI text;

    private Color damagedColor;
    private Color normalColor;
    private Color healedColor;

    void Start()
    {
        normalColor = new Color(1.0f, 1.0f, 1.0f);
    }

    public void SetMaxHealth(float hp)
    {
        slider.maxValue = hp;
        slider.value = hp;

        text.text = hp + "/" + hp;
    }

    public void SetHealth(float hp)
    {
        slider.value = hp;

        text.text = hp + "/" + slider.maxValue;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        textObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData data)
    {
        textObject.SetActive(false);
    }

    // Method for smooth fading in color
    public IEnumerator Fading(Color color, bool heal)
    {
        if (heal)
        {
            Image image = transform.GetChild(0).GetComponent<Image>();
            image.color = color;

            for (int i = 0; i < 20; i++)
            {
                image.color = new Color(image.color.r, image.color.g + 0.03f, image.color.b + 0.03f);
                yield return new WaitForSeconds(0.05f);
            }
            image.color = normalColor;
        }
        else
        {
            int n = transform.childCount - 1;
            Image[] images = new Image[n];
            for (int i = 0; i < n; i++)
            {
                images[i] = transform.GetChild(i).GetComponent<Image>();
            }

            for (int i = 0; i < n; i++)
            {
                images[i].color = color;
            }

            for (int j = 0; j < 20; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    images[i].color = new Color(images[i].color.r + 0.02f, images[i].color.g + 0.02f, images[i].color.b + 0.02f);
                }
                yield return new WaitForSeconds(0.05f);
            }

            for (int i = 0; i < n; i++)
            {
                images[i].color = normalColor;
            }
        }
    }
}
