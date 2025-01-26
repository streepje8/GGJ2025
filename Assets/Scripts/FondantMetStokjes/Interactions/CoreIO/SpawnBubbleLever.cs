using FondantMetStokjes.InteractionSystem;
using FondantMetStokjes.Player;
using UnityEngine;

public class SpawnBubbleLever : Interactable
{
    private static readonly int Lever = Animator.StringToHash("Lever");
    [field: SerializeField]public GameObject LiveBubblePrefab { get; private set; }
    //public override ControllerButton InteractionStartButton => ControllerButton.Circle;

    [field: SerializeField] public Transform BuldgePoint { get; private set; }
    [field: SerializeField] public AnimationCurve BuldgeCurve { get; private set; }
    [field: SerializeField] public Transform PointA { get; private set; }
    [field: SerializeField] public Transform PointB { get; private set; }
    [field: SerializeField] public Transform PointC { get; private set; }
    [field: SerializeField] public Transform PointD { get; private set; }
    [field: SerializeField] public SfxClip useClip;
    [field: SerializeField] public SfxClip shwoopClip;
    [field: SerializeField] public Transform BubbleSpawnPoint { get; private set; }
    [field: SerializeField] public Animator LeverAnimator { get; private set; }
    [field: SerializeField] public float Duration { get; private set; } = 2f;
    private bool shwooped = false;
    private bool isPlaying = false;
    private float T = 0f;
    private Transform lastSpawnedBubble;
    public override void OnInteract(Interactor interactor)
    {
        if (!isPlaying)
        {
            StartInteraction(interactor);
            LeverAnimator.SetTrigger(Lever);
            isPlaying = true;
            shwooped = false;
            SfxManager.instance.SpawnSound(useClip, transform.position);
            T = 0f;
            if (lastSpawnedBubble != null)
            {
                if (Vector3.Distance(lastSpawnedBubble.position, BubbleSpawnPoint.position) < 0.1f)
                {
                    Destroy(lastSpawnedBubble.gameObject);
                    lastSpawnedBubble = null;
                }
            }
        }
    }

    public override void InteractableUpdate()
    {
        if (isPlaying)
        {
            T += Time.deltaTime / Duration;
            switch (T)
            {
                case <= 1.0f / 3.0f:
                {
                    var localT = BuldgeCurve.Evaluate(Mathf.InverseLerp(0, 1f/3f, T));
                    BuldgePoint.position = Vector3.Lerp(PointA.position, PointB.position, localT);
                }
                    break;
                case > 1.0f / 3.0f and < 2.0f / 3.0f:
                {
                    var localT = BuldgeCurve.Evaluate(Mathf.InverseLerp(1f/3f, 2f/3f, T));
                    BuldgePoint.position = Vector3.Lerp(PointB.position, PointC.position, localT);
                }
                    break;
                case > 2.0f / 3.0f and <= 1.0f:
                {
                    if (!shwooped)
                    {
                        shwooped = true;
                        SfxManager.instance.SpawnSound(shwoopClip, transform.position);
                    }
                    var localT = BuldgeCurve.Evaluate(Mathf.InverseLerp(2f/3f, 1.0f, T));
                    BuldgePoint.position = Vector3.Lerp(PointC.position, PointD.position, localT);
                }
                    break;
            }

            if (T >= 1)
            {
                EndInteraction();
                Instantiate(LiveBubblePrefab, BubbleSpawnPoint.position, Quaternion.identity);
                T = 0;
                isPlaying = false;
                BuldgePoint.position = PointA.position + Vector3.up * 10f;
            }
        }
    }
}
