using UnityEngine;

[ExecuteInEditMode]
public class SetBulges : MonoBehaviour
{
    private static readonly int BulgePositionA = Shader.PropertyToID("_BulgePositionA");
    private static readonly int BulgePositionB = Shader.PropertyToID("_BulgePositionB");
    public Material material;
    public Transform bulgePointA;
    public Transform bulgePointB;

    void LateUpdate()
    {
        if (bulgePointA == null || bulgePointB == null) return;
        material.SetVector(BulgePositionA, bulgePointA.position);
        material.SetVector(BulgePositionB, bulgePointB.position);
    }
}
