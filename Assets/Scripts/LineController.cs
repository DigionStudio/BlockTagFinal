using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private Texture[] textures;
    private int animationStep;
    [SerializeField] private float fps = 30f;

    private Transform lineOrigin;
    private Vector3 linePosEnd;
    private float fpsCounter;
    void Start()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        fpsCounter += Time.deltaTime;
        if(fpsCounter >= 1f/ fps)
        {
            animationStep++;
            if(animationStep == textures.Length)
                animationStep = 0;

            lineRenderer.material.SetTexture("_MainTex", textures[animationStep]);
            fpsCounter = 0;
        }
        if (lineOrigin)
        {
            SetLinePos();
        }
    }
    public void SetLinePoints(Transform originTrans, Vector2 end)
    {
        if(lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineOrigin = originTrans;
        linePosEnd = end;
        SetLinePos();
    }

    private void SetLinePos()
    {
        lineRenderer.SetPosition(0, lineOrigin.transform.position);
        lineRenderer.SetPosition(1, linePosEnd);
    }
}
