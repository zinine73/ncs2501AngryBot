using System.Drawing;
using UnityEngine;

public class RPCBullet : MonoBehaviour
{
    public GameObject effect;

    private void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 1000.0f);
        Destroy(this.gameObject, 3.0f);    
    }

    void OnCollisionEnter(Collision collision)
    {
        var contact = collision.GetContact(0);
        var obj = Instantiate(effect,
                            contact.point,
                            Quaternion.LookRotation(-contact.normal));
        Destroy(obj, 2.0f);
        Destroy(this.gameObject);
    }
}
