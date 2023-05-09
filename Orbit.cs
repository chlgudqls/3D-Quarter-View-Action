using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    //공전 목표,속도,거리
    public Transform target;
    public float orbitSpeed;
    Vector3 offSet;
    void Start()
    {
        offSet = transform.position - target.position;
    }

    void Update()
    {
        transform.position = target.position + offSet;
        transform.RotateAround(target.position, Vector3.down, orbitSpeed * Time.deltaTime);

        offSet = transform.position - target.position;
    }
}
