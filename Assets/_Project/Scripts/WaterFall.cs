using UnityEngine;
using System.Collections.Generic;

public class WaterFall : MonoBehaviour
{
    [SerializeField] private float stunDuration = 8f;
    [SerializeField] private float stunCooldown = 12f;

    private readonly Dictionary<EnemyAI, float> nextStunTimes = new Dictionary<EnemyAI, float>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyAI enemy = collision.GetComponentInParent<EnemyAI>();
        if (enemy == null || enemy.IsBoss || !enemy.gameObject.activeInHierarchy)
            return;

        if (nextStunTimes.TryGetValue(enemy, out float nextStunTime) && Time.time < nextStunTime)
            return;

        if (stunDuration <= 0f)
            return;

        nextStunTimes[enemy] = Time.time + Mathf.Max(0f, stunCooldown);
        enemy.Stun(stunDuration, EnemyAI.StunType.Environmental);
        Debug.Log($"{name}: environmentally stunned {enemy.name} for at least {stunDuration} seconds.", enemy);
    }
}
