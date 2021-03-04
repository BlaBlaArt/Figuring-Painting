using UnityEngine;

namespace TFPlay.UI
{
	public abstract class TutorialItem : MonoBehaviour
	{
		public virtual void Play()
		{
			gameObject.SetActive();
		}
	}
}