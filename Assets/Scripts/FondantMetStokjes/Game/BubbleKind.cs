using UnityEngine;

[CreateAssetMenu(menuName = "Gnomez/Bubble Kind", fileName = "New Bubble Kind", order = 0)]
public class BubbleKind : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; } = "Unnamed Bubble";
    [field: SerializeField] public Texture2D Icon { get; private set; }
    [field: ColorUsage(true, true)]
    [field: SerializeField] public Color Tint { get; private set; }
    [field: SerializeField] public Vector3 Scale { get; private set; } = Vector3.one;
}