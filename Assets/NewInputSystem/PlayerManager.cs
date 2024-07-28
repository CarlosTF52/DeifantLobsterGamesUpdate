using Game.Scripts.LiveObjects;
using Game.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerManager : MonoBehaviour
{
    private PlayerInput _input;

    [SerializeField]
    private Player _player;

    [SerializeField]
    private Drone _drone;

    [SerializeField]
    private Crate _crate;

    [SerializeField]
    private Forklift _forklift;

    [SerializeField]
    private GameObject _forkLiftObject;

    [SerializeField]
    private GameObject _playerObj;

    [SerializeField]
    private Vector3 _forkLiftPosition;

    [SerializeField]
    private Laptop _laptop;

    public bool _interactHold;

    public bool _interact;

    [SerializeField]
    private InteractableZone _zone;


    // Start is called before the first frame update
    void Start()
    {
        InitializePlayerInputs();
        
        _drone = GameObject.Find("DroneMaster").GetComponent<Drone>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();   
        _forkLiftPosition = _forkLiftObject.transform.position;
    }

    private void InitializePlayerInputs()
    {
        _input = new PlayerInput();

        _input.PlayerActionMap.Enable();

        _input.PlayerActionMap.Interact.performed += Player_Interact_performed;

        _input.PlayerActionMap.Interact.started += Player_Interact_started;

        _input.PlayerActionMap.Interact.canceled += Player_Interact_canceled;

        _input.PlayerActionMap.CancelAction.performed += Player_CancelAction_performed;

    }

    private void PlayerMovement()
    {
        var move = _input.PlayerActionMap.Movement.ReadValue<Vector2>();
        _player.CalcutateMovement(move);
    }

    private void Player_CancelAction_performed(InputAction.CallbackContext obj)
    {
        if (_laptop != null)
        {
            _laptop.ExitCameras();
        }
    }

    private void Player_Interact_canceled(InputAction.CallbackContext obj)
    {
        _interact = false;
        _interactHold = false;
        if (_zone._zoneID == 3 && !_interactHold)
        {
            int zoneID = _zone._zoneID;
            _laptop.InteractableZone_onHoldEnded(zoneID);
        }
    }

    private void Player_Interact_started(InputAction.CallbackContext obj)
    {
        _interact = true;
        if(_zone != null )
        {
            _zone.Interact();
        }
        if(_laptop != null )
        {
            _laptop.CameraSwitch();
        }
        if(_zone._zoneID == 3) 
        {
            int zoneID = _zone._zoneID;
            _laptop.InteractableZone_onHoldStarted(zoneID);
        }        
    }

    private void Player_Interact_performed(InputAction.CallbackContext obj)
    {
        _interactHold = true;
        if(_zone != null )
        {
            _zone.HoldInteract();
            
        }
        if(_zone._zoneID == 6)
        {
            _crate.BreakParts();
        }
        
    }

    public void CantMove()
    {
        _input.PlayerActionMap.Movement.Disable();
    }

    public void CanMove()
    {
        _input.PlayerActionMap.Movement.Enable();
    }

    public void InitializeDroneInputs()
    {
        _input.PlayerActionMap.Disable();

        _input.Drone.Enable();

        _input.Drone.ExitFlight.performed += ExitFlight_performed; 

        DroneMovement();
        DroneRoation();
        DroneUpDown();
    }

    private void ExitFlight_performed(InputAction.CallbackContext obj)
    {
        _drone.ExitFlightMode();
        InitializePlayerInputs();
    }

    private void DroneMovement()
    {
        var move = _input.Drone.Movement.ReadValue<float>();
        var rot = _input.Drone.Rotation.ReadValue<float>();
        _drone.CalculateTilt(move, rot);
    }

    private void DroneRoation()
    {
        var turn = _input.Drone.Turning.ReadValue<float>();
        _drone.CalculateMovementUpdate(turn);
    }

    private void DroneUpDown()
    {
        var updown = _input.Drone.UpDown.ReadValue<float>();
        _drone.CalculateMovementFixedUpdate(updown);
    }

    public void InitiliazeLiftInputs()
    {
        _input.PlayerActionMap.Disable();

        _input.Forklift.Enable();

        _input.Forklift.ExitLift.performed += ExitLift_performed;

        LiftUp();

        LiftDown();

        LiftMovement();
    }

    private void LiftUp()
    {
        var raise = _input.Forklift.Lift.ReadValue<float>();
        _forklift.LiftUpRoutine(raise);
    }

    private void LiftDown()
    {
        var lower = _input.Forklift.Lower.ReadValue<float>();
        _forklift.LiftDownRoutine(lower);
    }

    private void ExitLift_performed(InputAction.CallbackContext obj)
    {
        _forklift.ExitDriveMode();
        InitializePlayerInputs();
        MovePlayerLift();


    }

    private void LiftMovement()
    {
        var move = _input.Forklift.Movement.ReadValue<float>();
        var rot = _input.Forklift.Rotation.ReadValue<float>();
        _forklift.CalcutateMovement(move, rot);
    }

    private void MovePlayerLift()
    {
        _playerObj.transform.position = new Vector3(_forkLiftPosition.x, _forkLiftPosition.y, _forkLiftPosition.z);
    }


    private void OnTriggerEnter(Collider other)
    {
        _zone = other.GetComponent<InteractableZone>();
       
    }

}
