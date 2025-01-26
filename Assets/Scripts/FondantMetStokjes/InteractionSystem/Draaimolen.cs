using FondantMetStokjes.InteractionSystem;
using UnityEngine;

public class Draaimolen : Interactable
{
    public MeshRenderer displayModel;
    public BubbleKind spinnedKind;
    public Transform bone;
    public float spinningTime = 2f;
    private float turningCooldown = 0.2f;
    private float totalTimer = 0f;
    private float turningSpeed = 600f;
    private MeshRenderer display;
    private LiveBubble bubble;

    public override void OnInteract(Interactor interactor)
    {
        if (!interactor.TryTakeBubble(out bubble)) return;
        StartInteraction(interactor);
        turningCooldown = 0.2f;
        totalTimer = 0f;
        display = Instantiate(displayModel);
        bubble.ToRenderer(display);
    }

    private Vector2 lastJoystick = Vector2.up;
    public override void InteractableUpdate()
    {
        //if Spinning
        if (!InInteraction) return;
        if (CurrentInteractor == null) return;
        var input = CurrentInteractor.Controller.RightStick.normalized;
        var angle = Vector2.SignedAngle(input, lastJoystick);
        if (angle is > 2f and < 10f)
        {
            turningCooldown = 0.2f;
            lastJoystick = input;
        }
        
        display.transform.position = bone.transform.position + new Vector3(-0.272000015f,-0.294999987f-0.504000008f);
        
        if (turningCooldown > 0)
        {
            turningCooldown -= Time.deltaTime;
            bone.localRotation *= Quaternion.Euler(0, turningSpeed * Time.deltaTime, 0);
            totalTimer += Time.deltaTime;
        }

        if (totalTimer >= spinningTime)
        {
            Destroy(display.gameObject);
            bubble.SwitchKind(spinnedKind);
            CurrentInteractor.GiveBubble(bubble);
            EndInteraction();
        }
    }
}
