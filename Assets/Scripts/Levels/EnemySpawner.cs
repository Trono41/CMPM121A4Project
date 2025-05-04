using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class RPNEvaluator
{

    Stack<int> stack = new Stack<int>();

    public RPNEvaluator()
    {
        
    }

    public int Eval(string expression, Dictionary<string, int> variables)
    {
        string[] tokens = expression.Split(' ');
        int final_result;

        foreach (string token in tokens)
        {
            int myInt;
            int value1;
            int value2;
            int result;

            if (int.TryParse(token, out myInt))
                stack.Push(myInt);

            if (variables.ContainsKey(token))
                stack.Push(variables[token]);

            switch (token)
            {
                case "%":
                    value2 = stack.Pop();
                    value1 = stack.Pop();
                    
                    result = value1 % value2;
                    stack.Push(result);
                    break;

                case "*":
                    value2 = stack.Pop();
                    value1 = stack.Pop();
                    result = value1 * value2;
                    stack.Push(result);
                    break;

                case "/":
                    value2 = stack.Pop();
                    value1 = stack.Pop();
                    result = value1 / value2;
                    stack.Push(result);
                    break;

                case "+":
                    value2 = stack.Pop();
                    value1 = stack.Pop();
                    result = value1 + value2;
                    stack.Push(result);
                    break;

                case "-":
                    value2 = stack.Pop();
                    value1 = stack.Pop();
                    result = value1 - value2;
                    stack.Push(result);
                    break;

            }
        }

        final_result = stack.Pop();
        return final_result;

    }
}

// Determines the type and values of constructed objects. 

public class Level
{

    public string name;
    public int waves;
    public List<Spawn> spawns;

    void Start()
    {

    }

    void Update()
    {

    }
}

public class Spawn
{
    public string enemy; // Which type of enemey to spawn (=name in enemies.json)
    public string count; // How many enemies of one type to spawn, overall
    public List<int> sequence = new List<int>(); // How many should be spawned at once
    public string delay = "2"; // The number of seconds between consecutive spawns
    public string location = "random"; // Where to spawn the enemy
    public string hp = "base"; // Modify the properties of the spawned enemy
    public string speed = "base";
    public string damage = "base";

    void Start()
    {

    }

    void Update()
    {

    }
}

public class Enemy
{

    public string name;
    public int sprite;
    public int hp;
    public int speed;
    public int damage;

    void Start()
    {

    }

    void Update()
    {

    }
}

public class EnemySpawner : MonoBehaviour
{
    public Image level_selector;
    public GameObject button;
    public GameObject enemy;
    public SpawnPoint[] SpawnPoints;
    Level level;
    int wave = 0;
    int damageTaken = 0;
    Dictionary<string, Enemy> enemy_types = new Dictionary<string, Enemy>();
    Dictionary<string, Level> level_types = new Dictionary<string, Level>();
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI betweenWaveText;
    public PlayerController player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int buttonPos = 130;
        
        var enemytext = Resources.Load<TextAsset>("enemies");

        JToken jo = JToken.Parse(enemytext.text);
        foreach (var enemy in jo)
        {
            Enemy en = enemy.ToObject<Enemy>();
            enemy_types[en.name] = en;
        }

        var leveltext = Resources.Load<TextAsset>("levels");

