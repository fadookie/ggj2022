using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Jobs;

[RequireComponent(typeof(CharacterSprite))]
public class Player : MonoBehaviour
{
    public GameColor CurrentColor => currentColor.Value;
    public IObservable<GameColor> ObservableColor => currentColor;

    private static Player instance;
    public static Player Instance {
        get { return instance; }
    }

    public float Radius => capsuleCollider.radius;

    [SerializeField] private ReactiveProperty<GameColor> currentColor;

    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speed = 1;

    [SerializeField] private MeshRenderer meshRenderer;

    [SerializeField] private Material blackMaterial;
    [SerializeField] private Material whiteMaterial;
    
    private CharacterSprite characterSprite;

    private Vector3 lastDirection;

    private float possessionStartTime;
    [SerializeField] private float possessionTimerDurationSec;
    [SerializeField] private float damageTimerReductionSec;
    
    public float PossessionTimerDurationSec => possessionTimerDurationSec;
    public float ElapsedPossessionTime => Time.time - possessionStartTime;

    private CapsuleCollider capsuleCollider;

    void Awake() {
        instance = this;
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    protected void Start()
    {
        characterSprite = GetComponent<CharacterSprite>();
        characterSprite.possessed = true;
        currentColor.Subscribe(OnColorChange);
        possessionStartTime = Time.time;
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

//        Debug.Log($"time:{Time.time} posStartTime:{possessionStartTime} duration:{Time.time - possessionStartTime}");
        if (ElapsedPossessionTime > possessionTimerDurationSec) {
            Die();
        }
    }

    protected void LateUpdate()
    {
        meshRenderer.transform.rotation = Quaternion.Euler(90, 0, 0);
        var scale = meshRenderer.transform.localScale;
        scale.x = Mathf.Abs(meshRenderer.transform.localScale.x) * ((lastDirection.x >= 0) ? 1 : -1);
        meshRenderer.transform.localScale = scale;
        
        var position = transform.position;
        position.y = 0;
        transform.position = position;
    }

    private void Die() {
        gameObject.SetActive(false);
        AudioManager.Instance.PlaySound(AudioManager.Sound.Death);
        Debug.LogWarning($"Player died. time:{Time.time} posStartTime:{possessionStartTime} duration:{ElapsedPossessionTime}");
    }

    private void OnColorChange(GameColor color)
    {
        meshRenderer.material = GetPlayerMaterial();
        characterSprite.color = currentColor.Value;

        gameObject.layer = GetPlayerLayer();
    }
    
    public void OnNPCCollisionEnter(NPC npc) {
//        Debug.Log($"OnNPCCollisionEnter: {npc}");
        if (npc.Color == currentColor.Value) {
            // damage
            Debug.LogWarning($"OnNPCCollisionEnter NPC hit current timer: {ElapsedPossessionTime} new timer:{Time.time - (possessionStartTime - damageTimerReductionSec)}");
            possessionStartTime -= damageTimerReductionSec;
            // Timer rundown will be checked next update
            AudioManager.Instance.PlaySound(AudioManager.Sound.TakeDamage);
        } else {
            // possess enemy
            StartCoroutine(DelayedSetPosition(npc.transform.position));
            currentColor.Value = npc.Color;
            if (ScoreTracker.TryGetInstance(out var scoreTracker)) {
                scoreTracker.Score += 1;
            }
            npc.Die();
            possessionStartTime = Time.time;
            AudioManager.Instance.PlaySound(AudioManager.Sound.Possession);
        }
    }

    private void OnCollisionStay(Collision other) {
        Debug.Log($"OnCollisionStay: {other} name:{other.gameObject.name}");
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
