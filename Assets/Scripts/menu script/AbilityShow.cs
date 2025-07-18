using DG.Tweening;
using UnityEngine;

public class AbilityShow : MonoBehaviour
{
    [SerializeField] private Transform animObjTrans;
    private Vector3 pointB;
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private GameObject abilityEffect;
    [SerializeField] private GameObject shockObj;
   
    public void MoveInAbilityEffect(Vector3 pos2, Sprite icon)
    {
        shockObj.SetActive(true);
        pointB = pos2;
        pointB.z = 0f;
        ShowIconChange(icon);
        animObjTrans.localScale = Vector3.zero;
        animObjTrans.DOScale(0.4f, 0.5f).OnComplete(() =>
        {
            float num = transform.position.x / 3;
            pointB += Vector3.right * num;
        });
        Invoke(nameof(Move_Straght), 2f);

    }
    private void Move_Straght()
    {
        ResetForShow();
        animObjTrans.DOScale(0f, 0.8f);
        // Calculate the middle point for the parabolic movement
        //Vector3 midpoint2 = new Vector3(pointB.x, pointB.y + 1f, 0);

        // Create the path for the parabolic movement
        Vector3[] path = new Vector3[] { transform.position, pointB };

        // Use DOTween to move along the path
        transform.DOPath(path, 0.5f, PathType.CatmullRom)
                 .SetEase(Ease.Linear).SetEase(Ease.Linear).OnComplete(() =>
                 {
                     AnimOver();
                 });
    }

    private void AnimOver()
    {
        if (abilityEffect != null)
        {
            abilityEffect.SetActive(true);
        }
        Invoke(nameof(DestroyObject), 1f);

    }



    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void ShowIconChange(Sprite icon)
    {
        animObjTrans.gameObject.GetComponent<SpriteRenderer>().sprite = icon;
        animObjTrans.gameObject.SetActive(true);
    }

    private void ResetForShow()
    {
        shockObj.SetActive(false);
        circleCollider.enabled = false;
        rb.isKinematic = true;
    }
}
