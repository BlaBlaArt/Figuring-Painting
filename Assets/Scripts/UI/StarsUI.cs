using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TFPlay.UI
{
	public class StarsUI : MonoBehaviour
	{
		[SerializeField] private List<Image> stars;
		[SerializeField] private Sprite starIcon;
		[SerializeField] private Sprite starBackgroundIcon;

		public void SetCount(int count)
		{
			for (var i = 0; i < stars.Count; i++)
				stars[i].sprite = i < count ? starIcon : starBackgroundIcon;
		}
	}
}