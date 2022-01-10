using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MothershipHealthComponent : HealthComponent
{
    [SerializeField]
    private GameObject enemiesToSpawn;

    [SerializeField]
    private int spawnCount = 7;


    private void Awake()
    {
        Assert.IsNotNull(enemiesToSpawn);
    }

    public override void Die()
    {
        for(int i = 0; i < spawnCount; i++)
        {
           GetComponent<EntityChunkComponent>()?.GetOwner().SpawnEnemyOnPos(enemiesToSpawn,new Vector3(transform.position.x + Random.Range(-2f, 2f), transform.position.y + Random.Range(-2f, 2f), transform.position.z));
        }
        GetComponent<EntityChunkComponent>()?.GetOwner().NotifyMonsterKilled(ScoreGiven);
        GetComponent<EntityChunkComponent>()?.GetOwner().NotifyEntityDestroyed(gameObject);
        Destroy(gameObject);
    }
}
