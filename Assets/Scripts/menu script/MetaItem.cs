using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MetaItem : MonoBehaviour
{
    private Image fillImage;
    private bool isFillAnimEffect;
    public bool HasFilled { get; private set; }
    private float fillValue;

    private void Awake()
    {
        if (!HasFilled && fillValue == 0)
        {
            if (fillImage == null)
                fillImage = transform.GetChild(0).GetComponent<Image>();
            fillImage.fillAmount = 0;
        }
    }
    public void Fill(int num)
    {
        if (fillImage == null)
            fillImage = transform.GetChild(0).GetComponent<Image>();
        fillImage.fillAmount = num;
        if(num == 1)
            HasFilled = true;
    }


    public void FillEffwct()
    {
        transform.DOScale(Vector2.one * 1.05f, 0.3f);
        Invoke(nameof(AnimEffect), 0.5f);
    }

    public void AnimEffect()
    {
        fillValue += 0.1f;
        fillImage.DOFillAmount(fillValue, 0.1f);
        transform.DOScale(Vector2.one, 0.1f);
    }
}
