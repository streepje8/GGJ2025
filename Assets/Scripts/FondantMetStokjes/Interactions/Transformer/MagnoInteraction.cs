using System;
using FondantMetStokjes.Interactions.Pickups;
using FondantMetStokjes.InteractionSystem;
using FondantMetStokjes.Player;
using UnityEngine;
using UnityEngine.Serialization;

public class MagnoInteraction : Interactable
{
    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Animate = Animator.StringToHash("Door");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int MessUp = Animator.StringToHash("MessUp");
    public Animator animator;
    public float closingDoorTime = 0.5f;
    public float waitTime = 10.0f;
    public float timeBeforeBad = 10.0f;
    public BubbleKind bubbleKindDone;
    public BubbleKind bubbleKindMessedUp;
    private LiveBubble bubble = null;
    private float time = 0;
    enum State
    {
        Idle, Closing, Waiting, Done, MessedUp
    }
    private State state = State.Idle;

    void Start()
    {
        animator.Play(Idle);
    }
    
    public override void OnInteract(Interactor interactor)
    {
        switch (state)
        {
            case State.Idle:
            {
                if (state != State.Idle)
                    return;
                bool tookBubble = interactor.TryTakeBubble(out bubble);
                if (!tookBubble)
                    return;
                time = 0;
                state = State.Closing;
                StartInteraction(interactor);
                animator.Play(Animate);
            }
                break;
            case State.Closing:
                break;
            case State.Waiting:
                break;
            case State.Done:
                bubble.SwitchKind(bubbleKindDone);
                StartInteraction(interactor);
                CurrentInteractor.GiveBubble(bubble);
                EndInteraction();
                state = State.Idle;
                break;
            case State.MessedUp:
                bubble.SwitchKind(bubbleKindMessedUp);
                StartInteraction(interactor);
                CurrentInteractor.GiveBubble(bubble);
                EndInteraction();
                state = State.Idle;
                break;
        }
    }

    public override void InteractableUpdate()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Closing:
                time += Time.deltaTime;
                if (time >= closingDoorTime)
                {
                    state = State.Waiting;
                    EndInteraction();
                }
                break;
            case State.Waiting:
                time += Time.deltaTime;
                if (time >= closingDoorTime + waitTime)
                {
                    state = State.Done;
                    animator.Play(Jump);
                }
                break;
            case State.Done:
                time += Time.deltaTime;
                if (time >= closingDoorTime + waitTime + timeBeforeBad)
                {
                    state = State.MessedUp;
                    animator.Play(MessUp);
                }
                break;
            case State.MessedUp:
                break;
        }
    }
}
