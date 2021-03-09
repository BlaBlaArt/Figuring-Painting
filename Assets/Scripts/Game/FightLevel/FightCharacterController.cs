using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FightCharacterController : MonoBehaviour
{
    public bool IsHero;

    [SerializeField] private GameObject myEnemy;

    private CharacterData myData;
    private Rigidbody rigidbody;

    [Space] private float speed;
    private float delayBetweenAttacks;
    private float distanceToStartAttack;

    private void Start()
    {
        myData = GetComponent<CharacterData>();
        rigidbody = GetComponent<Rigidbody>();
        speed = myData.Speed;
        delayBetweenAttacks = myData.DelayBetweenAttacks;
        distanceToStartAttack = myData.DistanceToStartAttack;
    }

    public void StartFight()
    {
        CalculateDistToEnemys();
        DisableAllExtraComponents();

        FightControl();
    }

    private void DisableAllExtraComponents()
    {
        GetComponent<DragObject>().enabled = false;
    }

    private void CalculateDistToEnemys()
    {
        List<GameObject> Enemys = new List<GameObject>();

        if (IsHero)
        {
            for (int i = 0; i < FightController.Instance.Enemys.Length; i++)
            {
                Enemys.Add(FightController.Instance.Enemys[i]);
            }
        }
        else
        {
            for (int i = 0; i < FightController.Instance.Characters.Count; i++)
            {
                Enemys.Add(FightController.Instance.Characters[i]);
            }
        }

        myEnemy = Enemys.OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position))
            .FirstOrDefault();
    }

    private void FightControl()
    {
        StartCoroutine(WalkController());
    }

    private IEnumerator WalkController()
    {    
        rigidbody.isKinematic = false;

      
        while (Vector3.Distance(transform.position, myEnemy.transform.position) >= distanceToStartAttack + transform.localScale.x/2)
        {
            var normalized = (myEnemy.transform.position - transform.position).normalized;
        
            var dir = new Vector3(normalized.x, 0, normalized.z);

            rigidbody.velocity = dir / speed;
            
            transform.LookAt(new Vector3(myEnemy.transform.position.x, transform.position.y, myEnemy.transform.position.z), Vector3.up);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitUntil(() =>
                 Vector3.Distance(transform.position, myEnemy.transform.position) <= distanceToStartAttack + transform.localScale.x/2);

        rigidbody.velocity = Vector3.zero;
        rigidbody.isKinematic = true;

        
        Debug.Log("walkEnd" + Vector3.Distance(transform.position, myEnemy.transform.position));

    }
        //   var walk = StartCoroutine(Walk());

        //float time = 0;
        //while (time < 1f)
        //{
        //    transform.position = Vector3.Lerp(transform.position, myEnemy.transform.position, time);
        //    time += Time.deltaTime / speed;
        //    yield return null;
        //    
        //    if (Vector3.Distance(transform.position, myEnemy.transform.position) < distanceToStartAttack)
        //    {
        //        yield break;
        //    }
        //}

        // while (Vector3.Distance(transform.position, myEnemy.transform.position) >= distanceToStartAttack)
        // {
        //     Debug.Log("walk");
        //     var vec = (myEnemy.transform.position - transform.position).normalized;
        //   // rigidbody.velocity = new Vector3(vec.x, 0, vec.z);
        //     //transform.Translate(new Vector3(vec.x, transform.position.y, vec.z));
        //     transform.Translate(vec);
        //     
        //     Debug.DrawRay(transform.position, vec * 20, Color.green);
        //     Debug.Break();
        //    //
        //     yield return new WaitForEndOfFrame();
        // }

        // yield return new WaitUntil(() =>
        //     Vector3.Distance(transform.position, myEnemy.transform.position) <= distanceToStartAttack);



        // rigidbody.velocity = Vector3.zero;
        // StopCoroutine(walk);
    

// private IEnumerator Walk()
    // {
    //   
    // }

}
