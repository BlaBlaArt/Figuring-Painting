using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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
    [Space] [SerializeField] private ParticleSystem vfxOnMe;
    [SerializeField] private GameObject vfxOnEnenmy;

    private GameObject bulletPref;
    
    public int Health;
    private int dammage;

    public bool isDead;

    private void Start()
    {
        isDead = false;
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
        Taptic.Medium();
        GameC.Instance.OnLevelEnd -= OnLevelEnd;
        StopAllCoroutines();
        FightController.Instance.StartWinCheck();
    }

    public void StartFight()
    {
        CalculateDistToEnemys();
        if (!myEnemy.GetComponent<FightCharacterController>().isDead)
        {
            DisableAllExtraComponents();
            FightControl();
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
            if (FightController.Instance.Enemys.Count > FightController.Instance.Characters.Count)
            {
                LevelEnd(false);
            }
            else
            {
                LevelEnd(true);
            }
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
        .FirstOrDefault(t => !t.GetComponent<FightCharacterController>().isDead);
    }

    public void TakeDammage(int dammage)
    {
            Health -= dammage;
            if (Health <= 0)
            {
                isDead = true;
                
                if (IsHero)
                {
                    FightController.Instance.Characters.Remove(gameObject);
                }
                else
                {
                    FightController.Instance.Enemys.Remove(gameObject);
                }

                StopAllCoroutines();
                StartCoroutine(OnDead());
            }
    }

    private IEnumerator OnDead()
    {
        myAnimatorController.OnDead();
        
        Debug.Log("End of Dead1");

        yield return new WaitForSeconds(3f);
        
        Debug.Log("End of Dead2");
        
        Destroy(gameObject);
    }
    
    private void FightControl()
    {
        StartCoroutine(WalkController());
    }

    private IEnumerator WalkController()
    {    
        rigidbody.isKinematic = false;

        myAnimatorController.Run();

        while (!myEnemy.GetComponent<FightCharacterController>().isDead && Vector3.Distance(transform.position, myEnemy.transform.position) >=
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
        
        if(myEnemy.GetComponent<FightCharacterController>().isDead)
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
            if (!enemyController.isDead)
            {
                yield return Attack();
                if(!enemyController.isDead)
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
        
        if(vfxOnMe != null)
            vfxOnMe.Play(true);

        yield return new WaitForSeconds(attackTime);

        switch (myClass)
        {
            case CharacterClass.Archer:
            {
                
                var tmpBullet = Instantiate(bulletPref);
                FightController.Instance.TmpObjects.Add(tmpBullet);
                tmpBullet.transform.position = new Vector3(transform.position.x, transform.position.y+0.1f, transform.position.z);
                tmpBullet.transform.LookAt(myEnemy.transform);
                var seq = DOTween.Sequence();

                var dist = Vector3.Distance(transform.position, myEnemy.transform.position);

                seq.Append(tmpBullet.transform
                    .DOMove(new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z),
                        delayBetweenAttacks / 4)
                    .SetEase(Ease.Linear));
                seq.Append(tmpBullet.transform.DOMove(myEnemy.transform.position, delayBetweenAttacks / 4)
                    .SetEase(Ease.InExpo));
                break;
            }
            case CharacterClass.Warior:
            {

                break;
            }
            case CharacterClass.Wizard:
            {
                var tmpBullet = Instantiate(vfxOnEnenmy);
                FightController.Instance.TmpObjects.Add(tmpBullet);
                tmpBullet.transform.position = myEnemy.transform.position;
              //  tmpBullet.transform.LookAt(myEnemy.transform);
              //  tmpBullet.transform.DOMove(myEnemy.transform.position, delayBetweenAttacks/4).SetEase(Ease.Linear);
                break;
            }
            case CharacterClass.Shield:
            {
                break;
            }
        }

        yield return new WaitForEndOfFrame();

    }


    
}
