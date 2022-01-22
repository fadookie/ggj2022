using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameColor CurrentColor => currentColor;
    [SerializeField] private GameColor currentColor;

    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speed = 1;

    [SerializeField] private MeshRenderer meshRenderer;

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
