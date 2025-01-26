using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdentifier : MonoBehaviour
{
    private static readonly int Gradient1 = Shader.PropertyToID("_Gradient");
    public List<Texture2D> textures;
    public Renderer rend;
    private Material mat;
    
    private void Awake()
    {
        mat = new Material(rend.sharedMaterial);
        rend.sharedMaterial = mat;
    }

    public int GetID()
    {
        return id;
    }

    private int id = -1;
    public void SetID(int id)
    {
        this.id = id;
        mat = new Material(rend.sharedMaterial);
        rend.sharedMaterial = mat;
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        mat.SetTexture(Gradient1, textures[id - 1]);
    }
}
