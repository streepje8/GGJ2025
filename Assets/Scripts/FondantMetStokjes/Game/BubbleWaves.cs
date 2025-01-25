using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gnomez/Bubble Waves Container", fileName = "New BubbleWaves", order = 0)]
public class BubbleWaves : ScriptableObject
{
    [field: SerializeField]public List<BubbleWave> Waves { get; private set; } = new();
}