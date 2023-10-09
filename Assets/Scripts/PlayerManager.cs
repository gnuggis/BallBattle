using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gnuggi.BallBattle
{

    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        public float Health = 1f;

        public static GameObject LocalPlayerInstance;

        public float fireRate = 3F;
        private float nextFire = 0.0F;

        #endregion

        #region Private Fields

        [SerializeField]
        private GameObject playerUiPrefab;

        [SerializeField]
        private GameObject playBallPrefab;

        [SerializeField]
        private AudioSource hitSound;


        #endregion

        #region MonoBehaviour CallBacks

        public void Awake()
        {
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }

            DontDestroyOnLoad(gameObject);
        }

        public void Start()
        {

            CameraWork _cameraWork = gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> CameraWork Component on player Prefab.", this);
            }

            if (this.playerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(this.playerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
            }

			UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }


		public override void OnDisable()
		{
			base.OnDisable();

			UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
		}

        private bool leavingRoom;

        public void Update()
        {
            if (photonView.IsMine)
            {
                this.ProcessInputs();

                if (this.Health <= 0f && !this.leavingRoom)
                {
                    this.leavingRoom = PhotonNetwork.LeaveRoom();
                }
            }
        }

        public override void OnLeftRoom()
        {
            this.leavingRoom = false;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }


            if (!other.name.Contains("PlayBall"))
            {
                return;
            }
            hitSound.Play();
            this.Health -= 0.2f;
        }

        void CalledOnLevelWasLoaded(int level)
        {
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }

            GameObject _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }

        #endregion

        #region Private Methods

		void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
		{
			this.CalledOnLevelWasLoaded(scene.buildIndex);
		}

        void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                if (Time.time > nextFire)
                {
                    nextFire = Time.time + fireRate;
                    PlayBallController ball = PhotonNetwork.Instantiate(this.playBallPrefab.name, LocalPlayerInstance.transform.Find("Model/ShotPosition").position, Quaternion.identity, 0).GetComponent<PlayBallController>();
                    ball.playerDirection = LocalPlayerInstance.transform.Find("Model/ShotPosition").position - LocalPlayerInstance.transform.position;
                }
            }
        }

        #endregion

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(this.Health);
            }
            else
            {
                this.Health = (float)stream.ReceiveNext();
            }
        }

        #endregion
    }
}