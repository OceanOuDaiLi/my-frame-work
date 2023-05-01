using UnityEngine;
//using TheNextMoba.Effect;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MOBABaseFX : MonoBehaviour
{
	#region ------------------------------变量------------------------------

	public bool playOnAwake;                                            // 是否自动播放
	protected bool bInitialized;                                            // 是否初始化
	protected bool isVisible;                                               // 是否被渲染
	protected bool ifCulling;
	public bool isPlaying
	{
		get { return m_IsPlaying; }
		set { m_IsPlaying = value; }
	}
	protected bool m_IsPlaying;
	public bool isScheduled
	{
		get { return m_IsScheduled; }
		set { m_IsScheduled = value; }
	}
	protected bool m_IsScheduled;
	public bool shoudUnschedule
	{
		get { return m_ShoudUnschedule; }
		set { m_ShoudUnschedule = value; }
	}
	protected bool m_ShoudUnschedule;
	protected float cullingTime;
	protected float cullingTimeDelta;

	public static Camera mainCam
	{
		get
		{
			return m_MainCam;
		}
		set
		{
			if (value == null)
			{
				m_MainCam = null;
				hasMainCam = false;
				mainCamTrans = null;
			}
			else
			{
				m_MainCam = value;
				hasMainCam = true;
				mainCamTrans = m_MainCam.transform;
			}
		}
	}
	static Camera m_MainCam;
	[System.NonSerialized]
	public static bool hasMainCam;
	[System.NonSerialized]
	public static Transform mainCamTrans;

	protected Transform transBuffer;

	#endregion


	#region ---------------------MonoBehaviour事件----------------------

	// 开始渲染时
	void OnBecameVisible()
	{
		isVisible = true;
		if (ifCulling == false) return;
		cullingTimeDelta = Time.time - cullingTime;
		if (isPlaying == true)
		{
			//MobaBaseScheduler.Instance.Schedule(this);
			UpdateMe();
		}
	}

	// 结束渲染时
	void OnBecameInvisible()
	{
		isVisible = false;
		if (ifCulling == false) return;
		cullingTime = Time.time;
		if (isScheduled == true)
		{
			m_ShoudUnschedule = true;
		}
	}

	void OnEnable()
	{
		if (playOnAwake == true)
		{
			Play();
		}
	}

	void OnDisable()
	{
		if (playOnAwake == true)
		{
			Stop(true);
		}
	}

	void Awake()
	{
		if (playOnAwake == true)
		{
			Init();
		}
	}

	void OnDestroy()
	{
		//MobaBaseScheduler.Instance.Unschedule(this);
	}

	#endregion


	#region -----------------------NextFXBase事件------------------------

	// 初始化
	public virtual void Init()
	{
		bInitialized = true;
		isPlaying = m_IsScheduled = m_ShoudUnschedule = false;
		cullingTime = cullingTimeDelta = 0.0f;
		transBuffer = transform;
	}

	// 播放
	public virtual void Play()
	{
		isPlaying = true;
		if (ifCulling == true)
		{
			if (isVisible == true)
			{
				//MobaBaseScheduler.Instance.Schedule(this);
				cullingTimeDelta = 0.0f;
			}
			else
			{
				cullingTime = Time.time;
			}
		}
		else
		{
			//MobaBaseScheduler.Instance.Schedule(this);
		}
	}

	// 停止
	public virtual void Stop(bool immidiate)
	{
		if (immidiate == true)
		{
			isPlaying = false;
			if (isScheduled == true)
			{
				m_ShoudUnschedule = true;
			}
		}
	}

	// 更新
	public virtual void UpdateMe()
	{

	}

	#endregion


#if UNITY_EDITOR

	#region -------------------------Editor变量-------------------------

	public SerializedObject serializedObject;

	#endregion


	#region -----------------------Editor继承方法-----------------------

	// 计算特效持续时间
	public virtual float GetDuration()
	{
		return 0.0f;
	}

	// 计算特效淡出时间
	public virtual float GetFadeTime()
	{
		return 0.0f;
	}

	// 预保存重置方法
	public virtual void PreSaveRenew()
	{
		if (bInitialized == true)
		{
			Stop(true);
			//MobaBaseScheduler.Instance.Unschedule(this);
			bInitialized = false;
		}
	}

	#endregion


	#region --------------------------GUI变量--------------------------

	// 工具集根路径 需按实际情况修改
	public static readonly string rootPath = "Assets/ArtScript/NextFX/";

	// GUI统一用色
	protected static readonly Color barColor = new Color(0.0f, 0.0f, 0.0f, 0.2f);
	protected static readonly Color lineColor = new Color(1.0f, 1.0f, 1.0f, 0.2f);
	protected static readonly Color buttonColor = new Color(1.0f, 1.0f, 1.0f, 0.1f);
	protected static readonly Color curveColor = new Color(1.0f, 0.4f, 0.0f, 0.7f);
	protected static readonly Color layerColor = new Color(0.0f, 0.0f, 0.0f, 0.1f);
	protected static readonly Color selLayerColor = new Color(1.0f, 0.4f, 0.0f, 0.1f);
	protected static readonly Color lightRed = new Color(1.0f, 0.0f, 0.0f, 0.1f);
	protected static readonly Color lightGreen = new Color(0.0f, 1.0f, 0.0f, 0.1f);
	protected static readonly Color lightBlue = new Color(0.0f, 0.0f, 1.0f, 0.1f);

	#endregion


	#region ------------------------GUI继承方法------------------------

	// Inspector激活时自检操作
	public virtual void GUIOnEableActions()
	{

	}

	// InspectorGUI
	public virtual void DrawGUI()
	{
		serializedObject = new SerializedObject(this);
	}

	// MiniBarGUI
	public virtual void DrawMiniBar(int id)
	{
		Rect rect = GUILayoutUtility.GetRect(100.0f, 20.0f);
		EditorGUI.DrawRect(new Rect(rect.x + 8.0f, rect.y + 9.0f, 8.0f, 2.0f), lineColor);
		rect = new Rect(rect.x + 16.0f, rect.y, rect.width - 30.0f, 20.0f);
		EditorGUI.DrawRect(rect, id % 2 == 0 ? buttonColor : layerColor);
		// 激活开关
		gameObject.SetActive(GUI.Toggle(new Rect(rect.x, rect.y + 2.0f, 15.0f, 18.0f), gameObject.activeInHierarchy, ""));
		// 特效对象
		System.Type thisType = this.GetType();
		EditorGUI.ObjectField(new Rect(rect.x + 15.0f, rect.y + 2.0f, rect.width - 200.0f, 18.0f), this, thisType, false);
		// 状态条
		DrawStateColor(new Rect(rect.x + rect.width, rect.y, 14.0f, 20.0f));
	}

	// 广告位
	protected void DrawBanner()
	{
		Rect rect = GUILayoutUtility.GetRect(100.0f, 0.0f);
		GUI.Label(new Rect(rect.x + rect.width - 210.0f, rect.y - 15.0f, 175.0f, 18.0f), "Powered by MoreFun.TATeam");
	}

	// 测试功能条
	protected void DrawTestingBar()
	{
		// 分割线
		Rect rect = GUILayoutUtility.GetRect(100.0f, 2.0f);
		EditorGUI.DrawRect(rect, lineColor);
		// 背景色
		rect = GUILayoutUtility.GetRect(100.0f, 20.0f);
		EditorGUI.DrawRect(new Rect(rect.x, rect.y, 20.0f, 20.0f), barColor);
		DrawStateColor(new Rect(rect.x + 20.0f, rect.y, rect.width - 170.0f, 20.0f));       // 状态标识色
		EditorGUI.DrawRect(new Rect(rect.x + rect.width - 150.0f, rect.y, 150.0f, 20.0f), barColor);
		// 帮助链接
		if (GUI.Button(new Rect(rect.x + 3.0f, rect.y, 18.0f, 20.0f), new GUIContent("₪", "Code by LookingLu:)"), EditorStyles.largeLabel))
		{
			OpenHelpLink();
		}
		// 调试按钮
		EditorGUI.BeginDisabledGroup(Application.isPlaying == false);
		DrawTestingButton(new Rect(rect.x + 30.0f, rect.y + 1.0f, rect.width - 190.0f, 17.0f));
		EditorGUI.EndDisabledGroup();
		// PlayOnAwake开关
		if (GUI.Button(new Rect(rect.x + rect.width - 142.0f, rect.y + 2.0f, 90.0f, 18.0f), playOnAwake == true ? "PlayOnAwake✔" : "PlayOnAwake✘", EditorStyles.label))
		{
			playOnAwake = !playOnAwake;
		}
		// 拷贝保存按钮
		rect = new Rect(rect.x + rect.width - 50.0f, rect.y, 45.0f, 20.0f);
		if (GUI.Button(new Rect(rect.x, rect.y, rect.width * 0.333333f, rect.height), "Ⓒ", EditorStyles.largeLabel))
		{
			CopeActions();
		}
		// PasteButton
		if (GUI.Button(new Rect(rect.x + rect.width * 0.333333f, rect.y, rect.width * 0.333333f, rect.height), "Ⓟ", EditorStyles.largeLabel))
		{
			PasteActions();
		}
		// SaveButton
		if (GUI.Button(new Rect(rect.x + rect.width * 0.666666f, rect.y, rect.width * 0.333333f, rect.height), "Ⓢ", EditorStyles.largeLabel))
		{
			//NextFXPlayer player = this.GetComponentInParent<NextFXPlayer>();
			//if (player != null)
			//{
			//	player.Save();
			//}
			//else {
			//	EditorUtility.DisplayDialog("NextFXBase", "无法保存, 父级缺少NextFXPlayer.", "好的");
			//}
		}
		// 分割线
		rect = GUILayoutUtility.GetRect(100.0f, 2.0f);
		EditorGUI.DrawRect(rect, lineColor);
	}

	// 状态色
	protected virtual void DrawStateColor(Rect rect)
	{

	}

	// 调试按钮
	protected virtual void DrawTestingButton(Rect rect)
	{

	}

	// 帮助链接
	protected virtual void OpenHelpLink()
	{

	}

	// 拷贝操作
	protected virtual void CopeActions()
	{

	}

	// 粘贴操作
	protected virtual void PasteActions()
	{

	}

	#endregion

#endif
}