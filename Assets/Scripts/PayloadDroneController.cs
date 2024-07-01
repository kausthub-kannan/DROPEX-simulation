using System.Collections;
using UnityEngine;

public class PayloadDroneController : MonoBehaviour
{
    public Animator anim;
    Vector3 _speed = new Vector3(0.0f, 0.0f, 0.0f);
    public float speedMultiplayer;

    enum DroneState
    {
        DroneStateIdle,
        DroneStateStartTakingoff,
        DroneStateTakingoff,
        DroneStateMovingUp,
        DroneStateFlying,
        DroneStateStartLanding,
        DroneStateLanding,
        DroneStateLanded,
    }

    DroneState _state;
    public GameObject sphere;

    // Initialise Drone State as Idle at the Start
    void Start()
    {
        _state = DroneState.DroneStateIdle;
        sphere.transform.parent = this.transform;
        sphere.transform.localPosition = new Vector3(0, -1, 0);
    }

    public bool IsIdle()
    {
        return (_state == DroneState.DroneStateIdle);
    }

    public bool IsFlying()
    {
        return (_state == DroneState.DroneStateFlying);
    }

    public void TakeOff()
    {
        _state = DroneState.DroneStateStartTakingoff;
    }

    public void Land()
    {
        _state = DroneState.DroneStateStartLanding;
    }

    // Initialise Drone Movement Speed
    public void Move(float posX, float posY, float posZ)
    {
        Vector3 targetPosition = new Vector3(posX, posY, posZ);
        StartCoroutine(MoveToPosition(targetPosition, 5));
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            UpdateDrone();
            yield return null;
        }

        sphere.transform.parent = null;
        Rigidbody rb = sphere.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = sphere.AddComponent<Rigidbody>();
        }
        rb.useGravity = true;
    }

        // Custom Wait method
    IEnumerator Wait(string task, float time)
    {
        yield return new WaitForSeconds(time);
        anim.SetBool(task, false);
    }

    // Update the Drone State for Animation
    void UpdateDrone()
    {
        switch (_state)
        {
            case DroneState.DroneStateIdle:
                break;

            case DroneState.DroneStateStartTakingoff:
                anim.SetBool("TakeOff", true);
                _state = DroneState.DroneStateTakingoff;
                break;

            case DroneState.DroneStateTakingoff:
                StartCoroutine(Wait("TakeOff", 1.0f));
                if (anim.GetBool("TakeOff") == false)
                {
                    _state = DroneState.DroneStateMovingUp;
                }
                break;

            case DroneState.DroneStateMovingUp:
                if (anim.GetBool("MoveUp") == false)
                {
                    _state = DroneState.DroneStateFlying;
                }
                break;

            case DroneState.DroneStateFlying:
                float angleZ = -30.0f * _speed.x * 60 * Time.deltaTime;
                float angleX = -30.0f * _speed.z * 60 * Time.deltaTime;

                Vector3 rotation = transform.localRotation.eulerAngles;
                transform.localPosition += _speed * (speedMultiplayer * Time.deltaTime);

                transform.localRotation = Quaternion.Euler(angleX, rotation.y, angleZ);
                break;

            case DroneState.DroneStateStartLanding:
                anim.SetBool("MoveDown", true);
                _state = DroneState.DroneStateLanding;
                break;

            case DroneState.DroneStateLanding:
                StartCoroutine(Wait("MoveDown", 2.0f));
                if (anim.GetBool("MoveDown") == false)
                {
                    _state = DroneState.DroneStateLanded;
                }
                break;

            case DroneState.DroneStateLanded:
                anim.SetBool("Land", true);
                StartCoroutine(Wait("Land", 1.1f));
                _state = DroneState.DroneStateIdle;
                break;
        }


    }
}
