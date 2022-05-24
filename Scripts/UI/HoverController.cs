using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HoverController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject hover;
    private BackpackController bc;
    private ItemController ic;
    private TextMeshProUGUI text;
    private bool hovering = false;

    private float offset = 20.0f;

    void Start()
    {
        bc = GameObject.FindGameObjectWithTag("Backpack").GetComponent<BackpackController>();
        hover = bc.hover;
        text = hover.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        ic = transform.GetComponent<ItemController>();
    }

    void Update()
    {
        if (hovering)
        {
            hover.transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y + offset);
        }
    }

    public void OnPointerEnter(PointerEventData data)
    {
        text.text = name;
        if (ic != null)
        {
            GameObject special = transform.parent.GetChild(1).gameObject;
            if (special.activeSelf)
            {
                text.text += "<color=green>" + " [Use]" + "</color>";
            }
        }    
        hovering = true;
        hover.SetActive(true);
    }

    public void OnPointerExit(PointerEventData data)
    {
        hovering = false;
        hover.SetActive(false);
    }
}
