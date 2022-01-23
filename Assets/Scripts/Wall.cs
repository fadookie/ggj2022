using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

[ExecuteAlways]
public class Wall : MonoBehaviour
{
    public GameColor Color
    {
        get => color.Value;
        private set => color.Value = value;
    }

    [SerializeField] private ReactiveProperty<GameColor> color;
    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material whiteMaterial;

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private NavMeshModifierVolume navMeshModifierVolume;

    public void Setup(GameColor gameColor, Vector3 size)
    {
        Color = gameColor;
        color.Subscribe(OnColorChange);
        meshRenderer.transform.localScale = GetWorldScale(size);
        boxCollider.size = GetWorldScale(size);
        navMeshModifierVolume.size = GetWorldScale(size + new Vector3(.4f,0,.4f)*2);
        Update();
    }

    private void OnColorChange(GameColor color)
    {
        Color = color;

        if (meshRenderer != null)
        {
            meshRenderer.material = GetMaterial();
        }
        gameObject.layer = GetWallLayer();
        navMeshModifierVolume.area = GetNavAreaType();
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
    }

    private Material GetMaterial()
    {
        switch(Color)
        {
            case GameColor.Black: return blackMaterial;
            case GameColor.White: return whiteMaterial;
            default: throw new System.NotImplementedException();
        }
    }

    private int GetWallLayer()
    {
        switch (Color)
        {
            case GameColor.Black: return LayerMask.NameToLayer("Black Wall");
            case GameColor.White: return LayerMask.NameToLayer("White Wall");
            default: throw new System.NotFiniteNumberException();
        }
    }

    private int GetNavAreaType()
    {
        switch (Color)
        {
            case GameColor.Black: return 3;
            case GameColor.White: return 4;
            default: throw new System.NotFiniteNumberException();
        }
    }
}
