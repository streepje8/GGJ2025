using FondantMetStokjes.InteractionSystem;
using FondantMetStokjes.Player;
using UnityEngine;

public class ShakerInteraction : Interactable
{
    public GameObject model;
    public int sideChangesBeforeWin = 20;
    public float temporaryLmaoSpeedMod = 1.0f;
    public float temporaryLmaoMagnitudeMod = 1.0f;
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
            return;
        Ps4Controller controller = CurrentInteractor.Controller;
        float JoystickY = controller.RightStick.y;
        Debug.Log(JoystickY);
        
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
            model.transform.localPosition = Vector3.up * (Mathf.Sin(Time.time * animationSpeed * temporaryLmaoSpeedMod) * temporaryLmaoMagnitudeMod);
        }

        timeSinceSideChange += Time.deltaTime;

        if (sideChanges >= sideChangesBeforeWin)
        {
            bubble.SwitchKind(bubbleKind);
            bool gaveBubble = CurrentInteractor.TryGiveBubble(bubble);
            if (!gaveBubble)
                return;
            EndInteraction();
        }
    }
}
