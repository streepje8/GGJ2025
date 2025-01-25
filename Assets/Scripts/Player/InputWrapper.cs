using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputWrapper : MonoBehaviour
{
    public Ps4Controller CurrentController { get; private set; } = new Ps4Controller();
    [field: SerializeField]public float JoystickDeadZone { get; private set; } = 0.1f;
    private PlayerInput input;
    private InputAction leftStick;
    private InputAction rightStick;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        CurrentController.SetDeadZone(JoystickDeadZone * JoystickDeadZone);
        leftStick = input.actions["LeftStick"];
        rightStick = input.actions["RightStick"];
        input.actions["Cross"].performed += PsCross;
        input.actions["Square"].performed += PsSquare;
        input.actions["Triangle"].performed += PsTriangle;
        input.actions["Circle"].performed += PsCircle;
    }

    private void Update()
    {
        CurrentController.SetInput(ControllerButton.LeftStick, leftStick.ReadValue<Vector2>());
        CurrentController.SetInput(ControllerButton.RightStick, rightStick.ReadValue<Vector2>());
    }

    private void PsCross(InputAction.CallbackContext obj) => CurrentController.SetInput(ControllerButton.Cross, obj.ReadValueAsButton());
    private void PsSquare(InputAction.CallbackContext obj) => CurrentController.SetInput(ControllerButton.Square, obj.ReadValueAsButton());
    private void PsTriangle(InputAction.CallbackContext obj) => CurrentController.SetInput(ControllerButton.Triangle, obj.ReadValueAsButton());
    private void PsCircle(InputAction.CallbackContext obj) => CurrentController.SetInput(ControllerButton.Circle, obj.ReadValueAsButton());
    
}

public enum ControllerButton
{
    LeftStick,
    RightStick,
    Triangle,
    Circle,
    Cross,
    Square
}

public class Ps4Controller
{
    public Vector2 LeftStick { get; private set; }
    public Vector2 RightStick { get; private set; }
    public bool Triangle { get; private set; }
    public bool Circle { get; private set; }
    public bool Cross { get; private set; }
    public bool Square { get; private set; }
    public float SquareDeadZone { get; private set; }
    
    public void SetInput(ControllerButton button, object value)
    {
        switch (button)
        {
            case ControllerButton.LeftStick:
                {
                    var val = (Vector2)value;
                    if(val.magnitude < SquareDeadZone) val = Vector2.zero;
                    LeftStick = val; break;
                }
            case ControllerButton.RightStick: 
                {
                    var val = (Vector2)value;
                    if(val.magnitude < SquareDeadZone) val = Vector2.zero;
                    RightStick = (Vector2)value; 
                }
                break;
            case ControllerButton.Triangle: Triangle = (bool)value; break;
            case ControllerButton.Circle: Circle = (bool)value; break;
            case ControllerButton.Cross: Cross = (bool)value; break;
            case ControllerButton.Square: Square = (bool)value; break;
        }
    }

    public void SetDeadZone(float deadZone)
    {
        SquareDeadZone = deadZone * deadZone;
    }
}
