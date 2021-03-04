using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationTester_Animator : MonoBehaviour
{
	[SerializeField] private Animator animator;

	public void Init()
	{
		if (animator == null)
		{
			animator = GetComponent<Animator>();
		}
	}

	public void Play()
	{
		animator.SetTrigger("Play");
	}

#if UNITY_EDITOR

	[ContextMenu("Play Animation")]
	private void PlayAnimation()
	{
		Play();
	}

#endif
}