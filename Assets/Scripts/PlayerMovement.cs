using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;                                //�̵� �ӵ� ���� ���� 
    public float jumpForce = 5f;                                //������ �� ���� �ش�. 

    public bool isGrounded = true;                              //���� �ִ��� üũ �ϴ� ���� (true/false)

    public int coinCount = 0;                                   //���� ȹ�� ���� ����
    public int totalCoins = 5;                                  //�� ���� ȹ�� �ʿ� ���� ����

    public Rigidbody rb;                                        //�÷��̾� ��ü�� ���� 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //������ �Է�
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //�ӵ��� ���� �̵�
        rb.velocity = new Vector3(moveHorizontal * moveSpeed, rb.velocity.y, moveVertical * moveSpeed);

        //���� �Է�
        if (Input.GetButtonDown("Jump") && isGrounded)          //&& �� ���� �����Ҷ� -> (�����̽� ��ư�� �������� �� isGrounded �� True �϶�)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);         //�������� ������ ����ŭ ��ü�� �ش�. 
            isGrounded = false;                                             //������ �ϴ� ���� ������ �������� ������ false��� ���ش�. 
        }
    }

    void OnCollisionEnter(Collision collision)              //�浹 ó�� �Լ� 
    {
        if (collision.gameObject.tag == "Ground")            //�浹�� �Ͼ ��ü�� Tag�� Ground ���
        {
            isGrounded = true;                              //���� �浹�ϸ� true�� �����Ѵ�.
        }

    }


    private void OnTriggerEnter(Collider other)             //Ʈ���� ���� �ȿ� ���Գ��� �˻��ϴ� �Լ� 
    {
        //���� ����
        if(other.CompareTag("Coin"))                        //���� Ʈ���ſ� �浹 �ϸ� 
        {
            coinCount++;                                    //coinCount = coinCount + 1  ���� ���� 1�� �÷��ش�.
            Destroy(other.gameObject);                      //���� ������Ʈ�� ������. 
            Debug.Log($"���� ���� : {coinCount}/{totalCoins}");
        }

        //������ ���� �� ���� �α� �ⷰ
        if(other.gameObject.tag == "Door" && coinCount == totalCoins)           //��� ������ ȹ���Ŀ� ������ ���� ���� ����
        {
            Debug.Log("���� Ŭ����");
            //���� �Ϸ� ���� �߰� ����
        }
    }
}
