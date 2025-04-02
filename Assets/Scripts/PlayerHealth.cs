using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3;                    //최대 생명력
    public int currentLives;                    //현재 생명력 

    public float invincibleTime = 1.0f;         //피격 후 무적 시간(반복 피격 방지)
    public bool isInvincible = false;           //무적 여부의 값

    // Start is called before the first frame update
    void Start()
    {
        currentLives = maxLives;                    //생명력 초기화 
    }

    private void OnTriggerEnter(Collider other)             //트리거 영역 안에 들어왔나를 검사하는 함수 
    {
        //코인 수집
        if (other.CompareTag("Missile"))                        //미사일과 충돌 하면 
        {
            currentLives--;                                  
            Destroy(other.gameObject);                      //미사일 오브젝트를 없엔다. 

            if (currentLives <= 0)                              //지금 체력이 0 이하일 경우 
            {
                GameOver();
            }
        }
    }

    void GameOver()     //게임 오버 처리 
    {
        
        gameObject.SetActive(false);            //플레이어 비활성화
        Invoke("RestartGame", 3.0f);            //3초 후 현재 씬 재시작 
    }

    void RestartGame()
    {
        //현재 씬 재시작
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
