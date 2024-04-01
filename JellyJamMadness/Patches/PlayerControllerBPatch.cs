using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.UIElements;
using System.Security.Permissions;
using System.Runtime.CompilerServices;



namespace JellyJamMadness.Patches
{

    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        public static Vector3 savedPos = Vector3.zero;
        public static GameObject managerObject;
        public static Vector3 visorScale;
        public static bool inTerminal;
        public static bool isStarting = true;

        public static PlayerInfoSize infoScript;
        public static Rigidbody playerRigidbody;

        [HarmonyPatch("Update")]
        [HarmonyPostfix] // Pre-fix is done before the actual game code
        public static void UpdatePatch(ref float ___sprintMeter, ref Transform ___thisPlayerBody, ref float ___movementSpeed, ref float ___jumpForce, ref float ___grabDistance, ref Transform ___localVisor, PlayerControllerB __instance, ref bool ___inTerminalMenu, ref bool ___isPlayerDead)
        {
            // Run all of these at the start

            if (isStarting)
            {

                // Add the script to the player
                Debug.Log("Starting the Tiny Mod...");

                // Assign base values
                infoScript.baseGrab = ___grabDistance;
                infoScript.baseJump = ___jumpForce;
                infoScript.baseSize = 1;
                infoScript.baseSpeed = ___movementSpeed;

                isStarting = false;
            }

            inTerminal = ___inTerminalMenu;



            // Disable the visor
            __instance.localVisor.gameObject.SetActive(false);

            // Make player normal size when in terminal
            if (___inTerminalMenu == true)
            {
                ___thisPlayerBody.localScale = new Vector3(1f, 1f, 1f);
                Debug.Log("IN TERMINAL");
            }
            else
            {
                PlayerInfoSize infoScript = __instance.gameObject.GetComponent<PlayerInfoSize>();

                infoScript.UpdateSize();
            }

            if (infoScript.GetSize())
            {
                // Apply our own gravity when small
                playerRigidbody.AddForce(Physics.gravity * 0.75f, ForceMode.Acceleration);
            }


        }
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void NewStart(ref float ___sprintMeter, ref Transform ___thisPlayerBody, ref float ___movementSpeed, ref float ___jumpForce, ref float ___grabDistance, ref Transform ___localVisor, PlayerControllerB __instance, ref bool ___inTerminalMenu, ref Rigidbody ___playerRigidbody)
        {
            if (__instance.gameObject.GetComponent<PlayerInfoSize>() == null)
            {
                __instance.gameObject.AddComponent<PlayerInfoSize>();
            }

            GameObject _player = ___thisPlayerBody.gameObject;

            infoScript = __instance.gameObject.GetComponent<PlayerInfoSize>();

            playerRigidbody = _player.GetComponent<Rigidbody>();

            infoScript.IsSmall(true);




            HUDManager.Instance.DisplayTip("JellyJam Tiny Mod:", "Do /size to toggle being small!");
        }
    }

    public class PlayerInfoSize : MonoBehaviour
    {
        private bool isSmall = true;
        public bool useSmallSpeed = true;
        public float baseSpeed, baseJump, baseSize, baseGrab;
        private bool hasSetStats = false;
        public PlayerControllerB playerScript;
        public Rigidbody rb;

        public void ChangeSize()
        {
            GameObject player = this.gameObject;

            playerScript = player.GetComponent<PlayerControllerB>();

            IsSmall(!isSmall);

        }

        public void IsSmall(bool _isSmall)
        {
            isSmall = _isSmall;
        }

        public bool GetSize()
        {
            return isSmall;
        }

        public void UpdateSize()
        {
            GameObject player = this.gameObject;

            playerScript = player.GetComponent<PlayerControllerB>();


             rb = player.GetComponent<Rigidbody>();

            if (!hasSetStats)
            {
                baseSpeed = playerScript.movementSpeed;
                baseJump = playerScript.jumpForce;
                baseSize = 1;
                baseGrab = playerScript.grabDistance;

                hasSetStats = true;
            }

            if (isSmall && useSmallSpeed)
            {
                player.transform.localScale = new Vector3(0.33f, 0.33f, 0.33f);
                playerScript.movementSpeed = 2.7f;
                playerScript.jumpForce = 8;
                playerScript.grabDistance = 1f;
                rb.useGravity = false; // We are going to use our own gravity
            }
            else if (isSmall && !useSmallSpeed)
            {
                player.transform.localScale = new Vector3(0.33f, 0.33f, 0.33f);
                playerScript.movementSpeed = baseSpeed;
                playerScript.jumpForce = baseJump;
                playerScript.grabDistance = baseGrab;
                rb.useGravity = true;
            }
            else
            {
                player.transform.localScale = new Vector3(1f, 1f, 1f);
                playerScript.movementSpeed = baseSpeed;
                playerScript.jumpForce = baseJump;
                playerScript.grabDistance = baseGrab;
                rb.useGravity = true;
                
            }
        }
    }


}
