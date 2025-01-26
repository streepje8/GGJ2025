using FondantMetStokjes.InteractionSystem;
using UnityEngine;
using UnityEngine.Serialization;

public enum FlattenerState
{
    Base,
    Up,
    Down
}
public class FlattenerInteraction : Interactable
{
    private static readonly int Reset = Animator.StringToHash("Reset");
    private static readonly int Up = Animator.StringToHash("Up");
    private static readonly int Go = Animator.StringToHash("Go");
    private static readonly int Down = Animator.StringToHash("Down");
    public MeshRenderer displayModel;
    [FormerlySerializedAs("spinnedKind")] public BubbleKind squishedKind;
    public Animator squisher;
    public Animator trafficLight;
    private float niceTime = 0.2f;
    public Transform displayPosition;
    private MeshRenderer display;
    private LiveBubble bubble;
    private FlattenerState state; 
    
    public override void OnInteract(Interactor interactor)
    {
        if (!interactor.TryTakeBubble(out bubble)) return;
        StartInteraction(interactor);
        display = Instantiate(displayModel, displayPosition);
        bubble.ToRenderer(display);
        state = FlattenerState.Base;
        squisher.SetTrigger(Reset);
    }

    private float timer = 0f;
    public override void InteractableUpdate()
    {
        //if Spinning
        if (!InInteraction) return;
        if (CurrentInteractor == null) return;

        var controller = CurrentInteractor.Controller;
        if (state == FlattenerState.Base)
        {
            if (controller.RightStick.y < -0.4f)
            {
                squisher.SetTrigger(Up);
                trafficLight.SetTrigger(Go);
                state = FlattenerState.Up;
                timer = 0f;
            }
        }

        if (state == FlattenerState.Up)
        {
            timer += Time.deltaTime;
            if (controller.RightStick.y > -0.4f)
            {
                Destroy(display.gameObject);
                CurrentInteractor.GiveBubble(bubble);
                EndInteraction();
            }

            if (timer > 4f)
            {
                state = FlattenerState.Down;
                squisher.SetTrigger(Down);
                Destroy(display.gameObject);
                timer = 0f;
            }
        }

        if (state == FlattenerState.Down)
        {
            timer += Time.deltaTime;
            if (timer > 1f)
            {
                //player too slow
                squisher.SetTrigger(Reset);
                CurrentInteractor.GiveBubble(bubble);
                EndInteraction();
            }

            if (controller.RightStick.y > -0.4)
            {
                bubble.SwitchKind(squishedKind);
                squisher.SetTrigger(Reset);
                CurrentInteractor.GiveBubble(bubble);
                EndInteraction();
            }
        }
    }
}
