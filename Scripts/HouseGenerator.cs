using UnityEngine;

public class SceneGenerator : MonoBehaviour
{
    public GameObject[] prefabs;
    public int count = 10;
    public float stepWidth = 20f;
    public bool randomize = true;

    void Start()
    {
        Generate();
    }

    void Generate()
    {
        for (int i = 0; i < count; i++)
        {
            int index = randomize ? Random.Range(0, prefabs.Length) : i % prefabs.Length;

            Vector3 spawnPos = transform.position + new Vector3(i * stepWidth, 0, 0);

            GameObject segment = Instantiate(prefabs[index], spawnPos, Quaternion.identity);
            segment.transform.SetParent(transform);
        }
    }
}
