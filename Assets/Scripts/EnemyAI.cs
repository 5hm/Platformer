using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriter;
    Collider2D col;
    public int nextMove;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();

        //5�� �ڿ� �Լ� ȣ��
        Invoke("Think", 5);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //Platform Check                    ���� ������ + �̵��� ������ = �� ĭ ���� ��
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        //Layer�� "Platform"�ΰ͸� ��ĵ
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        //rayhit�� ���� collider�� ������(��������)
        if (rayHit.collider == null)
        {
            Turn();
        }
    }

    //����Լ�
    void Think()
    {


        //Set Next Move
        nextMove = Random.Range(-1, 2);
        anim.SetInteger("Run_Speed", nextMove);

        //Flip Sprite
        if (nextMove != 0)
            spriter.flipX = nextMove == 1;

        //Recursive
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }


    void Turn()
    {
        nextMove *= -1;
        spriter.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 2);
    }

    public void Ondamaged()
    {
        //Sprite Alpha
        spriter.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        spriter.flipY = true;
        //Collider Disable
        col.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //Destroy
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }

}
