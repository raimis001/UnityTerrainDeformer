using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cannon : MonoBehaviour
{

    [Header("Shoot")]
    [SerializeField]
    Transform ballPosition;
    [SerializeField]
    LineRenderer line;
    [SerializeField]
    Vector2 lineLimit = new Vector2(1, 15);
    [SerializeField]
    Vector2 forceLimits = new Vector2(10, 50);
    float force;
    float forceNormal = 1;

    [SerializeField]
    Transform barrel;
    [SerializeField]
    Vector2 angleLimits = new Vector2(0, -60);
    float angle;
    float angleNormal = 0.75f;

    [Header("Camera")]
    [SerializeField]    
    CinemachineVirtualCamera vCamera;


    [Header("Actions")]
    [SerializeField]
    InputAction clickAction;
    [SerializeField]
    InputAction actonPosition;
    [SerializeField]
    LayerMask uiLayer;

    [SerializeField]
    Transform victoryPanel;

    bool isVictory  = false;

    private void OnEnable()
    {
        clickAction.Enable();
        actonPosition.Enable();

        Enemy.OnVictory += OnVictory;
    }

    private void Update()
    {
        if (isVictory)
            return;

        if (clickAction.triggered)
        {
            Vector2 mouse = actonPosition.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mouse);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, uiLayer))
            {
                if (hit.collider.CompareTag("Shoot"))
                    Shoot();
            }
            return;
        }

        if (clickAction.IsPressed())
        {
            Vector2 mouse = actonPosition.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(mouse);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, uiLayer))
            {
                //TODO change force
                if (hit.collider.CompareTag("Force"))
                {
                    if (hit.collider.name == "Up")
                        forceNormal += Time.deltaTime;
                    if (hit.collider.name == "Down")
                        forceNormal -= Time.deltaTime;

                    forceNormal = Mathf.Clamp(forceNormal, 0, 1);

                }


                //TODO change angle
                if (hit.collider.CompareTag("Angle"))
                {
                    if (hit.collider.name == "Up")
                        angleNormal += Time.deltaTime;
                    if (hit.collider.name == "Down")
                        angleNormal -= Time.deltaTime;

                    angleNormal = Mathf.Clamp(angleNormal, 0, 1);

                }
            }
        }

        force = Mathf.Lerp(forceLimits.x, forceLimits.y, forceNormal);
        line.SetPosition(line.positionCount - 1, new Vector3(0, 0, Mathf.Lerp(lineLimit.x, lineLimit.y, forceNormal)));

        angle = Mathf.Lerp(angleLimits.x, angleLimits.y, angleNormal);
        Vector3 euler = barrel.localEulerAngles;
        euler.x = angle;
        barrel.localEulerAngles = euler;
    }

    void Shoot()
    {
        GameObject g = Pool.GetPoolObject("CannonBall", ballPosition);


        Rigidbody body = g.GetComponent<Rigidbody>();

        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;


        body.AddForce(ballPosition.forward * body.mass * force, ForceMode.Impulse);

        vCamera.Follow = g.transform;
        vCamera.LookAt = g.transform;

        StartCoroutine(BallFollow(g));
    }

    IEnumerator BallFollow(GameObject g)
    {
        while (g.activeInHierarchy)
            yield return null;

        yield return new WaitForSeconds(3);

        vCamera.Follow = ballPosition;
        vCamera.LookAt = ballPosition;

    }


    void OnVictory()
    {
        StopAllCoroutines();

        isVictory = true;
        victoryPanel.gameObject.SetActive(true);

        vCamera.Follow = victoryPanel;
        vCamera.LookAt = victoryPanel;
    }
}
