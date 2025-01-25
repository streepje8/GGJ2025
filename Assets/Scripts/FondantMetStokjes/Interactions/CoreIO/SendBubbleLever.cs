using FondantMetStokjes.Interactions.Pickups;
using FondantMetStokjes.InteractionSystem;
using UnityEngine;

public class SendBubbleLever : Interactable
{
    private static readonly int Lever = Animator.StringToHash("LeverTwo");
    [field: SerializeField] public Transform BuldgePoint { get; private set; }
    [field: SerializeField] public AnimationCurve BuldgeCurve { get; private set; }
    [field: SerializeField] public Transform PointA { get; private set; }
    [field: SerializeField] public Transform PointB { get; private set; }
    [field: SerializeField] public Transform PointC { get; private set; }
    [field: SerializeField] public Transform PointD { get; private set; }
    [field: SerializeField] public Animator LeverAnimator { get; private set; }
    [field: SerializeField] public float Duration { get; private set; } = 2f;
    private bool isPlaying = false;
    private float T = 0f;
    private LiveBubble currentlyDisposing;
    private Pickup currentlyDisposingPickup;
    public override void OnInteract(Interactor interactor)
    {
        StartInteraction(interactor);
        LeverAnimator.SetTrigger(Lever);
        var inv = interactor.GetComponent<Inventory>();
        if (inv != null)
        {
            if (inv.CurrentlyHolding != null)
            {
                var liveBubble = inv.CurrentlyHolding.GetComponent<LiveBubble>();
                if (liveBubble != null)
                {
                    currentlyDisposingPickup = inv.DropItem();
                    liveBubble.MakeImmortal();
                    currentlyDisposing = liveBubble;
                    if (!isPlaying)
                    {
                        isPlaying = true;
                        T = 0f;
                    }
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
                case < 1f / 3f:
                {
                    var localT = BuldgeCurve.Evaluate(T / (1f / 3f));
                    BuldgePoint.position = Vector3.Lerp(PointA.position, PointB.position, localT);
                    currentlyDisposing.transform.position = Vector3.Lerp(PointA.position, PointB.position, localT);
                    currentlyDisposing.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.05f, localT);
                }
                    break;
                case > 1f / 3f and < 2f / 3f:
                {
                    var localT = BuldgeCurve.Evaluate(T - (1f/3f)) / (1f / 3f);
                    BuldgePoint.position = Vector3.Lerp(PointB.position, PointC.position, localT);
                }
                    break;
                case > 2f / 3f and < 1:
                {
                    var localT = BuldgeCurve.Evaluate(T - (2f/3f)) / (1f / 3f);
                    BuldgePoint.position = Vector3.Lerp(PointC.position, PointD.position, localT);
                }
                    break;
            }

            if (T >= 1)
            {
                EndInteraction();
                T = 0;
                isPlaying = false;
                BuldgePoint.position = PointA.position + Vector3.up * 10f;
                GameManager.Instance.SubmitBubble(currentlyDisposing.Kind);
                Destroy(currentlyDisposingPickup.gameObject);
            }
        }
    }
}
