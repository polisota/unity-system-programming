using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public abstract class FireAction : NetworkBehaviour //
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int startAmmunition = 20;
    [SerializeField] private int damage = 20; //

    protected string countBullet = string.Empty;
    protected Queue<GameObject> bullets = new Queue<GameObject>();
    protected Queue<GameObject> ammunition = new Queue<GameObject>();
    protected bool reloading = false;

    protected virtual void Start()
    {
        for (var i = 0; i < startAmmunition; i++)
        {
            GameObject bullet;
            if (bulletPrefab == null)
            {
                bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                bullet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            else
            {
                bullet = Instantiate(bulletPrefab);
            }
            bullet.SetActive(false);
            ammunition.Enqueue(bullet);
        }
    }

    public virtual async void Reloading()
    {
        bullets = await Reload();
    }

    protected virtual void Shooting()
    {
        if (bullets.Count == 0)
        {
            Reloading();
        }
    }

    [Command]
    protected void CmdShooting(Vector3 start, Vector3 direction) //
    {
        var ray = new Ray(start, direction); //

        if (Physics.Raycast(ray, out var hit)) //
        {
            if (hit.collider.TryGetComponent<PlayerCharacter>(out var playerCharacter)) //
            {
                playerCharacter.RpcHit(damage); //
            }
        }

    }
    private async Task<Queue<GameObject>> Reload()
    {
        if (!reloading)
        {
            reloading = true;
            StartCoroutine(ReloadingAnim());
            return await Task.Run(delegate
            {
                var cage = 10;

                if (bullets.Count < cage)
                {
                    Thread.Sleep(3000);
                    var bullets = this.bullets;

                    while (bullets.Count > 0)
                    {
                        ammunition.Enqueue(bullets.Dequeue());
                    }
                    cage = Mathf.Min(cage, ammunition.Count);

                    if (cage > 0)
                    {
                        for (var i = 0; i < cage; i++)
                        {
                            var sphere = ammunition.Dequeue();
                            bullets.Enqueue(sphere);
                        }
                    }
                }
                reloading = false;
                return bullets;
            });
        }
        else
        {
            return bullets;
        }
    }

    private IEnumerator ReloadingAnim()
    {
        while (reloading)
        {
            RayShooter.bulletCount = " | "; //
            yield return new WaitForSeconds(0.01f);
            RayShooter.bulletCount = @" \ "; //
            yield return new WaitForSeconds(0.01f);
            RayShooter.bulletCount = "---"; //
            yield return new WaitForSeconds(0.01f);
            RayShooter.bulletCount = " / "; //
            yield return new WaitForSeconds(0.01f);
        }
        RayShooter.bulletCount = bullets.Count.ToString(); //
        yield return null;
    }
}

