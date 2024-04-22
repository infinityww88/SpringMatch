using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class HotUpdateLinkRef : MonoBehaviour
{
	Mesh mesh;
	MeshRenderer meshRenderer;
	Material material;
	MeshFilter meshFilter;
	
	AudioListener audioListener;
	AudioSource audioSource;
	
	Button button;
	Slider slider;
	Image image;
	
	TextMeshPro textMeshPro;
	TextMeshProUGUI textMeshProUGUI;
	
	DOTween doTween;
	Tween tween;
	Tweener tweener;
	
	UniTask uniTask;
	UniTask<bool> uniTaskBool;
	UniTaskVoid uniTaskVoid;
}
