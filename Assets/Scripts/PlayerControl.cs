using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviourPun
{
    #region Private Fields

    CharacterController playerControl;

    private float playerSpeed = 10f;
    private Vector3 rotation;
    private float rotationSpeed = 180;

    #endregion

    #region MonoBehaviour CallBacks

    void Start()
    {
        playerControl = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (!playerControl)
        {
            return;
        }

        this.rotation = new Vector3(0, Input.GetAxisRaw("Horizontal") * rotationSpeed * Time.deltaTime, 0);

        Vector3 move = new Vector3(0, 0, Input.GetAxisRaw("Vertical") * Time.deltaTime);
        move = this.transform.TransformDirection(move);
        playerControl.Move(move * playerSpeed);
        this.transform.Rotate(this.rotation);
    }

    #endregion

}
