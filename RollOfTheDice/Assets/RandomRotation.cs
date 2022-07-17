using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    void Start()
    {
        var ownRigidbody = GetComponent<Rigidbody>();
        var x = Random.Range(1, 10);
        var y = Random.Range(1, 20);
        var z = Random.Range(1, 10);
        var randomDirection = new Vector3(x, y, z).normalized;
        ownRigidbody.AddTorque(randomDirection, ForceMode.Impulse);
    }
}
