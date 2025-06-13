using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public Transform target;
    public int speed;
    public int baseSpeed;
    public Hittable hp;
    public int damage;
    public HealthBar healthui;
    public bool dead;
    public float defense = 1;
    public Damage.Type resistance;
    public Damage.Type weakness;
    public AudioClip damageSound;
    public AudioSource audioSource;

    public float last_attack;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameManager.Instance.player.transform;
        hp.OnDeath += Die;
        healthui.SetHealth(hp);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = target.position - transform.position;
        if (direction.magnitude < 2f)
        {
            DoAttack();
        }
        else
        {
            GetComponent<Unit>().movement = direction.normalized * speed;
        }
    }
    
    void DoAttack()
    {
        if (last_attack + 2 < Time.time)
        {
            last_attack = Time.time;
            target.gameObject.GetComponent<PlayerController>().hp.Damage(new Damage(damage, Damage.Type.PHYSICAL));

            audioSource.clip = damageSound;
            audioSource.Play();
        }
    }


    void Die()
    {
        if (!dead)
        {
            EventBus.Instance.DoEnemyDeath();
            UnityEngine.Debug.Log("EnemyDeath event sent.");
            dead = true;
            GameManager.Instance.RemoveEnemy(gameObject);
            Destroy(gameObject);
        }
    }

    public void SetSpeed(int newSpeed)
    {
        speed = newSpeed;
    }

    public void BaseSpeed(int initSpeed)
    {
        baseSpeed = initSpeed;
    }
}
