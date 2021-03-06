using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
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

    [SerializeField] private GameObject myTrail;

    [SerializeField] private GameObject deathPartickles;
    [SerializeField] private GameObject dammageParticleSystem1, dammageParticleSystem2;
    
    private GameObject bulletPref;
    
    public int Health;
    private int dammage;

    public bool isDead;

    [SerializeField] private GameObject MyOutline;

    private Image myOutlineImage;
    
    private void Start()
    {
        MyOutline.SetInactive();
        myOutlineImage = MyOutline.GetComponent<Image>();
        if(myTrail != null)
            myTrail.SetInactive();
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
        
        MyOutline.SetActive();

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
            
            myOutlineImage.DOFillAmount((float)(Health - dammage) / myData.Health, 0.35f);
            TakeDammageVisuals(dammage);

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

    private void TakeDammageVisuals(int dammage)
    {
       // myOutlineImage.DOFillAmount((float)(Health - dammage) / Health, 0.25f);
                
        if (dammage < 15)
        {
            var tmpPArt = Instantiate(dammageParticleSystem1);
            tmpPArt.transform.position = transform.position;
            FightController.Instance.TmpObjects.Add(tmpPArt);
        }
        else
        {
            var tmpPArt = Instantiate(dammageParticleSystem2);
            tmpPArt.transform.position = transform.position;
            FightController.Instance.TmpObjects.Add(tmpPArt);
        }
    }

    private IEnumerator OnDead()
    {
        myAnimatorController.OnDead();
        MyOutline.SetInactive();

        
       // Debug.Log("End of Dead1");

        yield return new WaitForSeconds(3f);

        var tmpPart = Instantiate(deathPartickles);
        tmpPart.transform.position = transform.position;
        FightController.Instance.TmpObjects.Add(tmpPart);
        
        //Debug.Log("End of Dead2");

        transform.DOMoveY(transform.position.y + Vector3.down.y * 0.1f, 0.5f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
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

        if(myTrail != null)
            myTrail.SetActive();
        
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
                myTrail.SetInactive();
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
                myTrail.SetInactive();
                break;
            }
        }

        yield return new WaitForEndOfFrame();

    }


    
}
