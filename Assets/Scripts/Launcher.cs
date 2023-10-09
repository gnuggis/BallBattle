using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

namespace Gnuggi.BallBattle
{

	public class Launcher : MonoBehaviourPunCallbacks
    {

		#region Private Serializable Fields

		[SerializeField]
		private GameObject controlPanel;

		[SerializeField]
		private Text feedbackText;

		[SerializeField]
		private byte maxPlayersPerRoom = 4;

		#endregion

		#region Private Fields

		bool isConnecting;


		string gameVersion = "1";

		#endregion

		#region MonoBehaviour CallBacks

		void Awake()
		{
			PhotonNetwork.AutomaticallySyncScene = true;
		}

		#endregion


		#region Public Methods

		public void Connect()
		{
			feedbackText.text = "";

			isConnecting = true;


			controlPanel.SetActive(false);



			if (PhotonNetwork.IsConnected)
			{
				LogFeedback("Joining Room...");
				PhotonNetwork.JoinRandomRoom();
			}else{

				LogFeedback("Connecting...");
				
				PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = this.gameVersion;
			}
		}


		void LogFeedback(string message)
		{
			if (feedbackText == null) {
				return;
			}

			feedbackText.text += System.Environment.NewLine+message;
		}

        #endregion


        #region MonoBehaviourPunCallbacks CallBacks

        public override void OnConnectedToMaster()
		{

			if (isConnecting)
			{
				LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");		
				PhotonNetwork.JoinRandomRoom();
			}
		}

		public override void OnJoinRandomFailed(short returnCode, string message)
		{
			LogFeedback("<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room");

			PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom});
		}

		public override void OnDisconnected(DisconnectCause cause)
		{
			LogFeedback("<Color=Red>OnDisconnected</Color> " + cause);

			isConnecting = false;
			controlPanel.SetActive(true);

		}

		public override void OnJoinedRoom()
		{
			LogFeedback("<Color=Green>OnJoinedRoom</Color> with "+PhotonNetwork.CurrentRoom.PlayerCount+" Player(s)");
		
			if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
			{
				Debug.Log("Loading Arena1");

				PhotonNetwork.LoadLevel("Arena1");

			}
		}

		#endregion
		
	}
}