using UnityEngine;

/// <summary>
/// Используем для самооуничтожения префабов на сцене
/// </summary>
public class AutoDestroy : MonoBehaviour
{
	/// <summary>
	/// Необходимое время задержки перед удалением
	/// Должно совпадать со временем анимации
	/// </summary>
	public float sec = 1;
	void OnEnable()
	{
		Destroy(gameObject, sec);
	}
}