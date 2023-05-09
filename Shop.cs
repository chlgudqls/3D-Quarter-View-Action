using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;

    //�� ��ư�� ���� ������������, ����, ��ġ �迭 ���� �����̶����
    //�� ������ �� �� ���������
    public GameObject[] itemObj;
    public int[] itemPrice;
    public Transform[] itemPos;
    public Text talkText;
    public string[] talkData;

    //�̰� ������ ������ �ڴʰ� �߰�
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

    //index�� Ű����� 
    public void Buy(int index)
    {
        //if (Input.GetButtonDown("Jump"))
        //    return;
        //Buy�� �ҷ����� �ε����� �ν����Ϳ��� �Ϸ��� ��ư
        int price = itemPrice[index];
        if(price > enterPlayer.coin)
        {
            //�̹� �����ϰ� �ִ��� ���ٰ��ϳ� ���δٰ� ���� ��¥ ���̴���
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }
        enterPlayer.coin -= price;
        //������ġ�� �ָ����� �����аžƴѰ�
        //������ ������ ��¦ ������ �����ذű���
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3)
                            + Vector3.forward * Random.Range(-3, 3);
        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation);
    }
    IEnumerator Talk()
    {
        //�� �ȿ� �ؽ�Ʈ�� �״�� ������� ���ؼ� �迭������
        
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
