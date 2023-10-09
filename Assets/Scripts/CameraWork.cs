using UnityEngine;
using Photon.Pun;

namespace Gnuggi.BallBattle
{
	public class CameraWork : MonoBehaviour
	{
        #region Private Fields

        private float smoothTime = 1f;
        private Vector3 velocity = Vector3.zero;

        Transform cameraTransform;

		[SerializeField]
		Transform cameraPos;

		bool isFollowing;

        #endregion

        #region MonoBehaviour Callbacks

		void LateUpdate()
		{
			if (cameraTransform == null && isFollowing)
			{
				OnStartFollowing();
			}

			if (isFollowing) {
				Follow();
			}
		}

		#endregion

		#region Public Methods

		public void OnStartFollowing()
		{	      
			cameraTransform = Camera.main.transform;
			isFollowing = true;
			Cut();
		}
		
		#endregion

		#region Private Methods

		void Follow()
		{		
            cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, cameraPos.transform.position, ref velocity, smoothTime * Time.deltaTime);
            cameraTransform.LookAt(this.transform.position);
        }

	   
		void Cut()
		{
			cameraTransform.position = this.cameraPos.position;
			cameraTransform.LookAt(this.transform.position);
		}
		#endregion
	}
}