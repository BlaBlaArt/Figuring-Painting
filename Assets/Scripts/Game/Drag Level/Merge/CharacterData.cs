using UnityEngine;

public class CharacterData : MonoBehaviour
{
    public CharacterClass myCharacterClass;
    public int CharacterNum;
    public int CharacterLevel;

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
