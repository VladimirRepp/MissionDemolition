using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sligshot : MonoBehaviour
{
    static private Sligshot S;

    [Header("Settings")]
    public GameObject prefabProjectile;
    public float velocityMult = 8f;

    [Header("Parameters")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    private float maxMagnitude;
    public Rigidbody projectileRigidbody;

    private void Awake()
    {
        S = this;

        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);

        launchPos = launchPointTrans.position;
        maxMagnitude = this.GetComponent<SphereCollider>().radius;
    }

    private void Update()
    {
        if (!aimingMode)
            return;

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        Vector3 mouseDelta = mousePos3D - launchPos;
        if(mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        if (Input.GetMouseButtonUp(0))
        {
            aimingMode = false;
            projectileRigidbody.isKinematic = false;
            projectileRigidbody.velocity = -mouseDelta * velocityMult;
            FollowCam.POI = projectile;
            projectile = null;
            MissionDemolition.ShotFired();
            ProjectileLine.S.poi = projectile;
        }
    }

    private void OnMouseEnter()
    {
        launchPoint.SetActive(true);
    }

    private void OnMouseExit()
    {
        launchPoint.SetActive(false);
    }

    private void OnMouseDown()
    {
        aimingMode = true;
        projectile = Instantiate(prefabProjectile) as GameObject;
        projectile.transform.position = launchPos;
        projectileRigidbody = projectile.GetComponent<Rigidbody>();
        projectileRigidbody.isKinematic = true;
    }

    static public Vector3 LAUNCH_POS
    {
        get
        {
            if(S == null)
            {
                return Vector3.zero;
            }

            return S.launchPos;
        }
    } 
}
