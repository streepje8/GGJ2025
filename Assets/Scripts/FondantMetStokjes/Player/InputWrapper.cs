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

        private void LateUpdate()
        {
            CurrentController.Update();
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


        private bool LeftStickPressedQueue = false;
        private bool RightStickPressedQueue = false;
        private bool SquarePressedQueue = false;
        private bool TrianglePressedQueue = false;
        private bool CirclePressedQueue = false;
        private bool CrossPressedQueue = false;
        
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
                            LeftStickPressedQueue = true;
                            return true;
                        }
                    }
                    return false;
                case ControllerButton.RightStick:
                    if (current)
                    {
                        if (!RightStickWasPressed)
                        {
                            RightStickPressedQueue = true;
                            return true;
                        }
                    }
                    return false;
                case ControllerButton.Triangle:
                    if (current)
                    {
                        if (!TriangleWasPressed)
                        {
                            TrianglePressedQueue = true;
                            return true;
                        }
                    }
                    return false;
                case ControllerButton.Circle:
                    if (current)
                    {
                        if (!CircleWasPressed)
                        {
                            CirclePressedQueue = true;
                            return true;
                        }
                    }
                    return false;
                case ControllerButton.Cross:
                    if (current)
                    {
                        if (!CrossWasPressed)
                        {
                            CrossPressedQueue = true;
                            return true;
                        }
                    }
                    return false;
                case ControllerButton.Square:
                    if (current)
                    {
                        if (!SquareWasPressed)
                        {
                            SquarePressedQueue = true;
                            return true;
                        }
                    }
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(button), button, null);
            }
        }

        public void Update()
        {
            if (LeftStickPressedQueue)
            {
                LeftStickWasPressed = true;
                LeftStickPressedQueue = false;
            }
            if (RightStickPressedQueue)
            {
                RightStickWasPressed = true;
                RightStickPressedQueue = false;
            }
            if (SquarePressedQueue)
            {
                SquareWasPressed = true;
                SquarePressedQueue = false;
            }
            if (CirclePressedQueue)
            {
                CircleWasPressed = true;
                CirclePressedQueue = false;
            }
            if (TrianglePressedQueue)
            {
                TriangleWasPressed = true;
                TrianglePressedQueue = false;
            }
            if (CrossPressedQueue)
            {
                CrossWasPressed = true;
                CrossPressedQueue = false;
            }
        }
    }
}