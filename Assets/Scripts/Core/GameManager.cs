using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager 
{
    public enum GameState
    {
        PREGAME,
        INWAVE,
        WAVEEND,
        COUNTDOWN,
        GAMEOVER
    }
    public GameState state;

    public int countdown;
    public int currWave;
    private static GameManager theInstance;
    public static GameManager Instance {  get
        {
            if (theInstance == null)
                theInstance = new GameManager();
            return theInstance;
        }
    }

    public GameObject player;
    
    public ProjectileManager projectileManager;
    public SpellIconManager spellIconManager;
    public EnemySpriteManager enemySpriteManager;
    public PlayerSpriteManager playerSpriteManager;
    public RelicIconManager relicIconManager;
    public RelicManager relicmanager;

    private List<GameObject> enemies;
    public int enemy_count { get { return enemies.Count; } }

    public void SetWaveEnd()
    {
        EventBus.Instance.DoWaveEnd();
        state = GameState.WAVEEND;
    }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }
    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
    }

    public GameObject GetClosestEnemy(Vector3 point)
    {
        if (enemies == null || enemies.Count == 0) return null;
        if (enemies.Count == 1) return enemies[0];
        return enemies.Aggregate((a,b) => (a.transform.position - point).sqrMagnitude < (b.transform.position - point).sqrMagnitude ? a : b);
    }

    public void SetWave(int wave)
    {
        currWave = wave;
    }

    public int GetWave()
    {
        return currWave;
    }

    public void ClearEnemies()
    {
        while (enemy_count != 0)
        {
            RemoveEnemy(enemies[enemy_count - 1]);
        }
    }

    private GameManager()
    {
        enemies = new List<GameObject>();
    }
}
