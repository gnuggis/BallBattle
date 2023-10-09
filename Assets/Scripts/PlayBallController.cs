using Photon.Pun;
using UnityEngine;

public class PlayBallController : MonoBehaviourPun
{

    public Vector3 playerDirection;
    public Vector3 throwDirection;
    private float velocity = 8f;

    public void Start()
    {
        throwDirection = new Vector3(playerDirection.x, 0, playerDirection.z);
    }

    public void Update()
    {
        transform.position += velocity * throwDirection * Time.deltaTime;
    }


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision: " + gameObject.name);
        PhotonNetwork.Destroy(gameObject);
    }
}
