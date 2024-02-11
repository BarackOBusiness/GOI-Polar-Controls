using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace polar;

[BepInPlugin("goiplugins.ext.polarcoordinates", "Polar Coordinate Input Scheme", "2.0.0")]
public class PolarCoordinates : BaseUnityPlugin
{
	static ConfigEntry<float> rotationSens;
	static ConfigEntry<float> sliderSens;

	static bool destroyedCursor = false;

	private void Awake() {
		rotationSens = Config.Bind("", "Hinge Sensitivity", 1.0f, "How sensitive the hammer's rotation is to your mouse input");
		sliderSens = Config.Bind("", "Slider Sensitivity", 1.0f, "How sensitive the hammer's extension is to your mouse input");

		Logger.LogInfo("Polar Input Scheme has loaded, welcome to pain :)");

		Harmony.CreateAndPatchAll(typeof(PolarCoordinates));
	}

	[HarmonyPatch(typeof(PlayerControl), "FixedUpdate")]
	[HarmonyPrefix]
	static bool RealFixedUpdate(PlayerControl __instance, ref JointMotor2D ___motor, ref JointMotor2D ___slider) {
		if (!destroyedCursor && __instance.fakeCursor.gameObject != null) {
			Component.Destroy(__instance.fakeCursor.GetComponent<SpriteRenderer>());
			destroyedCursor = true;
		}
		
		if (!__instance.loadFinished) {
			return false;
		}
	
		// The 3200 and 25 here are good default sensitivity values for a ~1500 dpi mouse
		// so sensitivity config values are just a multiplier of that
		float mouseX = Input.GetAxis("Mouse X") * 3200f * rotationSens.Value;
		float mouseY = Input.GetAxis("Mouse Y") * 25f * sliderSens.Value;

		float motorSpeed = CircularLerp(___motor.motorSpeed, mouseX, 1.0f / 3.0f);
		___motor.motorSpeed = -Mathf.Clamp(motorSpeed, -800f, 800f);
		__instance.hj.motor = ___motor;

		motorSpeed = CircularLerp(___slider.motorSpeed, mouseY, 0.125f);
		___slider.motorSpeed = Mathf.Clamp(motorSpeed, -50f, 50f);
		__instance.sj.motor = ___slider;

		return false;
	}

	static float CircularLerp(float start, float end, float t) {
		float factor = Mathf.Sin(0.5f * Mathf.PI * t);
		return start + ((end - start) * factor);
	}
}
