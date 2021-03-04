using DG.Tweening;
using UnityEngine;

public class ObjectRotateControl : MonoBehaviour
{
	[SerializeField] private bool _isLoop;

	[Header("Simple rotation setting")]
	[SerializeField] private Ease _rotateEaseType = Ease.Linear;
	[SerializeField] private Vector3 _rotateValue;
	[SerializeField] private float _rotateSpeed;

	private Sequence _rotateSequence;

	private void Awake() => SimpleRotateObject();

	private void SimpleRotateObject()
	{
		_rotateSequence = DOTween.Sequence();
		_rotateSequence.Append(transform.DOBlendableRotateBy(_rotateValue, _rotateSpeed, RotateMode.FastBeyond360).SetEase(_rotateEaseType));

		if (_isLoop)
			_rotateSequence.SetLoops(-1);
	}
}