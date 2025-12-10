using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    GameObject playerAPrefab;
    [SerializeField]
    private Transform[] startPositions;  // 시작 위치를 배열로 관리


    public static GameManager instance
    {
        get
        {
            if(m_instance == null)
            {
                m_instance = FindObjectOfType<GameManager>();
            }

            return m_instance;  
        }
    }

    private static GameManager m_instance;
    private void Awake()
    {
       if(instance !=this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady)
        {
            SpawnPlayer();
        }

    }
    private void SpawnPlayer()
    {
        // 플레이어 인덱스에 따라 다른 위치에 스폰
        int index = PhotonNetwork.LocalPlayer.ActorNumber - 1;  // ActorNumber starts at 1
        index = index % startPositions.Length;  // 위치가 부족할 경우를 대비한 나머지 연산
        PhotonNetwork.Instantiate(playerAPrefab.name, startPositions[index].position, Quaternion.identity);
    }

    //주기적으로 동기화 해야하는 데이터
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
          //  stream.SendNext();
        }
        else
        {
            //리모트 오브젝트면 읽기 실행
        }
    }

  

    public void EndGame()
    {

    }
  

}
