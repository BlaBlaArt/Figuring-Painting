using UnityEngine;

public class AnimationTester_Legacy : MonoBehaviour
{
	[SerializeField] private Animation anim;
	public void Init()
	{
		if (anim == null)
		{
			anim = GetComponent<Animation>();
		}
	}

	public void Play()
	{
		anim.Play();
	}

#if UNITY_EDITOR

	[ContextMenu("Play Animation")]
	private void PlayAnimation()
	{
		Play();
	}

#endif
}