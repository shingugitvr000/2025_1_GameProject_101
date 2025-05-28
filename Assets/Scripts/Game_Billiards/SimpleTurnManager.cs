using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTurnManager : MonoBehaviour
{
    //���� ���� (��� ���� ���� �ؼ� ��� �� �� ����)
    public static bool canPlay = true;                                  //���� ĥ �� �ִ��� 
    public static bool anyBallMoveing = false;                          //� ���̶� �����̰� �ִ���  

   
    // Update is called once per frame
    void Update()
    {
        CheckAllBalls();                                //��� ���� ������ Ȯ�� ȣ��

        if(!anyBallMoveing && !canPlay)
        {
            canPlay = true;                         //��� ���� ���߸� �ٽ� ĥ �� �ְ� ��
            Debug.Log("�� ���� ! �ٽ� ĥ �� �ֽ��ϴ�.");
        }
    }

    void CheckAllBalls()                                //��� ���� ������� Ȯ��
    {
        SimpleBallController[] allBalls = FindObjectsOfType<SimpleBallController>();    //Scene�� �ִ� SimpleBallController�� ��� �ϴ� ��� ������Ʈ�� �迭�� �ִ´�.
        anyBallMoveing = false;                                         //�ʱ�ȭ �����ش�. 

        foreach(SimpleBallController ball in allBalls)              //�迭 ��ü Ŭ������ ��ȯ �ϸ鼭
        {
            if(ball.IsMoving())                                     //���� �����̰� �ִ��� Ȯ�� �ϴ� �Լ��� ȣ��
            {
                anyBallMoveing = true;                              //���� �����δٰ� ���� ����
                break;                                              //�������� ���� ���´�. 
            }
        }
    }

    public static void OnBallHit()                                  //���� �÷��� ���� �� ȣ��
    {
        canPlay = false;                                            //�ٸ� ������ �� �����̰� ��
        anyBallMoveing = true;                                     //���� �����̱� �����ϱ� ������ bool �� ���� 
        Debug.Log("�� ����! ���� ���� �� ���� ��ٸ�����.");          
    }
}
