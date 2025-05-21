using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public int fruitType;                       //과일 타입 (0 : 사과 , 1: 블루 베리 , 2 : 코코넛 ) int 로 index 설정

    public bool hasMered = false;               //과일이 합쳐졌는지 확인하는 플래그 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasMered)                           //이미 합쳐진 과일은 무시 
            return;

        Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();      //다른 과일과 충돌했는지 확인 

        if(otherFruit != null && !otherFruit.hasMered && otherFruit.fruitType == fruitType) //충돌한 것이 과일이고 타일이 같다면 (합쳐지지 않았을 경우)
        {
            hasMered = true;            //합쳤다고 표시
            otherFruit.hasMered = true;

            Vector3 mergePosition = (transform.position + otherFruit.transform.position) / 2f;          //두 과일의 중간 위치 계산 

            //게임 매니저에서 Merge 구현된것을 호출 
            FruitGame gameManager = FindObjectOfType<FruitGame>();  
            if(gameManager != null)
            {
                gameManager.MergeFruits(fruitType, mergePosition);          //함수를 실행하고 호출 한다. 
            }

            //과일들 제거
            Destroy(otherFruit.gameObject);
            Destroy(gameObject);
        }
    }

}
