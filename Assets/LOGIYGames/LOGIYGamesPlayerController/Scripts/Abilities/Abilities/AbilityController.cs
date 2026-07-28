using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AbilityTargetingController))]
public class AbilityController : MonoBehaviour
{
    [SerializeField] private List<AbilitySO> abilities;
    public List<Ability> Abilities { get; private set; } = new List<Ability>();

    public AbilityTargetingController targetingManager;

    private void Awake()
    {
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