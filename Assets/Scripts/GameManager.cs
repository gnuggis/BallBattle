﻿using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;

namespace Gnuggi.BallBattle
{
	public class GameManager : MonoBehaviourPunCallbacks
    {

		#region Public Fields

		static public GameManager Instance;

		#endregion

		#region Private Fields

		private GameObject instance;

        [Tooltip("The prefab to use for representing the player")]
        [SerializeField]
        private GameObject playerPrefab;

        #endregion

        #region MonoBehaviour CallBacks

        void Start()
		{
			Instance = this;

			if (!PhotonNetwork.IsConnected)
			{
				SceneManager.LoadScene("Lobby");

				return;
			}

			if (playerPrefab == null) {

				Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
			} else {


				if (PhotonNetwork.InRoom && PlayerManager.LocalPlayerInstance==null)
				{
				    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

					PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f,5f,0f), Quaternion.identity, 0);
				}else{

					Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
				}


			}

		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				QuitApplication();
			}
		}

        #endregion

        #region Photon Callbacks

        public override void OnJoinedRoom()
        {
            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
        }

        public override void OnPlayerEnteredRoom( Player other  )
		{
			Debug.Log( "OnPlayerEnteredRoom() " + other.NickName); 

			if ( PhotonNetwork.IsMasterClient )
			{
				Debug.LogFormat( "OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient ); 

				LoadArena();
			}
		}

		public override void OnPlayerLeftRoom( Player other  )
		{
			Debug.Log( "OnPlayerLeftRoom() " + other.NickName );

			if ( PhotonNetwork.IsMasterClient )
			{
				Debug.LogFormat( "OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient ); 

				LoadArena(); 
			}
		}

		public override void OnLeftRoom()
		{
			SceneManager.LoadScene("Lobby");
		}

		#endregion

		#region Public Methods

		public void LeaveRoom() 
		{
			PhotonNetwork.LeaveRoom();
		}

		public void QuitApplication()
		{
			Application.Quit();
		}

		#endregion

		#region Private Methods

		void LoadArena()
		{
			if ( !PhotonNetwork.IsMasterClient )
			{
				Debug.LogError( "PhotonNetwork : Trying to Load a level but we are not the master Client" );
				return;
			}

			Debug.LogFormat( "PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount );

            PhotonNetwork.LoadLevel("Arena1");
        }

		#endregion

	}

}