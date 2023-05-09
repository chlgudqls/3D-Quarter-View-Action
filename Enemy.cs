using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    //여러 종류의 적이 생기면서 로직을 다르게 사용하고 싶어서 Type enum을 선언
    public enum Type {A, B, C, D};
    public Type enemyType;
    public GameManager gameManager;
    public Transform target;
    public int maxHealth;
    public int curHealth;
    public int score;
    //공격범위 변수
    public BoxCollider meleeArea;
    public GameObject bullet;
    public GameObject[] coins;
    //추적을 결정 
    //죽었을때 추적 멈추게
    public bool isChase;
    public bool isAttack;
    //보스 죽음이 없어서 만듬
    public bool isDead;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    //Material mat;  주석을 meshs로 바꿈
    public MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator anim;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        //mat = GetComponentInChildren<MeshRenderer>().material;
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if(enemyType != Type.D)
            Invoke("ChaseStart", 2);
    }
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    void Update()
    {
        
        //if(isChase) 목표만 잃어버리는 거라서 이동이 유지됨 이동은 유지됨
        if(nav.enabled && enemyType != Type.D)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }

    void FixedUpdate()
    {
        FreezeVelocity();
        Targeting();
    }
    void FreezeVelocity()
    {
        if (isChase)
        {
            //물리회전,속도
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    void Targeting()
    {
        if(enemyType != Type.D && !isDead)
        {
            float targetRadius = 0;
            float targetRange = 0;

            //변수만들고 제일 먼저 찾은 함수
            switch(enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3;
                    break;
                case Type.B:
                    targetRadius = 1f;
                    targetRange = 6;   
                    break;
                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 25;
                    break;
            }

            //이것도 플레이어의 자식객체 인지해서 배열로받은건가
            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 
                                                        targetRadius, 
                                                        transform.forward, 
                                                        targetRange, 
                                                        LayerMask.GetMask("Player"));

            if(rayHits.Length > 0 && !isAttack)
            StartCoroutine(Attack());
        }
     }
    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;
                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;
                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                //방향이나 노말라이즈, 속도,  맞으면사라지고 
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;
        }
         
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }
    void OnTriggerEnter(Collider other)
    {
        //이 방법도 있고 레이어를 추가하여 적 사후 레이어로 분리
        //if (curHealth == 0)
        //    return;
        //각 other의 스크립트를 가져온다 함수사용하기위해서
        if (other.tag == "Melee")
        {
            if (curHealth == 0)
                return;
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));
        }
        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(reactVec, false));
        }
    }
    public void HitByGrenade(Vector3 explosionPos)
    {
        //이건 웨폰이랑 로직이 다르구나 충돌을 Grenade에서 ray로 했음
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }
    //리액션을 다르게 하기위해서 bool값으로 구분시켰음
    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
      

        foreach(MeshRenderer mesh in meshs)
            mesh.material.color = Color.red; 

            yield return new WaitForSeconds(0.1f);

        if(curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
        }
        else
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;

            gameObject.layer = 12;
            isDead = true;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");

            //적이 죽는 로직에 점수 부여와 동전 드랍 구현
            Player player = target.GetComponent<Player>();
            player.score += score;

            int ranCoin = Random.Range(0, 3);
            Instantiate(coins[ranCoin], transform.position, Quaternion.identity);

            switch(enemyType)
            {
                case Type.A:
                    gameManager.enemyCntA--;
                    break;
                case Type.B:
                    gameManager.enemyCntB--;
                    break;
                case Type.C:
                    gameManager.enemyCntC--;
                    break;
                case Type.D:
                    gameManager.enemyCntD--;
                    break;
            }
            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 2;

                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 10, ForceMode.Impulse);
            }
            else
            {
            //피격위치에따라서 방향값이 달라진다고함
            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }

            //if(enemyType != Type.D)
                Destroy(gameObject, 4);
        }
    }
}
