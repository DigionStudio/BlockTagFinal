using DG.Tweening;
using UnityEngine;

public class ColorBombEffect : MonoBehaviour
{
    private BlockTile Tile;
    private MetaItem metaItem;
    [SerializeField]private SpriteRenderer spriteRen;
    [SerializeField]private AudioSource fillSfx;
    private Transform originTrans;
    public void TweenMove(Vector2 pos, BlockTile tile)
    {
        Tile = tile;
        transform.DOMove(pos, 0.4f).OnComplete(() =>
        {
            Tile.ColorBombMod();
            Invoke(nameof(DestroyObj), 0.3f);

        });

    }

    public void SetUp(Sprite icon, Transform origin)
    {
        originTrans = origin;
        if (spriteRen != null)
        {
            spriteRen.sprite = icon;
        }
        transform.DOScale(Vector3.one, 0.4f);
    }

    public void MetaFillTweenMove(MetaItem item)
    {
        metaItem = item;
        if(originTrans != null)
            transform.DOMove(originTrans.position, 0.6f);
        metaItem.FillEffwct();
        Invoke(nameof(ModSetup), 0.4f);
        
    }
    private void ModSetup()
    {
        fillSfx.Play();
        spriteRen.DOFade(0, 0.2f).OnComplete(() =>
        {
            DestroyObj();
        });
    }

    private void DestroyObj()
    {
        Destroy(gameObject);
    }
}
