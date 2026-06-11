using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class Shooting : NetworkBehaviour
{
    [SerializeField]
    private InputActionReference pointerPosition;
    public SpriteRenderer characterRenderer, weaponRenderer;
    public bool isFiring = false;


    public GameObject bullet;
    public Transform bulletTransform;
    public bool canFire = true;
    private float timer;
    public float timeBetweenFiring;

    public Quaternion gunRotation;
    public Vector2 direction;
    private float safeState1;
    private float safeState2;

    private double _nextFireTime;

    void Start()
    {
        safeState1 = transform.localScale.y;
        safeState2 = -1 * transform.localScale.y;
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            Aim();
            ShouldShoot();
        }
        //if(isServer)
        //    ShouldShoot();
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isFiring = true;
        }
        if (context.canceled)
        {
            isFiring = false;
        }
    }

    private void ShouldShoot()
    {
        if (canFire && isFiring)
        {
            CmdFire(direction, gunRotation, bulletTransform.position);
            canFire = false;
            timer = 0;
        }
        else if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }
    }

    private void Aim()
    {
        Vector2 pointerPosition = GetPointerInput();
        direction = (pointerPosition - (Vector2)transform.position).normalized;
        transform.right = direction;
        gunRotation = transform.rotation;

        Vector2 scale = transform.localScale;
        if (gunRotation.y == 1)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (direction.x <= 0)
        {
            scale.y = safeState2;
        }
        else if (direction.x > 0)
        {
            scale.y = safeState1;
        }
        transform.localScale = scale;

        if (transform.eulerAngles.z > 0 && transform.eulerAngles.z < 180)
        {
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder - 2;
        }
        else
        {
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder + 2;
        }
    }

    private Vector2 GetPointerInput()
    {
        Vector3 mousePos = pointerPosition.action.ReadValue<Vector2>();
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    [Command]
    private void CmdFire(Vector2 dir, Quaternion rot, Vector3 spawnPos)
    {
        //if (NetworkTime.time < _nextFireTime) return;
        //_nextFireTime = NetworkTime.time + timeBetweenFiring;

        GameObject go = null;
        if (ObjectPool.instance != null)
        {
            go = ObjectPool.instance.GetServerObject();
            if (go != null)
            {
                go.transform.SetPositionAndRotation(spawnPos, rot * Quaternion.Euler(0, 0, -90));
                go.SetActive(true);
                NetworkServer.Spawn(go);
            }
        }

        if (go == null)
        {
            go = Instantiate(bullet, spawnPos, rot * Quaternion.Euler(0, 0, -90));
            NetworkServer.Spawn(go);
        }

        var bulletScript = go.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.LaunchServer(dir, rot, spawnPos);
            bulletScript.RpcLaunch(dir, rot, spawnPos);
        }
    }
}