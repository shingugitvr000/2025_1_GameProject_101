using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("�⺻ �̵� ����")]
    public float moveSpeed = 5f;                                //�̵� �ӵ� ���� ���� 
    public float jumpForce = 7f;                                //������ �� ���� �ش�. 
    public float turnSpeed = 10f;                               //ȸ�� �ӵ� 

    [Header("���� ���� ����")]
    public float fallMultiplier = 2.5f;                         //�ϰ� �߷� ����
    public float lowJumpMultiplier = 2.0f;                      //ª�� ���� ���� 

    [Header("���� ���� ����")]
    public float coyoteTime = 0.15f;                            //���� ���� �ð� 
    public float coyoteTimeCounter;                             //���� Ÿ�̸� 
    public bool realGrouned = true;                             //���� ���� ���� 

    [Header("�۶��̴� ����")]
    public GameObject gliderObject;                             //�۶��̴� ������Ʈ
    public float gliderFallSpeed = 1.0f;                        //�۶��̴� ���� �ӵ�
    public float gliderMoveSpeed = 7.0f;                        //�۶��̴� �̵� �ӵ�
    public float gliderMaxTime = 5.0f;                          //�ִ� ��� �ð�
    public float gliderTimeLeft;                                //���� ��� �ð�
    public bool isGliding = false;                              //�۶��̵� ������ ����       


    public bool isGrounded = true;                              //���� �ִ��� üũ �ϴ� ���� (true/false)

    public int coinCount = 0;                                   //���� ȹ�� ���� ����
    public int totalCoins = 5;                                  //�� ���� ȹ�� �ʿ� ���� ����

    public Rigidbody rb;                                        //�÷��̾� ��ü�� ���� 

    // Start is called before the first frame update
    void Start()
    {


        //�۶��̴� ������Ʈ �ʱ�ȭ 
        if (gliderObject != null)
        {
            gliderObject.SetActive(false);                      //���� �� ��Ȱ��ȭ
        }

        gliderTimeLeft = gliderMaxTime;                         //�۶��̴� �ð� �ʱ�ȭ 


        coyoteTimeCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //���� ���� ����ȭ
        UpdateGroundedState();

        //������ �Է�
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //�̵� ���� ���� 
        Vector3 movement = new Vector3 (moveHorizontal, 0, moveVertical);   //�̵� ���� ���� 
        
        if (movement.magnitude > 0.1f) //�Է��� ���� ���� ȸ�� 
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);          //�̵� ������ �ٶ󺸷� �ε巴�� ȸ��
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        //GŰ�� �۶��̴� ���� (������ ���ȸ� Ȱ��ȭ)
        if(Input.GetKey(KeyCode.G) && !isGrounded && gliderTimeLeft > 0)  //GŰ�� �����鼭 ���� ���� �ʰ� �۶��̴����� �ð��� ������ (3���� ����)
        {
            if(!isGliding)                              //�۶��̾� Ȱ��ȭ (������ �ִ� ����)
            {
                //�۶��̴� Ȱ��ȭ �Լ� (�Ʒ� ����)
                EnableGlider();
            }

            //�۶��̴� ��� �ð� ���� 
            gliderTimeLeft -= Time.deltaTime;

            //�۶��̴� �ð��� �� �Ǹ� ��Ȱ��ȭ 
            if(gliderTimeLeft <= 0)
            {
                //�۶��̴� ��Ȱ��ȭ �Լ� (�Ʒ� ����)
                DisableGlider();
            }
        }
        else if(isGliding)
        {
            //GŰ�� ���� �۶��̴� ��Ȱ��ȭ 
            DisableGlider();
        }

        if(isGliding)   //������ ó�� 
        {
            //�۶��̴� ��� �� �̵�
            ApplyGliderMovement(moveHorizontal, moveVertical);
        }
        else // (���� ������ �ڵ���� else ���ȿ� �ִ´�.)
        {   
            //�ӵ��� ���� �̵� 
            rb.velocity = new Vector3(moveHorizontal * moveSpeed, rb.velocity.y, moveVertical * moveSpeed);

            //���� ���� ���� ����
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;  //�ϰ� �� �߷� ��ȭ
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))  //��� �� ���� ��ư�� ���� ���� ����
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }       

        //���� �Է�
        if (Input.GetButtonDown("Jump") && isGrounded)          //&& �� ���� �����Ҷ� -> (�����̽� ��ư�� �������� �� isGrounded �� True �϶�)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);         //�������� ������ ����ŭ ��ü�� �ش�. 
            isGrounded = false;                                             //������ �ϴ� ���� ������ �������� ������ false��� ���ش�. 
            realGrouned = false;
            coyoteTimeCounter = 0;                                          //�ڿ��� Ÿ�� ��� ����
        }

        //���鿡 ������ �۶��̴� �ð� ȸ�� �� �۶��̴� ��Ȱ��ȭ 
        if(isGrounded)
        {
            if(isGliding)
            {
                DisableGlider();
            }

            //���� ���� �� �ð� ȸ�� 
            gliderTimeLeft = gliderMaxTime;
        }
    }

    //�۶��̴� Ȱ��ȭ �Լ� 
    void EnableGlider()
    {
        isGliding = true;   

        //�۶��̴� ������Ʈ ǥ��
        if (gliderObject != null)
        {
            gliderObject.SetActive(true);
        }

        //�ϰ� �ӵ� �ʱ�ȭ 
        rb.velocity = new Vector3(rb.velocity.x, -gliderFallSpeed, rb.velocity.z);

    }

    //�۶��̴� ��Ȱ��ȭ �Լ� 
    void DisableGlider()
    {
        isGliding = false;

        //�۶��̴� ������Ʈ �����
        if (gliderObject != null)
        {
            gliderObject.SetActive(false);
        }

        //��� �����ϵ��� �߷� ����
        rb.velocity = new Vector3(rb.velocity.x, 0 , rb.velocity.z);

    }

    //�۶��̴� �̵� ����
    void ApplyGliderMovement(float horizontal, float vertical)      //����� ������ �Լ��� �μ��� �޴´�. 
    {
        //�۶��̴� ȿ�� : õõ�� �������� ���� �������� �� ������ �̵�

        Vector3 gliderVelocity = new Vector3(
            horizontal * gliderMoveSpeed,           //X��
            -gliderFallSpeed,                       //Y�� (������ �ӵ��� õõ�� �ϰ�)
            vertical * gliderMoveSpeed              //Z��
        );

        rb.velocity = gliderVelocity;   
    }


    void OnCollisionEnter(Collision collision)              //�浹 ó�� �Լ� (����)
    {
        if (collision.gameObject.CompareTag("Ground"))            //�浹�� �Ͼ ��ü�� Tag�� Ground ���
        {
            realGrouned = true;                              //���� �浹�ϸ� true�� �����Ѵ�.
        }
    }

    void OnCollisionStay(Collision collision)       //������� �浹�� �����Ǵ��� Ȯ�� (�߰�)
    {
        if (collision.gameObject.CompareTag("Ground"))            //�浹�� �����Ǵ� ��ü�� Tag�� Ground ���
        {
            realGrouned = true;                              //�浹�� �����Ǳ⶧���� true
        }
    }

    void OnCollisionExit(Collision collision)       //���鿡�� ���������� Ȯ�� (�߰�)
    {
        if (collision.gameObject.CompareTag("Ground"))            
        {
            realGrouned = false;                              //���鿡�� �������� ������ false
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

    //���� ���� ������Ʈ �Լ� 
    void UpdateGroundedState()
    {
        if(realGrouned)                 //���� ���鿡 ������ �ڿ��� Ÿ�� ����
        {
            coyoteTimeCounter = coyoteTime;
            isGrounded = true;      
        }
        else
        {
            //�����δ� ���鿡 ������ �ڿ��� Ÿ�� ���� ������ ������ �������� �Ǵ�
            if (coyoteTimeCounter > 0)
            {
                coyoteTimeCounter -= Time.deltaTime;                //�ð��� ���������� ���� ��Ų��.
                isGrounded = true;                              
            }
            else
            {
                isGrounded = false;                                 //Ÿ���� ������ FALSE
            }

        }
    }
}
