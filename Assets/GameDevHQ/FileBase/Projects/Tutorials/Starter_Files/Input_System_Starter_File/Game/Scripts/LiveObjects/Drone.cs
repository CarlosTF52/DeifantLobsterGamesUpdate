using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Game.Scripts.UI;
using UnityEngine.Animations;
using UnityEngine.Rendering;

namespace Game.Scripts.LiveObjects
{
    public class Drone : MonoBehaviour
    {
        private enum Tilt
        {
            NoTilt, Forward, Back, Left, Right
        }

        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private float _speed = 5f;
        private bool _inFlightMode = false;
        [SerializeField]
        private Animator _propAnim;
        [SerializeField]
        private CinemachineVirtualCamera _droneCam;
        [SerializeField]
        private InteractableZone _interactableZone;

        [SerializeField]
        private PlayerManager _playerManager;
        

        public static event Action OnEnterFlightMode;
        public static event Action onExitFlightmode;

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += EnterFlightMode;
            _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        }

        private void EnterFlightMode(InteractableZone zone)
        {
            if (_inFlightMode != true && zone.GetZoneID() == 4) // drone Scene
            {
                _propAnim.SetTrigger("StartProps");
                _droneCam.Priority = 11;
                _inFlightMode = true;
                OnEnterFlightMode?.Invoke();
                UIManager.Instance.DroneView(true);
                _interactableZone.CompleteTask(4);
                Debug.Log("taskComplete!");
            }
        }

        public void ExitFlightMode()
        {
            _inFlightMode = false;
            onExitFlightmode?.Invoke();
            _droneCam.Priority = 9;
            _inFlightMode = false;
            UIManager.Instance.DroneView(false);
            InteractableZone.onZoneInteractionComplete -= EnterFlightMode;
        }

        private void Update()
        {
            if (_inFlightMode)
            {
                _playerManager.InitializeDroneInputs();
                
            }
        }

    

        private void FixedUpdate()
        {
            _rigidbody.AddForce(transform.up * (9.81f), ForceMode.Acceleration);
            
        }

        public void CalculateMovementUpdate(float turn)
        {
            var tempRot = transform.localRotation.eulerAngles;
            if (turn < 0)
            {        
                tempRot.y -= _speed / 3;
                transform.localRotation = Quaternion.Euler(tempRot);
            }
            else if (turn > 0)
            {
                tempRot.y += _speed / 3;
                transform.localRotation = Quaternion.Euler(tempRot);
            }    
        }

        public void CalculateMovementFixedUpdate(float updown)
        {
            
            if (updown > 0)
            {
                _rigidbody.AddForce(transform.up * _speed, ForceMode.Acceleration);
            }
            if (updown < 0)
            {
                _rigidbody.AddForce(-transform.up * _speed, ForceMode.Acceleration);
            }
        }

        public void CalculateTilt(float move, float rot)
        {
            transform.rotation = Quaternion.Euler(move * 30, transform.localRotation.eulerAngles.y, rot * 30);
                
        }

        private void OnDisable()
        {
            
        }
    }
}
