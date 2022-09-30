using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMusicManager : MonoBehaviour
{
    [Header("開始時")]
    public AudioClip normalBegin;
    public AudioClip firstStrikeBegin;
    public AudioClip damageBegin;
    [Header("バトルBGM")]
    public AudioClip battleMusic;
    public float loopBegin;
    public float loopEnd;
    [Header("勝利BGM")]
    public AudioClip victoryMusic;
    [Header("レベルアップBGM")]
    public AudioClip levelUpMusic;
    

    public void BattleMusic (int beginIndex) {
        AudioSource audioSource = GetComponent<AudioSource>();
        MusicManager music = GetComponent<MusicManager>();
        
        music.latestMusic = audioSource.clip;
        music.latestLBegin = music.loopBegin;
        music.latestLEnd = music.loopEnd;
        music.latestClipTime = audioSource.time;
        
        switch (beginIndex) {
            case 0:
            //通常
            if (normalBegin != null) audioSource.PlayOneShot(normalBegin);
            break;

            case 1:
            //先制攻撃成功！！
            if (firstStrikeBegin != null) audioSource.PlayOneShot(firstStrikeBegin);
            break;
            
            case 2:
            //先制攻撃を受けた！
            if (damageBegin != null) audioSource.PlayOneShot(damageBegin);
            break;
        }
        PlayMusic(battleMusic, loopBegin, loopEnd);
    }

    public void VictoryMusic() {
        PlayMusic(victoryMusic, 0, 0);
    }
    public void LevelUpMusic() {
        PlayMusic(levelUpMusic, 0, 0);
    }

    public void PlayMusic(AudioClip clip, float begin, float end) {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.time = 0f;
        audioSource.Play();

        MusicManager music = GetComponent<MusicManager>();
        music.loopBegin = begin;
        music.loopEnd = end;
    }
}
