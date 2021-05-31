using UnityEngine;

public class ActorController : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;
    
    private Rigidbody2D _actorRb;
    private GameObject _collectedBlock;
    private BlockManager _blockManager;
    private Block.Color _collectedColor;

    private enum BlockStatus
    {
        Getting,
        Holding,
        Free
    }

    private BlockStatus _blockStatus;

    // Start is called before the first frame update
    void Start()
    {
        _actorRb = GetComponent<Rigidbody2D>();

        _blockManager = FindObjectOfType<BlockManager>();
        
        // ignore rigidbody collisions with boxes (Layer8 = Actor, Layer9 = Block)
        Physics2D.IgnoreLayerCollision(8, 9);

        target = transform;
    }

    // Update is called once per frame
    void Update()
    {
        // if target block is destroyed before collected, set target to current position and set block status free
        if (!target)
        {
            target = transform;
            _blockStatus = BlockStatus.Free;
        }
        
        // if character not holding or getting block, get next available one
        if (_blockStatus == BlockStatus.Free)
        {
            _blockStatus = BlockStatus.Getting;
            StartCoroutine(_blockManager.GetTargetBlock());
        } else
        {
            MoveActor();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        switch (other.tag)
        {
            // if collided element is the target block
            case "Block" when other.gameObject.transform == target.transform:
                CollectBlock(other.transform.parent.gameObject);
                break;
            // if collided element is a box of the matching color as the collected block;
            case "Box" when other.gameObject.GetComponent<Block>().blockColor == _collectedColor:
            {
                if(_blockStatus == BlockStatus.Holding) DropBlock();
                break;
            }
        }
    }

    private void MoveActor()
    {
        // find direction of the target
        var direction = target.transform.position - transform.position;
        direction.Normalize();

        // flip sprite if change in direction happened
        if (direction.x * transform.localScale.x > 0)
        {
            var newTransform = transform;
            var localScale = newTransform.localScale;
            localScale = new Vector2(-1 * localScale.x, localScale.y);
            newTransform.localScale = localScale;
        }
        
        // move character in direction of the target
        _actorRb.velocity = new Vector2(direction.x * speed, _actorRb.velocity.y);
    }

    private void CollectBlock(GameObject block)
    {
        // collecting the box
        _collectedBlock = block;
        var blockInfo = _collectedBlock.GetComponentInChildren<Block>();
        _collectedColor = blockInfo.blockColor;
            
        Debug.Log("I collected " + blockInfo.blockColor + " block");

        // change target to box of matching color
        target = GameObject.Find(blockInfo.blockColor + " Box").transform;
        
        _blockStatus = BlockStatus.Holding;
        
        // attaching box to actor
        _collectedBlock.transform.SetParent(transform);
        // enable block to follow actor
        var blockRb = _collectedBlock.GetComponent<Rigidbody2D>();
        blockRb.isKinematic = true;
        // disable block from moving other blocks
        var blockCollider = _collectedBlock.GetComponent<Collider2D>();
        blockCollider.enabled = false;
    }

    private void DropBlock()
    {
        if(_collectedBlock != null) Destroy(_collectedBlock);
        _blockStatus = BlockStatus.Free;
        
        Debug.Log("I dropped a block");
    }
}
