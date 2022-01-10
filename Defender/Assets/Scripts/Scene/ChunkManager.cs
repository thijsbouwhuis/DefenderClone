using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class levelInfo
{
    [SerializeField]
    private List<GameObject> enemyTypes;

    [SerializeField]
    private int monsterCount;

    [SerializeField]
    private int maxMonstersAlive;
        
    public int GetMonsterCount()
    {
        return monsterCount;
    }

    [SerializeField]
    private float healthMultiplier = 1f;

    public float GetHealthMultiplier()
    {
        return healthMultiplier;
    }

    public List<GameObject> GetEnemyTypes()
    {
        return enemyTypes;
    }

    
    [SerializeField]
    private float enemySpawnCooldown = 2f;

    public float GetEnemySpawnCooldown()
    {
        return enemySpawnCooldown;
    }

    public int GetMaxMonstersAlive()
    {
        return maxMonstersAlive;
    }
}



public class ChunkManager : MonoBehaviour
{

    [SerializeField]
    private List<Sprite> backgroundSprites;


    [SerializeField]
    private GameObject chunkPrefab;

    [SerializeField]
    private List<levelInfo> levelInfos;


    [SerializeField]
    private float chunkSize = 200f;

    [SerializeField]
    private float resetToCenterOffset = 50f;

    private List<GameObject> chunks;
    
    private MovementComponent playerMovementComponent;

    private int currentLevel = 0;

    private int monstersKilled = 0;

    //Should keep track of this only in chunk entity array and not in 2 places but this is easy for now
    private int monsterCount = 0;


    [SerializeField]
    private ScoreTracker scoreTracker;


    [SerializeField]
    private LevelTracker levelTracker;


    [SerializeField]
    private UIManager UIComponent;


    void Awake()
    {
        chunks = new List<GameObject>();

        //Get player movement component, only GameObject that has playertag should be player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerMovementComponent = player.GetComponent<MovementComponent>();
        Assert.IsNotNull(playerMovementComponent);

        Assert.IsNotNull(scoreTracker);
        Assert.IsNotNull(levelTracker);
        Assert.IsNotNull(UIComponent);
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            //Add 1 chunk size to make the player start in the middle of the screen
            Vector3 chunkPosition = new Vector3(transform.position.x - chunkSize + chunkSize * i, transform.position.y, transform.position.z);
            GameObject chunk = Instantiate(chunkPrefab, chunkPosition, transform.rotation);
            chunk.GetComponent<Chunk>().SetSprite(backgroundSprites[i]);
            chunk.GetComponent<Chunk>().SetManagerRef(gameObject);
            chunks.Add(chunk);
        }
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3(0f, 0f, 0f);

