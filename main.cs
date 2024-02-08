// I don't want the mod to display "Getting Over It with Barack OBusiness" on the title screen for player builds
// #define RELEASE

using BepInEx;
using BepInEx.Configuration;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#if !RELEASE
	using TMPro;
#endif

namespace polar;

[BepInPlugin("goiplugins.ext.polarcoordinates", "Polar Coordinate Inputs", "0.9.0")]
public class PolarCoordinates : BaseUnityPlugin
{
	ConfigEntry<float> RotationSens;
	ConfigEntry<float> SliderSens;

    private void Awake()
    {
		RotationSens = Config.Bind("", "Rotation Sensitivity", 1f, "How sensitive the hinge joint is to your mouse input.");
		SliderSens = Config.Bind("", "Slider Sensitivity", 1f, "How sensitive the slider is to your mouse inputs.");
	
        SceneManager.sceneLoaded += OnSceneLoaded;
    
        Debug.Log("Welcome to a world of suffering :)");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		#if !RELEASE
        	if (scene.name == "Loader") {
				GameObject Author = GameObject.Find("Canvas/Mask/Title/Author");
				Author.GetComponent<TextMeshProUGUI>().text = "with Barack OBusiness";
			}
		#endif
		if (scene.name == "Mian") {
			PlayerControl OrigController = GameObject.Find("Player").GetComponent<PlayerControl>();
			OrigController.enabled = false;
			Component.Destroy(OrigController);
			PlayerControl2 Controls = GameObject.Find("Player").AddComponent<PlayerControl2>();
			Controls.Init(RotationSens.Value, SliderSens.Value);
		}
    }
}

class PlayerControl2 : MonoBehaviour {
	public HingeJoint2D hj;
	public SliderJoint2D sj;

	private JointMotor2D motor;
	private JointMotor2D slider;

	private float HingeSensitivity = 3600f;
	private float SliderSensitivity = 50f;

	private float pauseInputTimer;

	public bool loadedFromSave;

	public bool loadFinished;

	// Necessary for ghost hammer... whatever that is
	//private PolygonCollider2D hammerCollider;
	//private List<Collider2D> ghostedCols;

	private PoseControl pose;

	private int numWins;

	public void Init(float hingeSens, float sliderSens) {
		HingeSensitivity = hingeSens * 3600f; // A reasonable default sensitivity 
		SliderSensitivity = sliderSens * 50f; // A reasonable default sensitivity here too
	}

	private void Awake() {
		loadedFromSave = false;
		loadFinished = false;
	}

	private void Start() {
		hj = GameObject.Find("Player/Hub").GetComponent<HingeJoint2D>();
		motor = hj.motor;
		motor.motorSpeed = 0f;
		
		sj = GameObject.Find("Player/Hub/Slider").GetComponent<SliderJoint2D>();
		slider = sj.motor;
		slider.motorSpeed = 0f;
		// For some reason max torque gets set to 4300 without this line, this restores it to default.
		slider.maxMotorTorque = 10000f;

		GameObject.Destroy(GameObject.Find("Cursor"));
		
		numWins = PlayerPrefs.GetInt("NumWins", 0);
		//ghostedCols = new List<Collider2D>();
		//hammerCollider = tip.GetComponent<PolygonCollider2D>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		pauseInputTimer = 2f;
		Debug.Log("enabling input");
	}

	private void OnApplicationFocus(bool focus) {
		if (focus)
		{
			StartCoroutine(LockCursor());
		}
	}

	private IEnumerator LockCursor() {
		yield return new WaitForSeconds(0.1f);
		Cursor.lockState = CursorLockMode.Locked;
	}

	//private void GhostHammer(PolygonCollider2D otherCol) {
	//	ghostedCols.Add(otherCol);
	//	Physics2D.IgnoreCollision(hammerCollider, otherCol);
	//}

	public void SetSensitivity(float newSensitivity) {
		// Do nothing, since sensitivity isn't valid here
	}

	public void PauseInput(float timeToPause) {
		pauseInputTimer = timeToPause * 2f;
		if (timeToPause < 0f)
		{
			Debug.Log("disabling input");
		}
		else if (timeToPause == 0f)
		{
			Debug.Log("enabling input");
		}
	}

	public void StopAnimator() {
		if (pose == null)
		{
			pose = GetComponentInChildren<PoseControl>();
		}
		pose.StopAnimator();
	}

	public void StartAnimator() {
		if (pose == null)
		{
			pose = GetComponentInChildren<PoseControl>();
		}
		pose.StartAnimator();
	}

	public void PlayOpeningAnimation() {
		GetComponentInChildren<PoseControl>().PlayOpeningAnimation();
	}

	private void Update() {
		if (numWins > 0 && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKey(KeyCode.R))
		{
			PlayerPrefs.DeleteKey("NumSaves");
			PlayerPrefs.DeleteKey("SaveGame0");
			PlayerPrefs.DeleteKey("SaveGame1");
			PlayerPrefs.Save();
			SceneManager.LoadScene("Mian");
		}
	}

	private void FixedUpdate() {
		float mouseX = Input.GetAxis("Mouse X") * HingeSensitivity;
		float mouseY = Input.GetAxis("Mouse Y") * SliderSensitivity;

		float motorSpeed = Mathf.Lerp(motor.motorSpeed, mouseX, 1.0f / 3.0f);
		float sliderSpeed = Mathf.Lerp(slider.motorSpeed, mouseY, 0.1f);
		
		motor.motorSpeed = -Mathf.Clamp(motorSpeed, -800f, 800f);
		hj.motor = motor;

		slider.motorSpeed = Mathf.Clamp(sliderSpeed, -50f, 50f);
		sj.motor = slider;
	}
}

// The reference implementation of the polar coordinates control scheme
//class ReferenceImpl : MonoBehaviour {
//	public HingeJoint2D hj;
//	public SliderJoint2D sj;
//
//	private JointMotor2D motor;
//	private JointMotor2D slider;
//
//	private float HingeSensitivity = 1000f;
//	private float SliderSensitivity = 250f;
//
//	private float lastX;
//	private float currentX;
//	private float lastY;
//	private float currentY;
//
//	public void Init(float hingeSens, float sliderSens) {
//		HingeSensitivity = hingeSens * 2000f;
//		SliderSensitivity = sliderSens * 50f;
//	}
//
//	private void Awake() {
//		hj = GameObject.Find("Player/Hub").GetComponent<HingeJoint2D>();
//		sj = GameObject.Find("Player/Hub/Slider").GetComponent<SliderJoint2D>();
//
//		motor = hj.motor;
//		
//		slider = sj.motor;
//		slider.maxMotorTorque = 10000f;
//	}
//
//	private void FixedUpdate() {
//		lastX = currentX;
//		lastY = currentY;
//		currentX = Input.GetAxis("Mouse X") * HingeSensitivity;
//		currentY = Input.GetAxis("Mouse Y") * SliderSensitivity;
//
//		float interX = Mathf.Lerp(lastX, currentX, 0.5f);
//		float interY = Mathf.Lerp(lastY, currentY, 0.5f);
//		
//		motor.motorSpeed = -Mathf.Clamp(interX, -800f, 800f);
//		hj.motor = motor;
//
//		slider.motorSpeed = Mathf.Clamp(interY, -50f, 50f);
//		sj.motor = slider;
//
//		// Distance:
//		//	[-1.1,0.7]
//		// Don't think I need this anymore
//	}
//}

