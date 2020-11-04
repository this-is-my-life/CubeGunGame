using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public Rigidbody body;
    public PhotonView View;
    public Material MineMaterial;
    public Material NotMineMaterial;
    public Renderer Object;
    public Transform transform;
    public Camera mainCamera;

    private int jumped = 0;

    private Vector3 offset;
 
    [Space]
    [Range(0f, 10f)]
    public float turnSpeed;

    private int kill = 0;
    private int death = 0;
    
    private GameObject killGUI;
    private GameObject deathGUI;

    void Start () {
        killGUI = GameObject.Find("Canvas/LoadingMenu/Kill");
        deathGUI = GameObject.Find("Canvas/LoadingMenu/Death");
    }

    void Awake () {
        mainCamera = Camera.main;
        mainCamera.enabled = true;
        Object.material = View.IsMine ? MineMaterial : NotMineMaterial;
        if (View.IsMine) transform.position = new Vector3(0, 3, 0);

        if (View.IsMine) {
            offset = new Vector3(transform.position.x + mainCamera.transform.position.x, transform.position.y + mainCamera.transform.position.y, transform.position.z + mainCamera.transform.position.z);
            mainCamera.transform.rotation = Quaternion.Euler(mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);
        }
    }

    void Update () {
        if (View.IsMine) {
            if (Input.GetKeyDown(KeyCode.Space) && jumped < 2) {
                body.AddForce(Vector3.up * 5, ForceMode.VelocityChange);
                jumped++;
            }

            if (Input.GetKeyDown(KeyCode.R)) respawn();
            if (Input.GetMouseButtonDown(0)) {
                PhotonNetwork.Instantiate("Bullet", transform.position, transform.rotation);
            }

            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");
            body.velocity = new Vector3(horizontalAxis * 5, body.velocity.y, verticalAxis * 5);
        }
    }

    private void LateUpdate()
    {
        if (View.IsMine) {
            offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up) * Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * turnSpeed, Vector3.right) * offset;
            mainCamera.transform.position = transform.position + offset;
            mainCamera.transform.LookAt(transform.position);
        }
    }

    void FixedUpdate () {
        if (View.IsMine) {
            if (body.position.y < 0) respawn();
        }
    }

    void respawn () {
        transform.position = new Vector3(0, 4, 0);
        body.velocity = new Vector3(0, 0, 0);
        death++;
        deathGUI.GetComponent<TextMeshProUGUI>().SetText("Death: {0}", death);
    }

    void killpUp () {
        kill++;
        killGUI.GetComponent<TextMeshProUGUI>().SetText("Kill: {0}", kill);
    }

    void OnCollisionEnter (Collision collision) {
        if (View.IsMine && collision.gameObject.tag == "Bullet" && !collision.gameObject.GetComponent<PhotonView>().IsMine) respawn();
        if (!View.IsMine && collision.gameObject.tag == "Bullet" && collision.gameObject.GetComponent<PhotonView>().IsMine) killpUp();
        if (collision.gameObject.tag == "Platform") jumped = 0;
    }

    public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info) {
        if (View.IsMine) {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        } else {
            transform.position = (Vector3) stream.ReceiveNext();
            transform.rotation = (Quaternion) stream.ReceiveNext();
        }
    }
}
