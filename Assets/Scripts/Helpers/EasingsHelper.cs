using UnityEngine;

public enum EaseType
{
	easeNone,
	easeInOutSine,
	easeOutBounce,
	easeOutElastic,
}

public static class EasingsHelper
{
	public static float EaseValue(float x, EaseType ease)
	{
		switch (ease)
		{
			case EaseType.easeNone:
				return x;
			case EaseType.easeOutElastic:
				return easeOutElastic(x);
			case EaseType.easeOutBounce:
				return easeOutBounce(x);
			case EaseType.easeInOutSine:
				return easeInOutSine(x);
			default:
				return x;
		}
	}

	private static float easeInOutSine(float x)
	{
		return -(Mathf.Cos(Mathf.PI * x) - 1f) / 2f;
	}

	public static float easeOutElastic(float x)
	{
		const float c4 = (2 * Mathf.PI) / 3f;
		return Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10f - 0.75f) * c4) + 1f;
	}

	public static float easeOutBounce(float x)
	{
		const float n1 = 7.5625f;
		const float d1 = 2.75f;
		if (x < 1 / d1)
		{
			return n1 * x * x;
		}
		else if (x < 2 / d1)
		{
			return n1 * (x -= 1.5f / d1) * x + 0.75f;
		}
		else if (x < 2.5 / d1)
		{
			return n1 * (x -= 2.25f / d1) * x + 0.9375f;
		}
		else
		{
			return n1 * (x -= 2.625f / d1) * x + 0.984375f;
		}
	}
}