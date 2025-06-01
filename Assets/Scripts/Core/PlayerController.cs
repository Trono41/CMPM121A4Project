using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Diagnostics;
using UnityEngine.InputSystem;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using TMPro;

public class PlayerController : MonoBehaviour
{

    public ClassSelector class_selector;

    public Hittable hp;
    public HealthBar healthui;
    public ManaBar manaui;

    public SpellCaster spellcaster;
    public SpellUI spellui;

    public int speed = 5;
    public int spellpower = 0;

    // Class Stats

    int max_hp;
    int mana;
    int mana_regen;

    string hp_scalar = "95 wave 5 * +";
    string mana_scalar = "90 wave 10 * +";
    string mana_regen_scalar = "10 wave +";
    string spellpower_scalar = "wave 10 *";
    string speed_scalar = "5";

    public Unit unit;

    public List<Relic> relics = new List<Relic>();

    Dictionary<string, int> variables = new Dictionary<string, int>();
    RPNEvaluator rpn = new RPNEvaluator();

    public TextMeshProUGUI gameOverText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unit = GetComponent<Unit>();
        GameManager.Instance.player = gameObject;

        class_selector = GetComponent<ClassSelector>();

        //RelicBuilder relicBuilder = new RelicBuilder();
        //relics.Add(relicBuilder.BuildThisRelic("Cursed Scroll"));
        //Debug.Log(relics);


    }

    public void SetClass(JToken class_stats)
    {
        UnityEngine.Debug.Log(class_stats);

        //sprite = class_stats["sprite"].ToObject<int>();
        hp_scalar = class_stats["health"].ToObject<string>();
        mana_scalar = class_stats["mana"].ToObject<string>();
        mana_regen_scalar = class_stats["mana_regeneration"].ToObject<string>();
        spellpower_scalar = class_stats["spellpower"].ToObject<string>();
        speed_scalar = class_stats["speed"].ToObject<string>();

    }

    public void StartLevel()
    {
        variables["wave"] = 1;

        max_hp = rpn.Eval(hp_scalar, variables);
        mana = rpn.Eval(mana_scalar, variables);
        mana_regen = rpn.Eval(mana_regen_scalar, variables);
        spellpower = rpn.Eval(spellpower_scalar, variables);
        speed = rpn.Eval(speed_scalar, variables);

        spellcaster = new SpellCaster(mana, mana_regen, spellpower, Hittable.Team.PLAYER);
        StartCoroutine(spellcaster.ManaRegeneration());
        
        hp = new Hittable(max_hp, Hittable.Team.PLAYER, gameObject);
        hp.OnDeath += Die;
        hp.team = Hittable.Team.PLAYER;

        // tell UI elements what to show
        healthui.SetHealth(hp);
        manaui.SetSpellCaster(spellcaster);
        spellui.SetSpell(spellcaster.spellbook[0]);

        //Debug.Log("Player HP: " + hp.hp);
        //Debug.Log("Player Mana: " + spellcaster.mana);
        //Debug.Log("Player Mana Regen: " + spellcaster.mana_reg);
    }

    public void NextWave()
    {
        variables["wave"] = GameManager.Instance.GetWave();
        int max_hp = rpn.Eval(hp_scalar, variables);
        hp.SetMaxHP(max_hp);
        spellcaster.mana = rpn.Eval(mana_scalar, variables);
        spellcaster.mana_reg = rpn.Eval(mana_regen_scalar, variables);
        spellcaster.spellpower = rpn.Eval(spellpower_scalar, variables);
        speed = rpn.Eval(speed_scalar, variables);

        //Debug.Log("Player Max_HP: " + hp.max_hp);
        //Debug.Log("Player Mana: " + spellcaster.mana);
        //Debug.Log("Player Mana Regen: " + spellcaster.mana_reg);
        UnityEngine.Debug.Log("Relics");
        foreach (var r in relics)
        {
            UnityEngine.Debug.Log(r.GetName());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnChangeSpell()
    {
        spellcaster.ChangeSpell();
    }

    void OnAttack(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;
        Vector2 mouseScreen = Mouse.current.position.value;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;
        StartCoroutine(spellcaster.Cast(transform.position, mouseWorld));
    }

    void OnMove(InputValue value)
    {
        if (GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.GAMEOVER) return;
        unit.movement = value.Get<Vector2>()*speed;
    }

    void Die()
    {
        Debug.Log("You Lost");
        gameOverText.text = "You Lost...";
        GameManager.Instance.state = GameManager.GameState.GAMEOVER;
    }
}