        SetupLevel();
    }

    public void RestartGame()
    {
        //Set current level to -1 because NextLevel will increment it back to 0
        currentLevel = -1;
        monstersKilled = 0;
        monsterCount = 0;
        scoreTracker.SetScore(0);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3(0f, 0f, 0f);
        foreach(GameObject chunk in chunks)
        {
            chunk.GetComponent<Chunk>().ClearChunk();
        }
        NextLevel();
    }


    void Update()
    {
        if (playerMovementComponent.transform.position.x > resetToCenterOffset || playerMovementComponent.transform.position.x < resetToCenterOffset * -1f)
        {
            ResetChunksToCenter();
        }

    }

    void LateUpdate()
    {
        if(chunks.Count > 0)
        {
            //Check player movement if it passed certain treshholds to move a chunk.       
            if(chunks[0].transform.position.x + chunkSize > playerMovementComponent.transform.position.x)
            {
                MoveChunkLeft();
            }
            if(chunks[chunks.Count -1].transform.position.x - chunkSize < playerMovementComponent.transform.position.x)
            {
                MoveChunkRight();
            }
        }
    }

    private void SetupLevel()
    {
        monstersKilled = 0;
        InvokeRepeating(nameof(SpawnEnemies), 0f, levelInfos[currentLevel].GetEnemySpawnCooldown());
        levelTracker.SetLevel(currentLevel);
    }

    //We use this to ensure that the player can't move towards huge x numbers
    private void ResetChunksToCenter()
    {
        Vector3 relativePlayerToFirstChunkPos = chunks[0].transform.position - playerMovementComponent.transform.position;
        //Reset player
        playerMovementComponent.SetPositionOnChunkReset(new Vector3(transform.position.x - chunkSize, transform.position.y, transform.position.z) - relativePlayerToFirstChunkPos);
        //Reset all chunks
        for (int i = 0; i < 4; i++)
        {
            chunks[i].GetComponent<Chunk>().MoveChunk(new Vector3(transform.position.x - chunkSize + chunkSize * i, transform.position.y, transform.position.z));
        }

    }

    private void MoveChunkLeft()
    {
        Vector3 newPos = chunks[0].transform.position;
        newPos.x -= chunkSize;
        chunks[chunks.Count - 1].GetComponent<Chunk>().MoveChunk(newPos);

        //Move first chunk to last slot
        chunks.Insert(0, chunks[chunks.Count - 1]);
        chunks.Remove(chunks[chunks.Count - 1]);

    }
    private void MoveChunkRight()
    {
        Vector3 newPos = chunks[chunks.Count - 1].transform.position;
        newPos.x += chunkSize;
        chunks[0].GetComponent<Chunk>().MoveChunk(newPos);

        //Move first chunk to last slot
        chunks.Add(chunks[0]);
        chunks.Remove(chunks[0]);
    }
    public bool MoveEntityToNewChunk(GameObject entity, bool moveLeft, GameObject currentChunk)
    {
        int currentChunkIndex = chunks.IndexOf(currentChunk);
        if(moveLeft)
        {
            if (currentChunkIndex != 0)
            {
                return chunks[currentChunkIndex - 1].GetComponent<Chunk>().MoveEntityToChunk(entity);
            }
            else
            {
                //warp enemy from most left to most right chunk with proper x axis value
                float xOffset = (chunks[0].transform.position.x - chunks[0].GetComponent<SpriteRenderer>().size.x) - entity.transform.position.x;
                entity.transform.position = new Vector3(chunks[chunks.Count - 1].transform.position.x - xOffset, entity.transform.position.y, entity.transform.position.z);

                //Move enemy to new chunk
                return chunks[chunks.Count - 1].GetComponent<Chunk>().MoveEntityToChunk(entity);
            }
        }
        else
        {
            if (currentChunkIndex != chunks.Count - 1)
            {
                return chunks[currentChunkIndex + 1].GetComponent<Chunk>().MoveEntityToChunk(entity);
            }
            else
            {
                //warp enemy from most left to most right chunk with proper x axis value
                float xOffset = (chunks[chunks.Count - 1].transform.position.x + chunks[chunks.Count - 1].GetComponent<SpriteRenderer>().size.x) - entity.transform.position.x;
                entity.transform.position = new Vector3(chunks[0].transform.position.x - xOffset, entity.transform.position.y, entity.transform.position.z);

                //Move enemy to new chunk
                return chunks[0].GetComponent<Chunk>().MoveEntityToChunk(entity);
            }
        }
    }
    private GameObject GetPlayerCurrentChunk()
    {
        GameObject nearestChunk = null;
        float nearestChunkDistance = float.MaxValue;
        foreach(GameObject chunk in chunks)
        {
            float chunkDistance = Mathf.Abs(chunk.transform.position.x - playerMovementComponent.transform.position.x);
            if (chunkDistance < nearestChunkDistance)
            {
                nearestChunkDistance = chunkDistance;
                nearestChunk = chunk;
            }

        }
        return nearestChunk;
    }
    private List<GameObject> GetAdjacentChunks(GameObject chunk)
    {
        List<GameObject> adjacentChunks = new List<GameObject>();
        int currentChunkIndex = chunks.IndexOf(chunk);

        if(currentChunkIndex != 0 && currentChunkIndex != chunks.Count - 1)
        {
            adjacentChunks.Add(chunks[currentChunkIndex - 1]);
            adjacentChunks.Add(chunks[currentChunkIndex + 1]);
        }
        else if(currentChunkIndex == 0)
        {
            adjacentChunks.Add(chunks[chunks.Count - 1]);
            adjacentChunks.Add(chunks[1]);
        }
        else if (currentChunkIndex == chunks.Count - 1)
        {
            adjacentChunks.Add(chunks[chunks.Count - 2]);
            adjacentChunks.Add(chunks[0]);
        }
        return adjacentChunks;
    }
    private void SpawnEnemies()
    {
        GameObject currentChunk = GetPlayerCurrentChunk();
        if (currentChunk != null)
        {
            List<GameObject> chunks = GetAdjacentChunks(currentChunk);
            chunks.Add(currentChunk);

            List<GameObject> enemyTypes = levelInfos[currentLevel].GetEnemyTypes();
            foreach (GameObject chunk in chunks)
            {
                if(monsterCount < levelInfos[currentLevel].GetMaxMonstersAlive())
                {
                    monsterCount += 1;
                    chunk.GetComponent<Chunk>().SpawnEnemy(enemyTypes, levelInfos[currentLevel].GetHealthMultiplier());
                }
            }
        }
    }

    public void OnMonsterKilled(int pointsScored)
    {
        monstersKilled += 1;
        monsterCount -= 1;
        scoreTracker.AddScore(pointsScored);
        if(monstersKilled >= levelInfos[currentLevel].GetMonsterCount())
        {
            NextLevel();
        }
    }
    
    public void NextLevel()
    {
        currentLevel += 1;
        CancelInvoke(nameof(SpawnEnemies));
        if(levelInfos.Count > currentLevel)
        {
            SetupLevel();
        }
        else
        {
            FinishGame();
        }
    }

    private void FinishGame()
    {
        UIComponent.OpenGameFinishedScreen();
    }
}
