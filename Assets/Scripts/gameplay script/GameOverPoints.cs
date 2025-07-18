using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameOverPoints : MonoBehaviour
{
    public Text text;
    private int valueInt;
    private int prevhighScoreInt;
    private int countInt;
    private int addValue;
    private bool isEffect;

    public void SetUp(int value, int prevValue = 0)
    {
        valueInt = value;
        countInt = prevValue;
        addValue = (value - prevValue) / 200;
        isEffect = true;
        InvokeRepeating(nameof(AddValueEffect), 0, 0.05f);
        Invoke(nameof(CompleteEffect), 2f);
    }

    private void AddValueEffect()
    {
        if (isEffect)
        {
            if (countInt < valueInt - addValue)
            {
                countInt += addValue;
            }
            else
            {
                countInt = valueInt;
                isEffect = false;
            }

            text.text = countInt.ToString();
        }
        else
        {
            CancelInvoke(nameof(AddValueEffect));
            CompleteEffect();
        }
    }

    private void CompleteEffect()
    {
        if (!isEffect) return;
        CancelInvoke(nameof(AddValueEffect));
        isEffect = false;
        text.text = valueInt.ToString();
    }
   
}
