using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Unit : MonoBehaviour
{

    [Header("Offset coordinates")]
    [SerializeField]
    private Vector3Int offsetCoordinates;

    [SerializeField]
    private int movementPoints = 20;

    [SerializeField]
    private int damagePoint = 10;

    [SerializeField]
    private int attackRange = 1;

    [SerializeField]
    private float movementDuration = 1, rotationDuration = 0.3f;

    public static float xOffset = 2, yOffset = 1, zOffset = 1.74f;

    public bool isAlive;

    private GlowHighlight glowHighlight;
    private Queue<Vector3> pathPositions = new Queue<Vector3>();

    public event Action<Unit> MovementFinished;

    private Animator animator;

    private Health health;

    public int MovementPoints { get => movementPoints; }
    public int DamagePoints { get => damagePoint; }
    public int AttackRange { get => attackRange; }

    internal Vector3Int GetUnitHexCoords()
        => offsetCoordinates;


    public void Awake()
    {
        offsetCoordinates = ConvertPositionToOffset(transform.position);
        glowHighlight = GetComponent<GlowHighlight>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();


    }

    public void Update()
    {


        isAlive = health.aliveCheck();

        if (!health.aliveCheck())
        {
            Death();
        }
        offsetCoordinates = ConvertPositionToOffset(transform.position);
    }


    public void DamageDealt(int damage)
    {
        ReceiveHit();
        health.ModifyHealth(damage * (-1));

    }



    public static Vector3Int ConvertPositionToOffset(Vector3 position)
    {
        int x = Mathf.CeilToInt(position.x / xOffset);
        int y = Mathf.RoundToInt(position.y / yOffset);
        int z = Mathf.RoundToInt(position.z / zOffset);

        return new Vector3Int(x, y - 1, z);
    }

    public void Deselect()
    {
        glowHighlight.ToggleGlow(false);
    }

    public void Select()
    {
        
        glowHighlight.ToggleGlow();
    }

    public void MoveThroughPath(List<Vector3> currentPath, Unit unitReference)
    {
        pathPositions = new Queue<Vector3>(currentPath);
        Vector3 firstTarget = pathPositions.Dequeue();
        StartCoroutine(RotationCoroutine(firstTarget, rotationDuration,unitReference));
    }

    private IEnumerator RotationCoroutine(Vector3 endPosition, float rotationDuration,Unit unitReference)
    {  
        Quaternion startRotation = transform.rotation;
        //Debug.Log("Start rotation" + startRotation);
        endPosition.y = transform.position.y;
        //Debug.Log("End position y" + endPosition.y);
        Vector3 direction = endPosition - transform.position;
        //Debug.Log("Direction" + direction);
        Quaternion endRotation = Quaternion.LookRotation(direction, Vector3.up);
        //Debug.Log("End position" + endPosition);

        if (Mathf.Approximately(Mathf.Abs(Quaternion.Dot(startRotation, endRotation)), 1.0f) == false)
        {
            float timeElapsed = 0;
            while (timeElapsed < rotationDuration)
            {
                timeElapsed += Time.deltaTime;
                float lerpStep = timeElapsed / rotationDuration; // 0-1
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, lerpStep);
                yield return null;
            }
            transform.rotation = endRotation;
        }
        StartCoroutine(MovementCoroutine(endPosition,unitReference));
    }

    private IEnumerator MovementCoroutine(Vector3 endPosition,Unit unitReference)
    {
        Vector3 startPosition = transform.position;
        endPosition.y = startPosition.y;
        float timeElapsed = 0;

        while (timeElapsed < movementDuration)
        {
            timeElapsed += Time.deltaTime;
            float lerpStep = timeElapsed / movementDuration;
            transform.position = Vector3.Lerp(startPosition, endPosition, lerpStep);
            yield return null;
        }
        transform.position = endPosition;

        if (pathPositions.Count > 0)
        {

            //Debug.Log($"Selecting the next position!");
            StartCoroutine(RotationCoroutine(pathPositions.Dequeue(), rotationDuration,unitReference));
        }
        else
        {
            unitReference.Idle();
            Debug.Log("Movement finished!");
            MovementFinished?.Invoke(this);
        }
    }

    public void Idle()
    {
        animator.SetFloat("Speed", 0);
    }

    public void Walk()
    {
        animator.SetFloat("Speed", 1);
    }

    public void Attack()
    {
        animator.SetTrigger("AttackTrigger");
    }


    public void Death()
    {
        animator.SetTrigger("DeathTrigger");
    }

    public void ReceiveHit()
    {
        animator.SetTrigger("ReceiveHitTrigger");
    }


}