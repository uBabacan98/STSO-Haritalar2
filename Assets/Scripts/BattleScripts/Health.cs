using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    private int currentHealth ;

    [SerializeField]
    private bool isAlive = true;

    public event Action<float> OnHealthPctChanged = delegate { };

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public void ModifyHealth(int amount)
    {
        if (isAlive)
        {
            currentHealth += amount;

            float currenHealthPct = (float)currentHealth / (float)maxHealth;
            OnHealthPctChanged(currenHealthPct);
        }
        else
        {
            Debug.Log("Died");
        }
    }

    private void Update()
    {
        if(currentHealth <= 0)
        {
            isAlive = false;
        }
    }
    
    public bool aliveCheck()
    {
        return isAlive;
    }

}
