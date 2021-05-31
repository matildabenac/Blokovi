using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public float spawnRate = 3.0f;
    public Sprite redBlock;
    public Sprite blueBlock;
    public GameObject prefab;

    private Queue<Block> _blocks;
    private ActorController _actor;
    
    private static BlockManager _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        var uiManager = FindObjectOfType<UIManager>();
        // set spawn rate
        spawnRate = uiManager ? uiManager.spawnRate : spawnRate; 
        
        // set periodically repeating event for spawning blocks
        InvokeRepeating(nameof(SpawnBlock), spawnRate, spawnRate);
        
        _blocks = new Queue<Block>();
        _actor = FindObjectOfType<ActorController>();
        
        // get first box
        StartCoroutine(GetTargetBlock());
    }

    // Update is called once per frame
    // void Update()
    // {
    //     
    // }

    private void SpawnBlock()
    {
        // get color by chance
        var det = Random.Range(-1.0f, 1.0f);
        prefab.GetComponent<SpriteRenderer>().sprite = det > 0 ? redBlock : blueBlock;
        prefab.GetComponentInChildren<Block>().blockColor = det > 0 ? Block.Color.Red : Block.Color.Blue;
        
        // random x position of generated blocks
        var x = Random.Range(-6.0f, 6.0f);

        // spawn block
        var newBlock = Instantiate(prefab, new Vector2(x, 4), Quaternion.identity);
        
        // add block to queue to be collected by player
        var eBlock = newBlock.GetComponentInChildren<Block>();
        _blocks.Enqueue(eBlock);
    }

    public IEnumerator GetTargetBlock()
    {
        Debug.Log("Getting target block");

        // setting new block target
        Block newBlock;
        do
        {
            // if queue empty wait for block to spawn
            if (_blocks.Count <= 0)
            {
                yield return new WaitUntil(() => _blocks.Count >= 1);
            }
            newBlock = _blocks.Dequeue();
        } while (!newBlock); // repeat until queue block is not null (in case a block in queue is destroyed)
        
        _actor.target = newBlock.transform;
    }
}
