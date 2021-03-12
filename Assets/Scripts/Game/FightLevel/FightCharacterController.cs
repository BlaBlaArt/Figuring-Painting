using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FightCharacterController : MonoBehaviour
{
    public bool IsHero;
    
    [SerializeField] private GameObject myEnemy;
    
    private IAnimationController myAnimatorController;
    
    private CharacterData myData;
    private Rigidbody rigidbody;

    private CharacterClass myClass;
    
    [Space] 
    private float speed;
    private float delayBetweenAttacks;
    private float distanceToStartAttack;
    private float attackTime;

    private GameObject bulletPref;
    
    public int Health;
    private int dammage;

    private void Start()
    {
        myAnimatorController = GetComponentInChildren<IAnimationController>();
        myData = GetComponent<CharacterData>();
        rigidbody = GetComponent<Rigidbody>();
        bulletPref = myData.Bullet;
        myClass = myData.myCharacterClass;
        Health = myData.Health;
        dammage = myData.Dammage;
        speed = myData.Speed;
        delayBetweenAttacks = myData.DelayBetweenAttacks;
        attackTime = myData.AttackTime;
        distanceToStartAttack = myData.DistanceToStartAttack;
        GameC.Instance.OnLevelEnd += OnLevelEnd;
    }
    
    
    private void OnLevelEnd(bool obj)
    {
        myAnimatorController.OnWin();
    }

    private void OnDestroy()
    {
        WinCheck();
        GameC.Instance.OnLevelEnd -= OnLevelEnd;
        StopAllCoroutines();
    }

    public void StartFight()
    {
        CalculateDistToEnemys();
        if (myEnemy != null)
        {
            DisableAllExtraComponents();
            FightControl();
        }

    }

    private void WinCheck()
    {
        if (FightController.Instance.Enemys.Count == 0 && FightController.Instance.Characters.Count == 0)
        {
            LevelEnd(false);
        }
        else if (FightController.Instance.Enemys.Count > FightController.Instance.Characters.Count)
        {
            LevelEnd(false);
        }
        else
        {
            LevelEnd(true);
        }
 
    }
    
    private void ContinueFightCheck()
    {
        if (FightController.Instance.Enemys.Count > 0 && FightController.Instance.Characters.Count > 0)
        {
            StartFight();
        }
        else
        {
           WinCheck();
        }
    }

    private void LevelEnd(bool isWin)
    {
        GameC.Instance.LevelEnd(isWin);
    }
    
    private void DisableAllExtraComponents()
    {
        Destroy(GetComponent<DragObject>());
    }

    private void CalculateDistToEnemys()
    {
        List<GameObject> Enemys = new List<GameObject>();

        if (IsHero)
        {
            for (int i = 0; i < FightController.Instance.Enemys.Count; i++)
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

    public void TakeDammage(int dammage)
    {
            Health -= dammage;
            if (Health <= 0)
            {
                if (IsHero)
                {
                    FightController.Instance.Characters.Remove(gameObject);
                }
                else
                {
                    FightController.Instance.Enemys.Remove(gameObject);
                }

                Destroy(gameObject);
            }
    }
    
    private void FightControl()
    {
        StartCoroutine(WalkController());
    }

    private IEnumerator WalkController()
    {    
        rigidbody.isKinematic = false;

        myAnimatorController.Run();

        while (myEnemy != null && Vector3.Distance(transform.position, myEnemy.transform.position) >=
            distanceToStartAttack + transform.localScale.x / 2)
        {

            var normalized = (myEnemy.transform.position - transform.position).normalized;

            var dir = new Vector3(normalized.x, 0, normalized.z);

            rigidbody.velocity = dir / speed;

            transform.LookAt(
                new Vector3(myEnemy.transform.position.x, transform.position.y, myEnemy.transform.position.z),
                Vector3.up);
            yield return new WaitForFixedUpdate();
        }
        
        if(myEnemy == null)
        {
            myAnimatorController.Idle();
            rigidbody.velocity = Vector3.zero;
            StartFight();
            yield break;
        }

        yield return new WaitUntil(() =>
                 Vector3.Distance(transform.position, myEnemy.transform.position) <= distanceToStartAttack + transform.localScale.x/2);

        myAnimatorController.Idle();
        
        rigidbody.velocity = Vector3.zero;

        StartCoroutine(Fighting());
    }

    private IEnumerator Fighting()
    {
        var enemyController = myEnemy.GetComponent<FightCharacterController>();
        int enemyHealth = enemyController.Health;
       
        while (enemyHealth >= 0)
        {
            if (myEnemy != null)
            {
                yield return Attack();
                if(myEnemy != null)
                    enemyController.TakeDammage(dammage);
                else
                    break;
                
                enemyHealth = enemyController.Health;
                yield return new WaitForSeconds(delayBetweenAttacks);
            }
            else
            {
                break;
            }
        }
        
        StopAllCoroutines();
        ContinueFightCheck();
    }

    private IEnumerator Attack()
    {
        myAnimatorController.Attack();
        
        switch (myClass)
        {
            case CharacterClass.Archer:
            {
                
                var tmpBullet = Instantiate(bulletPref);
                FightController.Instance.TmpObjects.Add(tmpBullet);
                tmpBullet.transform.position = transform.position;
                tmpBullet.transform.LookAt(myEnemy.transform);
                tmpBullet.transform.DOMove(myEnemy.transform.position, attackTime).SetEase(Ease.Linear);
                break;
            }
            case CharacterClass.Warior:
            {

                break;
            }
            case CharacterClass.Wizard:
            {
                var tmpBullet = Instantiate(bulletPref);
                FightController.Instance.TmpObjects.Add(tmpBullet);
                tmpBullet.transform.position = transform.position;
                tmpBullet.transform.LookAt(myEnemy.transform);
                tmpBullet.transform.DOMove(myEnemy.transform.position, attackTime).SetEase(Ease.Linear);
                break;
            }
            case CharacterClass.Shield:
            {
                break;
            }
        }
        
        yield return new WaitForSeconds(attackTime);
    }


    
}
