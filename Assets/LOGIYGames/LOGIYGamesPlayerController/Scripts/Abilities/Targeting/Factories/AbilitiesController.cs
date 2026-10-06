using LOGIYGames.CharacterCore;
using System.Collections.Generic;
using UnityEngine;
public class AbilitiesController : MonoBehaviour
{
    [SerializeField] private List<AbilitySO> abilities;
    public List<Ability> Abilities { get; private set; } = new List<Ability>();

    private AbilityTargeting targetingManager;

    private Actor Character;

    private void Awake()
    {
        Character = GetComponent<Actor>();
        targetingManager = new AbilityTargeting(Character);
        foreach (var ability in abilities)
        {
            Abilities.Add(new Ability(ability));
        }
    }

    void Update()
    {
        targetingManager.Update();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Cast(Abilities[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Cast(Abilities[1]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Cast(Abilities[2]);
        }
    }

    public void Cast(Ability ability)
    {
        if (!ability.CooldownTimer.IsRunning)
        {

            ability.Target(targetingManager);
        }
    }
}