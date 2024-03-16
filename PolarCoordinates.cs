using GOIModManager.Core;
using HarmonyLib;
using UnityEngine;
using Rewired;

namespace PolarCoordinates;

public class PolarCoordinates : IMod {
	public string Name { get; } = "Polar Coordinates"; // A user friendly name for the mod
	public string Description { get; } = "A challenge mod that replaces the input scheme of the game.\nYour mouse's x axis will control the rotation of the hammer, and the y axis the extension of the hammer.";
	public static PolarCoordinatesConfig MyConfiguration = new PolarCoordinatesConfig();
	public ModConfiguration Configuration => MyConfiguration;

	private Harmony patcher;

	public void Init() {
		patcher = new Harmony(Name);
		Enable();
	}

	public void Deinit() {
		Disable();
	}

	public void Toggle() {
		Configuration.IsEnabled = !Configuration.IsEnabled;
		// Toggle is a universal switch for enabling or disabling the mod, it's up to you
		// to handle those cases.
		if (Configuration.IsEnabled)
			Enable();
		else
			Disable();
	}

	private void Enable() {
		patcher.PatchAll(typeof(MyPatches));
	}

	private void Disable() {
		patcher.UnpatchSelf();
	}
}

// More code can be put here, and instantiated by the mod, as expected
public class MyPatches {
	[HarmonyPatch(typeof(PlayerControl), "FixedUpdate")]
	[HarmonyPrefix]
	static bool FixedUpdate(
		PlayerControl __instance, 
		ref Player ___player, 
		ref JointMotor2D ___motor, 
		ref JointMotor2D ___slider,
		ref float ___pauseInputTimer
	) {
		if (___pauseInputTimer > 0f) {
			___pauseInputTimer -= Time.fixedDeltaTime * 2f;
			if (___pauseInputTimer < 0f) {
				___pauseInputTimer = 0f;
			}
			return false;
		}
	
		float mouseX = ___player.GetAxis("mouseX") * 400f * PolarCoordinates.MyConfiguration.RotationSens;
		float mouseY = ___player.GetAxis("mouseY") * 1.5625f * PolarCoordinates.MyConfiguration.SliderSens;

		float motorSpeed = Mathf.SmoothStep(___motor.motorSpeed, mouseX, 0.5f);
		___motor.motorSpeed = -Mathf.Clamp(motorSpeed, -800f, 800f);
		__instance.hj.motor = ___motor;

		motorSpeed = Mathf.SmoothStep(___slider.motorSpeed, mouseY, 0.25f);
		___slider.motorSpeed = Mathf.Clamp(motorSpeed, -50f, 50f);
		__instance.sj.motor = ___slider;
	
		return false;
	}
}

public class PolarCoordinatesConfig : ModConfiguration {
	public override bool IsEnabled { get; set; } = true;

	[ConfigurationItem("Rotation Sensitivity", "How sensitive the hammer's rotation is to your mouse input")]
	public float RotationSens = 1.0f;
	
	[ConfigurationItem("Slider Sensitivity", "How sensitive the hammer's extension is to your mouse input")]
	public float SliderSens = 1.0f;
}
