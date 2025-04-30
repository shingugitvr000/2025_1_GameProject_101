using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        RetrunToOriginalPosition();
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
        }
    }

    bool IsOverArea(Transform area)         //카드가 특정 영역 위에 있는지 확인 
    {
        if(area == null)
        {
            return false;
        }

        //영역의 콜라이더를 가져옴
        Collider2D areaCollider = area.GetComponent<Collider2D>();
        if (areaCollider == null)
            return false;

        //카드가 영역 안에 있는지 확인
        return areaCollider.bounds.Contains(transform.position);
    }
}
