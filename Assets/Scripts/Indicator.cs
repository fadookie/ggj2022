using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material whiteMaterial;


    public void Setup(GameColor gameColor, float scale, Vector3 position)
    {
        gameObject.SetActive(true);

        meshRenderer.material = GetMaterial(gameColor);

        transform.localScale = Vector3.one * scale;
        position.y = transform.position.y;
        transform.position = position;
    }

    private Material GetMaterial(GameColor gameColor)
    {
        switch(gameColor)
        {
            case GameColor.Black: return blackMaterial;
            case GameColor.White: return whiteMaterial;
            default: throw new System.NotImplementedException();
        }
    }
}
