using System;
using System.Collections.Generic;

public class Pool<T> where T : class
{
	private readonly Func<T> _customCreationFunc;

	private readonly Stack<T> _objectStack;

	private readonly Action<T> _resetAction;
	private readonly Action<T> _onetimeInitAction;

	// provide a customCreationFunc if you want to Instantiate prefab
	public Pool(int initialBufferSize, Func<T> customCreationFunc = null, Action<T> ResetAction = null, Action<T> OnetimeInitAction = null)
	{
		_customCreationFunc = customCreationFunc;
		_objectStack = new Stack<T>(initialBufferSize);
		_resetAction = ResetAction;
		_onetimeInitAction = OnetimeInitAction;
	}

	public T Create()
	{
		T result;
		if (_objectStack.Count > 0)
		{
			result = _objectStack.Pop();
		}
		else
		{
			if (_customCreationFunc != null)
				result = _customCreationFunc();
			else
				result = default(T);

			if (_onetimeInitAction != null)
				_onetimeInitAction(result);
		}

		return result;
	}

	public void Store(T obj)
	{
		if (_resetAction != null)
			_resetAction(obj);

		_objectStack.Push(obj);
	}

	// this method for debug only
	public int GetSize() => _objectStack.Count;
}