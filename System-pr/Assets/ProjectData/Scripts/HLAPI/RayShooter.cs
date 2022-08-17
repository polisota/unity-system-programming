using System.Collections;
using UnityEngine;

public class RayShooter : FireAction
{
    private Camera camera;
    public static string bulletCount { get; set; } //

    protected override void Start()
    {
        base.Start();
        camera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shooting();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reloading();
        }

        if (Input.anyKey && !Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    protected override void Shooting()
    {
        base.Shooting();

        if (bullets.Count > 0)
        {
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        if (hasAuthority) //
        {
            if (reloading)
            {
                yield break;
            }

            var point = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0);
            var ray = camera.ScreenPointToRay(point);

            if (!Physics.Raycast(ray, out var hit))
            {
                yield break;
            }

            var shoot = bullets.Dequeue();
            countBullet = bullets.Count.ToString();
            ammunition.Enqueue(shoot);
            
            if (shoot != null) //
            {
                shoot.SetActive(true);
                shoot.transform.position = hit.point;
                shoot.transform.parent = hit.transform;
            }

            CmdShooting(ray.origin, ray.direction); //

            yield return new WaitForSeconds(2.0f);           

            if (shoot != null) //
            {
                shoot.SetActive(false);
            }
        }       
    }
}

