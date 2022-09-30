using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Test : MonoBehaviour
{
    public RawImage img;
    public VideoPlayer video;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayVideo());
    }

    // Update is called once per frame
    IEnumerator PlayVideo() {
        video.Prepare();

        while (!video.isPrepared) {
            yield return new WaitForSeconds(1);
            break;
        }

        this.GetComponent<RawImage>().texture = video.texture;
        video.Play();
    }
}
