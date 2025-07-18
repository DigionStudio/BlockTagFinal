using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class AbilityObject : MonoBehaviour
{
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private LayerMask layerMask;
    private List<BlockTile> blockSelectedList = new List<BlockTile>();
    private List<string> bombBlockList = new List<string>();
    [SerializeField] private GameObject hammerObject;
    [SerializeField] private SpriteRenderer hammerIconObject;
    [SerializeField] private GameObject hammerEffectShowObject;
    [SerializeField] private Animator hammerAnim;
    [SerializeField] private AudioSource hammerSFX;


    [SerializeField] private GameObject showAbilityObject;
    [SerializeField] private Transform animObjTrans;
    [SerializeField] private GameObject abilityEffect;
    [SerializeField] private GameObject freezeEffect;
    [SerializeField] private GameObject shockObj;



    [SerializeField] private GameObject bombObject;
    [SerializeField] private SpriteRenderer bombRenderer;
    [SerializeField] private GameObject bombEffect;
    [SerializeField] private Animator bombAnim;
    [SerializeField] private AudioSource bombAnimSfx;

    private BoardManager boardManager;
    private float timereturn;
    private List<BlockType> desBlockType = new List<BlockType>();

    public void SetUp(Vector2 pos)
    {
        if (boardManager == null)
        {
            boardManager = BoardManager.Instance;
            CrushTileCreator.ColorBombType += DesBlockType;
        }
        blockSelectedList.Clear();
        bombBlockList.Clear();
        desBlockType.Clear();
        timereturn = 0.5f;
        transform.localScale = Vector3.one * 0.3f;
        circleCollider.radius = 0;
        gameObject.SetActive(true);
        circleCollider.transform.tag = "Untagged";
        transform.position = pos;
        hammerObject.SetActive(false);
        hammerEffectShowObject.SetActive(false);
        hammerAnim.SetBool("IsHammer", false);
        hammerIconObject.enabled = true;
        circleCollider.enabled = false;
        showAbilityObject.SetActive(false);
        bombObject.SetActive(false);
        bombEffect.SetActive(false);
        bombAnim.SetBool("isBomb", false);
        bombRenderer.enabled = true;

    }
    private void OnDestroy()
    {
        CrushTileCreator.ColorBombType -= DesBlockType;
    }

    private void DesBlockType(BlockType type)
    {
        if (!desBlockType.Contains(type))
        {
            desBlockType.Add(type);
            timereturn += 1f;
        }
    }

    public void ChangePos(Vector2 pos)
    {
        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Tile") && !boardManager.HasTurn)
        {
            if (collision.gameObject.TryGetComponent<BlockTile>(out var block))
            {
                if (block != null)
                {
                    AddRemoveBlockTile(block, true);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Tile") && !boardManager.HasTurn)
        {
            if (collision.gameObject.TryGetComponent<BlockTile>(out var block))
            {
                if (block != null)
                {
                    AddRemoveBlockTile(block, false);
                }
            }
        }
    }

    private void AddRemoveBlockTile(BlockTile tile, bool isadd)
    {
        string code = tile.RowValue.ToString() + tile.ColumnValue.ToString();
        if (!isadd)
        {
            if (bombBlockList.Contains(code))
            {
                bombBlockList.Remove(code);
                blockSelectedList.Remove(tile);
            }
        }
        else
        {
            if (!bombBlockList.Contains(code))
            {
                blockSelectedList.Add(tile);
                bombBlockList.Add(code);
            }
        }
    }

    public void HammerAbilityActive(Vector2 pos)
    {
        hammerObject.SetActive(true);
        transform.DOScale(Vector3.one, 0.5f);
        transform.DOMove(pos, 0.5f).OnComplete(() =>
        {
            hammerAnim.SetBool("IsHammer", true);
            circleCollider.transform.tag = "Hammer";
            circleCollider.radius = 4.5f;

            Invoke(nameof(DestroyBlocks), 1.3f);
            Invoke(nameof(HammerEffect), 0.8f);
            HammerSFX();
        });
        

    }

    public void BombAbilityActive(Vector2 pos)
    {
        bombObject.SetActive(true);
        transform.DOScale(Vector3.one, 0.5f);
        transform.DOMove(pos, 0.5f).OnComplete(() =>
        {
            bombAnim.SetBool("isBomb", true);
            bombAnimSfx.Play();
            circleCollider.radius = 6f;
            circleCollider.transform.tag = "Damage";
            Invoke(nameof(BombEffect), 0.8f);
        });
        

    }

    private void HammerSFX()
    {
        hammerSFX.Play();
    }
    private void HammerEffect()
    {
        hammerEffectShowObject.SetActive(true);
    }
    private void BombEffect()
    {
        bombRenderer.enabled = false;
        bombEffect.SetActive(true);
        circleCollider.enabled = true;

        Invoke(nameof(DestroySelected), 0.3f);
        Invoke(nameof(DisableObj), 1f);
    }

    private void DestroyBlocks()
    {
        circleCollider.enabled = true;
        hammerIconObject.enabled = false;

        Invoke(nameof(DestroySelected), 0.2f);
        Invoke(nameof(DisableObj), 1f);
    }

    private void DestroySelected()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 5, Vector2.zero, 0, layerMask);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.TryGetComponent<BlockTile>(out var block))
            {
                string code = block.RowValue.ToString() + block.ColumnValue.ToString();
                if (!bombBlockList.Contains(code))
                {
                    blockSelectedList.Add(block);
                    bombBlockList.Add(code);
                }
            }
        }
        int num = 0;
        foreach (var item in blockSelectedList)
        {
            if (item != null)
            {
                num++;
                item.DestroyObject();
            }

        }
        boardManager.UpdatePoint(num);
    }

    private void DisableObj()
    {
        boardManager.CheckForDash(timereturn);
        DisableObject();
    }

    public void DisableObject()
    {
        Vector2 pos = new(12, -40);
        SetUp(pos);
        AbilityShowReset();
    }
    private void AnimOver()
    {
        if (abilityEffect != null)
        {
            abilityEffect.SetActive(true);
        }
        Invoke(nameof(DisableObject), 1f);

    }
    public void ShowAbility(Vector2 pos, bool isthunde, Sprite icon)
    {
        SetUp(pos);
        showAbilityObject.SetActive(true);
        transform.localScale = Vector2.one;
        if (isthunde)
        {
            ShowThunder(icon);
        }
        else
        {
            ShowFreeze(icon);
        }
    }
    private void AbilityShowReset()
    {
        gameObject.SetActive(false);
        abilityEffect.SetActive(false);
        freezeEffect.SetActive(false);
        shockObj.SetActive(false);
    }

    public void ShowThunder(Sprite icon)
    {
        ShowIconChange(icon);
        animObjTrans.localScale = Vector3.one * 2.1f;
        abilityEffect.transform.localScale = Vector3.one * 3f;
        transform.DOMove(new Vector3(10, 0, 0), 0.7f).OnComplete(() =>
        {
            animObjTrans.DOScale(2.4f, 0.3f).SetDelay(1f).OnComplete(() =>
            {
                animObjTrans.DOScale(0.1f, 0.3f);
                AnimOver();
            });
        });
    }

    public void ShowFreeze(Sprite icon)
    {
        ShowIconChange(icon);
        animObjTrans.localScale = Vector3.one * 2f;

        transform.DOMove(new Vector3(10, 0, 0), 0.7f).OnComplete(() =>
        {
            animObjTrans.DOScale(2.5f, 0.3f).OnComplete(() =>
            {
                animObjTrans.DOScale(1f, 0.3f).OnComplete(() =>
                {
                    animObjTrans.gameObject.SetActive(false);
                });
                freezeEffect.SetActive(true);
                Invoke(nameof(DisableObject), 1f);
            });
        });

    }

    private void ShowIconChange(Sprite icon)
    {
        animObjTrans.gameObject.GetComponent<SpriteRenderer>().sprite = icon;
        animObjTrans.gameObject.SetActive(true);
    }


}
