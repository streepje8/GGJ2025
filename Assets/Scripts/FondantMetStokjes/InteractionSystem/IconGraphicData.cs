using System;
using FondantMetStokjes.Player;
using UnityEngine;
using UnityEngine.UI;

namespace FondantMetStokjes.InteractionSystem
{
    [CreateAssetMenu(menuName = "Gnomez/Icon Graphic Data", fileName = "New IconGraphicData", order = 0)]
    public class IconGraphicData : ScriptableObject
    {
        [field: Header("Animation Settings")]
        [field: SerializeField] public float AnimationDuration { get; private set; } = 0.5f;
        [field: SerializeField] public GameObject IconObject { get; private set; }
        [field: SerializeField] public Vector3 IconOffset { get; private set; } = Vector3.up;
        [field: SerializeField] public Vector3 IconScale { get; private set; } = Vector3.one;
        [field: SerializeField] public AnimationCurve OffsetAnimation { get; private set; }
        [field: SerializeField] public AnimationCurve ScaleAnimation { get; private set; }

        [field: Header("Graphics")]
        [field: SerializeField] public Texture2D LeftStickIconTexture { get; private set; }
        [field: SerializeField] public Texture2D RightStickIconTexture { get; private set; }
        [field: SerializeField] public Texture2D TriangleButtonIconTexture { get; private set; }
        [field: SerializeField] public Texture2D CircleButtonIconTexture { get; private set; }
        [field: SerializeField] public Texture2D CrossButtonIconTexture { get; private set; }
        [field: SerializeField] public Texture2D SquareButtonIconTexture { get; private set; }
    
        public void ApplyVisual(Transform display, ControllerButton button)
        {
            RawImage image = display.GetComponentInChildren<RawImage>(true);
            switch (button)
            {
                case ControllerButton.LeftStick:
                    image.texture = LeftStickIconTexture;
                    break;
                case ControllerButton.RightStick:
                    image.texture = RightStickIconTexture;
                    break;
                case ControllerButton.Triangle:
                    image.texture = TriangleButtonIconTexture;
                    break;
                case ControllerButton.Circle:
                    image.texture = CircleButtonIconTexture;
                    break;
                case ControllerButton.Cross:
                    image.texture = CrossButtonIconTexture;
                    break;
                case ControllerButton.Square:
                    image.texture = SquareButtonIconTexture;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(button), button, null);
            }
        }
    }
}