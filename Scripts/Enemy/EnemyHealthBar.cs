using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{

    public Slider slider;
    public Slider sliderEffect;
    public Vector3 offset;

    private EnemyController hp;

    [SerializeField]
    private float hurtSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        hp = GetComponentInParent<EnemyController>();
        sliderEffect.value = hp.maxHealth;
        sliderEffect.maxValue = hp.maxHealth;
        slider.maxValue = hp.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        slider.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + offset);
        sliderEffect.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + offset);

        slider.value = hp.health;


        if (sliderEffect.value > slider.value)
            sliderEffect.value -= hurtSpeed;
        else
            sliderEffect.value = slider.value;
        
    }
}
