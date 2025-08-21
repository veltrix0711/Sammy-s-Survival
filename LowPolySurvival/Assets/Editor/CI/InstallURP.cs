using System.Collections;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public static class InstallURP
{
	static AddRequest _add;
	public static void Run()
	{
		EditorApplication.update += Tick;
		_add = Client.Add("com.unity.render-pipelines.universal");
	}
	static void Tick()
	{
		if (_add == null) return;
		if (!_add.IsCompleted) return;
		EditorApplication.update -= Tick;
		if (_add.Status == StatusCode.Success)
		{
			Debug.Log("URP installed: " + _add.Result.version);
			EditorApplication.Exit(0);
		}
		else
		{
			Debug.LogError("URP install failed: " + _add.Error.message);
			EditorApplication.Exit(1);
		}
	}
}
