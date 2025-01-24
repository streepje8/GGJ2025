using UnityEngine;

public class SetBulge : MonoBehaviour
{
    public Material material;
    public GameObject bulgePoint;
    void Start()
    {
        
    }

    void LateUpdate()
    {
        material.SetVector("_BulgePosition", bulgePoint.transform.position);
    }
}
