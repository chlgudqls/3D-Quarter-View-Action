using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform targetPos;
    public Vector3 offset;
    void Update()
    {
        transform.position = targetPos.position + offset;
    }
}
