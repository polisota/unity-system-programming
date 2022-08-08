using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour //Task 1
{
    [SerializeField] private int health = 0;
    [SerializeField] private int healthMax = 100;
    [SerializeField] private int healStep = 5;
    [SerializeField] private float time = 3;
    [SerializeField] private float waitTime = 0.5f;

    private bool isHealing;
    private float curTime;

    private void Start()
    {
        ReceiveHealing();
    }

    public void ReceiveHealing()
    {
        if (!isHealing)
        {
            isHealing = true;
            StartCoroutine(HealingCoroutine());
        }        
    }

    private IEnumerator HealingCoroutine()
    {
        curTime = 0.0f;

        while (true)
        {
            if (health >= healthMax || curTime >= time)
            {
                isHealing = false;
                yield break;
            }
            else
            {
                curTime += waitTime;
                health += Mathf.Min(healStep, healthMax - health);
                yield return new WaitForSeconds(waitTime);
            }            
        }            
    }
}
