using UnityEngine;

public class BulletOOP : MonoBehaviour
{
    public Vector3 Velocity;
    public float Lifetime;

    void Update()
    {
        transform.position += Velocity * Time.deltaTime;
        Lifetime -= Time.deltaTime;

        if (Lifetime <= 0f ||
            Mathf.Abs(transform.position.x) > 50f)
        {
            Destroy(gameObject);
        }
    }
}
