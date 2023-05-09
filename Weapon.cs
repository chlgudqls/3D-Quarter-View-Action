using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //무기에도 enum을 사용
    //무기하나 만드는데 변수 엄청 많음
    public enum Type { Melee, Range};
    public Type type;
    public int damage;
    //공격속도
    public float rate;
    //최대 채워지는 총알
    public int maxAmmo;
    //쓰다남은 현재총알
    public int curAmmo;

    //공격범위
    public BoxCollider meleeArea;
    //휘두를때 효과
    public TrailRenderer trailEffect;
    //총알이 발사되는위치 , 탄피가 배출되는위치
    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;
    public void Use() 
    {
        if(type == Type.Melee)  
        {
            //시작하던 코루틴 중지
            //놔두면 계속실행하는 함수라서 stop사용 해야되는건지
            //끝나기도 전에 호출하면 StopCoroutine이 아래 로직을 무시하고 다시 start시킨다
            //코루틴내부에서 실행여부차이가
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


    //Invoke 세개쓸걸 코루틴하나로 쓴다고함
    IEnumerator Swing()
    {
        //아래로직 비활성화 시킨다
        //yield break;
        //1번구역 로직실행되고 null에서 1프레임을 쉰다
        yield return new WaitForSeconds(0.1f);
        //2번구역실행
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        yield return new WaitForSeconds(0.4f);
        //3번 구역실행
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.3f);
        //4번 구역실행
        trailEffect.enabled = false;
    }
    IEnumerator Shot()
    {
        //1.총알발사
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        //bulletPos.forward 총알의 앞방향 곱하기 속도
        bulletRigid.velocity = bulletPos.forward * 50;
        yield return null;

        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        //힘을가할방향
        //파란색 화살표인 z값에 음수곱하면 방향나옴
        //방향에 거리까지 구한값인가
        //위로 살짝가면서 총바깥쪽으로 나감
        //위방향 * 약한속도 + 바깥쪽방향 * 약한속도 = 약간의 효과
        Vector3 caseVec = Vector3.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        //회전함수
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
        //가해지는 힘의방향 
        //트렌스폼 foward가 z축만의 값인지 디버그 찍어서 확인===============
    }

    //Use() 메인루틴 Swing() 서브루틴 -> Use() 메인루틴 (교차실행) 이게 Invoke인가
    //코루틴 함수 : 메인루틴 + 코루틴 (동시실행)
    //Use() 메인루틴 + Swing() 서브루틴이라고 부르지않고 코루틴이라고 부름 
}
