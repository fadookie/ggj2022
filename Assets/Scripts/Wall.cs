using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[ExecuteAlways]
public class Wall : MonoBehaviour
{
    public GameColor Color => Color;
    [SerializeField] private GameColor color;

    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material whiteMaterial;

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private NavMeshModifierVolume navMeshModifierVolume;

    public void Setup(GameColor gameColor, Vector3 size)
    {
        color = gameColor;

        meshRenderer.transform.localScale = GetWorldScale(size);
        boxCollider.size = GetWorldScale(size);
        navMeshModifierVolume.size = GetWorldScale(size + Vector3.one);
        Update();
    }

    private Vector3 GetWorldScale(Vector3 scale)
    {
        if(transform.parent != null)
        {
            scale.x /= transform.parent.lossyScale.x;
            scale.y /= transform.parent.lossyScale.y;
            scale.z /= transform.parent.lossyScale.z;
        }
        return scale;
    }

    protected void Start()
    {
        
    }

    protected void Update()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material = GetMaterial();
        }
        gameObject.layer = GetWallLayer();
        navMeshModifierVolume.area = GetNavAreaType();
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

    private int GetWallLayer()
    {
        switch (color)
        {
            case GameColor.Black: return LayerMask.NameToLayer("Black Wall");
            case GameColor.White: return LayerMask.NameToLayer("White Wall");
            default: throw new System.NotFiniteNumberException();
        }
    }

    private int GetNavAreaType()
    {
        switch (color)
        {
            case GameColor.Black: return 4;
            case GameColor.White: return 3;
            default: throw new System.NotFiniteNumberException();
        }
    }
}
