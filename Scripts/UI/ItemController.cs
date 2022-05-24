using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    private GameObject slot;
    private GameObject special;
    private Vector2 pos;
    private int index;
    private GameObject canvas;
    private BackpackController bc;

    void Start()
    {
        slot = transform.parent.gameObject;
        special = slot.transform.GetChild(1).gameObject;
        pos = transform.position;
        
        GameObject target = transform.parent.parent.parent.parent.gameObject;
        bc = target.GetComponent<BackpackController>();
        canvas = target.transform.parent.parent.gameObject;

        for (int i = 0; i < bc.slotCount; i ++)
        {
            if (bc.slots[i] == slot)
            {
                index = i;
            }
        }
    }

    public void OnPointerClick(PointerEventData data)
    {
        if (special.activeSelf)
        {
            if (data.clickCount >= 2)
            {
                bc.UseItem(name);
            }
        }
    }

    public void OnBeginDrag(PointerEventData data)
    {
        slot.transform.SetSiblingIndex(100);
    }

    public void OnDrag(PointerEventData data)
    {
        transform.position = data.position;
    }

    public void OnEndDrag(PointerEventData data)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        canvas.GetComponent<GraphicRaycaster>().Raycast(data, results);
        int yndex = -1;
        foreach (RaycastResult result in results)
        {
            for (int i = 0; i < bc.slotCount; i++)
            {
                if (bc.slots[i] == result.gameObject)
                {
                    yndex = i;
                    break;
                }
            }
            if (yndex >= 0)
            {
                break;
            }
        }

        slot.transform.SetSiblingIndex(index);
        transform.position = pos;

        if (yndex >= 0 && index != yndex)
        {
            bc.SwapItems(index, yndex);
        }
    }
}
