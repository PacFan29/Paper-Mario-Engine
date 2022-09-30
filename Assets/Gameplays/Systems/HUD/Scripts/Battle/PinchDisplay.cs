using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PinchDisplay : MonoBehaviour
{
    public MarioStatus mario;
    public Image background;
    [Header("ピンチ！")]
    public GameObject pinchObj;
    public AudioClip pinchSound;
    [Header("危険！")]
    public GameObject dangerObj;
    public AudioClip dangerSound;
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(SoundPlay());
    }

    // Update is called once per frame
    void Update()
    {
        background.gameObject.SetActive(mario.Pinch || mario.Danger);
        pinchObj.SetActive(mario.Pinch);
        dangerObj.SetActive(mario.Danger);

        float green = 0f;
        float t = (Time.time % 1);
        if (t > 0.5f) {
            green = (1f - t) * 2;
        } else {
            green = t * 2;
        }
        background.color = new Color(1f, t, 0f, 1f);
    }

    IEnumerator SoundPlay() {
        while (true) {
            AudioClip sound = null;
            if (mario.Danger) {
                sound = dangerSound;
            } else if (mario.Pinch) {
                sound = pinchSound;
            }

            if (sound != null) {
                this.GetComponent<AudioSource>().PlayOneShot(sound);
                yield return new WaitForSeconds(1f);
            } else {
                break;
            }
        }

        yield return null;
    }
}
