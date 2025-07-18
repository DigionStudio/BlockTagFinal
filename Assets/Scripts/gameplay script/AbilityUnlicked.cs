using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUnlicked : MonoBehaviour
{
    [SerializeField] private Image bgImage;
    [SerializeField] private GameObject conffetiEffect;
    [SerializeField] private Image[] iconImage;
    [SerializeField] private Transform iconTrans;
    [SerializeField] private Image lockImage;
    [SerializeField] private Sprite[] lockSprite;

    [SerializeField] private GameObject infoTextPanel;
    [SerializeField] private Text nameText;
    [SerializeField] private Text infoText;



    private Transform finalTrans;
    private bool isAnimated = false;
    void Awake()
    {
        bgImage.gameObject.SetActive(false);
        conffetiEffect.SetActive(false);
        infoTextPanel.SetActive(false);
    }

    public void AbilityUnlocked(Transform finaltrans, Sprite icon, string name, string working)
    {
        if (!isAnimated)
        {
            isAnimated = true;
            finalTrans = finaltrans;
            nameText.text = name;
            infoText.text = working;
            bgImage.color = new Color(0, 0, 0, 0);
            bgImage.gameObject.SetActive(true);
            iconTrans.gameObject.SetActive(false);
            lockImage.sprite = lockSprite[0];
            foreach (var im in iconImage)
            {
                im.sprite = icon;
            }
            float time = 3f;
            if (!GameAIManager.Instance.HasAsists)
            {
                time = 1.5f;
            }
            Invoke(nameof(BGSetUp), time);
        }
    }

    private void BGSetUp()
    {
        bgImage.DOFade(0.8f, 1f).OnComplete(() =>
        {
            StartUnlockAnim();
        });
    }

    private void StartUnlockAnim()
    {
        infoTextPanel.SetActive(true);
        iconTrans.gameObject.SetActive(true);
        CheckGameStatus(false);
        Invoke(nameof(ChangeLockSprite), 0.5f);
        
    }
    private void ChangeLockSprite()
    {
        lockImage.sprite = lockSprite[1];
        conffetiEffect.SetActive(true);
        iconImage[1].DOFade(0, 1f).OnComplete(() =>
        {
            CheckGameStatus(false);
            lockImage.gameObject.SetActive(false);
            Invoke(nameof(MoveImage), 2f);
        });
    }
    private void MoveImage()
    {
        infoTextPanel.SetActive(false);
        iconTrans.DOMove(finalTrans.position, 1f);
        iconTrans.DOScale(0.4f, 1f);
        Invoke(nameof(ResetUnlocked), 1.2f);
    }

    private void ResetUnlocked()
    {
        bgImage.gameObject.SetActive(false);
        CheckGameStatus(true);
    }

    private void CheckGameStatus(bool status)
    {
        BoardManager.Instance.GameStatus(status);
    }
}
