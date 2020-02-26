using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;//utilizamos el namespace .AI para utilizar las funcionalidades de NavMesh

namespace MovementTutorial{

    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Transform))]
    public class Movement : MonoBehaviour
    {
        #region Variables & References
        public bool keyControl;

        public LayerMask floorLayer;

        [BoxGroup("Inputs"), ShowIf("keyControl")]
        public float horizontal, vertical, moveAmount, rotationSpeedCamOriented, movementSpeed;

        private Vector3 _scaleVector = new Vector3(1f, 0f, 1f);
        private Vector3 _moveDirection, _pointMouse;

        private Transform _myTransform;
        private NavMeshAgent _myNavMeshAgent;
        private Rigidbody _myRigidBody;
        #endregion

        private void Awake()
        {
            _myTransform = GetComponent<Transform>();
            _myNavMeshAgent = GetComponent<NavMeshAgent>();
            _myRigidBody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _pointMouse = _myTransform.position;
        }

        private void Update()
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");



            if (keyControl)
                JoystickControl();
            else
                MouseControl();


            KeyController();
        }

        private void JoystickControl()
        {
            _myNavMeshAgent.isStopped = true;

            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            Vector3 camForward = Vector3.Scale(Camera.main.transform.forward, _scaleVector).normalized;

            _moveDirection = (vertical * camForward) + (horizontal * Camera.main.transform.right);
            _moveDirection.Normalize();

            Vector3 targetDirection = new Vector3(0f, 0f, 1f);

            if (moveAmount > 0.1f)
            {
                targetDirection = _moveDirection;
                Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);

                _myTransform.rotation = Quaternion.Slerp(_myTransform.rotation, targetRotation, Time.deltaTime * rotationSpeedCamOriented);
            }

            Vector3 forward = _myTransform.forward * moveAmount * movementSpeed;
            _myRigidBody.velocity = forward;
        }

        private void MouseControl()
        {
            _myNavMeshAgent.isStopped = false;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Input.GetMouseButtonDown(0))
            {
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, floorLayer))
                {
                    _pointMouse = hit.point;
                }
            }

            _myNavMeshAgent.SetDestination(_pointMouse);
        }

        private void KeyController()
        {
            if (!keyControl)
            {
                if (horizontal != 0f || vertical != 0f)
                {
                    keyControl = true;
                    Cursor.visible = false;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    keyControl = false;
                    _pointMouse = _myTransform.position;
                    Cursor.visible = true;
                }
            }
        }
    }
}