using UnityEngine;
using System.Collections;
public class MinotaurSkill : MonoBehaviour
{
    [Header("Skill Settings")]
    public float skillCooldown = 10f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private EnemyBehavior enemyBehavior;
    private Animator animator;
    private Collider2D col;

    private bool isSkillOnCooldown = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        enemyBehavior = GetComponent<EnemyBehavior>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
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
            StartCoroutine(UseSkill());
        }
    }

    private IEnumerator UseSkill()
    {
        isSkillOnCooldown = true;
        int chosenSkill = Random.Range(0, 11);
        Debug.Log("random number got = " + chosenSkill);

        if (chosenSkill <= 5 && chosenSkill >= 0)
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

            yield return new WaitForSeconds(10f);

            spriteRenderer.color = Color.white;
            enemyBehavior.contactDamage = originalDamage;
            enemyBehavior.defense = originalDefense;
        }
        else if (chosenSkill >= 6 && chosenSkill <= 10)
        {
            Debug.Log("Use Skill 2");
            col.enabled = false;
            animator.SetTrigger("Skill");
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Minotaur_Skill"));
            float duration = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(duration);
            col.enabled = true;
        }

        isSkillOnCooldown = false;
    }
}
