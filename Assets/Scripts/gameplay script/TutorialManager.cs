using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    [SerializeField] private Transform cursorHoder;
    [SerializeField] private Transform cursorIcon;
    [SerializeField] private Transform tagHolder;
    [SerializeField] private CrushTileCreator tileCreator;
    [SerializeField] private Button skipButton;
    [SerializeField] private GameObject tutorialTextImageObj;

    [SerializeField] private Button[] Button;
    private int buttonPressId = -1;
    private Vector2 pos;

    [SerializeField] private Animator cursorAnimator;
    private int tuteCount = 3;
    private int currentShapeIndex;
    int index = 2;
    public int Index { get { return index; } }
    [SerializeField] private TutorialDrag tutorialDrag;
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject tutorialCutoffMain;

    [SerializeField] private GameObject[] tutorialTextObj;

    private BoardManager boardManager;
    private ShapeCreator shapeCreator;
    private GameDataManager gameDataManager;
    public bool isTutorial;
    Vector3 startPosition;
    Vector3 endPosition;
    private GameObject tileObjrct;
    private Tween dragTween;
    private int prefNum;
    private bool isLastStep;
    private int lastStepCount = 1;
    public bool DragTweenStatus { get; set; }

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        boardManager = BoardManager.Instance;
        gameDataManager = boardManager.gameDataManager;
        shapeCreator = ShapeCreator.Instance;
        DisableTutorial();
    }
    private void DisableTutorial()
    {
        tutorialTextImageObj.SetActive(false);
        cursorIcon.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        cursorHoder.gameObject.SetActive(false);
        tutorialPanel.SetActive(false);
        tutorialCutoffMain.SetActive(false);
        ChangeTutorialTextObject();
    }

    public void CheckTutorialStatus()
    {
        if (isTutorial)
        {
            DisableTutorial();
        }
    }

    public void IntroPanel()
    {
        TutorialActive();
    }

    private void TutorialActive()
    {
        prefNum = PlayerPrefs.GetInt(gameDataManager.TutorialPref);
        if (prefNum < 3 && gameDataManager.currentLevel < 3)
        {
            isTutorial = true;
            skipButton.onClick.AddListener(SkipTutorial);
        }
    }



    private void DragTween()
    {
        if (index >= 0)
        {
            ButtonsStatus(0);
            if (index > 0)
            {
                index--;
                shapeCreator.TutorialStatusSet(index);
            }
            if (tileObjrct != null)
            {
                Destroy(tileObjrct);
            }
            int shapeCode = shapeCreator.ShapeCode(index);
            int num = shapeCreator.ShapeCode2(index);
            tileObjrct = InstaCrushTile(shapeCode, index, num);
            cursorHoder.position = startPosition;


        }
    }

    private GameObject InstaCrushTile(int shapeCode, int index, int num)
    {
        CrushTileCreator tilecreator = Instantiate(tileCreator, cursorHoder);
        tilecreator.MakeShape(shapeCode, num);
        tilecreator.ChangeTileAlpha();
        GameObject crushTile = tilecreator.transform.GetChild(0).gameObject;
        float rotValue = shapeCreator.RotValue(index);
        Destroy(tilecreator.gameObject);
        crushTile.SetActive(false);
        crushTile.transform.localScale = Vector3.one * 0.5f;
        crushTile.transform.position = Vector3.zero;
        tagHolder.position = Vector3.zero;
        crushTile.transform.parent = tagHolder;
        startPosition = shapeCreator.shapePos[index].position;
        endPosition = startPosition + Vector3.up * 20f;
        currentShapeIndex = index;
        crushTile.transform.rotation = Quaternion.Euler(0, 0, rotValue);
        pos = startPosition;
        return crushTile;
    }
    public void ButtonPressStart(int id, int index = -1)
    {
        if (isTutorial)
        {
            tutorialTextImageObj.SetActive(true);
            if (!isLastStep)
            {
                if (tuteCount == 1 && buttonPressId == 2 && index == 0)
                {
                    tuteCount = 0;
                    isLastStep = true;

                }
                if (index <= -1 || currentShapeIndex == index)
                {
                    if (tuteCount > 1)
                    {
                        if (id == 1)
                            tuteCount--;

                        if (id == 2 && tuteCount <= 2)
                        {
                            id = 1;
                        }
                    }
                    if (buttonPressId != id || tuteCount <= 1)
                    {
                        buttonPressId = id;
                        ChangeTutorialAnim();
                    }
                }
            }
        }
        
    }
    private void ChangeTutorialAnim()
    {
        cursorAnimator.SetBool("isDown", false);
        cursorAnimator.SetBool("isPress", false);
        cursorIcon.gameObject.SetActive(false);
        if (tileObjrct != null)
            tileObjrct.SetActive(false);
        tutorialDrag.gameObject.SetActive(false);
        if (buttonPressId < 2)
        {
            pos = Button[buttonPressId].transform.position;
            ButtonsStatus(buttonPressId);
        }
        else
        {
            pos = Vector3.zero;
        }
        cursorIcon.localPosition = Vector3.zero;
        Invoke(nameof(InvokingFunction), 0.5f);
    }


    private void InvokingFunction()
    {
        tutorialPanel.SetActive(false);
        tutorialCutoffMain.SetActive(false);

        if (isTutorial)
        {
            tutorialDrag.ResetPos();
            cursorHoder.gameObject.SetActive(true);
            tutorialDrag.gameObject.SetActive(true);
            cursorIcon.gameObject.SetActive(true);
            //skipButton.gameObject.SetActive(true);


            if (buttonPressId == 2)
            {
                DragTween();
                shapeCreator.SetHasTutorial(Index);
                tutorialDrag.SetUp(tileObjrct, tagHolder, 100);
            }
            else
            {
                int val = 0;
                if (buttonPressId == 1 && tuteCount > 0)
                {
                    if (tuteCount == 2)
                    {
                        val = 1;
                        SecondTutorial();
                        ButtonsStatus();
                    }
                    else
                    {
                        pos = Button[0].transform.position;
                        ButtonsStatus(0);
                        ButtonTween();
                        shapeCreator.ResetCrushTileTutorial();
                    }
                }
                else
                {
                    
                    ButtonTween();
                }
                if (index >= 0 && index < 2)
                {
                    Vector3 position = shapeCreator.shapePos[index].position;
                    tutorialPanel.transform.position = position;
                    tutorialCutoffMain.transform.position = position;
                }
                else
                {
                    shapeCreator.TutorialStatusSet();
                }


                if (Vector2.Distance(pos, Button[1].transform.position) < 0.5f)
                {
                    val = 2;
                }
                ChangeTutorialTextObject(val);
                tutorialPanel.SetActive(true);
                tutorialCutoffMain.SetActive(true);

            }
            
        }
    }


    private void ButtonTween()
    {
        cursorHoder.position = pos;
        cursorAnimator.SetBool("isPress", true);
    }

    

    

    private void SecondTutorial()
    {
        DragTween();
        ButtonTween();
        skipButton.gameObject.SetActive(true);

    }

    private void SkipTutorial()
    {
        prefNum = 10;
        lastStepCount = 0;
        EndTutorial();
    }

    public void EndTutorial()
    {
        
        if (isTutorial)
        {
            if (tuteCount <= 0 || prefNum > 5)
            {
                if (lastStepCount <= 0)
                {
                    DisableTutorial();
                    skipButton.gameObject.SetActive(false);
                    cursorHoder.gameObject.SetActive(false);
                    if (tileObjrct != null)
                        tileObjrct.SetActive(false);
                    tutorialPanel.SetActive(false);
                    tutorialCutoffMain.transform.position = Vector2.down * 37f;
                    tutorialCutoffMain.SetActive(true);
                    isTutorial = false;
                    prefNum = 10;
                    TutorialEnded();
                    boardManager.GameStatus(true);
                    lastStepCount = 5;
                    Invoke(nameof(ChangeToMain), 0.5f);
                }
                else
                {
                    lastStepCount--;
                }
            }
            else
            {
                ChangedCrushTile();
            }
        }

    }
    private void TutorialEnded()
    {
        PlayerPrefs.SetInt(gameDataManager.TutorialPref, 10);
        AdsLeaderboardManager.Instance.CheckAnalyticsEvent(3);
    }
    private void ChangedCrushTile()
    {
        if(tuteCount >= 3)
            index = 2;
        else
        {
            index = 1;
        }
        ChangeTutorialAnim();
    }

    public void RotateTile()
    {
        if (tileObjrct != null && isTutorial)
        {
            int index = shapeCreator.SelectedShapeIndex;
            if(index == currentShapeIndex)
                tileObjrct.transform.Rotate(Vector3.forward, -90f);
        }
    }

    private void ChangeTutorialTextObject(int num = -1)
    {
        foreach (var item in tutorialTextObj)
        {
            item.SetActive(false);
        }

        if (num >= 0 && num < tutorialTextObj.Length)
        {
            tutorialTextObj[num].SetActive(true);
        }

        if(num == 2)
        {
            Vector3 position = shapeCreator.showObject.transform.position;
            tutorialPanel.transform.position = position;
            tutorialCutoffMain.transform.localScale = new Vector2(9, 3);
            tutorialCutoffMain.transform.position = position;
            //sizechange
        }
    }

    private void ChangeToMain()
    {
        gameDataManager.SetSaveValues(5, 0);
        gameDataManager.currentLevel = 0;
        SceneManager.LoadScene(0);
    }

    private int checkCount;
    public void CheckForTutorialEnd()
    {
        if(isTutorial)
            checkCount++;
        if(checkCount > 2)
        {
            TutorialEnded();
            boardManager.GameStatus(true);
            lastStepCount = 5;
            Invoke(nameof(ChangeToMain), 0.5f);
        }
    }


    public void AbilityTutorial(bool status, Vector2 pos)
    {
        print(status);
        if (status)
        {
            cursorIcon.gameObject.SetActive(true);
            cursorHoder.gameObject.SetActive(true);
            cursorAnimator.SetBool("isPress", true); 
            cursorHoder.position = pos;
        }
        else
        {
            DisableTutorial();
        }
    }

    private void ButtonsStatus(int num = -1)
    {
        foreach (var item in Button)
        {
            item.interactable = false;
        }
        if(num >= 0 && num < Button.Length)
        {
            Button[num].interactable = true;
        }
    }
}
