using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gnomez/Bubble Wave", fileName = "New BubbleWave", order = 0)]
public class BubbleWave : ScriptableObject
{
    [field: SerializeField] public List<BubbleKind> Requests { get; private set; } = new();
}