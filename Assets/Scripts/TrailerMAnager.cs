using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TrailerMAnager : MonoBehaviour
{
    public Transform blockHolder;
    public CircleCollider2D circleCollider;
    private Transform colliderTrans;
    private bool isCollierExpand = false;
    private float maxNum = 25;
    private float currentNum = 0.01f;

    private List<AudioSource> audioSources = new List<AudioSource>();
    public ScrollRect scrollRect;
    private List<LevelShow> levelShow = new List<LevelShow>();
    public RectTransform levelHolderRect;
    private float speed = 0f;
    public Transform gradientTrans;
    private bool isSpeedUp = true;
    private bool isMove = true;
    private float multiplier = 0.001f;
    private bool speedDec = true;
    public AudioSource StarSound;
    //private void OnValidate()
    //{
    //    for (int i = 0; i < levelHolderRect.childCount; i++)
    //    {
    //        // Get the Transform component of the child at index 'i'
    //        Transform childTransform = levelHolderRect.GetChild(i);

    //        LevelShow lvshow = childTransform.GetComponent<LevelShow>();
    //        lvshow.Set_Level_Index(i);
    //        levelShow.Add(lvshow);
    //    }
    //}
    void Start()
    {
        colliderTrans = circleCollider.transform;
        //blockHolder.DOMoveY(0f, 1f).SetDelay(0.5f).OnComplete(() =>
        //{
        //    isCollierExpand = true;
        //    circleCollider.gameObject.SetActive(true);

        //});

        //foreach (Transform child in blockHolder)
        //{
        //    AudioSource source = child.GetComponent<AudioSource>();
        //    if (source != null)
        //    {
        //        audioSources.Add(source);
        //    }
        //}
        //StartCoroutine(PlayPopSFX());
        //Invoke(nameof(StartAnim), 1f);
        for (int i = 0; i < levelHolderRect.childCount; i++)
        {
            // Get the Transform component of the child at index 'i'
            Transform childTransform = levelHolderRect.GetChild(i);

            LevelShow lvshow = childTransform.GetComponent<LevelShow>();
            lvshow.Set_Level_Index(i);
            levelShow.Add(lvshow);
        }
        gradientTrans.DOMove(Vector2.up * 16, 3f).SetDelay(1f).SetEase(Ease.OutQuad);
        Invoke(nameof(ChangeMove), 3.5f);
    }
    private void ChangeMove()
    {
        //isSpeedUp = true;
        //speedDec = false;
        multiplier = 0.0001f;
    }

    private void StartAnim()
    {
        circleCollider.enabled = true;
        colliderTrans.DOScale(Vector2.one * 25, 1f);
    }

    IEnumerator PlayPopSFX()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < audioSources.Count; i++)
        {
            float time = UnityEngine.Random.Range(0.01f, 0.1f);
            AudioSource source = audioSources[i];
            if (source != null)
            {
                source.Play();
                yield return new WaitForSeconds(time);
            }
        }
    }

    private void FixedUpdate()
    {
        if(isSpeedUp && speed < 0.002f)
            speed += Time.deltaTime * multiplier;
        else
        {
            if (speedDec)
            {
                isSpeedUp = false;
                if (speed > 0)
                {
                    speed -= Time.deltaTime * 0.001f;
                }
            }
        }

        if(isMove)
            scrollRect.verticalNormalizedPosition += speed;
    }
    private void Update()
    {
        if (isCollierExpand)
        {
            if (currentNum < maxNum)
            {
                currentNum += Time.deltaTime * 10;
            }
            else
            {
                isCollierExpand = false;
            }

            circleCollider.radius = currentNum;

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Finish"))
        {
            LevelShow lvshow = collision.gameObject.GetComponentInParent<LevelShow>();
            lvshow.TrailerAnim(true, 3);
            if(speedDec)
                StarSound.Play();
        }

        if (collision.gameObject.CompareTag("Respawn"))
        {
            MetaItem item = collision.gameObject.GetComponent<MetaItem>();
            item.Fill(1);
        }
    }
}
