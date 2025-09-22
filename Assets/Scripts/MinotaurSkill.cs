using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(EnemyBehavior))]
[RequireComponent(typeof(Animator))]
public class MinotaurSkill : MonoBehaviour
{
    [Header("Skill Settings")]
    public float skillCooldown = 10f;
    [Range(0f, 1f)] public float skillChance = 0.3f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private EnemyBehavior enemyBehavior;
    private Animator animator;

    private bool isSkillOnCooldown = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyBehavior = GetComponent<EnemyBehavior>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(SkillLoop());
    }

    private IEnumerator SkillLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(skillCooldown);

            if (Random.value <= skillChance && !isSkillOnCooldown)
            {
                StartCoroutine(UseSkill());
            }
        }
    }

    private IEnumerator UseSkill()
    {
        isSkillOnCooldown = true;
        int chosenSkill = Random.Range(0, 1);

        if (chosenSkill == 0)
        {
            Debug.Log("Use skill 1");
            int originalDamage = enemyBehavior.contactDamage;
            int originalDefense = enemyBehavior.defense;

            rb.simulated = false;
            spriteRenderer.color = Color.red;
            enemyBehavior.contactDamage = Mathf.RoundToInt(originalDamage * 2f);
            enemyBehavior.defense = Mathf.RoundToInt(originalDefense * 0.25f);

            yield return new WaitForSeconds(1f);

            rb.simulated = true;
            spriteRenderer.color = Color.white;

            yield return new WaitForSeconds(15f);

            enemyBehavior.contactDamage = originalDamage;
            enemyBehavior.defense = originalDefense;
        }
        else if (chosenSkill == 1)
        {
            Debug.Log("Use Skill 2");
            animator.SetTrigger("Skill");
        }

        isSkillOnCooldown = false;
    }
}
