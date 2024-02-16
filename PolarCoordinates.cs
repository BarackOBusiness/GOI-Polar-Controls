using System.Collections.Generic;
using GOIModManager.Core;
using Newtonsoft.Json;
using HarmonyLib;
using UnityEngine;
using Rewired;

namespace PolarCoordinates;

public class PolarCoordinates : IMod {
	public string Name { get; } = "Polar Coordinates"; // A user friendly name for the mod
	public ModConfiguration Configuration { get; private set; } = new PolarCoordinatesConfig();

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
	static bool FixedUpdate(PlayerControl __instance, ref Player ___player, ref JointMotor2D ___motor, ref JointMotor2D ___slider) {
		float mouseX = ___player.GetAxis("mouseX") * 400f * PolarCoordinatesConfig.RotationSens.Value;
		float mouseY = ___player.GetAxis("mouseY") * 1.5625f * PolarCoordinatesConfig.SliderSens.Value;

		float motorSpeed = Utils.CircularLerp(___motor.motorSpeed, mouseX, 1.0f / 3.0f);
		___motor.motorSpeed = -Mathf.Clamp(motorSpeed, -800f, 800f);
		__instance.hj.motor = ___motor;

		motorSpeed = Utils.CircularLerp(___slider.motorSpeed, mouseY, 0.125f);
		___slider.motorSpeed = Mathf.Clamp(motorSpeed, -50f, 50f);
		__instance.sj.motor = ___slider;
	
		return false;
	}
}

public class PolarCoordinatesConfig : ModConfiguration {
	public override bool IsEnabled { get; set; }

	[JsonProperty("Rotation Sensitivity")]
	public static ConfigurationItem<float> RotationSens;
	[JsonProperty("Slider Sensitivity")]
	public static ConfigurationItem<float> SliderSens;

	// [JsonProperty]
	// public static float RotationSens;
	// [JsonProperty]
	// public static float SliderSens;

	public PolarCoordinatesConfig() {
		IsEnabled = true;
		RotationSens = new ConfigurationItem<float>("How sensitive the hammer's rotation is to your mouse input", 1.0f);
		SliderSens = new ConfigurationItem<float>("How sensitive the hammer's extension is to your mouse input", 1.0f);
		Debug.Log("Constructed polar coordinates mod config");
	}
}

public static class Utils {
	public static float CircularLerp(float start, float end, float t) {
		float factor = Mathf.Sin(0.5f * Mathf.PI * t);
		return start + ((end - start) * factor);
	}
}
