using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class BulletScript : MonoBehaviour
{
    public GameObject gameObject;
    public Rigidbody body;
    public PhotonView View;
    public Renderer Object;
    public Material MineMaterial;
    public Material NotMineMaterial;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 3.5f);
        body.velocity = transform.forward * 10;
        Object.material = View.IsMine ? MineMaterial : NotMineMaterial;
    }

    void OnCollisionEnter (Collision collision) {
        if (collision.gameObject.tag == "Platform") Destroy(gameObject, 0f);
    }
}
