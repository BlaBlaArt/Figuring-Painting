using UnityEngine;

public class CharacterData : MonoBehaviour
{
    [Header("DATA")]
    public CharacterClass myCharacterClass;
    public int CharacterNum;
    public int CharacterLevel;
    public GameObject Bullet;

    [Header("FIGHT")]
    public float Speed;
    public float DelayBetweenAttacks;
    public float AttackTime;
    public int Dammage;
    public float DistanceToStartAttack;

    [Header("STATS")] 
    public int Health;
    

    private void Start()
    {
        OnLevelChange();
    }

    public void OnLevelChange()
    {
        switch (CharacterLevel)
        {
            case 0:
            {
                break;
            }
            case 1:
            {
                transform.localScale += transform.localScale;
                break;
            }
        }
    }
}

public enum CharacterClass
{
    Warior,
    Archer
}
