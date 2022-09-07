using System;
using Main;
using Mechanics;
using Network;
using UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Characters
{
    public class ShipController : NetworkMovableObject
    {
        public string PlayerName
        {
            get => _playerName;
            set => _playerName = value;
        }
        private Vector3 currentPositionSmoothVelocity; //
        private Vector3 startPosition; //

        protected override float _speed => _shipSpeed;
        [SerializeField] private Transform _cameraAttach;
        private CameraOrbit _cameraOrbit;
        private PlayerLabel _playerLabel;
        private float _shipSpeed;
        private Rigidbody _rb;
        [SyncVar] private string _playerName;

        private void OnGUI()
        {
            if (_cameraOrbit == null)
            {
                return;
            }
            _cameraOrbit.ShowPlayerLabels(_playerLabel);
        }

        public override void OnStartAuthority()
        {
            _rb = GetComponent<Rigidbody>();
            if (_rb == null)
            {
                return;
            }
            gameObject.name = _playerName;
            _cameraOrbit = FindObjectOfType<CameraOrbit>();
            _cameraOrbit.Initiate(_cameraAttach == null ? transform : _cameraAttach);
            _playerLabel = GetComponentInChildren<PlayerLabel>();
            startPosition = transform.position; //
            base.OnStartAuthority();
        }

        protected override void HasAuthorityMovement()
        {
            var spaceShipSettings = SettingsContainer.Instance?.SpaceShipSettings;
            if (spaceShipSettings == null)
            {
                return;
            }

            var isFaster = Input.GetKey(KeyCode.LeftShift);
            var speed = spaceShipSettings.ShipSpeed;
            var faster = isFaster ? spaceShipSettings.Faster : 1.0f;
            _shipSpeed = Mathf.Lerp(_shipSpeed, speed * faster, SettingsContainer.Instance.SpaceShipSettings.Acceleration);
            var currentFov = isFaster ? SettingsContainer.Instance.SpaceShipSettings.FasterFov : SettingsContainer.Instance.SpaceShipSettings.NormalFov;
            _cameraOrbit.SetFov(currentFov, SettingsContainer.Instance.SpaceShipSettings.ChangeFovSpeed);
            var velocity = _cameraOrbit.transform.TransformDirection(Vector3.forward) * _shipSpeed;
            _rb.velocity = velocity * Time.deltaTime;

            if (!Input.GetKey(KeyCode.C))
            {
                var targetRotation = Quaternion.LookRotation(Quaternion.AngleAxis(_cameraOrbit.LookAngle, -transform.right) * velocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            }

            if (isServer) //
            {
                SendToServer();
            }
            else //
            {
                CmdSendTransform(transform.position, transform.rotation.eulerAngles);
            }
        }

        protected override void FromServerUpdate() //
        {
            transform.position = Vector3.SmoothDamp(transform.position, _serverPosition, ref currentPositionSmoothVelocity, _speed);
            transform.rotation = Quaternion.Euler(_serverEuler);
        }

        protected override void SendToServer() //
        {
            _serverPosition = transform.position;
            _serverEuler = transform.eulerAngles;
        }

        [Command]
        private void CmdSendTransform(Vector3 position, Vector3 eulers) //
        {
            _serverPosition = position;
            _serverEuler = eulers;
        }


        //protected override void FromServerUpdate() { }

        //protected override void SendToServer() { }

        [ClientCallback]
        private void LateUpdate()
        {
            _cameraOrbit?.CameraMovement();
            gameObject.name = _playerName;
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlanetOrbit>() || other.GetComponent<ShipController>())
            {
                RpcChangePosition();
            }                
        }

        [ClientRpc]
        public Vector3 RpcChangePosition()
        {
            NetworkServer.SetClientNotReady(connectionToClient);
            gameObject.SetActive(false);
            transform.position = startPosition;
            gameObject.SetActive(true);
            NetworkServer.SetClientReady(connectionToClient);
            return startPosition;
        }      

    }
}

