using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;
    void Start()
    {
        StartCoroutine(Explosion());
    }
    //�浹������ ���⿡�� �ߴٴ��� �׷��� ������ ���� ��ũ��Ʈ�� ���������� ���� tranform���� ��ũ��Ʈ�ȿ� ������ ȣ�� ������ ���ʹ̴� ����������
    //transform�� ���ؼ� �׷� �����ΰ� 
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        //����ź �ǰ� ���������ε� ��¶�� �����̴ϱ� �迭�� 
        // Ÿ���� �迭�θ� �Ұ� �ƴ϶� SphereCastAll�Ἥ ������ �ɸ� ��������� �θ�����
        //���� ����ź�� �������� 15�� ���ȿ� ������ enemy ��ġ��
        RaycastHit[] rayHits = 
            Physics.SphereCastAll(transform.position, 
                                15, 
                                Vector3.up,
                                0f, 
                                LayerMask.GetMask("Enemy"));
        //�𸣰ڴ� ���ڱ� ��ġ�� �ѱ��
        foreach(RaycastHit hitObj in rayHits)
        {
            //��Ʈ�� �ɸ� ���ʹ��� 
            //hitObj ��ü�� ��ġ��
            //������ ��ġ�� �˾Ƴ��ǰ� ��ġ�� ���� Enemy��ũ��Ʈ�� �������� �Լ����ѱ�� �ش罺ũ��Ʈ�� �������� ���̸�Ǵϱ�
            //������ ��ġ���ϵ� ������Ʈ���ϵ� ����̾��°��� ��ũ��Ʈ�� �������� �������� ��
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }
}
