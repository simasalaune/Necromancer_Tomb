using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BackpackController : MonoBehaviour
{
    [HideInInspector]
    public GameObject[] slots;
    [HideInInspector]
    public int slotCount = 8;
    [HideInInspector]
    public GameObject hover;
    [HideInInspector]
    public bool inBackpack = false;

    public Sprite openSprite;
    public Sprite closedSprite;

    private int[] counts;
    private GameObject backpack;
    private GameObject button;
    private GameObject newItem;
    private GameObject tip;

    private bool open = false;
    
    void Start()
    {
        hover = GameObject.FindGameObjectWithTag("Hover");
        tip = GameObject.FindGameObjectWithTag("Tip");
        hover.SetActive(false);
        slots = new GameObject[slotCount];
        counts = new int[slotCount];
        button = transform.GetChild(0).gameObject;
        newItem = button.transform.GetChild(0).gameObject;
        backpack = transform.GetChild(1).gameObject;

        GameObject panel = backpack.transform.GetChild(0).gameObject;

        for (int i = 0; i < slotCount; i ++)
        {
            slots[i] = panel.transform.GetChild(i).gameObject;
            counts[i] = 0;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            BackpackOpen();
        }

        if (hover.activeSelf)
        {
            inBackpack = true;
        }
        else
        {
            inBackpack = false;
        }
    }
 
    public void AddItem(Sprite sprite, string name)
    {
        if (!open)
        {
            newItem.SetActive(true);
        }

        int index = -1;
        for (int i = 0; i < slotCount; i ++)
        {
            Image item = slots[i].transform.GetChild(0).GetComponent<Image>();
            if (item.sprite == sprite)
            {
                index = i;
                break;
            }
        }

        if (IsFull())
        {
            tip.SetActive(true);
            tip.GetComponent<TipController>().SetTip("Your backpack is full!", 10);
            return;
        }

        if (index == -1)
        {
            // Item type is not on inventory
            for (int i = 0; i < slotCount; i++)
            {
                // Find an empty slot in inventory
                if (counts[i] == 0)
                {
                    Image item = slots[i].transform.GetChild(0).GetComponent<Image>();
                    GameObject special = slots[i].transform.GetChild(1).gameObject;
                    TextMeshProUGUI count = item.transform.GetChild(0).GetComponent<TextMeshProUGUI>();;

                    if (name == "Health potion" || name == "Mana flask" || name == "Shard of power" || name == "Boots of speed")
                    {
                        special.SetActive(true);
                    }

                    ChangeScale(item.rectTransform, sprite.rect.width, sprite.rect.height);

                    item.sprite = sprite;
                    item.name = name;
                    item.gameObject.SetActive(true);
                    counts[i] = 1;
                    break;
                }
            }
        }
        else
        {
            // Item type is already in inventory
            counts[index]++;
            TextMeshProUGUI count = slots[index].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            count.text = counts[index].ToString();
        }
    }

    public void RemoveItem(string itemName, int count)
    {
        int index = -1;
        for (int i = 0; i < slotCount; i++)
        {
            string name = slots[i].transform.GetChild(0).name;
            if (itemName == name)
            {
                index = i;
                break;
            }
        }

        counts[index] -= count;
        TextMeshProUGUI text = slots[index].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        if (counts[index] == 0)
        {
            Image item = slots[index].transform.GetChild(0).GetComponent<Image>();
            GameObject special = slots[index].transform.GetChild(1).gameObject;
            item.name = "Item";
            text.text = "";
            item.sprite = null;
            item.gameObject.SetActive(false);
            special.SetActive(false);
            hover.SetActive(false);

            NullScale(item.rectTransform);
        }
        else
        {     
            if (counts[index] == 1)
            {
                text.text = "";
            }
            else
            {
                text.text = counts[index].ToString();
            }
        }
    }

    public void SwapItems(int id, int yd)
    {
        GameObject ci = slots[id].transform.GetChild(0).gameObject;
        GameObject cy = slots[yd].transform.GetChild(0).gameObject;

        GameObject spi = slots[id].transform.GetChild(1).gameObject;
        GameObject spy = slots[yd].transform.GetChild(1).gameObject;

        bool spec = spi.activeSelf;
        spi.SetActive(spy.activeSelf);
        spy.SetActive(spec);

        Image si = ci.GetComponent<Image>();
        Image sy = cy.GetComponent<Image>();

        Sprite sprite = si.sprite;
        si.sprite = sy.sprite;
        sy.sprite = sprite;

        TextMeshProUGUI ti = ci.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ty = cy.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        string text = ti.text;
        ti.text = ty.text;
        ty.text = text;

        string name = ci.name;
        ci.name = cy.name;
        cy.name = name;

        
        if (counts[yd] == 0)
        {
            ChangeScale(sy.rectTransform, sy.sprite.rect.width, sy.sprite.rect.height);
            cy.SetActive(true);
            ci.SetActive(false);
        }
        else
        {
            ChangeScale(si.rectTransform, si.sprite.rect.width, si.sprite.rect.height);
            ChangeScale(sy.rectTransform, sy.sprite.rect.width, sy.sprite.rect.height);
            cy.SetActive(true);
            ci.SetActive(true);
        }

        int count = counts[id];
        counts[id] = counts[yd];
        counts[yd] = count;
    }

    public int SearchItem(string itemName)
    {
        int index = -1;
        for (int i = 0; i < slotCount; i ++)
        {
            string name = slots[i].transform.GetChild(0).name;
            if (itemName == name)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            return 0;
        }
        else
        {
            return counts[index];
        } 
    }

    public void UseItem(string name)
    {
        PlayerController pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        MovementController mc = GameObject.FindGameObjectWithTag("Player").GetComponent<MovementController>();

        if (name ==  "Health potion")
        {
            RemoveItem(name, 1);

            pc.OnPlayerHealed(10.0f);  
        }
        else if (name == "Mana flask")
        {
            if (pc.hasSpecial)
            {
                RemoveItem(name, 1);

                pc.OnPlayerRestoredMana(true);  
            }
            else
            {
                tip.SetActive(true);
                tip.GetComponent<TipController>().SetTip("You have to learn your special ability first!", 10);
            }
        }
        else if (name == "Shard of power")
        {
            RemoveItem(name, 1);

            tip.SetActive(true);
            tip.GetComponent<TipController>().SetTip("You have learnt your special ability!\nReplenish your mana and use it by pressing right mouse button", 2);

            StartCoroutine(pc.RestoreMana());
            pc.OnPlayerRestoredMana(false);
            pc.hasSpecial = true;
        }
        else if (name == "Boots of speed")
        {
            RemoveItem(name, 1);

            mc.speed += 1;
        }
    }

    public void ChangeScale(RectTransform transform, float width, float height)
    {
        float widthScale = 0;
        float heightScale = 0;

        if (width > height)
        {
            widthScale = 50;
            heightScale = 50 * height / width;
        }
        else if (height > width)
        {
            heightScale = 50;
            widthScale = 50 * width / height;
        }

        transform.sizeDelta = new Vector2(widthScale, heightScale);
    }

    public void NullScale(RectTransform transform)
    {
        transform.sizeDelta = new Vector2(50, 50);
    }

    public bool IsFull()
    {
        bool full = true;
        for (int i = 0; i < slotCount; i++)
        {
            if (counts[i] == 0)
            {
                full = false;
                break;
            }
        }
        return full;
    }

    public void BackpackOpen()
    {
        if (open)
        {
            open = false;
            button.transform.GetComponent<Image>().sprite = closedSprite;
            hover.SetActive(false);
        }
        else
        {
            open = true;
            button.transform.GetComponent<Image>().sprite = openSprite;
        }

        backpack.SetActive(open);
        if (newItem.activeSelf)
        {
            newItem.SetActive(false);
        }
    }
}
