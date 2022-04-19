using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static float xOffset = 2, yOffset = 1, zOffset = 1.74f;
    private GlowHighlight glowHighlight;
    private Health healt;
    private Animator animator;
    public bool isAlive;
    internal Vector3Int GetEnemyHexCoords()
        => offsetCoordinates;


    [Header("Offset coordinates")]
    [SerializeField]
    private Vector3Int offsetCoordinates;


    public void DamageDealt(int damage)
    {
        ReceiveHit();
        healt.ModifyHealth(damage*(-1));

    }

    private void Update()
    {
        isAlive = healt.aliveCheck();

        if (!healt.aliveCheck())
        {
            Death();
        }
    }

    public void Awake()
    {
        offsetCoordinates = ConvertPositionToOffset(transform.position);
        glowHighlight = GetComponent<GlowHighlight>();
        healt = GetComponent<Health>();
        animator = GetComponent<Animator>();
    }


    public static Vector3Int ConvertPositionToOffset(Vector3 position)
    {
        int x = Mathf.CeilToInt(position.x / xOffset);
        int y = Mathf.RoundToInt(position.y / yOffset);
        int z = Mathf.RoundToInt(position.z / zOffset);

        return new Vector3Int(x, y-1, z);
    }


    public void DeselectEnemy()
    {
        glowHighlight.ToggleGlow(false);
    }

    public void SelectEnemy()
    {

        glowHighlight.ToggleGlow();
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
