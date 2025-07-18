using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEffect : MonoBehaviour
{
    [SerializeField] private GameObject[] textEffects;
    private GameObject currentEffect;
    [SerializeField] private AudioSource startAudioEffect;
    [SerializeField] private AudioSource endAudioEffect;
    void Start()
    {
        SetUpEffect();
    }

    public void StartGame()
    {
        startAudioEffect.enabled = true;
        endAudioEffect.enabled = true;
        SetUpEffect();
    }

    private void SetUpEffect()
    {
        foreach (var effect in textEffects)
        {
            effect.gameObject.SetActive(false);
        }
    }

    public void ShowEffect(int num, Vector2 pos)
    {
        startAudioEffect.Play();
        if (currentEffect!= null)
        {
            currentEffect.SetActive(false);
        }
        if(pos.x < 3)
        {
            pos.x = 3f;
        }
        if (pos.x > 17)
        {
            pos.x = 17f;
        }
        if(pos.y < -12)
        {
            pos.y = -12;
        }
        if(pos.y > 12)
        {
            pos.y = 12;
        }
        transform.position = pos;
        currentEffect = textEffects[num];
        currentEffect.SetActive(true);

        CancelInvoke(nameof(DiableEffect));
        Invoke(nameof(DiableEffect), 1.3f);
    }

    private void OffEffect()
    {
        if (currentEffect != null)
        {
            currentEffect.SetActive(false);
        }
    }
    public void GameEnd()
    {
        startAudioEffect.enabled = false;
        endAudioEffect.enabled = false;
        SetUpEffect();
    }

    public void DiableEffect()
    {
        if(endAudioEffect.isActiveAndEnabled)
            endAudioEffect.Play();
        Invoke(nameof(OffEffect), 0.7f);

    }
}
