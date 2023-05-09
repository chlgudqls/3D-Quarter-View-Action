using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    //���� ������ ���� ����鼭 ������ �ٸ��� ����ϰ� �; Type enum�� ����
    public enum Type {A, B, C, D};
    public Type enemyType;
    public GameManager gameManager;
    public Transform target;
    public int maxHealth;
    public int curHealth;
    public int score;
    //���ݹ��� ����
    public BoxCollider meleeArea;
    public GameObject bullet;
    public GameObject[] coins;
    //������ ���� 
    //�׾����� ���� ���߰�
    public bool isChase;
    public bool isAttack;
    //���� ������ ��� ����
    public bool isDead;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    //Material mat;  �ּ��� meshs�� �ٲ�
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
        
        //if(isChase) ��ǥ�� �Ҿ������ �Ŷ� �̵��� ������ �̵��� ������
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
            //����ȸ��,�ӵ�
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

            //��������� ���� ���� ã�� �Լ�
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

            //�̰͵� �÷��̾��� �ڽİ�ü �����ؼ� �迭�ι����ǰ�
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
                //�����̳� �븻������, �ӵ�,  ������������ 
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
        //�� ����� �ְ� ���̾ �߰��Ͽ� �� ���� ���̾�� �и�
        //if (curHealth == 0)
        //    return;
        //�� other�� ��ũ��Ʈ�� �����´� �Լ�����ϱ����ؼ�
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
        //�̰� �����̶� ������ �ٸ����� �浹�� Grenade���� ray�� ����
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }
    //���׼��� �ٸ��� �ϱ����ؼ� bool������ ���н�����
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

            //���� �״� ������ ���� �ο��� ���� ��� ����
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
            //�ǰ���ġ������ ���Ⱚ�� �޶����ٰ���
            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }

            //if(enemyType != Type.D)
                Destroy(gameObject, 4);
        }
    }
}
