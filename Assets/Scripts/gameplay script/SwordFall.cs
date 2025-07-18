using DG.Tweening;
using UnityEngine;

public class SwordFall : MonoBehaviour
{
    public LayerMask blockLayer;
    public Transform arrowHead;
    public GameObject pointEffect;
    public GameObject effectObj;
    public void SetUp(Vector2 finalPos)
    {
        effectObj.SetActive(false);
        transform.DOMove(finalPos, 0.7f).OnComplete(() =>
        {
            arrowHead.DOScale(Vector2.zero, 0.2f);
            EffectActive();

        });
    }

    private void EffectActive()
    {
        pointEffect.SetActive(false);
        effectObj.SetActive(true);
    }

    

    private bool CheckBlock(Vector2 pos)
    {
        bool isTile = false;
        RaycastHit2D hit = Physics2D.CircleCast(pos, 0.5f, Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.tag == "Tile")
            {
                isTile = true;
            }
        }
        return isTile;
    }
}
