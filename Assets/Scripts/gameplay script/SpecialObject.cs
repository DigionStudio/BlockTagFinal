using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialObject : MonoBehaviour
{
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private SpriteRenderer bgspriteRenderer;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject desEffect;
    private Special_Object_Type SpObjectType;
    public Special_Object_Type SPObjectType { get { return SpObjectType; } }

    private bool isSPDes;
    private int hitPoint;
    private BoardManager boardManager;
    private bool isActivated;
    private float desTime;
    public void SetUp(Sprite icon, Special_Object_Type sptype, int hitpoint)
    {
        if(boardManager == null)
        {
            boardManager = BoardManager.Instance;
        }
        desTime = 0;
        desEffect.SetActive(false);
        SpObjectType = sptype;
        spriteRenderer.sprite = icon;
        spriteRenderer.DOFade(1, 0.5f);
        hitPoint = hitpoint;
    }
    private void Update()
    {
        if (BoardManager.Instance.isGameStarted && !isSPDes)
        {
            if(desTime < 20)
            {
                desTime += Time.deltaTime;
            }
            else
            {
                DestroySpecialObject(false);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckTag(collision.tag);
    }


    private void CheckTag(string tag)
    {
        if (string.IsNullOrEmpty(tag)) return;

        if (string.Equals(tag, "Damage"))
        {
            if (CheckForDamage())
                DestroySpecialObject(true);
        }
        else if (string.Equals(tag, "Disturb"))
        {
            DestroySpecialObject(false);
        }else if (string.Equals(tag, "Tile"))
        {
            DestroySpecialObject(false);
            ActiveSpecialObject();
        }
    }
    public void DestroySpecialObject(bool isDes = false)
    {
        if (!isSPDes)
        {
            circleCollider.enabled = false;
            isSPDes = true;
            desEffect.SetActive(true);
            bgspriteRenderer.enabled = false;
            spriteRenderer.enabled = false;
            Invoke(nameof(DestroyObject), 1.5f);
            if (isDes)
            {
                GameManager.BlockDes.Invoke(Normal_Block_Type.none, BlockType.None, SpObjectType, transform.position);
            }
        }
    }
    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    private bool CheckForDamage()
    {
        bool isDes = false;
        if (transform.position.y < 15)
        {
            if(hitPoint > 1)
            {
                hitPoint--;
            }
            else
            {
                hitPoint = 0;
                isDes = true;
            }
        }
        return isDes;
    }

    private void ActiveSpecialObject()
    {
        if (!isActivated)
        {
            isActivated = true;
            boardManager.CheckForSpecialObject(SpObjectType, transform.position);
        }
    }

}
