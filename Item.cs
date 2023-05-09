using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //enum������ ���� type�� ���� ���� 
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };
    //������ ������ ���� ������  ����
    public Type type;
    public int value;

    Rigidbody rigid;
    SphereCollider sphereCollider;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //GetComponent�� ù��° ������Ʈ�� ������ ����
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
            //������ ��� �������� �ٴڿ� �����Ŵϱ� �ܺι���ȿ�� ����, �浹����
            //�ܺι��� ȿ���� ���ؼ� �������� ���ϰ���
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }
            
    }
}
