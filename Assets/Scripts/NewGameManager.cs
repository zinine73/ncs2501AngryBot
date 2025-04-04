using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
public class NewGameManager : MonoBehaviourPunCallbacks
{
    void Awake()
    {
        CreatePlayer();
    }

    void CreatePlayer()
    {
        // 출현 위치 정보를 배열에 저장
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);

        // 네트워크상에 캐릭터 생성
        PhotonNetwork.Instantiate("Player",
                            points[idx].position,
                            points[idx].rotation,
                            0);
    }
}
