using UnityEngine;
using UnityEngine.UI;

public class Request : MonoBehaviour
{
    private static readonly int Progress = Shader.PropertyToID("_Progress");
    public RawImage bubbleIcon;
    public RawImage background;
    public float duration;
    public BubbleKind Kind { get; set; }

    public float t { get; private set; } = 0f;
    public void BindToRequest(BubbleKind request)
    {
        Kind = request;
        bubbleIcon.color = Kind.Tint;
    }

    private Material mat;
    public void Activate()
    {
        t = 0;
        mat = new Material(background.material);
        background.material = mat;
        switch (GameManager.Instance.GameState.Difficulty)
        {
            case < 2:
                duration = 60;
                break;
            case > 2 and < 5:
                duration = 40;
                break;
            case > 5 and < 10:
                duration = 20;
                break;
            case > 20:
                duration = 10;
                break;
        }
    }

    private void Update()
    {
        t += Time.deltaTime / duration;
        mat.SetFloat(Progress,t);
    }
}