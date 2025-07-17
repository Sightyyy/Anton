using UnityEngine;
using System.Collections;

public class PlayerAttackAnimation : MonoBehaviour
{
    public GameObject slashEffect;
    public float frameRate = 15f; // Misalnya 60 frame per detik
    public float animationFrames = 13f; // Animasi berjalan selama 13 frame

    private int attackCombo = 1; // 1: slash pertama, 2: slash kedua

    public void PlayBasicAttack()
    {
        if (attackCombo == 1)
        {
            StartCoroutine(PlaySlash(new Vector3(-0.71f, 1f, 0), 30f, new Vector3(-0.71f, -1f, 0), 150f));
            attackCombo = 2;
        }
        else
        {
            StartCoroutine(PlaySlash(new Vector3(-0.71f, -1f, 0), 150f, new Vector3(-0.71f, 1f, 0), 30f));
            attackCombo = 1;
        }
    }

    private IEnumerator PlaySlash(Vector3 startPos, float startRot, Vector3 endPos, float endRot)
    {
        if (slashEffect == null)
        {
            Debug.LogError("Slash Effect tidak di-assign!");
            yield break;
        }

        slashEffect.SetActive(true);
        slashEffect.transform.localPosition = startPos;
        slashEffect.transform.localRotation = Quaternion.Euler(0, 0, startRot);

        float duration = animationFrames / frameRate;
        float elapsed = 0;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            slashEffect.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            slashEffect.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(startRot, endRot, t));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Pastikan posisi akhir
        slashEffect.transform.localPosition = endPos;
        slashEffect.transform.localRotation = Quaternion.Euler(0, 0, endRot);
        yield return new WaitForSeconds(0.5f);
        slashEffect.SetActive(false);
    }
}
