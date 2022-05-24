using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour
{
    private Vector2 target;

    void Start()
    {
        Vector2 pos = transform.position;
        target = pos + Random.insideUnitCircle / 2;
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * 5);
        }
    }
}
