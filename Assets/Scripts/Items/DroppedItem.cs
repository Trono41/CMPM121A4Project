using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public Piece piece;
    private float pickupRadius = 1.5f;
    private float moveSpeed = 5f;
    private Transform player;

    void Start()
    {
        player = GameManager.Instance.player.transform;
    }

    void Update()
    {
        // Move towards player if they're close enough
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= pickupRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += new Vector3(direction.x, direction.y, 0) * moveSpeed * Time.deltaTime;
        }

        // Check for pickup
        if (distanceToPlayer <= 0.5f)
        {
            Pickup();
        }
    }

    void Pickup()
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            if (piece is Spell)
            {
                playerController.spell_pieces.Add((Spell)piece);
            }
            else if (piece is RelicPart)
            {
                playerController.relic_pieces.Add((RelicPart)piece);
            }
            Destroy(gameObject);
        }
    }
} 