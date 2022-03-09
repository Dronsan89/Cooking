using Newtonsoft.Json;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameField : MonoBehaviour
{
    [SerializeField] private Text textTimeLevel;
    [SerializeField] private Text textCountMaxVisitors;
    [SerializeField] private Text textCountRemainingVisitors;
    [SerializeField] private Text textWinLoss;
    [SerializeField] private Text textCountBoost;
    [SerializeField] private GameObject windowWinLoss;
    [SerializeField] private GameObject windowShopBoost;

    private int countRemainingVisitors;
    public int CountRemainingVisitors
    {
        get
        {
            return countRemainingVisitors;
        }
        set
        {
            countRemainingVisitors = value;
            textCountRemainingVisitors.text = countRemainingVisitors.ToString();
        }
    }

    public int CountMaxVisitors
    {
        get
        {
            return countMaxVisitors;
        }
        set
        {
            countMaxVisitors = value;
            textCountMaxVisitors.text = countMaxVisitors.ToString();
            if (countMaxVisitors <= 0) Win();
        }
    }
    [Header("Установите max кол-во посетителей")]
    [SerializeField] private int countMaxVisitors = 6;

    [Header("Установите кол-во видов еды")]
    public int countTypeFoodForLevel = 3;
    [Header("Установите время уровня")]
    public float timeLevel = 200;
    [Header("Установите кол-во еды в заказе")]
    public int maxCountFoodInOrder = 3;
    [Header("Установите строгий порядок заказа")]
    public bool isStrongQueueOrder = false; //только по 1шт каждого вида блюда
    [Header("Установите кол-во бустеров")]
    public int countBoost;

    private SpawnerVisitors spawnerVisitors;
    private bool isBuyBoost;

    private void Awake()
    {
        spawnerVisitors = FindObjectOfType<SpawnerVisitors>();

        SaveData();
        LoadData();

        CountRemainingVisitors = CountMaxVisitors;
        textCountBoost.text = countBoost.ToString();

        isBuyBoost = false;
    }

    private void Update()
    {
        if (timeLevel > 0)
        {
            timeLevel -= Time.deltaTime;
            textTimeLevel.text = ((int)(timeLevel / 60)).ToString() + " : " + ((int)(timeLevel % 60)).ToString();
        }
        else
        {
            timeLevel = 0;
            textTimeLevel.text = "0 : 0";
            Loss();
        }
    }

    private void Win()
    {
        windowWinLoss.SetActive(true);
        textWinLoss.text = "You win!";
    }

    private void Loss()
    {
        windowWinLoss.SetActive(true);
        textWinLoss.text = "You loss!";
    }

    private void LoadData()
    {
        FieldData fieldData = File.Exists("FieldData.json") ? JsonConvert.DeserializeObject<FieldData>(File.ReadAllText("FieldData.json")) : /*new FieldData(this);*/ new FieldData
        {
            _CountMaxVisitors = CountMaxVisitors,
            _CountFoodForLevel = countTypeFoodForLevel,
            _TimeLevel = timeLevel,
            _MaxCountFoodInOrder = maxCountFoodInOrder,
            _isStrongQueueOrder = isStrongQueueOrder,
            _CountBoost = countBoost
        };

        CountMaxVisitors = fieldData._CountMaxVisitors;
        countTypeFoodForLevel = fieldData._CountFoodForLevel;
        timeLevel = fieldData._TimeLevel;
        maxCountFoodInOrder = fieldData._MaxCountFoodInOrder;
        isStrongQueueOrder = fieldData._isStrongQueueOrder;
        countBoost = fieldData._CountBoost;
    }

    private void SaveData()
    {
        FieldData fieldData = new FieldData
        {
            _CountMaxVisitors = CountMaxVisitors,
            _CountFoodForLevel = countTypeFoodForLevel,
            _TimeLevel = timeLevel,
            _MaxCountFoodInOrder = maxCountFoodInOrder,
            _isStrongQueueOrder = isStrongQueueOrder,
            _CountBoost = countBoost
        };

        File.WriteAllText("FieldData.json", JsonConvert.SerializeObject(fieldData));
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    public void Cooked(int numberFood)
    {
        if (timeLevel <= 0) return;

        if (spawnerVisitors.isSetAllVisitors)
        {
            for (int i = 0; i < spawnerVisitors.poolVisitors.Count; i++)
            {
                for (int j = 0; j < spawnerVisitors.poolVisitors[i].GetComponent<Visitor>().currentCountFootInOrder; j++)
                {
                    if (numberFood == spawnerVisitors.poolVisitors[i].GetComponent<Visitor>().numberFoodInOrder[j] &&
                        spawnerVisitors.poolVisitors[i].activeSelf)
                    {
                        spawnerVisitors.poolVisitors[i].GetComponent<Visitor>().CompleteFoodInOrder(numberFood);
                        return;
                    }
                }
            }
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UseBoost()
    {
        if (timeLevel <= 0) return;

        if (countBoost > 0)
        {
            countBoost--;
            textCountBoost.text = countBoost.ToString();

            if (spawnerVisitors.isSetAllVisitors)
            {
                for (int j = 0; j < spawnerVisitors.poolVisitors[0].GetComponent<Visitor>().numberFoodInOrder.Length; j++)
                {

                    spawnerVisitors.poolVisitors[0].GetComponent<Visitor>().placeForFood[j].gameObject.SetActive(false);
                }
                spawnerVisitors.poolVisitors[0].GetComponent<Visitor>().CompleteOrder();
            }
        }

        if (countBoost <= 0 && !isBuyBoost)
        {
            Time.timeScale = 0;
            windowShopBoost.SetActive(true);
        }
    }

    public void BuyBoost()
    {
        if (!isBuyBoost)
        {
            countBoost++;
            textCountBoost.text = countBoost.ToString();
            isBuyBoost = true;
            windowShopBoost.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
