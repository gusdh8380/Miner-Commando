using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBullet : NormalBullet
{
    protected override void OnTriggerEnter2D(Collider2D other) {
        if (PhotonNetwork.IsMasterClient) {
            if (other.CompareTag("Wall")) {
                //gameObject.SetActive(false);
                PhotonNetwork.Destroy(gameObject);
            }
            else {
                DamageableEntity hitTarget = other.GetComponent<DamageableEntity>();
                if (hitTarget != null) {
                    hitTarget.OnDamage(damage);
                }
            }
        }
    }



    ////임시 구현
    //void Update() {
    //    if (Vector2.Distance(startPosition, transform.position) >= maxDistance) {
    //        DestroyBullet();
    //    }
    //}
}
