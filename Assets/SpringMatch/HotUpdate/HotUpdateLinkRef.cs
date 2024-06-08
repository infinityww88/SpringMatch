using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class HotUpdateLinkRef : MonoBehaviour
{

#region EventSystems
	AbstractEventData abstractEventData;
	AxisEventData axisEventData;
	BaseEventData baseEventData;
	BaseInput baseInput;
	BaseInputModule baseInputModule;
	BaseRaycaster baseRaycaster;
	EventHandle eventHandle;
	EventSystem eventSystem;
	EventTrigger eventTrigger;
	EventTriggerType eventTriggerType;
	EventTrigger.Entry eventTriggerEntry;
	EventTrigger.TriggerEvent eventTriggerEvent;
	ExecuteEvents.EventFunction<IBeginDragHandler> executeEventsFunc;
	IBeginDragHandler iBeginDragHandler;
	ICancelHandler iCancelHandler;
	IDeselectHandler iDeselectHandler;
	IDragHandler iDragHandler;
	IDropHandler iDropHandler;
	IEndDragHandler iEndDragHandler;
	IEventSystemHandler iEventSystemHandler;
	IInitializePotentialDragHandler iInitializePotentialDragHandler;
	IMoveHandler iMoveHandler;
	IPointerClickHandler iPointerClickHandler;
	IPointerDownHandler iPointerDownHandler;
	IPointerEnterHandler iPointerEnterHandler;
	IPointerExitHandler iPointerExitHandler;
	IPointerMoveHandler iPointerMoveHandler;
	IPointerUpHandler iPointerUpHandler;
	IScrollHandler iScrollHandler;
	ISelectHandler iSelectHandler;
	ISubmitHandler iSubmitHandler;
	IUpdateSelectedHandler iUpdateSelectedHandler;
	MoveDirection moveDirection;
	Physics2DRaycaster physics2DRaycaster;
	PhysicsRaycaster physicsRaycaster;
	PointerEventData pointerEventData;
	PointerEventData.FramePressState pointerEventDataFramePressState;
	PointerEventData.InputButton pointerEventDataInputButton;
	RaycastResult raycastResult;
	StandaloneInputModule standaloneInputModule;
	UIBehaviour uIBehaviour;
#endregion

#region UGUI
	AnimationTriggers animationTriggers;
	AspectRatioFitter aspectRatioFitter;
	AspectRatioFitter.AspectMode aspectMode;
	Button button;
	Button.ButtonClickedEvent buttonClickedEvent;
	CanvasScaler canvasScaler;
	CanvasScaler.ScaleMode canvasScalerMode;
	CanvasScaler.ScreenMatchMode canvasScalerScreenMatchMode;
	CanvasScaler.Unit canvasScalerUnit;
	CanvasUpdate canvasUpdate;
	CanvasUpdateRegistry canvasUpdateRegistry;
	ContentSizeFitter contentSizeFitter;
	ContentSizeFitter.FitMode contentSizeFitterFitMode;
	Dropdown dropdown;
	Dropdown.DropdownEvent dropdownEvt;
	Dropdown.OptionData dropdownOptData;
	Dropdown.OptionDataList dropdownOptDataList;
	FontData fontData;
	Graphic graphic;
	GraphicRaycaster graphicRaycaster;
	GraphicRaycaster.BlockingObjects graphicRaycasterBlockingObjects;
	GraphicRegistry graphicRegistry;
	GridLayoutGroup gridLayoutGroup;
	GridLayoutGroup.Axis gridLayoutGroupAxis;
	GridLayoutGroup.Constraint gridLayoutGroupConstraint;
	GridLayoutGroup.Corner gridLayoutGroupCorner;
	HorizontalLayoutGroup horizontalLayoutGroup;
	HorizontalOrVerticalLayoutGroup horizontalOrVerticalLayoutGroup;
	ICanvasElement iCanvasElement;
	IClippable iClippable;
	IClipper iClipper;
	ILayoutController iLayoutController;
	ILayoutElement iLayoutElement;
	ILayoutGroup iLayoutGroup;
	ILayoutIgnorer iLayoutIgnorer;
	ILayoutSelfController iLayoutSelfController;
	IMaskable iMaskable;
	IMaterialModifier iMaterialModifier;
	IMeshModifier iMeshModifier;
	Image image;
	Image.FillMethod imageFillMethod;
	Image.Origin180 imageOrigion180;
	Image.Origin360 imageOrigin360;
	Image.Origin90 imageOrigin30;
	Image.OriginHorizontal imageOriginHorz;
	Image.OriginVertical imageOriginVert;
	Image.Type imageType;
	InputField inputField;
	InputField.CharacterValidation inputFieldCharValidation;
	InputField.ContentType inputFieldContentType;
	InputField.EndEditEvent inputFieldEndEditEvent;
	InputField.InputType inputFieldInputType;
	InputField.LineType inputFieldLineType;
	InputField.OnChangeEvent inputFieldOnChangeEvent;
	InputField.OnValidateInput inputFieldOnValidateInput;
	InputField.SubmitEvent inputFieldSubmitEvent;
	LayoutElement layoutElement;
	LayoutGroup layoutGroup;
	LayoutRebuilder layoutRebuilder;
	Mask mask;
	MaskUtilities maskUtilities;
	MaskableGraphic maskableGraphic;
	MaskableGraphic.CullStateChangedEvent maskableGraphicCullStateChangedEvent;
	Navigation navigation;
	Navigation.Mode navigationMode;
	Outline outline;
	PositionAsUV1 positionAsUV1;
	RawImage rawImage;
	RectMask2D rectMask2D;
	ScrollRect scrollRect;
	ScrollRect.MovementType scrollRectMoveType;
	ScrollRect.ScrollRectEvent scrollRectEvent;
	ScrollRect.ScrollbarVisibility scrollRectVisibility;
	Scrollbar scrollbar;
	Scrollbar.Direction scrollbarDirection;
	Scrollbar.ScrollEvent scrollbarEvent;
	Selectable selectable;
	Selectable.Transition selectableTransition;
	Shadow shadow;
	Slider slider;
	Slider.Direction sliderDirection;
	Slider.SliderEvent sliderSliderEvent;
	SpriteState spriteState;
	Text text;
	Toggle toggle;
	Toggle.ToggleEvent toggleEvent;
	Toggle.ToggleTransition toggleTransition;
	ToggleGroup toggleGroup;
	VertexHelper vertexHelper;
	VerticalLayoutGroup verticalLayoutGroup;
#endregion

#region TextMeshPro
	TextMeshPro textMeshPro;
	TextMeshProUGUI textMeshProUGUI;
#endregion

#region DOTween
	DOTween doTween;
	Tween tween;
	Tweener tweener;
#endregion

#region UniTask
	UniTask uniTask;
	UniTask<bool> uniTaskBool;
	UniTaskVoid uniTaskVoid;
#endregion

#region UnityEvent
	UnityEvent unityEvent;
	UnityEvent<bool> unityEventBool;
#endregion

#region UnityNetwork
	CertificateHandler certHandler;
	DownloadHandler downloadHandler;
	DownloadHandlerAudioClip downloadHandlerAudioClip;
	DownloadHandlerFile downloadHandlerFile;
	DownloadHandlerAssetBundle downloadHandlerAssetBundle;
	DownloadHandlerTexture downloadHandlerTexture;
	UnityWebRequest webRequest;
	UnityWebRequestAsyncOperation webRequestAsyncOperation;
	UnityWebRequest.Result webRequestResult;
	UploadHandler uploadHandler;
	UploadHandlerFile uploadHandlerFile;
	UploadHandlerRaw uploadHandlerRaw;
	MultipartFormFileSection multipartFormFileSection;
	MultipartFormDataSection multipartFormDataSection;
#endregion

#region UnityPool
	DictionaryPool<object, object> dictPool;
	HashSetPool<object> hashSetPool;
	LinkedPool<object> linkedPool;
	ListPool<object> listPool;
	ObjectPool<object> objectPool;
#endregion

#region SceneManagement
	SceneManager sceneManager;
	Scene scene;
	LoadSceneParameters parameters;
	CreateSceneParameters createSceneParameters;
	SceneManagerAPI sceneManagerAPI;
	LoadSceneMode loadSceneMode;
	LocalPhysicsMode localPhysicsMode;
	UnloadSceneOptions unloadSceneOptions;
#endregion

#region UnityEngine
	AccelerationEvent accelerationEvent;
	AnimationClip animationClip;
	AnimationCurve animationCurve;
	AnimationEvent animationEvent;
	Animator animator;
	Application application;
	AssetBundle assetBundle;
	AsyncOperation asyncOperation;
	AudioClip audioClip;
	AudioListener audioListener;
	AudioSource audioSource;
	Bounds bounds;
	BoundsInt boundsInt;
	BoxCollider boxCollider;
	Camera cameraV;
	Canvas canvas;
	CanvasGroup canvasGroup;
	CanvasRenderer canvasRenderer;
	CapsuleCollider capsuleCollider;
	Collider colliderV;
	ColliderHit colliderHit;
	Collision collision;
	Color color;
	Color32 color32;
	ColorUtility colorUtility;
	Component component;
	ConstantForce constantForceV;
	ContactPoint contactPoint;
	ControllerColliderHit controllerColliderHit;
	Coroutine coroutine;
	Cursor cursor;
	Debug debug;
	Event evt;
	FixedJoint fixedJoint;
	GameObject go;
	GeometryUtility geometryUtility;
	Gizmos gizmos;
	Gradient gradient;
	GradientAlphaKey gradientAlphaKey;
	GradientColorKey gradientColorKey;
	Hash128 hash128;
	HingeJoint hingeJointV;
	Input input;
	Joint joint;
	Keyframe keyframe;
	LayerMask layerMask;
	Light lightV;
	LineRenderer lineRenderer;
	LineUtility lineUtility;
	Material material;
	Mathf mathf;

	Matrix4x4 matrix4x4;
	Mesh mesh;
	MeshCollider meshCollider;
	MeshFilter meshFilter;
	MeshRenderer meshRenderer;
	MonoBehaviour monoBehaviour;
	Object obj;
	ParticleCollisionEvent particleCollisionEvent;
	ParticleSystem particleSystemV;
	ParticleSystemForceField particleSystemForceField;
	ParticleSystemRenderer particleSystemRenderer;
	PhysicMaterial physicMaterial;
	Physics physics;
	PhysicsScene physicsScene;
	
	Plane plane;
	PlayerPrefs playerPrefs;
	PlayerPrefsException playerPrefsException;
	Projector projector;
	Quaternion quaternion;

	RangeInt rangeInt;
	Ray ray;
	RaycastHit raycastHit;
	Rect rect;
	RectInt rectInt;
	RectOffset rectOffset;
	RectTransform rectTransform;
	RectTransformUtility rectTransformUtility;
	Renderer rendererV;
	RenderTexture renderTexture;
	RenderTextureDescriptor renderTextureDescriptor;
	Resolution resolution;
	Resources resources;
	Rigidbody rigidbodyV;
	Screen screen;
	ScriptableObject scriptableObject;
	Shader shader;
	ShaderVariantCollection shaderVariantCollection;
	SkinnedMeshRenderer skinnedMeshRenderer;
	Skybox skybox;
	SpringJoint springJoint;
	Sprite sprite;
	SpriteMask spriteMask;
	SpriteRenderer spriteRenderer;
	TextAsset textAsset;
	Texture texture;
	Texture2D texture2D;
	Texture2DArray texture2DArray;
	Texture3D texture3D;
	Time time;
	Touch touch;
	TouchScreenKeyboard touchScreenKeyboard;
	TrailRenderer trailRenderer;
	Transform trans;
	Vector2 vector2;
	Vector2Int vector2Int;
	Vector3 vector3;
	Vector3Int vector3Int;
	Vector4 vector4;
	WaitForEndOfFrame waitForEndOfFrame;
	WaitForFixedUpdate waitForFixedUpdate;
	WaitForSeconds waitForSeconds;
	WaitForSecondsRealtime waitForSecondsRealtime;
	WaitUntil waitUntil;
	WaitWhile waitWhile;
	WheelCollider wheelCollider;
	WheelFrictionCurve wheelFrictionCurve;
	WheelHit wheelHit;
	WindZone windZone;
	YieldInstruction yieldInstruction;
#endregion
	
	void StaticClassRef() {
		UnityWebRequestMultimedia.GetAudioClip("", AudioType.MPEG);
		UnityWebRequestAssetBundle.GetAssetBundle("");
		UnityWebRequestTexture.GetTexture("");
		DOTween.Sequence();
		DOTweenAsyncExtensions.AwaitForPlay(null);
		Transform transform = new GameObject().transform;
		transform.DOMove(Vector3.zero, 0);
		Random.Range(0, 1);
		Scene scene = SceneManager.GetSceneAt(0);
		scene.GetPhysicsScene();
		ParticleSystem ps = transform.GetComponent<ParticleSystem>();
		ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, null);
		JsonUtility.ToJson(null);
		Hash128 hash128 = new	Hash128();
		HashUtilities.ComputeHash128(null, ref hash128);
		LayoutUtility.GetMinSize(null, 0);
		#if UNITY_Android || UNITY_IOS
		Vibration.VibrateAndroid(1000);
		#endif
	}
}
