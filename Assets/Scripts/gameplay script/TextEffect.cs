using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TextEffect : MonoBehaviour
{
    [SerializeField] private UpperObject[] textEffects;
    private int currentEffectIndex = -1;
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
            effect.holder.SetActive(false);
            effect.addEffect.SetActive(false);
        }
    }

    public void ShowEffect(int num, Vector2 pos, bool isEffect)
    {
        startAudioEffect.Play();
        if(pos.x < 4)
        {
            pos.x = 4f;
        }
        if (pos.x > 16)
        {
            pos.x = 16f;
        }
        if(pos.y < -11)
        {
            pos.y = -11;
        }
        if(pos.y > 11)
        {
            pos.y = 11;
        }
        if(!isEffect)
            transform.position = pos;
        else
        {
            transform.position = new Vector2(10, 1);
        }
        if(num >= 0)
        {
            currentEffectIndex = num;
        }
        textEffects[currentEffectIndex].addEffect.SetActive(isEffect);
        textEffects[currentEffectIndex].holder.SetActive(true);

        CancelInvoke(nameof(DiableEffect));
        Invoke(nameof(DiableEffect), 1.4f);
    }

    private void OffEffect()
    {
        if (currentEffectIndex >= 0)
        {
            textEffects[currentEffectIndex].addEffect.SetActive(false);
            textEffects[currentEffectIndex].holder.SetActive(false);
            currentEffectIndex = -1;
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
