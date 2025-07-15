using UnityEngine;

public class StageSpawnArea : MonoBehaviour
{
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(20f, 10f);
    [SerializeField] private Transform[] spawnSlotArr;

    //@tk 이건 무작위 몬스터 생성
    public Vector3 GetRandomPosByArea() 
    {
        float randomX = transform.localPosition.x + UnityEngine.Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomY = transform.localPosition.y + UnityEngine.Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);

        return new Vector3(randomX, randomY, 0f);
    }

    public Vector3 GetRandomPosBySlot()
    {
        int rand = UnityEngine.Random.Range(0, spawnSlotArr.Length);
        return spawnSlotArr[rand].position;
    }

    public Vector3 GetPosByIndex(int index)
    {
        return spawnSlotArr[index].position;
    }
    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
