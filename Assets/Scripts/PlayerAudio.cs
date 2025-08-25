using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    AudioCollection audioCollection;
    string clip;
    void Awake()
    {
        audioCollection = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioCollection>();
    }

    public void SkillSlashSFX()
    {
        audioCollection.PlaySFX(audioCollection.skillSlash);
    }
    public void UltSlashSFX()
    {
        audioCollection.PlaySFX(audioCollection.ultSlash);
    }
    public void UltAreaSFX()
    {
        audioCollection.PlaySFX(audioCollection.ultFall);
    }
    public void HealSFX()
    {
        audioCollection.PlaySFX(audioCollection.heal);
    }
    public void DeadSFX()
    {
        audioCollection.PlaySFX(audioCollection.death);
    }
    public void UltExplodeSFX()
    {
        audioCollection.PlaySFX(audioCollection.ultArea);
    }
    public void BasicAttackSFX()
    {
        audioCollection.PlaySFX(audioCollection.attack);
    }
}
