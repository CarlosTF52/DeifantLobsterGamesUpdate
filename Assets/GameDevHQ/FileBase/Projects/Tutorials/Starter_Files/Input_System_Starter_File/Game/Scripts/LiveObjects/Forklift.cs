using System;
using UnityEngine;
using Cinemachine;

namespace Game.Scripts.LiveObjects
{
    public class Forklift : MonoBehaviour
    {
        [SerializeField]
        private GameObject _lift, _steeringWheel, _leftWheel, _rightWheel, _rearWheels;
        [SerializeField]
        private Vector3 _liftLowerLimit, _liftUpperLimit;
        [SerializeField]
        private float _speed = 5f, _liftSpeed = 1f;
        [SerializeField]
        private CinemachineVirtualCamera _forkliftCam;
        [SerializeField]
        private GameObject _driverModel;
        [SerializeField]
        private bool _inDriveMode = false;
        [SerializeField]
        private InteractableZone _interactableZone;

        [SerializeField]
        private PlayerManager _player;

        public static event Action onDriveModeEntered;
        public static event Action onDriveModeExited;

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += EnterDriveMode;
        }

        private void EnterDriveMode(InteractableZone zone)
        {
            if (_inDriveMode !=true && zone.GetZoneID() == 5) //Enter ForkLift
            {
                _inDriveMode = true;
                _forkliftCam.Priority = 11;
                onDriveModeEntered?.Invoke();
                _driverModel.SetActive(true);
                _interactableZone.CompleteTask(5);
            }
        }

        public void ExitDriveMode()
        {
            _inDriveMode = false;
            _forkliftCam.Priority = 9;            
            _driverModel.SetActive(false);
            onDriveModeExited?.Invoke();
        
            
        }

        private void Update()
        {
            if (_inDriveMode == true)
            {
                _player.InitiliazeLiftInputs();
          
            }

        }

        public void CalcutateMovement(float move, float rot)
        {

            //var direction = new Vector3(0, 0, v)

            var direction = transform.forward * move;
            var velocity = direction * _speed;

            transform.Translate(velocity * Time.deltaTime);

            transform.Rotate(transform.up, rot);
        }



        public void LiftUpRoutine(float raised)
        {

                if (_lift.transform.localPosition.y < _liftUpperLimit.y)
                {
                    Vector3 tempPos = _lift.transform.localPosition;
                    tempPos.y += Time.deltaTime * _liftSpeed * raised;
                    _lift.transform.localPosition = new Vector3(tempPos.x, tempPos.y , tempPos.z);
                }
                else if (_lift.transform.localPosition.y >= _liftUpperLimit.y)
                    _lift.transform.localPosition = _liftUpperLimit;
            }


        public void LiftDownRoutine(float lower)
        {
            if (_lift.transform.localPosition.y > _liftLowerLimit.y)
            {
                Vector3 tempPos = _lift.transform.localPosition;
                tempPos.y -= Time.deltaTime * _liftSpeed * lower;
                _lift.transform.localPosition = new Vector3(tempPos.x, tempPos.y, tempPos.z);
            }
            else if (_lift.transform.localPosition.y <= _liftUpperLimit.y)
                _lift.transform.localPosition = _liftLowerLimit;
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= EnterDriveMode;
        }

    }
}