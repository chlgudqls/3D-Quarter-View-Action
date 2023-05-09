using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    //�� ���� �� ��������� ���� �ѹ� �غ���
    Rigidbody rigid;
    float angularPower = 2;
    float scaleValue = 0.1f;
    bool isShoot;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }

    //�ڷ�ƾ �ΰ� �� �������� �Լ��� ���ذ� �� �Ȱ���
    IEnumerator GainPowerTimer()
    { 
        yield return new WaitForSeconds(2.2f);
        isShoot = true;
    }
    IEnumerator GainPower()
    {
        while (!isShoot)
        {
            angularPower += 0.04f;
            scaleValue += 0.01f;
            transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            yield return null;
        }
    }
}
