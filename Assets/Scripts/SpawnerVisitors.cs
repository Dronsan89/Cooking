using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerVisitors : MonoBehaviour
{
    [SerializeField] public RectTransform[] posPlace;
    [SerializeField] public List<GameObject> poolVisitors;
    [SerializeField] private GameObject visitorsPrefab;
    [SerializeField] private RectTransform posEnterInLevel;
    [SerializeField] private float timeAnimationSetVisitors;

    public bool isSetAllVisitors;

    private GameField gameField;

    void Start()
    {
        gameField = FindObjectOfType<GameField>();
        StartCoroutine(RespawnVisitors());
    }

    IEnumerator RespawnVisitors()
    {
        isSetAllVisitors = false;

        for (int i = 0; i < posPlace.Length; i++)
        {
            var newVisitor = Instantiate(visitorsPrefab);
            newVisitor.transform.parent = posPlace[i];
            newVisitor.GetComponent<RectTransform>().position = posEnterInLevel.position;
            newVisitor.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, timeAnimationSetVisitors);

            poolVisitors.Add(newVisitor);

            if (gameField.CountMaxVisitors <= 0)
            {
                newVisitor.SetActive(false);
            }

            gameField.CountRemainingVisitors--;
            yield return new WaitForSeconds(timeAnimationSetVisitors);
        }

        isSetAllVisitors = true;
    }

    public void SetNewVisitors(Visitor visitor)
    {
        poolVisitors.Remove(visitor.gameObject);
        if (gameField.CountMaxVisitors > 4)
        {
            isSetAllVisitors = false;
            visitor.GetComponent<RectTransform>().position = posEnterInLevel.position;
            visitor.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, timeAnimationSetVisitors);
            poolVisitors.Add(visitor.gameObject);
            StartCoroutine(AllVisitorsSet());

            gameField.CountRemainingVisitors--;
        }
        else
        {
            visitor.GetComponent<RectTransform>().position = posEnterInLevel.position;
        }
        gameField.CountMaxVisitors--;
    }

    IEnumerator AllVisitorsSet()
    {
        yield return new WaitForSeconds(timeAnimationSetVisitors);
        isSetAllVisitors = true;
    }
}
