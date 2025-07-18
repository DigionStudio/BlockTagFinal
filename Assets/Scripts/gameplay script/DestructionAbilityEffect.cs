using DG.Tweening;
using UnityEngine;

public class DestructionAbilityEffect : MonoBehaviour
{
    public GameObject obj;
    public GameObject circleCollider;
    public GameObject effectObj;
    private Vector2 pointB;
    private float duration = 1f;
    public void SetUpDesEffect(Vector2 pos1, Vector2 pos2)
    {
        obj.SetActive(false);
        circleCollider.SetActive(false);
        effectObj.SetActive(false);
        transform.position = pos1;
        float time = duration/4;
        pointB = pos2;
        //CalculateAngle(pos1, pointB, time);
    }
    public void StartSetup()
    {
        obj.SetActive(true);

        transform.DOMove(pointB, duration).OnComplete(() =>
        {
            //effectObj.SetActive(true);
            Invoke(nameof(ColliderActive), 0.2f);

        });
    }

    private void CalculateAngle(Vector2 pos1, Vector2 pos2, float time)
    {
        // Calculate the direction vector from the target to the point
        Vector2 direction = pos2 - pos1;

        // Calculate the angle in degrees between the target's current forward direction and the target point
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
        // Assign the calculated rotation to the target's Transform
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        transform.DORotateQuaternion(rotation, time);
    }

    private void ColliderActive()
    {
        circleCollider.SetActive(true);
        Invoke(nameof(DestroyObject), duration);

    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
