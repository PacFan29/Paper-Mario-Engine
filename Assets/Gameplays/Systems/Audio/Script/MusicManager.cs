using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip music;
    private AudioSource source;
    public float loopBegin;
    public float loopEnd;

    [HideInInspector] public AudioClip latestMusic;
    [HideInInspector] public float latestLBegin;
    [HideInInspector] public float latestLEnd;
    [HideInInspector] public float latestClipTime;
    // Start is called before the first frame update
    void Start()
    {
        if (this.GetComponent<AudioSource>() == null) {
            source = gameObject.AddComponent<AudioSource>();
        } else {
            source = this.GetComponent<AudioSource>();
        }
        source.clip = music;
        source.volume = 0.6f;
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (loopEnd > 0) {
            if (source.time > loopEnd){
                source.time = loopBegin;
                source.Play();
            }
        }

        // if (Input.GetButtonDown("Y")) {
        //     source.time += 5f;
        // }

        if (GameManager.Death) {
            if (SceneManager.GetActiveScene().name == "BattleScene") {
                source.volume -= 1.333f * 0.4f * Time.deltaTime;
            } else {
                source.volume -= 0.4f * 0.4f * Time.deltaTime;
            }
        }
    }

    public void BackToFieldMusic() {
        source.clip = latestMusic;
        loopBegin = latestLBegin;
        loopEnd = latestLEnd;
        source.Play();

        source.time = latestClipTime;
    }
}
