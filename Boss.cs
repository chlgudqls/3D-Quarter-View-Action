using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;

    //가는 방향 예측
    Vector3 lookVec;
    //내려찍는 방향
    Vector3 tauntVec;
    //플레이어 바라보는 플래그
    public bool isLook;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        //여기선 true로 초기화해서 쓰네 뭐지 테스트였다고 하면서 지우네 Update문만있을때
        //isLook = true;
        nav.isStopped = true;
        StartCoroutine(Think());
    }

    void Update()
    {
        if(isDead)
        {
            StopAllCoroutines();
            //코루틴 스탑한다고해도 다시 think를 함 실행중이던 코루틴이 종료될 뿐 아닐수도 그래서 return으로 다음 로직실행안되게막음
            return;
        }

        if(isLook)
        {
            //플레이어 입력값을 가져오기 위해서 input을 사용함
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            //5를 곱하는게 왜 얼만큼 예측할거냐인지 곱했으니까 이동하는 거리보다 더 멀리 보는건가 맞네
            lookVec = new Vector3(h, 0, v) * 5;
            transform.LookAt(target.position + lookVec);
        }
        else
            nav.SetDestination(tauntVec);
    }
    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);
        //시작하면 코루틴에 의해서 think 호출 ranAcion 값에 따라서 case 로 나눔 case에 들어갈 코루틴 작성
        int ranAction = Random.Range(0, 5);
        switch(ranAction)
        {
            case 0:
            case 1:
                //미사일 발사 패턴
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3: 
                //돌 굴러가는 패턴
                StartCoroutine(RockShot());
                break;
            case 4:
                //점프 공격 패턴
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;
        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;
        yield return new WaitForSeconds(2f);
        //패턴이 끝나고 think를 다시 하면 무한반복 되겠네 while문, Invoke 안쓰고도 무한반복가능
        StartCoroutine(Think());
    }
    IEnumerator RockShot()
    {
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);

        isLook = true;
        StartCoroutine(Think());
    }
    IEnumerator Taunt()
    {
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1.5f); 
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        boxCollider.enabled = true;
        StartCoroutine(Think());
    }
}
