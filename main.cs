using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using Rewired;

namespace polar;

[BepInPlugin("goiplugins.ext.polarcoordinates", "Polar Coordinate Input Scheme", "2.2.0")]
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
	static bool RealFixedUpdate(PlayerControl __instance, ref Player ___player, ref JointMotor2D ___motor, ref JointMotor2D ___slider) {
		if (!destroyedCursor && __instance.fakeCursor.gameObject != null) {
			Component.Destroy(__instance.fakeCursor.GetComponent<SpriteRenderer>());
			destroyedCursor = true;
		}
	
		// The 400 and 1.5625 here are good default sensitivity values for a ~1600 dpi mouse
		// so sensitivity config values are just a multiplier of that
		float mouseX = ___player.GetAxis("mouseX") * 400f * rotationSens.Value;
		float mouseY = ___player.GetAxis("mouseY") * 1.5625f * sliderSens.Value;

		float motorSpeed = Mathf.SmoothStep(___motor.motorSpeed, mouseX, 0.5f);
		___motor.motorSpeed = -Mathf.Clamp(motorSpeed, -800f, 800f);
		__instance.hj.motor = ___motor;

		motorSpeed = Mathf.SmoothStep(___slider.motorSpeed, mouseY, 0.25f);
		___slider.motorSpeed = Mathf.Clamp(motorSpeed, -50f, 50f);
		__instance.sj.motor = ___slider;

		return false;
	}

	[HarmonyPatch(typeof(PlayerControl), "Start")]
	[HarmonyPostfix]
	static void Cleanup() {
		destroyedCursor = false;
	}
}
