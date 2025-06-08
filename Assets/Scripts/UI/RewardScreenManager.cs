using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using TMPro;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;

public class RewardScreenManager : MonoBehaviour
{
    public GameObject rewardUI;

    // Reward Spell

    public GameObject newSpell;
    public GameObject newSpellIcon;
    public TextMeshProUGUI spellInfo;

    // Reward Relics

    public GameObject container;
    public RewardRelicContainer rewardRelicContainer;

    // Player 

    public GameObject player;
    public PlayerController playerController;
    public SpellCaster spellcaster;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        rewardRelicContainer = container.GetComponent<RewardRelicContainer>();

        EventBus.Instance.OnWaveEnd += DoRewardRelics;

    }

    // Update is called once per frame
    void Update()
    {
        spellcaster = playerController.spellcaster;
        if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {

            // Reward Spell UI

            GameManager.Instance.spellIconManager.PlaceSprite(spellcaster.reward_spell.GetIcon(), newSpellIcon.GetComponent<Image>());

            spellInfo.text = spellcaster.reward_spell.GetName();
            Spell rewardSpellCopy = spellcaster.reward_spell;
            while (rewardSpellCopy.isModifier)
            {
                spellInfo.text += ' ' + rewardSpellCopy.GetInnerSpell().GetName();
                rewardSpellCopy = rewardSpellCopy.GetInnerSpell();
            }
            spellInfo.text += '\n' + spellcaster.reward_spell.GetDescription();
            rewardSpellCopy = spellcaster.reward_spell;
            while (rewardSpellCopy.isModifier)
            {
                spellInfo.text += '\n' + rewardSpellCopy.GetInnerSpell().GetDescription();
                rewardSpellCopy = rewardSpellCopy.GetInnerSpell();
            }

        }
        else
        {
            newSpell.SetActive(true);
            container.SetActive(false);
            rewardUI.SetActive(false);
        }
    }

    public bool OnThirdWave()
    {
        if (GameManager.Instance.GetWave() % 3 == 0)
        {
            return true;
        }

        return false;
    }

    public void DoRewardRelics()
    {
        if (OnThirdWave())
        {
            for (int i = 0; i< rewardRelicContainer.rewardRelics.Length; i++)
            {
                Relic r = RelicManager.Instance.BuildRelic();

                rewardRelicContainer.SetRelicUI(r, i);
            }

            container.SetActive(true);
        }
    }
}
