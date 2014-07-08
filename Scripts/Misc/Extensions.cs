using UnityEngine;

public static class Extensions {

	public static I GetIComponent<I>(this Transform t) where I : class {
		return t.GetComponent(typeof(I)) as I;
	}

	public static I[] GetIComponents<I>(this Transform t) where I : class {
		return t.GetComponents(typeof(I)) as I[];
	}

	public static I GetIComponent<I>(this GameObject t) where I : class {
		return t.GetComponent(typeof(I)) as I;
	}

	public static I[] GetIComponents<I>(this GameObject t) where I : class {
		return t.GetComponents(typeof(I)) as I[];
	}

}
