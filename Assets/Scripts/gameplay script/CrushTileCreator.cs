using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CrushTileType
{
    Normal,
    Freeze,
    ReAlign,
    Bomb,
    Magnet
}
public class CrushTileCreator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    [SerializeField] private CrushTile crushTile;
    [SerializeField] private Transform holder;
    private float rotateValue;
    public float RotateValue { get { return rotateValue; } }
    private int shapeCode2;
    public int ShapeCode2 { get { return shapeCode2; } }
    private bool isTutorial;
    public bool HasTutorial { set { isTutorial = value;} }
    private bool isActiveTutorial = true;
    [SerializeField] private AudioSource sfxObject;
    [SerializeField] private AudioSource[] sfx;
    private float dragOffset = 10f;
    private ShapeCreator shapeCreator;
    private BoardManager boardManager;
    private List<CrushTile> m_CrushTiles = new List<CrushTile>();
    private Vector2 m_Pos;
    private int m_Index;
    private bool dragging = false;
    private Vector2 mousePos;
    private Vector2 mouseDragPos;
    private bool moveTrans;
    private CrushTileType thisCrushTileType = CrushTileType.Normal;
    private bool isReset = true;
    private bool isAbility = false;
    private bool destroyed = false;
    [SerializeField] private LineController lineController;
    private float timereturn = 0.5f;
    private bool isTimer;
    private float currentTime = 0;
    private List<BlockType> desBlockType = new List<BlockType>();
    public static Action<BlockType> ColorBombType = delegate { };
    private void Start()
    {
        boardManager = BoardManager.Instance;
        shapeCreator = ShapeCreator.Instance;
        ColorBombType += DesBlockType;
    }
    private void OnDisable()
    {
        ColorBombType -= DesBlockType;
    }
    private void Shape_1()
    {
        Vector2 spawnPosition = Vector2.zero;

        for (int i = 1; i < 3; i++)
        {
            spawnPosition += Vector2.up * 1.25f * i;
            InstaCrushTile(spawnPosition);
        }
        spawnPosition = Vector2.zero;

        for (int i = 1; i < 3; i++)
        {
            spawnPosition += Vector2.down * 1.25f * i;
            InstaCrushTile(spawnPosition);
        }

    }

    private void Shape_2()
    {
        Vector2 spawnPosition = Vector2.zero;
        for (int i = 1; i < 3; i++)
        {
            spawnPosition += Vector2.right * 1.25f * i;
            InstaCrushTile(spawnPosition);
        }

        spawnPosition = Vector2.zero;
        for (int i = 1; i < 3; i++)
        {
            spawnPosition += Vector2.left * 1.25f * i;
            InstaCrushTile(spawnPosition);
        }

    }
    private void Shape_3()
    {
        int num = 1;
        for (int i = 1; i < 3; i++)
        {
            float posX = 1.25f * num;
            Vector2 spawnPosition = new Vector2(posX, 1.25f);
            InstaCrushTile(spawnPosition);
            num = -num;
        }

        for (int i = 1; i < 3; i++)
        {
            float posX = 1.25f * num;
            Vector2 spawnPosition = new Vector2(posX, - 1.25f);
            InstaCrushTile(spawnPosition);
            num = -num;

        }
    }

    private void Shape_4()
    {
        Vector2 spawnPosition = Vector2.zero;
        spawnPosition.x -= 2.5f;

        for (int i = -1; i < 2; i++)
        {
            InstaCrushTile(spawnPosition);
            spawnPosition.x += 2.5f;

        }

        spawnPosition.x -= 2.5f;
        spawnPosition.y -= 2.5f;
        InstaCrushTile(spawnPosition);

    }
    private void Shape_5()
    {
        Vector2 spawnPosition = Vector2.zero;
        spawnPosition.x -= 2.5f;
        Vector2 pos = spawnPosition + Vector2.up * 2.5f;
        InstaCrushTile(pos);
        for (int i = 0; i < 3; i++)
        {
            InstaCrushTile(spawnPosition);

            spawnPosition.x += 2.5f;
        }
    }

    private void Shape_6()
    {
        Vector2 spawnPosition = Vector2.zero;
        spawnPosition.x -= 2.5f;
        for (int i = 0; i < 3; i++)
        {
            InstaCrushTile(spawnPosition);

            if (i == 1)
            {
                Vector2 pos = spawnPosition + Vector2.down * 2.5f;
                InstaCrushTile(pos);
            }
            spawnPosition.x += 2.5f;
        }
    }
    private void Shape_7()
    {
        Vector2 spawnPosition = Vector2.zero;
        spawnPosition.x -= 2.5f;

        for (int i = 0; i < 3; i++)
        {
            InstaCrushTile(spawnPosition);

            if (i == 1)
            {
                Vector2 pos = spawnPosition + Vector2.up * 2.5f;
                InstaCrushTile(pos);
            }
            spawnPosition.x += 2.5f;
        }
    }

    private void Shape_8()
    {
        Vector2 spawnPosition = Vector2.zero;
        spawnPosition.y -= 2.5f;

        for (int i = 0; i < 3; i++)
        {
            InstaCrushTile(spawnPosition);

            if (i == 1)
            {
                Vector2 pos = spawnPosition + Vector2.left * 2.5f;
                InstaCrushTile(pos);
            }
            spawnPosition.y += 2.5f;
        }
    }

    private void Shape_9()
    {
        Vector2 spawnPosition = Vector2.zero;
        spawnPosition.y -= 2.5f;

        for (int i = 0; i < 3; i++)
        {
            InstaCrushTile(spawnPosition);
            if (i == 1)
            {
                Vector2 pos = spawnPosition + Vector2.right * 2.5f;
                InstaCrushTile(pos);
            }
            spawnPosition.y += 2.5f;
        }
    }
    private void Shape_10()
    {
        Vector2 spawnPosition = Vector2.zero;
        spawnPosition.y -= 2.5f;
        Vector2 pos = spawnPosition + Vector2.right * 2.5f;
        InstaCrushTile(pos);
        for (int i = 0; i < 3; i++)
        {
            InstaCrushTile(spawnPosition);
            spawnPosition.y += 2.5f;
        }
    }

    private void Shape_11()
    {
        Vector2 spawnPosition = Vector2.zero;
        spawnPosition.y -= 2.5f;
        Vector2 pos = spawnPosition + Vector2.left * 2.5f;
        InstaCrushTile(pos);
        for (int i = 0; i < 3; i++)
        {
            InstaCrushTile(spawnPosition);
            spawnPosition.y += 2.5f;
        }
    }

    private void Shape_12()
    {
        Vector2 spawnPosition = Vector2.zero;
        spawnPosition.y -= 2.5f;


        for (int i = 0; i < 3; i++)
        {
            InstaCrushTile(spawnPosition);
            spawnPosition.y += 2.5f;
        }
        spawnPosition.y -= 2.5f;
        Vector2 pos = spawnPosition + Vector2.left * 2.5f;
        InstaCrushTile(pos);
    }

    private void Shape_13()
    {
        Vector2 spawnPosition = Vector2.zero;
        spawnPosition.y -= 2.5f;

        for (int i = 0; i < 3; i++)
        {
            InstaCrushTile(spawnPosition);
            spawnPosition.y += 2.5f;
        }
        spawnPosition.y -= 2.5f;
        Vector2 pos = spawnPosition + Vector2.right * 2.5f;
        InstaCrushTile(pos);

    }

    private void Shape_14()
    {
        Vector2 spawnPosition = Vector2.zero;
        spawnPosition.x -= 2.5f;

        for (int i = 0; i < 3; i++)
        {
            InstaCrushTile(spawnPosition);
            spawnPosition.x += 2.5f;
        }
        spawnPosition.x -= 2.5f;
        spawnPosition.y += 2.5f;

        InstaCrushTile(spawnPosition);

    }

    private void Shape_15()
    {
        Vector2 spawnPosition = Vector2.zero;
        spawnPosition.x -= 2.5f;
        Vector2 pos = spawnPosition + Vector2.down * 2.5f;
        InstaCrushTile(pos);

        for (int i = 0; i < 3; i++)
        {
            InstaCrushTile(spawnPosition);
            spawnPosition.x += 2.5f;
        }
    }

    private void Shape_16()
    {
        Vector2 spawnPosition = Vector2.zero;
        for (int i = -1; i < 3; i += 3)
        {
            spawnPosition += Vector2.up * 1.25f * i;
            InstaCrushTile(spawnPosition);
        }
    }

    private void Shape_17()
    {
        Vector2 spawnPosition = Vector2.zero;
        for (int i = -1; i < 3; i += 3)
        {
            spawnPosition += Vector2.right * 1.25f * i;
            InstaCrushTile(spawnPosition);
        }
    }

    private void Shape_18()
    {
        Vector2 pos = Vector2.zero;
        for (int i = 0; i < 2; i++)
        {
            Vector2 spawnPosition = pos + Vector2.left * 1.25f;
            spawnPosition += Vector2.up * 2.5f * i;
            InstaCrushTile(spawnPosition);
        }

        for (int i = 0; i < 2; i++)
        {
            Vector2 spawnPosition = pos + Vector2.right * 1.25f;
            spawnPosition += Vector2.down * 2.5f * i;
            InstaCrushTile(spawnPosition);
        }
    }
    private void Shape_24()
    {
        Vector2 pos = Vector2.zero;
        for (int i = 0; i < 2; i++)
        {
            Vector2 spawnPosition = pos + Vector2.up * 1.25f;
            spawnPosition += Vector2.right * 2.5f * i;
            InstaCrushTile(spawnPosition);
        }

        for (int i = 0; i < 2; i++)
        {
            Vector2 spawnPosition = pos + Vector2.down * 1.25f;
            spawnPosition += Vector2.left * 2.5f * i;
            InstaCrushTile(spawnPosition);
        }
    }

    private void Shape_19()
    {
        Vector2 pos = Vector2.zero;
        for (int i = 0; i < 2; i++)
        {
            Vector2 spawnPosition = pos + Vector2.up * 1.25f;
            spawnPosition += Vector2.left * 2.5f * i;
            InstaCrushTile(spawnPosition);
        }

        for (int i = 0; i < 2; i++)
        {
            Vector2 spawnPosition = pos + Vector2.down * 1.25f;
            spawnPosition += Vector2.right * 2.5f * i;
            InstaCrushTile(spawnPosition);
        }
    }

    private void Shape_25()
    {
        Vector2 pos = Vector2.zero;
        for (int i = 0; i < 2; i++)
        {
            Vector2 spawnPosition = pos + Vector2.left * 1.25f;
            spawnPosition += Vector2.down * 2.5f * i;
            InstaCrushTile(spawnPosition);
        }

        for (int i = 0; i < 2; i++)
        {
            Vector2 spawnPosition = pos + Vector2.right * 1.25f;
            spawnPosition += Vector2.up * 2.5f * i;
            InstaCrushTile(spawnPosition);
        }
    }


    private void Shape_20(int index1, int index2)
    {
        int num = 1;
        for (int i = 1; i < 3; i++)
        {
            if (index1 == i) continue;
            float posX = 1.25f * num;
            Vector2 spawnPosition = new Vector2(posX, 1.25f);
            InstaCrushTile(spawnPosition);
            num = -num;
        }

        for (int i = 1; i < 3; i++)
        {
            if (index2 == i) continue;
            float posX = 1.25f * num;
            Vector2 spawnPosition = new Vector2(posX, -1.25f);
            InstaCrushTile(spawnPosition);
            num = -num;

        }
    }

    private void Shape_26()
    {
        Vector2 spawnPosition = Vector2.zero;
        spawnPosition.x -= 2.5f;

        for (int i = 0; i < 3; i++)
        {
            InstaCrushTile(spawnPosition);
            spawnPosition.x += 2.5f;
        }
    }
    private void Shape_27()
    {
        Vector2 spawnPosition = Vector2.zero;
        spawnPosition.y -= 2.5f;
        for (int i = 0; i < 3; i++)
        {
            InstaCrushTile(spawnPosition);
            spawnPosition.y += 2.5f;
        }
    }
    /// <summary>
    /// index = 0 Normal, index = 1 freeze, index = 2 reorder
    /// index = 3 bomb, index = 4 magnet
    /// </summary>
    /// <param name="isModBomb"></param>
    /// <param name="index"></param>
    private void Shape_28(bool isModBomb, int index = 0)
    {
        isAbility = false;
        Vector2 spawnPosition = transform.position;
        CrushTile crushtile = Instantiate(crushTile, spawnPosition, Quaternion.identity, holder);
        m_CrushTiles.Add(crushtile);

        if (index > 0)
        {
            isAbility = true;
            thisCrushTileType = (CrushTileType)index;
            crushtile.SpecialCrushSetUp(thisCrushTileType, isModBomb);
            if(thisCrushTileType == CrushTileType.Bomb)
            {
                timereturn = 1.4f;
            }
        }
    }


    private void InstaCrushTile(Vector2 pos)
    {
        CrushTile crushtile = Instantiate(crushTile,transform.position, Quaternion.identity, holder);
        crushtile.transform.position = (Vector2)transform.position + pos;
        m_CrushTiles.Add(crushtile);
    }

    public void SetUp_Shape(int shapeCode, int index, int num, bool isModBomb = false)
    {
        isActiveTutorial = !TutorialManager.Instance.isTutorial;
        if (TutorialManager.Instance != null && TutorialManager.Instance.isTutorial)
            num = 0;
        
        MakeShape(shapeCode, num, isModBomb);
        m_Index = index;
        shapeCode2 = num;
    }

    public void MakeShape(int shapeCode, int num, bool isModBomb = false)
    {

        if (shapeCode == 0)
        {
            if (num == 0)
                Shape_1();
            else if (num == 1)
                Shape_2();
        }
        else if (shapeCode == 1)
        {
            Shape_3();
        }
        else if (shapeCode == 2)
        {
            if (num == 0)
                Shape_4();
            else if (num == 1)
                Shape_5();
            else if (num == 2)
                Shape_11();
            else if (num == 3)
                Shape_13();
            
        }
        else if (shapeCode == 3)
        {
            if (num == 0)
                Shape_10();
            else if (num == 1)
                Shape_12();
            else if (num == 2)
                Shape_14();
            else if (num == 3)
                Shape_15();

        }
        else if (shapeCode == 4)
        {
            if (num == 0)
                Shape_6();
            else if (num == 1)
                Shape_7();
            else if (num == 2)
                Shape_8();
            else if (num == 3)
                Shape_9();
        }
        else if (shapeCode == 5)
        {
            if (num == 0)
                Shape_16();
            else if (num == 1)
                Shape_17();
        }
        else if (shapeCode == 6)
        {
            if (num == 0)
                Shape_18();
            else if (num == 1)
                Shape_24();
        }
        else if (shapeCode == 7)
        {
            if (num == 0)
                Shape_19();
            else if (num == 1)
                Shape_25();

        }else if (shapeCode == 8)
        {
            if (num == 0)
                Shape_20(1, 4);
            else if (num == 1)
                Shape_20(2, 4);
            else if (num == 2)
                Shape_20(4, 1);
            else if (num == 3)
                Shape_20(4, 2);
        }
        else if (shapeCode == 9)
        {
            if (num == 0)
                Shape_20(1, 2);
            else if (num == 1)
                Shape_20(2, 1);

        }
        else if (shapeCode == 10)
        {
            if (num == 0)
                Shape_26();
            else if (num == 1)
                Shape_27();
        }
        else if (shapeCode == 11)
        {
            Shape_28(false);

        }else if((shapeCode == 12 || shapeCode == 13) && num >= 0)
        {
            Shape_28(isModBomb, num);
        }
        else
        {
            Shape_28(false);
        }
        
    }


    public void SetPosistion(Vector2 pos)
    {
        m_Pos = pos;
        transform.position = m_Pos;

    }

    private void Move(Vector2 pos, float time, bool isTrans)
    {
        transform.DOMove(pos, time).OnComplete(() => {
            
                moveTrans = isTrans;
        });
    }

    private void Movement(Vector3 pos)
    {
        transform.position = pos;
    }

    public void InstaSize(bool isModBomb)
    {
        transform.localScale = Vector2.zero;
        float time = 0f;
        if (isModBomb)
            time = 1.5f;
        Invoke(nameof(InstaModSize), time);
    }
    private void InstaModSize()
    {
        transform.DOScale(0.5f, 0.5f);
        Sfx();
    }
    private void Sfx()
    {
        sfxObject.Play();
    }

    private void Re_Size(bool isNormal)
    {
        if (isNormal)
        {
            transform.DOScale(Vector2.one, 0.1f);
            shapeCreator.SetCrushTile(m_Index);
            ChamgeCrushColot(2);
        }
        else
        {
            transform.DOScale(Vector2.one * 0.5f, 0.1f);
        }

        if (thisCrushTileType == CrushTileType.Bomb)
        {
            m_CrushTiles[0].BombCollider(isNormal);
        }
    }

    private bool CrushTileEngage()
    {
        bool isActive = true;
        if (thisCrushTileType == CrushTileType.Normal)
        {
            foreach (CrushTile ct in m_CrushTiles)
            {
                if (ct.blockSelected == null)
                {
                    isActive = false;
                    break;
                }
            }
        }
        else
        {
            if(m_CrushTiles[0].BlockSelectedListCount == 0)
            {
                isActive = false;
            }
        }
        return isActive;
    }
    


    private void Check()
    {
        bool checkTile = CrushTileEngage();
        if(checkTile)
        {
            if (!destroyed)
            {
                destroyed = true;
                isReset = true;
                Destroy();
            }
        }
        else
        {
            dragging = false;
            MoveToOrogin();
        }
    }

    private void MoveToOrogin()
    {
        if (!isReset)
        {
            isReset = true;
            sfx[1].Play();
            Move(m_Pos, 0.1f, true);
            Re_Size(false);
            ChamgeCrushColot(1);
        }
    }

    private void ChamgeCrushColot(int num)
    {
        foreach (CrushTile ct in m_CrushTiles)
        {
            bool isResize = false;
            if (num == 1)
            {
                isResize = true;
            }
            ct.SizeStatus(isResize);
            ct.ChangeColor(num);
        }
    }

    public void ChangeTileAlpha()
    {
        foreach (CrushTile ct in m_CrushTiles)
        {
            ct.ChangeAlpha();
        }
    }

    private void Destroy()
    {
        boardManager.BoardTurn(m_Index);
        shapeCreator.UsedCrushTile(m_Index);
        int num = 0;
        if (isAbility)
        {
            CheckForAbility();
            if(thisCrushTileType != CrushTileType.Bomb)
            {
                m_CrushTiles[0].DestroyBlock();
            }
        }
        else
        {
            foreach (CrushTile ct in m_CrushTiles)
            {
                if (ct.blockSelected != null)
                {
                    ct.DestroyBlock();
                    num++;
                }
            }
            holder.gameObject.SetActive(false);
            boardManager.TotalBlockDes = num;
            isTimer = true;
        }
    }

    private void DesBlockType(BlockType type)
    {
        if (destroyed && !desBlockType.Contains(type))
        {
            desBlockType.Add(type);
            timereturn += 1f;
        }
    }

    private void DestroyCrush()
    {
        if (isTutorial)
        {
            TutorialCall();
        }else
        {
            TutorialManager.Instance.CheckForTutorialEnd();
        }
        boardManager.CallUnturn();
        BoardManager.OnCrushTileDisable.Invoke(transform.position);
        Destroy(gameObject);
    }

    private IEnumerator ThunderStrike()
    {
        List<BlockTile> thunderBlocks = boardManager.ThunderAbility(true);
        float time = (0.8f/ (float)thunderBlocks.Count);
        for (int i = 0; i < thunderBlocks.Count; i++)
        {
            BlockTile tile = thunderBlocks[i];
            if(tile != null && !tile.HasBlockSelected)
            {
                LineController line = Instantiate(lineController, transform);
                line.SetLinePoints(m_CrushTiles[0].transform, tile.transform.position);
                yield return new WaitForSeconds(time);
            }
        }
        for (int i = 0; i < thunderBlocks.Count; i++)
        {
            BlockTile tile = thunderBlocks[i];
            if (tile != null && !tile.HasBlockSelected)
            {
                tile.DestroyObject();
            }
        }
        DestroyCrush();

    }

    private void Update()
    {
        if (dragging && !boardManager.HasTurn)
        {
            mouseDragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (isTimer && destroyed)
        {
            if(currentTime >= timereturn)
            {
                isTimer = false;
                DestroyCrush();
            }
            else
            {
                currentTime += Time.deltaTime;
            }
        }
    }
    void FixedUpdate()
    {
        if (!boardManager.HasTurn && boardManager.isGameStarted)
        {
            if (dragging)
            {
                if (!moveTrans)
                {
                    Vector2 pos = mouseDragPos + Vector2.up * dragOffset;
                    Movement(pos);

                }
            }
            else
            {
                if(moveTrans)
                {
                    float distance = Vector2.Distance(transform.position, m_Pos);
                    if(distance > 0.1f)
                    {
                        Movement(m_Pos);
                        moveTrans = false;
                    }

                }
            }
        }
        else
        {
            //moveTrans = false;
            //MoveToOrogin();
        }
    }


    private void MouseDown()
    {
        if (boardManager.isGameStarted && boardManager.HasMove())
        {
            // Record the difference between the objects centre, and the clicked point on the camera plane.
            shapeCreator.HasCrushTileSelected(m_Index, true);
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float posx = transform.position.x - mousePos.x;
            isReset = false;
            Re_Size(true);
            dragging = true;
            sfx[0].Play();
            moveTrans = true;
            Vector2 pos = mousePos + Vector2.up * dragOffset;
            Move(pos, 0.1f, false);
            SetUpCrushTile(true);
            if (!isTutorial)
            {
                TutorialCall();
            }
            FollowCursor.OnMousePressed.Invoke(true);
            boardManager.ChangeMoveStatus(false);
        }
    }
    private void MouseUp()
    {
        shapeCreator.HasCrushTileSelected(m_Index, false);
        Check();
        SetUpCrushTile(false);
        FollowCursor.OnMousePressed.Invoke(false);
        boardManager.ChangeMoveStatus(true);

    }

    private void TutorialCall()
    {
        if(TutorialManager.Instance != null)
        {
            TutorialManager.Instance.ButtonPressStart(1, m_Index);
        }
    }

    public void RotateObj()
    {
        Sfx();
        holder.Rotate(Vector3.forward, -90);
        rotateValue = holder.transform.eulerAngles.z;
        foreach (CrushTile ct in m_CrushTiles)
        {
            if (ct != null)
            {
                ct.transform.Rotate(Vector3.forward, 90f);
            }
        }
    }

    private void SetUpCrushTile(bool status)
    {
        foreach (var item in m_CrushTiles)
        {
            if (item != null)
                item.SetUpCollider(status);
        }
    }

    private void CheckForAbility()
    {
        int num = 0;
        if (thisCrushTileType == CrushTileType.Freeze)
        {
            boardManager.FreezeAbilityActive();
            Invoke(nameof(DestroyCrush), 1.5f);


        }
        else if (thisCrushTileType == CrushTileType.ReAlign)
        {
            float time = boardManager.Re_Alignr_Ability();
            Invoke(nameof(DestroyCrush), time);

        }
        else if (thisCrushTileType == CrushTileType.Bomb)
        {
            isTimer = true;
            num = m_CrushTiles[0].BlockSelectedListCount;
            boardManager.TotalBlockDes = num;
            m_CrushTiles[0].DestroyBlock();

        }
        else if (thisCrushTileType == CrushTileType.Magnet)
        {
            float time = boardManager.MagnaticAbility();
            Invoke(nameof(DestroyCrush), time);
        }
        else
        {
            DestroyCrush();
        }
    }

    

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!shapeCreator.CheckSelected(-5) && isActiveTutorial)
            MouseDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (shapeCreator.CheckSelected(m_Index))
            MouseUp();
    }

    public void MouseDownTutorial()
    {
        MouseUp();
    }

    public void SetUpTilesTutorials(bool status)
    {
        isActiveTutorial = status;
    }

}
