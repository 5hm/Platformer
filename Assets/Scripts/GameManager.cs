using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//점수 , 스테이지 전역 변수 생성
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
            Debug.Log("게임클리어");
            //ReStart Button UI
            //버튼 텍스트는 자식오브젝트이므로 InChildren을 사용
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
            Debug.Log("죽음");
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
    {//Player Reposition             낙하 속도 zero
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
