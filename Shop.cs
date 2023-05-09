using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;

    //각 버튼에 따른 아이템프리펩, 가격, 위치 배열 변수 생성이라고함
    //이 변수는 또 왜 만들었을까
    public GameObject[] itemObj;
    public int[] itemPrice;
    public Transform[] itemPos;
    public Text talkText;
    public string[] talkData;

    //이거 가져온 이유가 뒤늦게 발견
    Player enterPlayer;
    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        anim.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    //index가 키워드네 
    public void Buy(int index)
    {
        //if (Input.GetButtonDown("Jump"))
        //    return;
        //Buy를 불러오는 인덱스는 인스펙터에서 하려나 버튼
        int price = itemPrice[index];
        if(price > enterPlayer.coin)
        {
            //이미 실행하고 있던건 끈다고하네 꼬인다고 빼면 진짜 꼬이는지
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }
        enterPlayer.coin -= price;
        //랜덤위치를 왜만들지 만들어둔거아닌가
        //만들어둔 곳에서 살짝 랜덤을 더해준거구나
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3)
                            + Vector3.forward * Random.Range(-3, 3);
        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation);
    }
    IEnumerator Talk()
    {
        //이 안에 텍스트를 그대로 집어넣지 못해서 배열생성함
        
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
