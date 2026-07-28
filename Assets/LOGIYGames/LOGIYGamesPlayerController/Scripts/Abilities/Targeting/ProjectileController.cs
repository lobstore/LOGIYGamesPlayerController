using LOGIYGames;
using LOGIYGames.CharacterCore;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    AbilityContext ability;
    float speed;

    public void Initialize(AbilityContext ability, float speed)
    {
        this.ability = ability;
        this.speed = speed;
        Destroy(gameObject, 5f);
    }

    void Update() => transform.Translate(Vector3.forward * (speed * Time.deltaTime));

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == ability.Source) return;

        ability.Ability.Execute(other.GetComponent<Character>());
        Destroy(gameObject);
    }
}