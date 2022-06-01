using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopController : MonoBehaviour
{
    public TextMeshProUGUI textBox;
    public Sprite shopIcon;
    public Sprite dialogIcon;

    public Sprite healthPotion;
    public Sprite manaFlask;
    public Sprite boots;
    public Sprite powerShard;

    private RectTransform rectDialog;
    private RectTransform rextShop;
    private Vector2 dialogPosition;
    private Vector2 shopPosition;
    private Vector2 dialogScale;
    private Vector2 shopScale;
    private Vector2 textPosition;
    private GameObject canvas;
    private GameObject dialogPanel;
    private GameObject shopPanel;
    private GameObject button;
    private Transform player;
    private PlayerController pc;
    private BackpackController bc;
    private Coroutine coroutine;
    private bool openShop = false;
    private string[] options = { "Psss!.. You`ve got coins?", "Hey! Would I interest you with some upgrades? Shh.. Keep it quiet..", "Come closer stranger, I might have something especially for you.." };
    private string[] options2 = { "Those creatures have an interesting phenomena. I`m not a fighter though..", "What will I do with those things? Dont worry about that.. You want to trade or not?", "I don`t know you, you don`t know me. Understood?" };
    private string[] accepted = { "It was a pleasure doing business with you.", "Here you go. Anything else?" };
    private string[] rejected = { "Are you trying to scam me?..", "Sorry, you don`t have required ingredients.", "I am a vendor, not a charity!" };

    private float range = 2.0f;
    private float yOffset = 2.0f;

    void Start()
    {
        canvas = transform.GetChild(0).gameObject;
        dialogPanel = canvas.transform.GetChild(0).gameObject;
        shopPanel = canvas.transform.GetChild(1).gameObject;
        button = dialogPanel.transform.GetChild(0).gameObject;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        pc = player.GetComponent<PlayerController>();
        bc = GameObject.FindGameObjectWithTag("Backpack").GetComponent<BackpackController>();
        dialogPosition = new Vector2(transform.position.x, transform.position.y + yOffset);
        shopPosition = new Vector2(transform.position.x, transform.position.y + yOffset);
        dialogScale = new Vector2(365, 205);
        shopScale = new Vector2(520, 325);
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, player.position) < range)
        {
            if (!openShop)
            {
                openShop = true;
                pc.weapon.SetActive(false);
                pc.inShop = true;

                dialogPanel.transform.position = Camera.main.WorldToScreenPoint(dialogPosition);
                dialogPanel.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 10);
                dialogPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(275, 130);
                dialogPanel.GetComponent<Image>().sprite = dialogIcon;
                dialogPanel.GetComponent<RectTransform>().sizeDelta = dialogScale;
                shopPanel.transform.position = Camera.main.WorldToScreenPoint(shopPosition);
                canvas.SetActive(true);
                int rand = Random.Range(0, options.Length);
                coroutine = StartCoroutine(Print(textBox, options[rand], button));
            }
            else
            {
                dialogPanel.transform.position = Camera.main.WorldToScreenPoint(dialogPosition);
                shopPanel.transform.position = Camera.main.WorldToScreenPoint(shopPosition);
            }
        }
        else
        {
            openShop = false;
            if (pc.weapon != null)
            {
                pc.weapon.SetActive(true);
            }
            pc.inShop = false;
            shopPanel.SetActive(false);
            canvas.SetActive(false);
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
    }

    public void OnOpenShop()
    {
        button.SetActive(false);
        dialogPanel.GetComponent<Image>().sprite = shopIcon;
        dialogPanel.GetComponent<RectTransform>().sizeDelta = shopScale;
        dialogPanel.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -25);
        dialogPanel.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(300, 150);
        int rand = Random.Range(0, options2.Length);
        coroutine = StartCoroutine(Print(textBox, options2[rand], shopPanel));
    }

    public void BuyPotion()
    {
        int shroomCount = bc.SearchItem("JellyShroom");
        int coins = pc.coins;

        if (shroomCount >= 1 && coins >= 10)
        {
            coins -= 10;
            pc.SetCoins(coins);

            bc.RemoveItem("JellyShroom", 1);

            bc.AddItem(healthPotion, "Health potion");

            int rand = Random.Range(0, accepted.Length);
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(Print(textBox, accepted[rand], null));
        }
        else
        {
            int rand = Random.Range(0, rejected.Length);
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(Print(textBox, rejected[rand], null));
        }
    }

    public void BuyFlask()
    {
        int crystalCount = bc.SearchItem("Crystal");
        int coins = pc.coins;

        if (crystalCount >= 1 && coins >= 10)
        {
            coins -= 10;
            pc.SetCoins(coins);

            bc.RemoveItem("Crystal", 1);

            bc.AddItem(manaFlask, "Mana flask");

            int rand = Random.Range(0, accepted.Length);
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(Print(textBox, accepted[rand], null));
        }
        else
        {
            int rand = Random.Range(0, rejected.Length);
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(Print(textBox, rejected[rand], null));
        }
    }

    public void UpgradePower(GameObject gObject)
    {
        int shroomCount = bc.SearchItem("JellyShroom");
        int stingerCount = bc.SearchItem("Stinger");
        int crystalCount = bc.SearchItem("Crystal");

        if (shroomCount >= 1 && stingerCount >= 1 && crystalCount >= 1)
        {
            bc.RemoveItem("JellyShroom", 1);
            bc.RemoveItem("Stinger", 1);
            bc.RemoveItem("Crystal", 1);

            bc.AddItem(powerShard, "Shard of power");

            int rand = Random.Range(0, accepted.Length);
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(Print(textBox, accepted[rand], null));

            DisableUpgrade(gObject);
        }
        else
        {
            int rand = Random.Range(0, rejected.Length);
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(Print(textBox, rejected[rand], null));
        }
        
    }

    public void UpgradeBoots(GameObject gObject)
    {
        int shroomCount = bc.SearchItem("JellyShroom");
        int stingerCount = bc.SearchItem("Stinger");

        if (shroomCount >= 1 && stingerCount >= 3)
        {
            bc.RemoveItem("JellyShroom", 1);
            bc.RemoveItem("Stinger", 3);

            bc.AddItem(boots, "Boots of speed");

            DisableUpgrade(gObject);
        }
        else
        {
            int rand = Random.Range(0, rejected.Length);
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(Print(textBox, rejected[rand], null));
        }      
    }

    public void DisableUpgrade(GameObject gObject)
    {
        Image image = gObject.GetComponent<Image>();
        image.color = new Color(0.9f, 0.2f, 0.75f);
        Destroy(gObject.GetComponent<Button>());
    }

    IEnumerator Print(TextMeshProUGUI gui, string text, GameObject button)
    {
        gui.text = "";
        if (button != null)
        {
            button.SetActive(false);
        }
        
        string start = "<color=yellow>" + "Wandering vendor" + ": </color>";
        for (int i = 1; i <= text.Length; i++)
        {
            gui.text = start + text.Substring(0, i);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.5f);

        if (button != null)
        {
            button.SetActive(true);
        }    
    }

    [SerializeField]
    private Vector3 center = new Vector3(0f, 0f, 0f);

    [SerializeField] [Range(0f, 4f)] float lerpTime;
    [SerializeField] Vector3[] myPos;

    int posIndex = 0;
    int length;

    float t = 0f;

    private float cd = 0.0f;
    private Vector3 oldPosition;
    private Vector3 oldScale;

    [SerializeField]
    private ParticleSystem rain;

    // Start is called before the first frame update
    void Start()
    {
        length = myPos.Length;
        oldPosition = transform.position;
        oldScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (cd <= 10.0f)
        {
            StartCoroutine(MoveCloud());
            cd = Time.time + 1f;
        }
        if (cd > 10.0f && cd <= 20.0f)
        {
            StartCoroutine(CenterCloud(center));
            cd = Time.time + 1f;
        }
        if (cd > 20.0f && cd <= 30.0f)
        {
            StartCoroutine(BackCloud());
            cd = Time.time + 1f;
        }
        cd = Time.time + 1f;
        Debug.Log(cd);
    }
    private IEnumerator MoveCloud()
    {
        transform.position = Vector3.Lerp(transform.position, myPos[posIndex], lerpTime * Time.deltaTime);

        t = Mathf.Lerp(t, 1f, lerpTime * Time.deltaTime);

        if (t > 0.9f)
        {
            t = 0f;
            posIndex++;
            posIndex = (posIndex >= length) ? 0 : posIndex;
        }
        yield return new WaitForSeconds(1f);
    }
    private IEnumerator CenterCloud(Vector3 position)
    {
        transform.position = Vector3.Lerp(transform.position, position, lerpTime * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.5f, 1.5f, 1.5f), lerpTime * Time.deltaTime);
        if (!rain.isPlaying)
            rain.Play();
        yield return new WaitForSeconds(1f);
    }
    private IEnumerator BackCloud()
    {
        if (rain.isPlaying)
            rain.Stop();
        transform.position = Vector3.Lerp(transform.position, oldPosition, lerpTime * Time.deltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, oldScale, lerpTime * Time.deltaTime);
        yield return new WaitForSeconds(1f);
    }
}
