using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{

    [SerializeField]
    private int maxHealth = 50;

    private int curHealth = 0;

    [SerializeField]
    protected int ScoreGiven = 10;

    // Start is called before the first frame update
    void Start()
    {
        curHealth = maxHealth;
    }

    public void initialize(float maxhealthMultiplier)
    {
       curHealth = (int)((float)maxHealth * maxhealthMultiplier);
    }
    public void TakeDamage(int damage)
    {
        curHealth -= damage;
        if(curHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        GetComponent<EntityChunkComponent>()?.GetOwner().NotifyMonsterKilled(ScoreGiven);
        GetComponent<EntityChunkComponent>()?.GetOwner().NotifyEntityDestroyed(gameObject);
        Destroy(gameObject);
    }
}
