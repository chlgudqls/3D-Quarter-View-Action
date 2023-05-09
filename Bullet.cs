using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    //근접만 아니면 놔두니까
    //근접인 것도 파괴될 수 있어서 변수추가
    //추가하고 인스펙터창에서 직접 체크했음
    public bool isMelee;
    public bool isRock;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor" && !isRock)
            Destroy(gameObject, 3);
    }
    //에너미때문에 trigger체크해서 바꿔줌
    void OnTriggerEnter(Collider collision)
    {
        //뭐지 집에서가서 봐야됨
        if (collision.gameObject.tag == "Wall" && !isMelee)
            Destroy(gameObject);
    } 

      
}
