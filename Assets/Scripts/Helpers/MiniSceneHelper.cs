using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MiniSceneHelper : MonoBehaviour
{
	[SerializeField] private Transform[] CameraPositions;

	public void Start()
	{
		gameObject.SetActive(false);
	}

	public void StartScene()
	{
		gameObject.SetActive(true);
	}

	public void StopScene()
	{
		gameObject.SetActive(false);
	}

	public void SetCamera(int position = 0, float time = 0, UnityAction callback = null)
	{
		var tr = CameraPositions[position];
		Camera.main.transform.parent = tr;
		if (time > 0)
		{
			StartCoroutine(SetCameraCor(tr, time, callback));
		}
		else
		{
			Camera.main.transform.localPosition = Vector3.zero;
			Camera.main.transform.localRotation = Quaternion.identity;
			callback?.Invoke();
		}
	}

	private IEnumerator SetCameraCor(Transform tr, float time, UnityAction callback)
	{
		Vector3 startPos = Camera.main.transform.localPosition;
		Vector3 endPos = Vector3.zero;
		Quaternion startRot = Camera.main.transform.localRotation;
		Quaternion endRot = Quaternion.identity;

		for (float t = 0; t < 1; t += Time.deltaTime / time)
		{
			Camera.main.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
			Camera.main.transform.localRotation = Quaternion.Lerp(startRot, endRot, t);
			yield return null;
		}

		Camera.main.transform.localPosition = endPos;
		Camera.main.transform.localRotation = endRot;
		//Debug.Log("Before callback");
		callback?.Invoke();
	}
}