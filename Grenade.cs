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
    //충돌감지를 무기에서 했다는점 그러고 감지된 적의 스크립트를 뭐때문인지 몰라도 tranform으로 스크립트안에 데미지 호출 감지된 에너미는 데미지먹음
    //transform을 구해서 그런 이유인가 
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        //수류탄 피격 범위공격인데 어쨋든 적들이니까 배열로 
        // 타입을 배열로만 할게 아니라 SphereCastAll써서 범위에 걸린 모든적들을 부를거임
        //던진 수류탄의 반지름이 15인 원안에 감지된 enemy 위치들
        RaycastHit[] rayHits = 
            Physics.SphereCastAll(transform.position, 
                                15, 
                                Vector3.up,
                                0f, 
                                LayerMask.GetMask("Enemy"));
        //모르겠다 갑자기 위치를 넘기네
        foreach(RaycastHit hitObj in rayHits)
        {
            //히트에 걸린 에너미의 
            //hitObj 자체가 위치라서
            //각각의 위치를 알아낸건가 위치를 토대로 Enemy스크립트를 가져오고 함수를넘긴다 해당스크립트에 데미지만 먹이면되니까
            //구별을 위치로하든 오브젝트로하든 상관이없는거임 스크립트만 가져오면 데미지가 들어감
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }
}
