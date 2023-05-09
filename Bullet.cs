using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    //������ �ƴϸ� ���δϱ�
    //������ �͵� �ı��� �� �־ �����߰�
    //�߰��ϰ� �ν�����â���� ���� üũ����
    public bool isMelee;
    public bool isRock;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor" && !isRock)
            Destroy(gameObject, 3);
    }
    //���ʹ̶����� triggerüũ�ؼ� �ٲ���
    void OnTriggerEnter(Collider collision)
    {
        //���� ���������� ���ߵ�
        if (collision.gameObject.tag == "Wall" && !isMelee)
            Destroy(gameObject);
    } 

      
}
