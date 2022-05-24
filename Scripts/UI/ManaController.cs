using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ManaController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject textObject;
    public Slider slider;
    public TextMeshProUGUI text;
    public GameObject border;
    public GameObject hover;

    private Color damagedColor;
    private Color normalColor;

    void Start()
    {
        normalColor = new Color(1.0f, 1.0f, 1.0f);
        damagedColor = new Color(0.83f, 0.54f, 0.54f);
    }
    public void SetMaxMana(float mana, float maxMana)
    {
        slider.maxValue = maxMana;
        slider.value = mana;

        text.text = mana + "/" + maxMana;
    }

    public void SetMana(float mana)
    {
        slider.value = mana;

        text.text = mana + "/" + slider.maxValue;
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
    public IEnumerator NotEnoughMana()
    {
        Image image = border.GetComponent<Image>();
        image.color = damagedColor;
        for (int j = 0; j < 20; j++)
        {
            image.color = new Color(image.color.r + 0.01f, image.color.g + 0.03f, image.color.b + 0.03f);
            yield return new WaitForSeconds(0.05f);
        }
        image.color = normalColor;
    }
}