        JToken jo2 = JToken.Parse(leveltext.text);
        foreach (var level in jo2)
        {
            Level lev = level.ToObject<Level>();
            level_types[lev.name] = lev;

            GameObject selector = Instantiate(button, level_selector.transform);
            selector.transform.localPosition = new Vector3(0, buttonPos);
            selector.GetComponent<MenuSelectorController>().spawner = this;
            selector.GetComponent<MenuSelectorController>().SetLevel(lev.name);
            buttonPos -= 50;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLevel(string levelname)
    {
        level = level_types[levelname];
        level_selector.gameObject.SetActive(false);
        // this is not nice: we should not have to be required to tell the player directly that the level is starting
        GameManager.Instance.player.GetComponent<PlayerController>().StartLevel();
        StartCoroutine(SpawnWave());
    }

    public void NextWave()
    {
        StartCoroutine(SpawnWave());
    }


    IEnumerator SpawnWave()
    {
        wave++;
        int playerStartingHP = player.hp.hp;
        GameManager.Instance.state = GameManager.GameState.COUNTDOWN;
        GameManager.Instance.countdown = 3;
        for (int i = 3; i > 0; i--)
        {
            yield return new WaitForSeconds(1);
            GameManager.Instance.countdown--;
        }
        GameManager.Instance.state = GameManager.GameState.INWAVE;
        
        foreach (Spawn spawn in level.spawns) // For each enemy type . . .
        {
            yield return SpawnEnemies(spawn.enemy, spawn.count, spawn.delay, spawn.location, spawn.hp, spawn.speed, spawn.damage, spawn.sequence, wave);
        }

        yield return new WaitWhile(() => GameManager.Instance.enemy_count > 0);
        if (wave < level.waves || level.name == "Endless")
        {
            damageTaken = playerStartingHP - player.hp.hp;
            betweenWaveText.text = "Damage Taken: " + damageTaken.ToString();
            GameManager.Instance.state = GameManager.GameState.WAVEEND;
        }
        else
        {
            gameOverText.text = "You Win!";
            GameManager.Instance.state = GameManager.GameState.GAMEOVER;
        }
    }

    IEnumerator SpawnEnemies(string e, string count, string delay, string location, string hp, string speed, string damage, List<int> sequence, int wave)
    {
        int n = 0;
        int seq = 0;

        if (sequence.Count == 0)
            sequence.Add(1);

        Dictionary<string, int> variables = new Dictionary<string, int>();
        variables["wave"] = wave;

        RPNEvaluator rpn = new RPNEvaluator();
        int new_count = rpn.Eval(count, variables);

        while (n < new_count)
        { 
            int required = sequence[seq];

            for (int i = 1; i <= required; i++)
            {
                StartCoroutine(SpawnEnemy(e, delay, location, hp, speed, damage, wave));
                n++;

                if (n == new_count)
                    break;
            }

            if (seq == sequence.Count - 1)
                seq = 0;
            else
                seq++;

            float delayFloat = float.Parse(delay);

            yield return new WaitForSeconds(delayFloat);
        }
    }

    IEnumerator SpawnEnemy(string e, string delay, string location, string hp, string speed, string damage, int wave)
    {
        Enemy enemyObject = enemy_types[e];

        Dictionary<string, int> variables = new Dictionary<string, int>();
        variables["base"] = enemyObject.hp;
        variables["wave"] = wave;

        RPNEvaluator rpn = new RPNEvaluator();

        List<SpawnPoint> spawn_types = new List<SpawnPoint>();

        if (location == "random")
        {
            foreach (SpawnPoint spawn in SpawnPoints)
            {
                spawn_types.Add(spawn);
            }
        }

        else if (location == "random green")
        {
            foreach (SpawnPoint spawn in SpawnPoints)
            {
                if (spawn.kind == SpawnPoint.SpawnName.GREEN)
                    spawn_types.Add(spawn);
            }
        }

        else if (location == "random red")
        {
            foreach (SpawnPoint spawn in SpawnPoints)
            {
                if (spawn.kind == SpawnPoint.SpawnName.RED)
                    spawn_types.Add(spawn);
            }
        }

        else if (location == "random bone")
        {
            foreach (SpawnPoint spawn in SpawnPoints)
            {
                if (spawn.kind == SpawnPoint.SpawnName.BONE)
                    spawn_types.Add(spawn);
            }
        }

        SpawnPoint spawn_point = spawn_types[UnityEngine.Random.Range(0, spawn_types.Count)];
        Vector2 offset = UnityEngine.Random.insideUnitCircle * 1.8f;
        Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);

        GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity); // Creates a new enemy

        // Enemy Parameters
        new_enemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(enemyObject.sprite);
        EnemyController en = new_enemy.GetComponent<EnemyController>();

        en.hp = new Hittable(rpn.Eval(hp, variables), Hittable.Team.MONSTERS, new_enemy);

        variables["base"] = enemyObject.speed;
        en.speed = rpn.Eval(speed, variables);

        variables["base"] = enemyObject.damage;
        en.damage = rpn.Eval(damage, variables);

        GameManager.Instance.AddEnemy(new_enemy);

        float delayFloat = float.Parse(delay);

        yield return new WaitForSeconds(delayFloat);
    }

    IEnumerator SpawnZombie()
    {
        SpawnPoint spawn_point = SpawnPoints[UnityEngine.Random.Range(0, SpawnPoints.Length)];
        Vector2 offset = UnityEngine.Random.insideUnitCircle * 1.8f;

        Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);
        GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity); // Creates a new enemy

        new_enemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(0);
        EnemyController en = new_enemy.GetComponent<EnemyController>();
        en.hp = new Hittable(50, Hittable.Team.MONSTERS, new_enemy);
        en.speed = 10;

        GameManager.Instance.AddEnemy(new_enemy);
        yield return new WaitForSeconds(0.5f);
    }

    public void RestartLevel()
    {
        GameManager.Instance.state = GameManager.GameState.PREGAME;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
