using LOGIYGames.Shared.Extensions;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AbilityTargetingController))]
public class AbilitiesController : MonoBehaviour
{
    [SerializeField] private List<AbilitySO> abilities;
    public List<Ability> Abilities { get; private set; } = new List<Ability>();

    private AbilityTargetingController targetingManager;

    private void Awake()
    {

        targetingManager = gameObject.GetOrAddComponent<AbilityTargetingController>();
        foreach (var ability in abilities)
        {
            Abilities.Add(new Ability(ability));
        }
    }

    void Update()
    {
        for (int i = 0; i < Abilities.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                Cast(Abilities[i]);
            }
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