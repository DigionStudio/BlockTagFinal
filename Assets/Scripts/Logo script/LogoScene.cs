using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoScene : MonoBehaviour
{
    public Transform holder;
    public Transform nameTrans;

    void Start()
    {
        Vector2 finalPos = holder.transform.position;
        holder.DOMove(new Vector2(1.3f, 20f), 0f);
        holder.DOMove(finalPos, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
