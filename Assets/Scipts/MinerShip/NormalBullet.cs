using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NormalBullet : MonoBehaviourPun
{
    private Rigidbody2D rb;
    [SerializeField]
    private float speed;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * speed;
    }
}
