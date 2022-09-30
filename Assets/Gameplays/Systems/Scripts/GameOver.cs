using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public EventData data;
    public Text message;
    public ButtonSelect[] selectCheckers = new ButtonSelect[2];
    private bool accepted;

    public Image fade;
    private float fadeOpacity = 0f;
    private AudioSource music;
    private float volume = 1f;
    

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<AudioSource>() != null) {
            music = GetComponent<AudioSource>();
        }
        bool isEvent = data.isEvent;
        /*
        JP
        Font : HGRSMP
        Font Style : Bold
        FontSize : 14
        
        Eng
        Font : heygorgeous
        Font Style : Normal
        FontSize : 9
        */

        //トクベツに直前から　ゲームをやりなおします。  Retry that last scene.
        //さいごにセーブした場所から　ゲームを再開します。  Restart from your last save location.
        //Quit and return to the title screen.

        /*
        直前からやりなおす  Try again
        セーブした場所から再開  Restart from last save
        タイトル画面にもどる  Return to title screen
        */
        int patternIndex = 0;
        if (isEvent) {
            patternIndex = 1;
            switch (selectCheckers[1].selectIndex) {
                case 0:
                message.text = "トクベツに直前から　ゲームをやりなおします。";
                break;
                
                case 1:
                message.text = "さいごにセーブした場所から　ゲームを再開します。";
                break;

                case 2:
                message.text = "トクベツに直前から　ゲームをやりなおします。";
                break;
            }
        } else {
            switch (selectCheckers[0].selectIndex) {
                case 0:
                message.text = "さいごにセーブした場所から　ゲームを再開します。";
                break;
                
                case 1:
                message.text = "さいごにセーブした場所から　ゲームを再開します。";
                break;
            }
        }

        selectCheckers[0].gameObject.SetActive(!isEvent);
        selectCheckers[1].gameObject.SetActive(isEvent);
        if (selectCheckers[patternIndex].accepted && !accepted) {
            StartCoroutine(Branch(selectCheckers[patternIndex].selectIndex, isEvent));
            accepted = true;
        }

        if (fadeOpacity > 0) {
            fadeOpacity += 4 * Time.deltaTime;
            volume -= Time.deltaTime;

            music.volume = volume * 0.4f;
            fade.color = new Color(0f, 0f, 0f, fadeOpacity);
        }
    }

    IEnumerator Branch(int selectIndex, bool isEvent) {
        yield return new WaitForSeconds(0.1f);
        fadeOpacity = 4 * Time.deltaTime;

        yield return new WaitForSeconds(3f);

        if (isEvent) {
            switch (selectIndex) {
                case 0:
                break;

                case 1:
                SceneManager.LoadScene("テストエリア");
                break;
                
                case 2:
                SceneManager.LoadScene("TitleScreen");
                break;
            }
        } else {
            switch (selectIndex) {
                case 0:
                SceneManager.LoadScene("テストエリア");
                break;

                case 1:
                SceneManager.LoadScene("TitleScreen");
                break;
            }
        }
    }
}
