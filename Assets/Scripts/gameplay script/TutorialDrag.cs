using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDrag : MonoBehaviour
{
    private Animator cursorAnimator;
    private Transform cursorHoder;


    private GameObject tileObjrct;
    private Transform tagHolder;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private int maxitter;

    private bool isMove;
    private float time;
    private float currentTime;
    private Vector3 currentStarPosition;
    private Vector3 currentEndPosition;
    void Start()
    {
        cursorHoder = GetComponent<Transform>();
        cursorAnimator = GetComponent<Animator>();
    }

    public void SetUp(GameObject tileobjrct, Transform tagholder, int maxi)
    {
        if(cursorHoder == null)
            cursorHoder = GetComponent<Transform>();

        if(cursorAnimator == null)
            cursorAnimator = GetComponent<Animator>();


        tileObjrct = tileobjrct;
        tagHolder = tagholder;
        startPosition = Vector3.zero;
        cursorHoder.localPosition = Vector3.zero;
        endPosition = startPosition + Vector3.up * 19;
        maxitter = maxi;
        StartCoroutine(StartPositionAnimationLoopCO());
    }

    public void ResetPos()
    {
        if (cursorHoder == null)
            cursorHoder = GetComponent<Transform>();
        isMove = false;
        cursorHoder.transform.localPosition = Vector2.zero;
    }
    private IEnumerator StartPositionAnimationLoopCO()
    {
        tileObjrct.transform.localPosition = Vector3.zero;
        currentTime = 0;
        time = 0.3f;
        currentStarPosition = transform.localPosition;
        currentEndPosition = startPosition;
        isMove = true;
        yield return new WaitForSeconds(0.3f);
        isMove = false;

        while (maxitter > 0)
        {
            maxitter--;
            tagHolder.transform.localPosition = Vector3.zero;
            cursorAnimator.SetBool("isDown", true);
            yield return new WaitForSeconds(1f);
            tileObjrct.SetActive(true);
            tileObjrct.transform.DOScale(1, 0.5f);
            Vector2 pos = (Vector2)tagHolder.position + Vector2.up * 7f;
            tagHolder.transform.DOMove(pos, 0.5f);
            yield return new WaitForSeconds(0.6f);
            currentTime = 0;
            currentStarPosition = startPosition;
            currentEndPosition = endPosition;
            time = 2f;
            isMove = true;
            yield return new WaitForSeconds(2f);
            isMove = false;
            cursorAnimator.SetBool("isDown", false);
            yield return new WaitForSeconds(0.6f);
            tileObjrct.SetActive(false);
            currentTime = 0;
            time = 0.3f;
            currentStarPosition = endPosition;
            currentEndPosition = startPosition;
            isMove = true;
            yield return new WaitForSeconds(0.3f);
            isMove = false;
        }
        if(maxitter <= 0)
        {
            cursorHoder.transform.position = startPosition;
        }
    }
    
    private void Update()
    {
        if (isMove)
        {
            if(currentTime < time)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                currentTime = time;
                isMove = false;
            }
            float interpolation = currentTime/ time;
            Vector3 pos = Vector3.Lerp(currentStarPosition, currentEndPosition, interpolation);
            cursorHoder.transform.localPosition = pos;
        }
    }


}
