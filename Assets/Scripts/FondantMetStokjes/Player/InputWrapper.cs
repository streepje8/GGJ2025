using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FondantMetStokjes.Player
{
    public class InputWrapper : MonoBehaviour
    {
        public Ps4Controller CurrentController { get; private set; } = new Ps4Controller();
        [field: SerializeField]public float JoystickDeadZone { get; private set; } = 0.1f;
        private PlayerInput input;
        private InputAction leftStick;
        private InputAction rightStick;
        private InputAction cross;
        private InputAction square;
        private InputAction triangle;
        private InputAction circle;

        private void Awake()
        {
            input = GetComponent<PlayerInput>();
            CurrentController.SetDeadZone(JoystickDeadZone * JoystickDeadZone);
            leftStick = input.actions["LeftStick"];
            rightStick = input.actions["RightStick"];
            cross = input.actions["Cross"];
            square = input.actions["Square"];
            triangle = input.actions["Triangle"];
            circle = input.actions["Circle"];
        }

        private void Update()
        {
            CurrentController.SetInput(ControllerButton.LeftStick, leftStick.ReadValue<Vector2>());
            CurrentController.SetInput(ControllerButton.RightStick, rightStick.ReadValue<Vector2>());
            CurrentController.SetInput(ControllerButton.Cross, cross.ReadValue<float>() > 0);
            CurrentController.SetInput(ControllerButton.Square, square.ReadValue<float>() > 0);
            CurrentController.SetInput(ControllerButton.Triangle, triangle.ReadValue<float>() > 0);
            CurrentController.SetInput(ControllerButton.Circle, circle.ReadValue<float>() > 0);
        }
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
        private bool LeftStickWasPressed { get; set; }
        public Vector2 RightStick { get; private set; }
        private bool RightStickWasPressed { get; set; }
        public bool Triangle { get; private set; }
        private bool TriangleWasPressed { get; set; }
        public bool Circle { get; private set; }
        private bool CircleWasPressed { get; set; }
        public bool Cross { get; private set; }
        private bool CrossWasPressed { get; set; }
        public bool Square { get; private set; }
        private bool SquareWasPressed { get; set; }
        public float SquareDeadZone { get; private set; }
    
        public void SetInput(ControllerButton button, object value)
        {
            switch (button)
            {
                case ControllerButton.LeftStick:
                {
                    var val = (Vector2)value;
                    if (val.magnitude < SquareDeadZone)
                    {
                        val = Vector2.zero;
                        LeftStickWasPressed = false;
                    }
                    LeftStick = val; break;
                }
                case ControllerButton.RightStick: 
                {
                    var val = (Vector2)value;
                    if (val.magnitude < SquareDeadZone)
                    {
                        val = Vector2.zero;
                        RightStickWasPressed = false;
                    }
                    RightStick = (Vector2)value; 
                }
                    break;
                case ControllerButton.Triangle: 
                    Triangle = (bool)value;
                    TriangleWasPressed &= Triangle;
                    break;
                case ControllerButton.Circle: 
                    Circle = (bool)value; 
                    CircleWasPressed &= Circle;
                    break;
                case ControllerButton.Cross: 
                    Cross = (bool)value; 
                    CrossWasPressed &= Cross;
                    break;
                case ControllerButton.Square: 
                    Square = (bool)value;
                    SquareWasPressed &= Square;
                    break;
            }
        }

        public void SetDeadZone(float deadZone)
        {
            SquareDeadZone = deadZone * deadZone;
        }

        public bool GetButton(ControllerButton button)
        {
            switch (button)
            {
                case ControllerButton.LeftStick:
                    return LeftStick.magnitude > SquareDeadZone;
                case ControllerButton.RightStick:
                    return RightStick.magnitude > SquareDeadZone;
                case ControllerButton.Triangle:
                    return Triangle;
                case ControllerButton.Circle:
                    return Circle;
                case ControllerButton.Cross:
                    return Cross;
                case ControllerButton.Square:
                    return Square;
                default:
                    throw new ArgumentOutOfRangeException(nameof(button), button, null);
            }
        }

        public bool GetButtonPressed(ControllerButton button)
        {
            var current = GetButton(button);
            switch (button)
            {
                case ControllerButton.LeftStick:
                    if (current)
                    {
                        if (!LeftStickWasPressed)
                        {
                            LeftStickWasPressed = true;
                            return true;
                        }
                    }
                    return false;
                case ControllerButton.RightStick:
                    if (current)
                    {
                        if (!RightStickWasPressed)
                        {
                            RightStickWasPressed = true;
                            return true;
                        }
                    }
                    return false;
                case ControllerButton.Triangle:
                    if (current)
                    {
                        if (!TriangleWasPressed)
                        {
                            TriangleWasPressed = true;
                            return true;
                        }
                    }
                    return false;
                case ControllerButton.Circle:
                    if (current)
                    {
                        if (!CircleWasPressed)
                        {
                            CircleWasPressed = true;
                            return true;
                        }
                    }
                    return false;
                case ControllerButton.Cross:
                    if (current)
                    {
                        if (!CrossWasPressed)
                        {
                            CrossWasPressed = true;
                            return true;
                        }
                    }
                    return false;
                case ControllerButton.Square:
                    if (current)
                    {
                        if (!SquareWasPressed)
                        {
                            SquareWasPressed = true;
                            return true;
                        }
                    }
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(button), button, null);
            }
        }
    }
}