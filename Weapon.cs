using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //���⿡�� enum�� ���
    //�����ϳ� ����µ� ���� ��û ����
    public enum Type { Melee, Range};
    public Type type;
    public int damage;
    //���ݼӵ�
    public float rate;
    //�ִ� ä������ �Ѿ�
    public int maxAmmo;
    //���ٳ��� �����Ѿ�
    public int curAmmo;

    //���ݹ���
    public BoxCollider meleeArea;
    //�ֵθ��� ȿ��
    public TrailRenderer trailEffect;
    //�Ѿ��� �߻�Ǵ���ġ , ź�ǰ� ����Ǵ���ġ
    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;
    public void Use() 
    {
        if(type == Type.Melee)  
        {
            //�����ϴ� �ڷ�ƾ ����
            //���θ� ��ӽ����ϴ� �Լ��� stop��� �ؾߵǴ°���
            //�����⵵ ���� ȣ���ϸ� StopCoroutine�� �Ʒ� ������ �����ϰ� �ٽ� start��Ų��
            //�ڷ�ƾ���ο��� ���࿩�����̰�
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if(type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            //StopCoroutine("Shot");
            StartCoroutine("Shot");
        }
    }


    //Invoke �������� �ڷ�ƾ�ϳ��� ���ٰ���
    IEnumerator Swing()
    {
        //�Ʒ����� ��Ȱ��ȭ ��Ų��
        //yield break;
        //1������ ��������ǰ� null���� 1�������� ����
        yield return new WaitForSeconds(0.1f);
        //2����������
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        yield return new WaitForSeconds(0.4f);
        //3�� ��������
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.3f);
        //4�� ��������
        trailEffect.enabled = false;
    }
    IEnumerator Shot()
    {
        //1.�Ѿ˹߻�
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        //bulletPos.forward �Ѿ��� �չ��� ���ϱ� �ӵ�
        bulletRigid.velocity = bulletPos.forward * 50;
        yield return null;

        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        //�������ҹ���
        //�Ķ��� ȭ��ǥ�� z���� �������ϸ� ���⳪��
        //���⿡ �Ÿ����� ���Ѱ��ΰ�
        //���� ��¦���鼭 �ѹٱ������� ����
        //������ * ���Ѽӵ� + �ٱ��ʹ��� * ���Ѽӵ� = �ణ�� ȿ��
        Vector3 caseVec = Vector3.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        //ȸ���Լ�
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
        //�������� ���ǹ��� 
        //Ʈ������ foward�� z�ุ�� ������ ����� �� Ȯ��===============
    }

    //Use() ���η�ƾ Swing() �����ƾ -> Use() ���η�ƾ (��������) �̰� Invoke�ΰ�
    //�ڷ�ƾ �Լ� : ���η�ƾ + �ڷ�ƾ (���ý���)
    //Use() ���η�ƾ + Swing() �����ƾ�̶�� �θ����ʰ� �ڷ�ƾ�̶�� �θ� 
}
