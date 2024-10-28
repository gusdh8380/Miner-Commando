using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MinerShipHealth : DamageableEntity
{
    public override int currentHp {
        get { return base.currentHp; }
        set {
            base.currentHp = value;
            UIManager.instance.UpdateShipHp(currentHp);

        }
    }

    public bool sheildOn;

    [SerializeField]
    float spawnRad;

    [SerializeField]
    LayerMask whatIsSpawner;

    private void Start() {
        if (PhotonNetwork.IsMasterClient) {
            InvokeRepeating("SpawnTrigger", 5, 0.2f);
        }
        UIManager.instance.SetShipHp(maxHp);
    }

    private void SpawnTrigger() {
        Collider2D[] spawners = Physics2D.OverlapCircleAll(transform.position, spawnRad, whatIsSpawner);
        Debug.Log("s");
        foreach (Collider2D collider in spawners) {
            MonsterSpawner spawner = collider.GetComponent<MonsterSpawner>();
            if (spawner != null) {
                spawner.Spawn();
                Debug.Log("spawnTrigger");
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, spawnRad);
    }

    [PunRPC]
    public override void OnDamage(int damage) {
        if (PhotonNetwork.IsMasterClient) {
            if (sheildOn) {
                damage /= 2;
            }
            base.OnDamage(damage);
        }
        if (currentHp <= 0) {
            Die();
        }
    }

    public override void Die() {
        base.Die();
        StageManager.Instance.MissionFail();
    }
}
