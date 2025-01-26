using FondantMetStokjes.Interactions.Pickups;
using FondantMetStokjes.InteractionSystem;
using FondantMetStokjes.Player;
using UnityEngine;
using UnityEngine.Serialization;

public class ShakerInteraction : Interactable
{
    private static readonly int Shake = Animator.StringToHash("Shake");
    public Animator model;
    public int sideChangesBeforeWin = 20;
    [FormerlySerializedAs("temporaryLmaoSpeedMod")] public float permanentLmaoSpeedMod = 1.0f;
    public BubbleKind bubbleKind;
    private LiveBubble bubble = null;
    enum State
    {
        Start, Up, Down
    }
    private const float upDownZone = 0.25f;
    private State state = State.Start;
    private float timeSinceSideChange = 0;
    private int sideChanges = 0;
    private float animationSpeed = 0;
    
    public override void OnInteract(Interactor interactor)
    {
        bool tookBubble = interactor.TryTakeBubble(out bubble);
        if (!tookBubble)
            return;
        state = State.Start;
        timeSinceSideChange = 0;
        sideChanges = 0;
        animationSpeed = 0;
        StartInteraction(interactor);
    }

    public override void InteractableUpdate()
    {
        if (!InInteraction)
        {
            model.SetFloat(Shake, 0);
            return;
        }
        Ps4Controller controller = CurrentInteractor.Controller;
        float JoystickY = controller.RightStick.y;
        
        if (state != State.Down && JoystickY <= -upDownZone)
        {
            state = State.Down;
            timeSinceSideChange = 0;
            sideChanges++;
        } else if (state != State.Up && JoystickY >= upDownZone)
        {
            state = State.Up;
            timeSinceSideChange = 0;
            sideChanges++;
        }

        if (state != State.Start)
        {
            if (timeSinceSideChange != 0)
            {
                animationSpeed = 1.0f / timeSinceSideChange;
            }
            model.SetFloat(Shake, 1);
            model.speed = animationSpeed * permanentLmaoSpeedMod;
        }

        timeSinceSideChange += Time.deltaTime;

        if (sideChanges >= sideChangesBeforeWin)
        {
            bubble.SwitchKind(bubbleKind);
            CurrentInteractor.GiveBubble(bubble);
            EndInteraction();
        }
    }
}
