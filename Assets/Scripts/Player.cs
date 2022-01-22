using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Player : MonoBehaviour
{
    public GameColor CurrentColor => currentColor;

    private static Player instance;
    public static Player Instance {
        get { return instance; }
    }

    [SerializeField] private GameColor currentColor;

    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speed = 1;

    [SerializeField] private MeshRenderer meshRenderer;

    private readonly ReactiveProperty<Vector3> observablePosition = new ReactiveProperty<Vector3>();
    public IObservable<Vector3> ObservablePosition => observablePosition;

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

        meshRenderer.material.color = currentColor.GetColor();

        observablePosition.Value = transform.position;
    }

    protected void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var npc = hit.gameObject.GetComponent<NPC>();
        if(npc != null)
        {
            if (npc.Color == currentColor)
            {
                //death
            }
            else
            {
                npc.Die();
                currentColor = npc.Color;
            }
        }
    }
}
