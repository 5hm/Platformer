using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//���� , �������� ���� ���� ����
public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject UIReStartBtn;

    void Update()
    {
      UIPoint.text = (totalPoint + stagePoint).ToString();
    }
    // Start is called before the first frame update
    public void NextStage()
    {
        //Change Stage
        if (stageIndex < (Stages.Length) - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerRePosition();

            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        //Game Clear
        else
        {
            //Player Controll Lock
            Time.timeScale = 0;
            //Result UI
            Debug.Log("����Ŭ����");
            //ReStart Button UI
            //��ư �ؽ�Ʈ�� �ڽĿ�����Ʈ�̹Ƿ� InChildren�� ���
            Text btnText = UIReStartBtn.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            ViewBtn();
        }
       

        //Calculate Point
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if(health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);
        }
        else
        {
            //All Health UI Off
            UIhealth[0].color = new Color(1, 0, 0, 0.4f);

            //Player Die Effect
            player.OnDie();
            //Result UI
            Debug.Log("����");
            //Retry Button UI
            ViewBtn();
        }
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            
           if(health > 1)
            {   
                PlayerRePosition();
            }
          
            //Health Down
            HealthDown();
        }
    }

    void PlayerRePosition()
    {//Player Reposition             ���� �ӵ� zero
        player.transform.position = new Vector3(0, 0, 0);
        player.VelocityZero();
    }

    void ViewBtn()
    {
        UIReStartBtn.SetActive(true);
    }
    public void ReStart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
