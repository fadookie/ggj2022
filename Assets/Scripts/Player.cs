using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

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

    private readonly ReactiveProperty<Vector3> observablePosition = new ReactiveProperty<Vector3>();
    public ReadOnlyReactiveProperty<Vector3> ObservablePosition => observablePosition.ToReadOnlyReactiveProperty();

    void Awake() {
        instance = this;
    }

    protected void Start()
    {
    }

    protected void Update()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.D)) direction.x += 1;
        if (Input.GetKey(KeyCode.A)) direction.x -= 1;
        if (Input.GetKey(KeyCode.W)) direction.z += 1;
        if (Input.GetKey(KeyCode.S)) direction.z -= 1;

        characterController.Move(direction.normalized * speed * Time.deltaTime);

        meshRenderer.material.color = currentColor.Value.GetColor();

        gameObject.layer = GetPlayerLayer();
        observablePosition.Value = transform.position;
    }

    protected void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var npc = hit.gameObject.GetComponent<NPC>();
        if(npc != null)
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
