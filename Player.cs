using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    //웨폰을 획득할때 필요한 변수생성 
    //이런생각을 어떻게 하지
    //배열의 길이를 정해줘야 한다는거 생각 이런종류는 
    //weapons는 손에 비활성화돼있던 무기를 활성화시킴
    public GameObject[] weapons;
    //has는 true가 되면서 무기 입수고
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;
    public Camera followCamera;
    public GameObject grenadeObj;
    public GameManager gameManager;

    public AudioSource jumpSound;

    //탄약, 동전, 체력, 수류탄 변수 생성
    public int ammo;
    public int coin;
    public int health;
    public int score;

    //아이템을 입수하다보면 최대값에 도달
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;
    bool fDown;
    bool gDown;
    bool rDown;
    bool iDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;
    
    bool isJump;
    bool isDodge;
    //지워도됨
    bool isSwap = false;
    //초기값이 true인것처럼 쓰였는데 아닌가 다시
    public bool isFireReady = true;
    bool isReload;
    //벽충돌플레그 생성
    bool isBorder;
    bool isDamage;
    bool isShop;
    bool isDead;

    Vector3 moveVec;
    Vector3 dodgeVec;
    Animator anim;
    MeshRenderer[] meshs;
    Rigidbody rigid;
    GameObject nearObject;
    //무기가 중복착용되서 기존에 장착된 무기를 저장하는 변수 선언
    //이건 그냥 스크립트인데
    public Weapon equipWeapon;
    int equipWeaponIndex = -1;
    //무기사용을 위해서 키입력, 공격딜레이, 공격준비 변수 선언  
    float fireDelay;
    void Awake()
    {
        //에니메이터가 자식한테있음 못가져와서 칠드런
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        //복수는 s를 붙임
        meshs = GetComponentsInChildren<MeshRenderer>();

        //PlayerPrefs.SetInt("MaxScore", 112500);
        Debug.Log(PlayerPrefs.GetInt("MaxScore"));
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Grenade();
        Dodge();
        Interation();
        Swap();
        Attack();
        Reload();
    }
    //업데이트 안은 깔끔하게 나중에 헷갈림
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
        rDown = Input.GetButtonDown("Reload");
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        //3에선 x,z축을 동시에 눌렀을때 대각선으로 = 피타고라스 그런데 정사각형기준으로 대각선은 길이가 길어짐
        //normalized로 보정
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec = dodgeVec;

        //움직일때도 무기교체 못하게 하려하는데 Dodge나 Jump랑은 다르게 함
        //isFireReady가 true인건 딜레이가 넘었다는거
        //false면 딜레이가 rate보다 낮다는거 아예안움직임 무기줍기전이라그런지
        if (isSwap || !isFireReady || isReload || isDead)
            moveVec = Vector3.zero;
        //if(wDown)
        //    transform.position += moveVec * speed * Time.deltaTime * 0.3f;
        //else
        //    //변수 더 해주고 속도랑 transform 이동은 델타타임 
        //    transform.position += moveVec * speed * Time.deltaTime;
        //transform.position += wDown? moveVec * speed * Time.deltaTime * 0.3f : transform.position += moveVec * speed * Time.deltaTime;
        if(!isBorder)
        {
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        }

        //true , false 말고 비교연사자도 들어갈수있음
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }
    void Turn()
    {
        //이동하는 방향으로 바라봄 
        transform.LookAt(transform.position + moveVec);
        //마우스에 의한 회전
        //스크린에서 월드로 레이를쏨
        if(fDown && !isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            //hit에 정보저장 바닥에쏜 ray
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }
    void Jump()
    {
        //시작이 true인가 !면 트루일텐데 해석을 false로함 그런가보다
        if (jDown && !isJump && moveVec == Vector3.zero && !isDodge && !isSwap && !isShop && !isDead)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump"); 
            isJump = true;

            jumpSound.Play();
        }
    }
    void Grenade()
    {
        if (hasGrenades == 0)
            return;

        if(gDown && !isReload && !isSwap && !isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            //hit에 정보저장 바닥에쏜 ray
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 10;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
            }
        }
    }
    //스크립트하나 쓰려고 이렇게 바꿈 무기의 attack을 쓰려면 
    void Reload()
    {
        if (equipWeapon == null)
            return;
        if (equipWeapon.type == Weapon.Type.Melee)
            return;
        if (ammo == 0)
            return;

        if(rDown && !isJump && !isSwap && !isDodge && isFireReady && !isDead)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2f);
        }
    }
    //장전시간걸려서 플래그변수추가함
    void ReloadOut()
    {
        //기능은 여기서 장전이 완료라서
        //maxAmmo는 소지할수있는 최대값이고 가지고있는게 ammo기때문에 maxAmmo는 쓸일이없음
        
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        //총알 갯수구하는 변수만든거고 권총에 총알 더해주진않았음
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }
    void Attack()
    {
        //손에 무기가 있을때만 실행되도록 현재장비 체크
        if (equipWeapon == null)
            return;
         
        fireDelay += Time.deltaTime;
        //false값이 대입되다가 rate 지나면 true 한발쏘면 다시 false되는 반복구조
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && !isDodge && !isSwap && !isShop && !isDead)
        {
            equipWeapon.Use();
            //플레이어가 실행하기때문에 에니메이션을 여기에 집어넣었네
            //매개변수안에 삼항연산자를 넣어서 사용 이것도 다시 봐야됨
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }
    void Dodge()
    {
        if (jDown && !isJump && !isDodge && moveVec != Vector3.zero && !isDodge && !isSwap && !isShop && !isDead)
        {
            speed *= 2;
            //회피할때 방향값을 변수하나에 추가
            //무브가 실행되면 회피했을때 이 값을 무브에 넣음
            //회피방향이되는거임
            //한번 더 봐야될듯
            dodgeVec = moveVec;
            anim.SetTrigger("doDodge");
            isDodge = true;
            Invoke("DodgeOut", 0.5f); 
        }
    }
    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }
    void Swap()
    {
        //equipWeaponIndex 변수 왜 만든거지 weaponIndex가 있는데
        //같은 무기라는걸 알리기 위해서 equipWeaponIndex 선언했는데 기존에 잇던거 쓰면되는거아닌지
        //같은 무기면 에니메이션 동작안하게 하는건데
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        //이러면 그냥 눌렀을때만 인덱스 추가되는데 아이템도 안먹었는데 인덱스 추가되서 그냥 착용되는데
        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;
        //괄호 왜 하는건지 빼봐야 알것같은데
        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge && !isDead)
        {
            //널인데 false로 하면 널에러가 난다 그래서 조건을 null이 아닐때만으로 추가
            //변수이름으로 어떤걸 위한 변수인지 알아보기 쉽게하려고 만드는 경우도있네
            if(equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            //이거 순서바꼇다 에러가 나네 인덱스에러 -1을 넣어 주니까 맞네 왜바뀐거지 어이없네ㅋ
            equipWeaponIndex = weaponIndex;
            //이게 무기 바꿔주는거
            //컴포넌트를 갑자기 불러오는데 뭐지 
            //이러면 붙어있는 무기의 컴포넌트를 가져옴
            //컴포넌트의 객체를 활성화 이렇게 되나 일단 그렇다치고
            //바꾼이유는 웨폰에있는 함수쓰려고 단순히 어쩔수없음
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            //gameObject로 바꿀수가있나 갑자기
            equipWeapon.gameObject.SetActive(true);

            //무기교체 에니메이션은 교체작업부터하고 마지막에 만듬
            anim.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut", 0.4f);
        }
    }
    void SwapOut()
    {
        isSwap = false;
    }
    void Interation()
    {
        //키 조건을 왜 다넣지 굳이  다 넣을 필요가 있을지
        //뭔가 막 연결되있음 다시 한번 생각을 좀 해야됨 보면서
        if (iDown && nearObject != null && !isJump && !isDodge)
        {
            if (nearObject.tag == "Weapon")
            {
                //이렇게 하면 그 아이템에 해당하는 value를 가져올거고
                Item item = nearObject.GetComponent<Item>();
                //아이템에 정해놓은 value를 인덱스로 사용
                int weaponIndex = item.value;
                //닿은 아이템이 true가됨 == 먹었다는 의미
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
            else if(nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }  
        }
    }
    //캐릭터 회전당하는거 저지하는로직
    void FreezeRotation()
    {
        //물리회전속도
        rigid.angularVelocity = Vector3.zero;
    }
    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }
    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor" || collision.gameObject.tag == "Wall")
        {
            //정확하게 점프가 끝날때 착지를 해야되니까
            anim.SetBool("isJump", false);
            isJump = false;
        }
    
    }
    //오브젝트 테그를 검사해서 빈객체에 저장
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Shop")
            nearObject = other.gameObject;

        //Debug.Log(nearObject.name);
    }   
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
        else if(other.tag == "Shop")
        {
            Shop shop = other.GetComponent<Shop>();
            shop.Exit();
            isShop = false;
            nearObject = null;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch(item.type)
            {
                //enum은 이렇게 쓰는건가  자동완성 이렇게 쓰나
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;
                    break;
            }
            Destroy(other.gameObject);
        }
        else if (other.tag == "EnemyBullet")
        {
            if(!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                //if (other.GetComponent<Rigidbody>() != null)
                //    Destroy(other.gameObject);
                bool isBossAtk = other.name == "Boss Melee Area";
                StartCoroutine(OnDamage(isBossAtk));
            }
            //무적이여도 맞으면 사라지는데 데미지는 안달게함
            if (other.GetComponent<Rigidbody>() != null)
                Destroy(other.gameObject);
        }
    }
    //코루틴을 보통 리액션으로 사용
    //무적을 이런식으로도 사용
    //레이어무적, 슈팅에선 어떻게 했는지
    IEnumerator OnDamage(bool isBossAtk)
    {
        isDamage = true;
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }
        if (isBossAtk)
            rigid.AddForce(transform.position * 0.8f, ForceMode.Impulse);

        if (health <= 0 && !isDead)
            OnDie();

        yield return new WaitForSeconds(1f);
        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
        if (isBossAtk)
            rigid.velocity = Vector3.zero;

        //여긴 다시보기 계속 죽어서 이미 죽었으면 그만 두게 함
    }
    void OnDie()
    { 
        anim.SetTrigger("doDie");
        isDead = true;
        gameManager.GameOver();
    }
}
