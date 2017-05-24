//
//  Code by: Parth Darji
//  Company: Boundless Reality
//  (c) Boundless Reality, All rights reserved.
//
//  Details:
//
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class VRScreenFade : MonoBehaviour
{
	public delegate void OnFadeOutEnd ();
	public delegate void OnFadeInEnd ();
	public event OnFadeOutEnd onFadeOutEnd;
	public event OnFadeInEnd onFadeInEnd;

	public float fadeInTime = 2f;
	public float fadeOutTime = 1f;

	public Color fadeColor = new Color(0,0,0, 0.0f);

	private Material fadeMaterial = null;
	private bool isFading = false;
	private YieldInstruction fadeInstruction = new WaitForEndOfFrame();

	void Awake() {
		// Create the fade material
		fadeMaterial = new Material(Shader.Find("Oculus/Unlit Transparent Color"));
	}

	void OnEnable() {

	}

	void OnDestroy() {
		if (fadeMaterial != null) {
			Destroy (fadeMaterial);
		}
	}

	IEnumerator FadeIn() {
		float elapsedTime = 0f;
		fadeMaterial.color = fadeColor;
		Color color = fadeColor;
		isFading = true;
		while (elapsedTime < fadeInTime) {
			yield return fadeInstruction;
			elapsedTime += Time.deltaTime;
			color.a = 1.0f - Mathf.Clamp01 (elapsedTime / fadeInTime);
			fadeMaterial.color = color;
		}
		isFading = false;
		if (onFadeInEnd != null) {
			onFadeInEnd ();
		}
	}

	IEnumerator FadeOut() {
		float elapsedTime = 0f;
		fadeMaterial.color = fadeColor;
		Color color = fadeColor;
		isFading = true;

		while (elapsedTime < fadeOutTime) {
			yield return fadeInstruction;
			elapsedTime += Time.deltaTime;
			color.a = Mathf.Clamp01 (elapsedTime / fadeOutTime);
			fadeMaterial.color = color;
		}
		isFading = false;
		if (onFadeOutEnd != null) {
			onFadeOutEnd ();
		}
	}

	/// <summary>
	/// Helper class for accessing the FadeIn function
	/// </summary>
	public void PerformFadeIn() {
		StartCoroutine (FadeIn ());
	}

	/// <summary>
	/// Helper class for accessing the FadeOut function
	/// </summary>
	public void PerformFadeOut() {
		StartCoroutine (FadeOut ());
	}

	/// <summary>
	/// Renders the fade overlay when attached to a camera object
	/// </summary>
	void OnPostRender() {
		if (isFading) {
			fadeMaterial.SetPass (0);
			GL.PushMatrix ();
			GL.LoadOrtho ();
			GL.Color (fadeMaterial.color);
			GL.Begin (GL.QUADS);
			GL.Vertex3 (0, 0, -12);
			GL.Vertex3 (0, 1, -12);
			GL.Vertex3 (1, 1, -12);
			GL.Vertex3 (1, 0, -12);
			GL.End ();
			GL.PopMatrix ();
		}
	}
}

