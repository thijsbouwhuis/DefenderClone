using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Chunk : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private ChunkManager chunkManagerRef;

    private List<GameObject> entitiesInChunk;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Assert.IsNotNull(spriteRenderer);
    }
    void Start()
    {
        entitiesInChunk = new List<GameObject>();

        Color newColor = spriteRenderer.color;
        newColor.a = 0.1f;
        spriteRenderer.color = newColor;
    }
    private void Update()
    {
        //Iterate through each enemy to check if we should move them to another chunk
        for(int i = 0; i < entitiesInChunk.Count; i++)
        {
            GameObject entity = entitiesInChunk[i];
            Assert.IsNotNull(entity);
            float entityXPos = entity.transform.position.x;
            Bounds chunkBounds = GetChunkBounds();
            if (entityXPos < chunkBounds.min.x)
            {
                if (chunkManagerRef.MoveEntityToNewChunk(entity, true, gameObject))
                {
                    entitiesInChunk.Remove(entity);
                    i--;
                }
            }
            else if (entityXPos > chunkBounds.max.x)
            {
                if (chunkManagerRef.MoveEntityToNewChunk(entity, false, gameObject))
                {
                    entitiesInChunk.Remove(entity);
                    i--;
                }
            }
        }
    }
    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
    public void SetManagerRef(GameObject chunkManagerRef)
    {
        this.chunkManagerRef = chunkManagerRef.GetComponent<ChunkManager>();
        Assert.IsNotNull(this.chunkManagerRef);
    }
    public void MoveChunk(Vector3 newPosition)
    {
        Vector3 originalChunkPosition = transform.position;
        transform.position = newPosition;
        foreach(GameObject entity in entitiesInChunk)
        { 
            Vector3 offset = originalChunkPosition - entity.transform.position;
            entity.transform.position = transform.position - offset;
        }
    }
    public void SpawnEnemy(List<GameObject> enemyTypes, float healthMultiplier)
    {
        int randOffset = Random.Range(0, 2) == 0 ? -1 : 1;
        GameObject enemy = Instantiate(enemyTypes[Random.Range(0, enemyTypes.Count)], new Vector3(transform.position.x, transform.position.y - 10f * randOffset, transform.position.z), new Quaternion(0f, 0f, 0f, 0f));
        enemy.GetComponent<SquidEnemyMovement>()?.SetMoveToTarget(new Vector3(transform.position.x + Random.Range(-8f, 8f), transform.position.y + Random.Range(-1f, -6f), transform.position.z));
        enemy.GetComponent<HealthComponent>()?.initialize(healthMultiplier);
        enemy.GetComponent<EntityChunkComponent>().SetOwner(this);
        entitiesInChunk.Add(enemy);
    }

    public void SpawnEnemyOnPos(GameObject enemyType, Vector3 Position)
    {
        GameObject enemy = Instantiate(enemyType, Position, new Quaternion(0f, 0f, 0f, 0f));
        enemy.GetComponent<EntityChunkComponent>().SetOwner(this);
        entitiesInChunk.Add(enemy);
    }

    public void OnBulletSpawned(GameObject bullet)
    {
       entitiesInChunk.Add(bullet);
    }

    public bool MoveEntityToChunk(GameObject entity)
    {
        entitiesInChunk.Add(entity);
        entity.GetComponent<EntityChunkComponent>()?.SetOwner(this);
        return true;
    }
    public Bounds GetChunkBounds()
    {
        return GetComponent<SpriteRenderer>().bounds;
    }

    public void NotifyEntityDestroyed(GameObject entity)
    {
        entitiesInChunk.Remove(entity);
    }

    public void NotifyMonsterKilled(int pointsScored)
    {
        chunkManagerRef.OnMonsterKilled(pointsScored);
    }

    public void ClearChunk()
    {
        foreach (GameObject entity in entitiesInChunk)
        {
            Destroy(entity);
        }
        entitiesInChunk.Clear();
    }
}
