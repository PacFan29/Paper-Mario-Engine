using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerPhysics
{
    [Header("効果音")]
    public AudioClip jumpSound;
    public AudioClip damageSound;
    public AudioClip spikedSound;
    public AudioClip burnedSound;

    private bool jumped = false;
    private bool groundPound = false;
    private bool hammer = false;

    private void OnEnable() {
        PlayerInfo info = this.GetComponent<PlayerInfo>();
        
        if (info.actionId != 0) {
            hammer = false;
            canInput = true;
            info.actionId = 0;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (canInput) {
            input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

            if (Input.GetButtonDown("A") && Grounded) {
                groundPound = true;
                jumped = true;
                SoundPlay(jumpSound);
                Jump();
            }

            if (!Input.GetButton("A") && velocity.y > 2 && jumped) {
                velocity.y = 2f;
            } else if (velocity.y <= 2 && jumped) {
                jumped = false;
            }

            if (Input.GetButtonDown("A") && groundPound && !Grounded && !jumped && this.GetComponent<MarioStatus>().BootsRank >= 2) {
                //クルリンジャンプ
                groundPound = false;
                StartCoroutine("GroundPound");
            }
            if (Grounded) {
                groundPound = false;
            }

            if (Input.GetButtonDown("X") && !hammer && Grounded && GetComponent<MarioStatus>().HammerRank > 0) {
                StartCoroutine("Hammer");
            }
        } else {
            input = Vector3.zero;
        }
    }

    public void SoundPlay(AudioClip sound) {
        this.GetComponent<AudioSource>().PlayOneShot(sound);
    }

    public IEnumerator GroundPound() {
        PlayerInfo info = this.GetComponent<PlayerInfo>();

        info.actionId = 16;
        canInput = false;
        useGravity = false;
        velocity.y = 0f;

        yield return new WaitForSeconds(0.5f);

        info.actionId = 17;
        useGravity = true;
        velocity.y = -40f;

        while (!Grounded && info.actionId == 17) {
            yield return new WaitForSeconds(0f);
        }

        yield return new WaitForSeconds(0.3f);

        if (info.actionId == 17) {
            canInput = true;
            info.actionId = 0;
        }
    }

    public void Stomp() {
        velocity.y = 30f;
        canInput = true;
        this.GetComponent<PlayerInfo>().actionId = 0;
    }

    public IEnumerator Hammer() {
        canInput = false;
        PlayerInfo info = this.GetComponent<PlayerInfo>();

        info.actionId = 1;
        yield return new WaitForSeconds(0.06f);

        /* ここから接触に応じてアニメーションが変わるようにする */
        
        info.actionId = 3;
        yield return new WaitForSeconds(0.25f);

        hammer = false;
        canInput = true;
        info.actionId = 0;
    }
}
