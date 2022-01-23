using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CharacterSprite : MonoBehaviour
{
    [SerializeField] private Material blackCharGreyEyesAngry;
    [SerializeField] private Material blackCharGreyEyesScared;
    [SerializeField] private Material blackCharRedEyesAngry;
    [SerializeField] private Material blackCharRedEyesScared;
    [SerializeField] private Material whiteCharGreyEyesAngry;
    [SerializeField] private Material whiteCharGreyEyesScared;
    [SerializeField] private Material whiteCharRedEyesAngry;
    [SerializeField] private Material whiteCharRedEyesScared;
    [SerializeField] private Renderer renderer;
    
    public GameColor color;
    public bool posessed;
    public bool angry;

    // Update is called once per frame
    void Update() {
        renderer.material = GetActiveMaterial();
    }

    private Material GetActiveMaterial() {
        switch (color) {
            case GameColor.Black: {
                if (posessed) {
                    return angry ? blackCharRedEyesAngry : blackCharRedEyesScared;
                }
                return angry ? blackCharGreyEyesAngry : blackCharGreyEyesScared;
            }
            case GameColor.White: {
                if (posessed) {
                    return angry ? whiteCharRedEyesAngry : whiteCharRedEyesScared;
                }
                return angry ? whiteCharGreyEyesAngry : whiteCharGreyEyesScared;
            }
            default: {
                throw new InvalidOperationException("Unexpected GameColor value");
            }
        }
    }
}
