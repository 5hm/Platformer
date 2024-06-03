using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    public float maxSpeed;
    public float jumpPower;
    public EnemyAI enemyAI;

    SpriteRenderer spriter;
    Rigidbody2D rigid;
    Animator anim;
    Collider2D col;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
    }
   

    void Update()
    {
        //Jump                                  �������� �ƴҶ���
        if (Input.GetButtonDown("Jump") && !anim.GetBool("is_Jump"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("is_Jump", true);
            //Sound
            PlaySound("JUMP");
        }


        //����Ű���� ���� ���� �� ���� (Stop Speed)
        if (Input.GetButtonUp("Horizontal"))
        { 
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
                                            //���� �ӵ� ���ϱ�
        }

        //������ȯ (Direction Sprite)
        if(Input.GetButton("Horizontal"))
            spriter.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //�ִϸ��̼�                   ���� �ӵ� 0.3���ϸ� ���� -> Idle
        if(Mathf.Abs(rigid.velocity.x) < 0.3)
        {
            anim.SetBool("is_Run", false);
        }
        else
        {
            anim.SetBool("is_Run", true);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Max Speed
        if (rigid.velocity.x > maxSpeed)
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y); //right max speed
                                                  //�������� �� �̵��� �ӵ��� ����
           
        }
        else if (rigid.velocity.x < -maxSpeed)
        {
            rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y); //left max speed
            
        }

        //Landing Platform
        //������ �� : velocity.y�� ���� �����ϰ�� ray���
        if(rigid.velocity.y < 0)
        {                                                  //���
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            //Layer�� "Platform"�ΰ͸� ��ĵ
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            //rayhit�� ���� collider�� ������
            if (rayHit.collider != null)
            {
                //Player�� �⺻ ũ��� 1 ���� ������ 0.5�� ����
                if (rayHit.distance < 0.5)
                {
                    anim.SetBool("is_Jump", false);
                }

            }
        }
     }

    //�浹 �̺�Ʈ
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            //Enemy�� ���� ����
            if(collision.gameObject.layer == 7)
            {
                //Attack
                //���� ���϶�                 Player�� y��������       ����� y�����Ǻ��� �������
                if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
                {
                    Debug.Log(transform.position.y + " Player");
                    Debug.Log(collision.transform.position.y + " Enemy");
                    OnAttack(collision.transform);

                }
                // Damaged
                else
                {
                    OnDamage(collision.transform.position);

                }
            }
            else
            {
                OnDamage(collision.transform.position);
            }
           
            
        }
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            //Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
            {
                gameManager.stagePoint += 50;
            }
            else if (isSilver)
            {
                gameManager.stagePoint += 100;
            }
            else if (isGold)
            {
                gameManager.stagePoint += 300;
            }

            //Deactive Item
            collision.gameObject.SetActive(false);

            //sound
            PlaySound("ITEM");
        }
        else if(collision.gameObject.tag == "Finish")
        {
            // Next Stage
            gameManager.NextStage();
            //Sound
            PlaySound("FINISH");
        }
    }

    //����
    void OnAttack(Transform enemy)
    {
        //Point
        gameManager.stagePoint += 100;

        //Reaction Force ���� �� ���� ƨ��
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        
        //Enemy Die
        enemyAI = enemyAI.GetComponent<EnemyAI>();
        enemyAI.Ondamaged();

        //Sound
        PlaySound("ATTACK");
    }

    //�ǰݽ� ��������
    void OnDamage(Vector2 tagetPos)
    {
        //Health Down
        gameManager.HealthDown();

        //�ڱ� �ڽ� = Player
        //layer ����
        gameObject.layer = 9;

        //���� ����
        spriter.color = new Color(1, 1, 1, 0.4f);

        //ƨ�� ����
        //          Enemy position - Player position > 0 = ���ʿ��� �ε��� -> ���������� ƨ��
        //          Enemy position - Player position < 0 = �����ʿ��� �ε��� -> �������� ƨ��
        int dirc = transform.position.x - tagetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        //Animator
        anim.SetTrigger("is_Damaged");
        Invoke("OffDamaged", 2);

        //Sound
        PlaySound("DAMAGED");

    }

    //�������� ����
    void OffDamaged()
    {
        gameObject.layer = 8;
        spriter.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        //Sprite Alpha
        spriter.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        spriter.flipY = true;
        //Collider Disable
        col.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //Sound
        PlaySound("DIE");
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                audioSource.Play();
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                audioSource.Play();
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                audioSource.Play();
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                audioSource.Play();
                break;
            case "DIE":
                audioSource.clip = audioDie;
                audioSource.Play();
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                audioSource.Play();
                break;

        }
    }
}
