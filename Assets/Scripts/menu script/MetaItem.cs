using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MetaItem : MonoBehaviour
{
    private Image fillImage;
    private Image glowImage;
    private bool isFillAnimEffect;
    public bool HasFilled { get; private set; }
    private float fillValue;
    private float glowValue = 0;

    private void Awake()
    {
        if (!HasFilled && fillValue == 0)
        {
            GetImagesRefs();
            fillImage.fillAmount = 0;
            glowImage.DOFade(0, 0);
        }
    }
    private void GetImagesRefs()
    {

        if (fillImage == null)
            fillImage = transform.GetChild(0).GetComponent<Image>();
        if (transform.childCount > 1 && glowImage == null)
        {
            glowImage = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        }
    }
    public void Fill(int num)
    {
        GetImagesRefs();
        fillImage.fillAmount = num;
        float val = (120 * (float)num) / 255;
        glowImage.DOFade(val, 0);
        if (num == 1)
            HasFilled = true;
    }


    public void FillEffwct()
    {
        transform.DOScale(Vector2.one * 1.05f, 0.3f);
        Invoke(nameof(AnimEffect), 0.3f);
    }

    public void AnimEffect()
    {
        fillValue += 0.1f;
        glowValue += 12;
        glowImage.DOFade(glowValue/255f, 0.1f);
        fillImage.DOFillAmount(fillValue, 0.1f);
        transform.DOScale(Vector2.one, 0.1f);
    }
}
