using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTurnManager : MonoBehaviour
{
    //전역 변수 (모든 공이 공유 해서 사용 할 수 있음)
    public static bool canPlay = true;                                  //공을 칠 수 있는지 
    public static bool anyBallMoveing = false;                          //어떤 공이라도 움직이고 있는지  

   
    // Update is called once per frame
    void Update()
    {
        CheckAllBalls();                                //모든 공의 움직임 확인 호출

        if(!anyBallMoveing && !canPlay)
        {
            canPlay = true;                         //모든 공이 멈추면 다시 칠 수 있게 함
            Debug.Log("턴 종료 ! 다시 칠 수 있습니다.");
        }
    }

    void CheckAllBalls()                                //모든 공이 멈췄는지 확인
    {
        SimpleBallController[] allBalls = FindObjectsOfType<SimpleBallController>();    //Scene에 있는 SimpleBallController를 사용 하는 모든 오브젝트를 배열에 넣는다.
        anyBallMoveing = false;                                         //초기화 시켜준다. 

        foreach(SimpleBallController ball in allBalls)              //배열 전체 클래스를 순환 하면서
        {
            if(ball.IsMoving())                                     //공이 움직이고 있는지 확인 하는 함수를 호출
            {
                anyBallMoveing = true;                              //공이 움직인다고 변수 변경
                break;                                              //루프문을 빠저 나온다. 
            }
        }
    }

    public static void OnBallHit()                                  //공을 플레이 했을 때 호출
    {
        canPlay = false;                                            //다른 공들을 못 움직이게 함
        anyBallMoveing = true;                                     //공이 움직이기 시작하기 때문에 bool 값 변경 
        Debug.Log("턴 시작! 공이 멈출 때 까지 기다리세요.");          
    }
}
