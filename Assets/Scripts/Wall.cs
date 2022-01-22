using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Wall : MonoBehaviour
{
    public GameColor Color => Color;
    [SerializeField] private GameColor color;

    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material whiteMaterial;
    [SerializeField] private MeshRenderer meshRenderer;

    protected void Start()
    {
        
    }

    protected void Update()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material = GetMaterial();
        }
    }

    private Material GetMaterial()
    {
        switch(color)
        {
            case GameColor.Black: return blackMaterial;
            case GameColor.White: return whiteMaterial;
            default: throw new System.NotImplementedException();
        }
    }
}
