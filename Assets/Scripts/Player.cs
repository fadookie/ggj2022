using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[RequireComponent(typeof(CharacterSprite))]
public class Player : MonoBehaviour
{
    public GameColor CurrentColor => currentColor.Value;
    public IObservable<GameColor> ObservableColor => currentColor;

    private static Player instance;
    public static Player Instance {
        get { return instance; }
    }

    [SerializeField] private ReactiveProperty<GameColor> currentColor;

    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speed = 1;

    [SerializeField] private MeshRenderer meshRenderer;

    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material whiteMaterial;
    
    private CharacterSprite characterSprite;

    private Vector3 lastDirection;

    void Awake() {
        instance = this;
    }

    protected void Start()
    {
        characterSprite = GetComponent<CharacterSprite>();
        characterSprite.posessed = true;
        currentColor.Subscribe(OnColorChange);
    }
    protected void Update()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.D)) direction.x += 1;
        if (Input.GetKey(KeyCode.A)) direction.x -= 1;
        if (Input.GetKey(KeyCode.W)) direction.z += 1;
        if (Input.GetKey(KeyCode.S)) direction.z -= 1;

        if(direction != Vector3.zero)
        {
            characterController.Move(direction.normalized * speed * Time.deltaTime);
            lastDirection = direction;
        }
    }

    protected void LateUpdate()
    {
        meshRenderer.transform.rotation = Quaternion.Euler(90, 0, 0);
        var scale = meshRenderer.transform.localScale;
        scale.x = Mathf.Abs(meshRenderer.transform.localScale.x) * ((lastDirection.x >= 0) ? 1 : -1);
        meshRenderer.transform.localScale = scale;
    }

    private void OnColorChange(GameColor color)
    {
        meshRenderer.material = GetPlayerMaterial();
        characterSprite.color = currentColor.Value;

        gameObject.layer = GetPlayerLayer();
    }

    protected void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(NPC.TryGetNPC(hit.gameObject, out NPC npc))
        {
            if (npc.Color == currentColor.Value)
            {
                //death
            }
            else
            {
                StartCoroutine(DelayedSetPosition(npc.transform.position));
                currentColor.Value = npc.Color;
                if(ScoreTracker.TryGetInstance(out var scoreTracker))
                {
                    scoreTracker.Score += 1;
                }
                npc.Die();
            }
        }
    }

    IEnumerator DelayedSetPosition(Vector3 position)
    {
        transform.position = position;
        yield return null;
        transform.position = position;
    }

    private Material GetPlayerMaterial()
    {
        switch (currentColor.Value)
        {
            case GameColor.Black: return blackMaterial;
            case GameColor.White: return whiteMaterial;
            default:
                throw new System.NotFiniteNumberException();
        }
    }
    
    private int GetPlayerLayer()
    {
        switch(currentColor.Value)
        {
            case GameColor.Black: return LayerMask.NameToLayer("Black Player");
            case GameColor.White: return LayerMask.NameToLayer("White Player");
            default: throw new System.NotFiniteNumberException();
        }
    }
}
