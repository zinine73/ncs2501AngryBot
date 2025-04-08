using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Damage : MonoBehaviourPunCallbacks
{
    private Renderer[] renderers;
    private const int initHp = 100;
    public int currHp;
    private Animator anim;
    private CharacterController cc;
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashRespawn = Animator.StringToHash("Respawn");
    // GameManager접근을 위한 변수
    private NewGameManager gameManager;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        currHp = initHp;

        gameManager = GameObject.Find("GameManager").GetComponent<NewGameManager>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (currHp > 0 && collision.collider.CompareTag("BULLET"))
        {
            currHp -= 20;
            if (currHp <= 0)
            {
                // 자신의 photonView일 때만 메시지 출력
                if (photonView.IsMine)
                {
                    // 총알의 ActorNumber 추출
                    var actorNo = collision.collider.GetComponent<RPCBullet>().actorNumber;
                    // ActorNumber로 현재 룸에 입장한 플레이어를 추출
                    Player lastShootPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actorNo);
                    // 메시지 출력
                    string msg = string.Format("\n<color=#00ff00>{0}</color> is killed by <color=#ff0000>{1}</color>",
                                photonView.Owner.NickName, lastShootPlayer.NickName);
                    photonView.RPC("KillMessage", RpcTarget.AllBufferedViaServer, msg);
                }

                StartCoroutine(PlayerDie());
            }
        }
    }

    [PunRPC]
    void KillMessage(string msg)
    {
        gameManager.msgList.text += msg;
    }

    IEnumerator PlayerDie()
    {
        cc.enabled = false;
        anim.SetBool(hashRespawn, false);
        anim.SetTrigger(hashDie);
        yield return new WaitForSeconds(3.0f);

        anim.SetBool(hashRespawn, true);
        SetPlayerVisible(false);
        yield return new WaitForSeconds(1.5f);

        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);
        transform.position = points[idx].position;

        currHp = initHp;
        SetPlayerVisible(true);
        cc.enabled = true;
    }

    void SetPlayerVisible(bool isVisible)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = isVisible;
        }
    }
}
