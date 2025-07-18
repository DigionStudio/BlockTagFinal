using DG.Tweening;
using UnityEngine;

public class PointShow : MonoBehaviour
{
    [SerializeField] private Transform animTrans;
    private Vector3 pointB;
    private float duration = 1f;
    private float timeElapsed;
    private float lerpValue;

    public GameObject[] obj;
    private AudioSource audioSource;
    [SerializeField] private GameObject abilityEffect;

    private float num = 0.3f;
    public void SetUp(Vector3 pos2, int code)
    {
        pointB = pos2;
        pointB.z = 0f;
        if(code < obj.Length)
            obj[code].SetActive(true);
        Invoke(nameof(DestroyObject), duration);
        transform.DOScale(0.8f, duration + 0.5f);
        transform.DOMove(pointB, duration);

    }

    public void MoveInParabola(Vector3 pos2, int code)
    {
        audioSource = GetComponent<AudioSource>();
        pointB = pos2;
        pointB.z = 0f;
        if (code < obj.Length)
            obj[code].SetActive(true);
        if (code == 1)
        {
            num *= -1f;
        }
        transform.DOScale(0.26f, 0.5f).OnComplete(() =>
        {
            Invoke(nameof(Move), 0.5f);
            
        });
        
    }

    public void MoveInAbilityEffect(Vector3 pos2, Sprite icon)
    {
        audioSource = GetComponent<AudioSource>();
        pointB = pos2;
        pointB.z = 0f;
        obj[0].GetComponent<SpriteRenderer>().sprite = icon;
        obj[0].SetActive(true);

        animTrans.DOScale(0.3f, 0.3f);
        Invoke(nameof(Move_Straght), 2.3f);

    }
    private void Move()
    {
        audioSource.Play();
        Invoke(nameof(DestroyObject), 0.5f);
        transform.DOScale(0.23f, 0.5f);
        // Calculate the middle point for the parabolic movement
        Vector3 midpoint2 = new Vector3(pointB.x + num, pointB.y + 0.05f, 0);

        // Create the path for the parabolic movement
        Vector3[] path = new Vector3[] { transform.position, midpoint2, pointB };

        // Use DOTween to move along the path
        transform.DOPath(path, 0.5f, PathType.CatmullRom);
    }
    private void Move_Straght()
    {
        animTrans.DOScale(0f, 0.5f);
        audioSource.Play();
        // Calculate the middle point for the parabolic movement
        Vector3 midpoint2 = new Vector3(pointB.x, pointB.y + 1f, 0);

        // Create the path for the parabolic movement
        Vector3[] path = new Vector3[] { transform.position, midpoint2, pointB };

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
}
