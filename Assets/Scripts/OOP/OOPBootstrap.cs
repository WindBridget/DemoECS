using UnityEngine;

public class OOPBootstrap : MonoBehaviour
{
    public GameObject BulletPrefab;
    [SerializeField] int SpawnCount = 5000;
    [SerializeField] float Spread = 20f;
    [SerializeField] float Lifetime  = 10f;

    void OnEnable()
    {
        for (int i = 0; i < SpawnCount; i++)
        {
            var b = Instantiate(BulletPrefab);
            b.transform.position = new Vector3(
                Random.Range(-Spread, Spread),
                0f,
                Random.Range(-Spread, Spread));
            var bo = b.GetComponent<BulletOOP>();
            bo.Velocity = new Vector3(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f)).normalized * 5f;
            bo.Lifetime = Lifetime;
        }
    }
}
