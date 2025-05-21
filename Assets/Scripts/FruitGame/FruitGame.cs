using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitGame : MonoBehaviour
{

    public GameObject[] fruitPrefabs;                           //과일 프리팹 배열 선언

    public float[] fruitSizes = { 0.5f, 0.7f, 0.9f, 1.1f, 1.3f, 1.5f, 1.7f, 1.9f };     //과일 크기 선언

    public GameObject currentFruit;                             //현재 들고 있는 과일 
    public int currentFruitType;

    public float fruitStartHeight = 6.0f;                       //과일 시작시 높이 설정(인스펙트에서 조절 가능)
    public float gameWidth = 5.0f;                              //게임판 너비
    public bool isGameOver = false;                             //게임 상태
    public Camera mainCamera;                                   //카메라 참조(마우스 위치 변환에 필요)

    public float fruitTimer;                                    //잰 시간 설정을 위한 타이머 

    public float gameHeight;                                    //게임 종료 높이

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;                               //메인 카메라 참조 가져 오기
        SpawnNewFruit();                                        //게임 시작 시 첫 과일 생성
        fruitTimer = -3.0f;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver) return;         //게임 오버면 리턴

        if(fruitTimer >= 0)                     //타이머의 시간이 0보다 클 경우
        {
            fruitTimer -= Time.deltaTime;   
        }

        if (fruitTimer < 0 && fruitTimer > -2)      //타이머 시간이 0과 -2 사이에 있을때 잰 함수를 호출 하고 다른 시간대로 보낸다.
        {
            CheckGameOver();
            SpawnNewFruit();
            fruitTimer = -3.0f;                 //타이머 시간을 -3으로 보낸다. 
        }

        if (currentFruit != null)           //현재 생성된 과일이 있을 때만 처리 
        {
            Vector3 mousePosition = Input.mousePosition;            //마우스 위치를 받아온다. 
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);   //마우스 위치를 월드 좌표로 변환

            Vector3 newPosition = currentFruit.transform.position;                  //과일 위치 업데이트
            newPosition.x = worldPosition.x;

            float halfFruitSize = fruitSizes[currentFruitType] / 2f;
            if (newPosition.x < -gameWidth / 2 + halfFruitSize)
            {
                newPosition.x = -gameWidth / 2 + halfFruitSize;
            }
            if (newPosition.x > gameWidth / 2 + halfFruitSize)
            {
                newPosition.x = gameWidth / 2 + halfFruitSize;
            }

            currentFruit.transform.position = newPosition;                          //과일 좌표 갱신
        }

        if(Input.GetMouseButtonDown(0) && fruitTimer == -3.0f)          //마우스 좌 클릭하면 과일을 떨어트린다. && 타이머 시간은 -3.0f
        {
            DropFruit();
        }

    }

    void SpawnNewFruit()                    //과일 생성 함수 
    {
        if (!isGameOver)                     //게임 오버가 아닐 때만 새 과일 생성 
        {
            currentFruitType = Random.Range(0, 3);                  //0 ~ 2 사이의 랜덤 과일 타입 

            Vector3 mousePosition = Input.mousePosition;            //마우스 위치를 받아온다. 
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);   //마우스 위치를 월드 좌표로 변환

            Vector3 spawnPosition = new Vector3(worldPosition.x, fruitStartHeight, 0);      //X 좌표만 사용하고 나머지는 설정한 대로 한다. 

            float halfFruitSize = fruitSizes[currentFruitType] / 2;

            //X 의 위치가 게임 영역을 벗어나지 않도록 제한 
            spawnPosition.x = Mathf.Clamp(spawnPosition.x, -gameWidth / 2 + halfFruitSize, gameWidth / 2 - halfFruitSize);

            currentFruit = Instantiate(fruitPrefabs[currentFruitType], spawnPosition, Quaternion.identity);     //과일 생성 
            currentFruit.transform.localScale = new Vector3(fruitSizes[currentFruitType], fruitSizes[currentFruitType], 1);     //과일 크기 설정

            Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();          //강체를 받아와서
            if (rb != null)
            {

                rb.gravityScale = 0f;                                           //중력을 0으로 만들어 준다. 
            }
        }
    }

    void DropFruit()
    {
        Rigidbody2D rb = currentFruit.GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            rb.gravityScale = 1f;                   //중력을 원래 값으로 복구 시킨다. 

            currentFruit = null;                    //현재 들고 있는 과일 해제

            fruitTimer = 1.0f;                      //타이머 초기화 
        }
    }

    public void MergeFruits(int fruitType, Vector3 position)
    {
        if(fruitType < fruitPrefabs.Length - 1)         //마지막 과일 타입이 아니라면 
        {
            GameObject newFruit = Instantiate(fruitPrefabs[fruitType + 1], position, Quaternion.identity);
            newFruit.transform.localScale = new Vector3(fruitSizes[fruitType + 1], fruitSizes[fruitType + 1], 1.0f);

            //점수 추가 로직 등등
        }
    }

    public void CheckGameOver()          //게임 오버 체크 함수
    {
        Fruit[] allFruits = FindObjectsOfType<Fruit>();             //Scene에 있는 모든 과일 컴포넌트가 붙어있는 오브젝트를 가져온다. 작은 게임에서만 사용 비용이 쌤

        float gameOverHeight = gameHeight;              //일정 높이보다 높은 위치에 과일이 있는지 확인

        for(int i = 0; i < allFruits.Length; i++)               //모든 과일을 검사한다.
        {
            if (allFruits[i] != null)
            {
                Rigidbody2D rb = allFruits[i].GetComponent<Rigidbody2D>();

                //과일이 정지 상태이고 높은 위치에 있다면 
                if (rb != null && rb.velocity.magnitude < 0.1f && allFruits[i].transform.position.y > gameOverHeight)
                {
                    isGameOver = true;
                    Debug.Log("게임 오버");

                    break;
                }
            }
        }
    }
}
