using UnityEngine;

public class StageSpawnArea : MonoBehaviour
{
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(20f, 10f);

    public Vector3 GetRandomPosByArea() 
    {
        float randomX = transform.localPosition.x + UnityEngine.Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomY = transform.localPosition.y + UnityEngine.Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);

        return new Vector3(randomX, randomY, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
