using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering;

public class DragDrop : MonoBehaviour                   //카드 드래그 앤 드롭 처리를 위한 클래스 
{
    public bool isDragging = false;                     //드래그 중인지 판별하는 Bool 값
    public Vector3 startPosition;                       //드래그 시작 위치 
    public Transform startParent;                       //드래그 시작 시 있던 영역 (Area)

    private GameManager gameManager;                    //게임매니저를 참조 한다. 

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;         //시작 위치와 부모 저장
        startParent = transform.parent;

        gameManager = FindObjectOfType<GameManager>();          //게임 매니저 참조 
    }

    // Update is called once per frame
    void Update()
    {
        if(isDragging)      //드래그 중이면 마우스 위치로 카드 이동
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.position = mousePos;
        }
    }

    void OnMouseDown()          //마우스 클릭 시 드래그 시작
    {
        isDragging = true;

        startPosition = transform.position;         //시작 위치와 부모 저장
        startParent = transform.parent;

        GetComponent<SpriteRenderer>().sortingOrder = 10;           //드래그 중인 카드가 다른 카드보다 앞에 보이도록 한다. 
    }

    void OnMouseUp()                //마우스 버튼 놓을 때 
    {
        isDragging = false;
        GetComponent<SpriteRenderer>().sortingOrder = 1;           //드래그 중인 카드가 다른 카드보다 앞에 보이도록 한다. 

        if(gameManager == null)                                 //매니저가 없으면 함수를 종료 한다. 
        {
            RetrunToOriginalPosition();
            return;
        }

        bool wasInMergeArea = startParent == gameManager.mergeArea;         //현재 카드가 어느 영역에서 왔는지 확인 

        if(IsOverArea(gameManager.handArea))                            //손패 영역 위에 카드를 놓았는지 확인
        {
            Debug.Log("손패 영역으로 이동");

            if(wasInMergeArea)                                          //머지영역에서 왔다면 MoveToHand 함수 호출 
            {
                for(int i = 0; i < gameManager.mergeCount; i++)         //카드를 머지 영역에서 제거하고 손패로 이동 
                {
                    if (gameManager.mergeCards[i] == gameObject)            //핸드 배열과 내가 마우스 업하는 오브젝트와 같은 경우
                    {
                        for(int j = i; j < gameManager.mergeCount - 1; j++)
                        {
                            gameManager.mergeCards[j] = gameManager.mergeCards[j + 1];      //해당 카드를 제거하고 배열 뒤에서 앞으로 한칸씩 이동
                        }
                        gameManager.mergeCards[gameManager.mergeCount - 1] = null;      //맨 뒤의 카드를 null 로 설정
                        gameManager.mergeCount--;                                       //카드 수를 줄인다. 

                        transform.SetParent(gameManager.handArea);          //손패에 카드 추가
                        gameManager.handCards[gameManager.handCount] = gameObject;
                        gameManager.handCount++;

                        gameManager.ArrangeHand();                      //영역 정렬
                        gameManager.ArrangeMerge();
                        break;
                    }
                }
            }
            else
            {
                gameManager.ArrangeHand();          //이미 손패에 있는 카드라면 정렬 만 수행 
            }
        }
        else if(IsOverArea(gameManager.mergeArea))                   //머지 영역 위에 카드를 놓았는지 확인 
        {
            if(gameManager.mergeCount >= gameManager.maxMergeSize)  //머지 영역이 가득 찼는지 확인
            {
                Debug.Log("머지 영역이 가득 찼습니다!");
                RetrunToOriginalPosition();
            }
            else
            {
                gameManager.MoveCardToMerge(gameObject);            //머지 영역으로 이동
            }
        }
        else
        {
            RetrunToOriginalPosition();                     //아무 영역도 아니면 원래 위치로 돌아가기 
        }

        if (wasInMergeArea)              //머지 영역에 있을 경우 버튼 상태 업데이트 
        {
            if (gameManager.mergeButton != null)
            {
                bool canMerge = (gameManager.mergeCount == 2 || gameManager.mergeCount == 3);
                gameManager.mergeButton.interactable = canMerge;
            }
        }
       
    }

   
    void RetrunToOriginalPosition()                  //원래 위치로 돌아가는 함수 
    {
        transform.position = startPosition;
        transform.SetParent(startParent);

        if(gameManager != null)
        {
            if(startParent == gameManager.handArea)
            {
                gameManager.ArrangeHand();
            }
            if (startParent == gameManager.mergeArea)
            {
                gameManager.ArrangeMerge();
            }
        }
    }

    bool IsOverArea(Transform area)         //카드가 특정 영역 위에 있는지 확인 
    {
        if(area == null)
        {
            return false;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);        //현재 마우스 위치를 가져오기
        mousePosition.z = 0;                                                                //2D 이기 때문에

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);            //레이캐스트 생성 (마우스 위치에서 아래 방향으로)

        foreach(RaycastHit2D hit in hits)                   //레이캐스트로 감지된 모든 콜라이더 확인
        {
            if(hit.collider != null && hit.collider.transform == area)      //콜라이더의 게임 오브젝트가 찾고 있는 영역인지 확인 
            {
                Debug.Log(area.name + " 영역 감지됨");
                return true;    
            }
        }

        return false;
    }

}
