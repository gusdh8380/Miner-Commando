using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

//�⺻ ���� ź��
public class NormalBullet : MonoBehaviourPun
{
    Rigidbody2D rb;
    CircleCollider2D col;
    [SerializeField]
    protected int speed, damage;

    //public float maxDistance = 10f; 
    //public Vector2 startPosition;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
        rb.velocity = transform.right * speed;

        col.offset = new Vector2(0, Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad) * -0.2f);


        if (photonView != null) {
            Debug.Log($"총알 ID: {photonView.ViewID}");
        }
        else {
            Debug.LogError("총알 생성 못함");
        }


        //startPosition = transform.position;
        //Destroy(gameObject, 5f); 
    }
    //void Update() {
    //    if (Vector2.Distance(startPosition, transform.position) >= maxDistance) {
    //        DestroyBullet();
    //    }
    //}

    protected virtual void OnTriggerEnter2D(Collider2D other) {
        //충돌검사는 마스터에서만
        if (PhotonNetwork.IsMasterClient) {
            if (other.CompareTag("Wall")) {
                //gameObject.SetActive(false);

                Debug.Log("벽 명중");
                PhotonNetwork.Destroy(gameObject);
            }
            else {
                DamageableEntity hitTarget = other.GetComponent<DamageableEntity>();
                if (hitTarget != null) {
                    hitTarget.OnDamage(damage);
                    //gameObject.SetActive(false);
                    Debug.Log("몬스터 명중");
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }
    }

    //public void DestroyBullet() {
    //    if (photonView.IsMine) {
    //        PhotonNetwork.Destroy(gameObject);
    //    }
    //}
}
