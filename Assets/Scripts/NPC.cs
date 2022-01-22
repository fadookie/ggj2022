using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class NPC : MonoBehaviour
{
    public GameColor Color => color;
    [SerializeField] private GameColor color;

    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material whiteMaterial;
    [SerializeField] private MeshRenderer meshRenderer;

    private bool dead;

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
        switch (color)
        {
            case GameColor.Black: return blackMaterial;
            case GameColor.White: return whiteMaterial;
            default: throw new System.NotImplementedException();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
