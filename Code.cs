using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public int maxX;
    public int maxY;
    public int simTime;
    public int recoveryInterval;
    public float contaChance;
    public float xSpace;
    public float ySpace; 
    public float cellSize;
    public bool isSimStart;

    public TMP_Text status;
    public TMP_Text startButtonTMP;

    public GameObject prefab;
    public List<GameObject> cellList = new List<GameObject>();
    public List<int> cellStatus = new List<int>(); //0: 미감염, 1: 감염, 2: 면역
    public List<int> contaTime = new List<int>();

    public float _pointTime = 1.0f; //1초마다 실행
    private float _nextTime = 0.0f; //다음번 실행할 시간

    private GameObject cell;

    public int normalCellCount;
    public int contaCellCount;
    public int immuneCellCount;

    public TMP_InputField maxCellXInput;
    public TMP_InputField maxCellYInput;
    public TMP_InputField xSpaceInput;
    public TMP_InputField ySpaceInput;
    public TMP_InputField cellSizeInput;
    public TMP_InputField pointTimeInput;
    public TMP_InputField contaChanceInput;
    public TMP_InputField recoveryIntervalInput;

    public GameObject settingBtn;


    public GameObject settingPannel;
    public GameObject startBtn;
    public GameObject applyBtn;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (isSimStart)
        {
            if (Time.time > _nextTime)
            {
                _nextTime = Time.time + _pointTime; //다음번 실행할 시간

                status.text = "셀 개수: " + maxX * maxY + "\n미감염: " + normalCellCount + "\n감염: " + contaCellCount + "\n회복(면역): " + immuneCellCount + "\n경과일: " + simTime;

                if(contaCellCount == 0)
                {
                    SimStart();
                }
                //실행 할 코드작성
                Debug.Log("Ticked");
                simTime++;
                for (int i = 0; i < cellList.Count; i++)
                {
                    if((i+1) % maxX != 0 && cellStatus[i] == 0) //오른쪽 셀 감염 여부 확인
                    {
                        if (cellStatus[i+1] == 1 && Probability(contaChance) && contaTime[i+1] != simTime)
                        {
                            cellStatus[i] = 1;
                            cellList[i].gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                            contaTime[i] = simTime + Random.Range(recoveryInterval*-1, recoveryInterval);
                            contaCellCount++;
                            normalCellCount--;
                        }
                    }
                    if(i % maxX != 0 && cellStatus[i] == 0) //왼쪽 셀 감염 여부 확인
                    {
                        if (cellStatus[i-1] == 1 && Probability(contaChance) && contaTime[i-1] != simTime)
                        {
                            cellStatus[i] = 1;
                            cellList[i].gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                            contaTime[i] = simTime + Random.Range(recoveryInterval * -1, recoveryInterval);
                            contaCellCount++;
                            normalCellCount--;
                        }
                    }
                    if(i-maxX > 0 && cellStatus[i] == 0)
                    {
                        if (cellStatus[i-maxX] == 1 && Probability(contaChance) && contaTime[i-maxX] != simTime)
                        {
                            cellStatus[i] = 1;
                            cellList[i].gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                            contaTime[i] = simTime + Random.Range(recoveryInterval * -1, recoveryInterval);
                            contaCellCount++;
                            normalCellCount--;
                        }
;                   }
                    if(i+maxX+1 <= cellStatus.Count && cellStatus[i] == 0)
                    {
                        if (cellStatus[i+maxX] == 1 && Probability(contaChance) && contaTime[i+maxX] != simTime)
                        {
                            cellStatus[i] = 1;
                            cellList[i].gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                            contaTime[i] = simTime + Random.Range(recoveryInterval * -1, recoveryInterval);
                            contaCellCount++;
                            normalCellCount--;
                        }
                    }

                    if (cellStatus[i] == 1 && contaTime[i] != -10000 && simTime - contaTime[i] >= 14)
                    {
                        cellStatus[i] = 2;
                        cellList[i].gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
                        contaCellCount--;
                        immuneCellCount++;
                    }

                }
            }
        }
        

    }


    public void SimStart()
    {   if(!isSimStart)
        {
            _nextTime = Time.time + _pointTime;
            float xPos = (maxX - 1) * xSpace * -0.5f;
            float yPos = (maxY - 1) * ySpace * 0.5f;

            normalCellCount = maxX * maxY;
            contaCellCount = 0;
            immuneCellCount = 0;
            simTime = 0;

            settingBtn.GetComponent<Button>().interactable = false;
            for(int i = 0; i < cellList.Count; i++)
            {
                Destroy(cellList[i].gameObject);
            }
            cellList.Clear();
            cellStatus.Clear();
            contaTime.Clear();

            for (int i = 0; i < maxX * maxY; i++)
            {

                cell = Instantiate(prefab, new Vector3(xPos, yPos, 0), Quaternion.identity);
                cell.gameObject.transform.localScale = new Vector3(cellSize, cellSize, 1);

                cellList.Add(cell.gameObject);
                cellStatus.Add(0);
                contaTime.Add(-10000);
                if ((i + 1) % (maxX) > 0 || i == 0)
                {
                    xPos += xSpace;
                }
                else
                {
                    xPos = (maxX - 1) * xSpace * -0.5f;
                    yPos -= ySpace;
                }




                prefab.SetActive(true);
            }
            int randomStatus = Random.Range(0, cellStatus.Count);
            cellStatus[randomStatus] = 1;
            cellList[randomStatus].gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            contaTime[randomStatus] = 0 + Random.Range(recoveryInterval * -1, recoveryInterval);

            normalCellCount--;
            contaCellCount++;

            startButtonTMP.text = "중지";
            isSimStart = true;

        }
        else
        {
            isSimStart=false;
            startButtonTMP.text = "시작";
            settingBtn.GetComponent<Button>().interactable = true;

        }

    }
        



    public static bool Probability(float chance)
    {
        return (UnityEngine.Random.value <= chance);
    }

    public void TogglePannel()
    {
        startBtn.GetComponent<Button>().interactable = settingPannel.activeSelf;
        settingPannel.SetActive(!settingPannel.activeSelf);

    }

    public void applySettings()
    {
        maxX = int.Parse (maxCellXInput.text);
        maxY = int.Parse (maxCellYInput.text);
        xSpace = float.Parse(xSpaceInput.text);
        ySpace = float.Parse(ySpaceInput.text);
        cellSize = float.Parse(cellSizeInput.text);
        _pointTime = float.Parse (pointTimeInput.text);
        contaChance = int.Parse (contaChanceInput.text) / 100f;
        recoveryInterval = int.Parse(recoveryIntervalInput.text);

    }

    /*    
    public InputField maxCellXInput;
    public InputField maxCellYInput;
    public InputField xSpaceInput;
    public InputField ySpaceInput;
    public InputField cellSizeInput;
    public InputField pointTimeInput;
    public InputField contaChanceInput;
    public InputField recoveryIntervalInput;
    */
}
