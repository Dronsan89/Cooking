using UnityEngine;
using UnityEngine.UI;

public class Visitor : MonoBehaviour
{
    [SerializeField] public Image[] placeForFood;
    [SerializeField] private Image imageTimeOrder;
    [SerializeField] private Sprite[] spriteFoods;

    public int[] numberFoodInOrder;
    public int maxCountFoodInOrder;
    public int currentCountFootInOrder;

    private int countTypeFood;
    private bool isStrongQueueOrder;
    private GameField gameField;
    private SpawnerVisitors spawnerVisitors;

    void Start()
    {
        gameField = FindObjectOfType<GameField>();
        spawnerVisitors = FindObjectOfType<SpawnerVisitors>();

        maxCountFoodInOrder = gameField.maxCountFoodInOrder;
        isStrongQueueOrder = gameField.isStrongQueueOrder;
        countTypeFood = gameField.countTypeFoodForLevel;

        SetOrder();
    }

    private void SetOrder()
    {
        currentCountFootInOrder = Random.Range(1, maxCountFoodInOrder + 1);
        numberFoodInOrder = new int[currentCountFootInOrder];

        for (int i = 0; i < currentCountFootInOrder; i++)
        {
            placeForFood[i].gameObject.SetActive(true);
            if (isStrongQueueOrder)
            {
                numberFoodInOrder[i] = -1;
                int randFoodInOrder = 0;
                for (int j = 0; j < 100; j++)
                {
                    bool isSuitableNumber = true;
                    randFoodInOrder = Random.Range(0, countTypeFood + 1);
                    for (int l = 0; l < currentCountFootInOrder; l++)
                    {
                        if (randFoodInOrder == numberFoodInOrder[l])
                        {
                            isSuitableNumber = false;
                        }
                    }
                    if (isSuitableNumber)
                    {
                        numberFoodInOrder[i] = randFoodInOrder;
                        break;
                    }
                }
            }
            else
            {
                int randFoodInOrder = Random.Range(0, countTypeFood + 1);
                numberFoodInOrder[i] = randFoodInOrder;
            }
            placeForFood[i].gameObject.SetActive(true);
            placeForFood[i].sprite = spriteFoods[numberFoodInOrder[i]];
        }
    }

    public void CompleteFoodInOrder(int numberFood)
    {
        for (int i = 0; i < numberFoodInOrder.Length; i++)
        {
            if (numberFood == numberFoodInOrder[i])
            {
                placeForFood[i].gameObject.SetActive(false);
                numberFoodInOrder[i] = -1;
                break;
            }
        }

        bool isComleteOrder = true;

        for (int i = 0; i < numberFoodInOrder.Length; i++)
        {
            if (placeForFood[i].isActiveAndEnabled)
            {
                isComleteOrder = false;
            }
        }

        if (isComleteOrder) CompleteOrder();
    }

    public void CompleteOrder()
    {
        spawnerVisitors.SetNewVisitors(this);

        SetOrder();
    }
}
