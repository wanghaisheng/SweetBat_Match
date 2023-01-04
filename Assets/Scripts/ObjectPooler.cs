using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [Tooltip("All fruit prefabs")]
    [SerializeField] List<GameObject> fruitPrefabs;
    [Tooltip("Fruit list for object pooler")]
    [SerializeField] List<GameObject> fruitList;
    [Tooltip("Amount of fruits for each prefab at start")]
    [SerializeField] int fruitSize;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        AddFruitToPool(fruitSize);
    }

    // Method in charge of generating the fruits and adding it to the object pooler
    void AddFruitToPool(int amount, bool random = false)
    {
        if (!random)
        {
            for (int i = 0; i < fruitPrefabs.Count; i++)
            {
                Debug.Log("Hi");
                for (int j = 0; j < amount; j++)
                {
                    GameObject fruit = Instantiate(fruitPrefabs[i], gameObject.transform);

                    fruit.SetActive(false);
                    fruitList.Add(fruit);
                }
            }
        }
        else
        {
            int rn = Random.Range(0, fruitPrefabs.Count);

            GameObject fruit = Instantiate(fruitPrefabs[rn], gameObject.transform);
            fruit.SetActive(false);
            fruitList.Add(fruit);
        }
    }

    // Method in charge of delivering or returning a fruit from the grouper of objects.
    public GameObject GetFruitToPool()
    {
        for (int i = 0; i < fruitList.Count; i++)
        {
            if (!fruitList[i].activeSelf)
            {
                fruitList[i].SetActive(false);
                return fruitList[i];
            }
        }

        AddFruitToPool(1, true);

        fruitList[fruitList.Count - 1].SetActive(true);
        return fruitList[fruitList.Count - 1];
    }
}
