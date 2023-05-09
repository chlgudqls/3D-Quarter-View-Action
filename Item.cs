using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //enum열거형 지정 type은 따로 지정 
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };
    //아이템 종류와 값을 저장할  변수
    public Type type;
    public int value;

    Rigidbody rigid;
    SphereCollider sphereCollider;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //GetComponent는 첫번째 컴포넌트를 가져옴 유의
        sphereCollider = GetComponent<SphereCollider>();
    }
    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            //닿으면 계속 아이템은 바닥에 있을거니까 외부물리효과 무시, 충돌무시
            //외부물리 효과에 대해서 움직이지 못하게함
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }
            
    }
}
