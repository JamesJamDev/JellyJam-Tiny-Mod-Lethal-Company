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
        public static GameObject player;
        public static bool inTerminal;
        public static bool isStarting = true;

        public static PlayerInfoSize infoScript;

        [HarmonyPatch("Update")]
        [HarmonyPostfix] // Pre-fix is done before the actual game code
        public static void UpdatePatch(ref float ___sprintMeter, ref Transform ___thisPlayerBody, ref float ___movementSpeed, ref float ___jumpForce, ref float ___grabDistance, ref Transform ___localVisor, PlayerControllerB __instance, ref bool ___inTerminalMenu)
        {
            // Run all of these at the start

            if (isStarting)
            {
                if (player == null)
                {
                    player = ___thisPlayerBody.gameObject;
                }


                // Add the script to the player
                Debug.Log("Starting the Tiny Mod...");

                infoScript = new PlayerInfoSize();
                player.AddComponent<PlayerInfoSize>();

                if (player.GetComponent<PlayerInfoSize>())
                {
                    Debug.Log("MOD FOUND");
                }

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

            if (___inTerminalMenu == true)
            {
                ___thisPlayerBody.localScale = new Vector3(1f, 1f, 1f);
            }
            else
            {
                PlayerInfoSize infoScript = __instance.gameObject.GetComponent<PlayerInfoSize>();

                infoScript.UpdateSize();
            }




        }
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void NewStart(ref float ___sprintMeter, ref Transform ___thisPlayerBody, ref float ___movementSpeed, ref float ___jumpForce, ref float ___grabDistance, ref Transform ___localVisor, PlayerControllerB __instance, ref bool ___inTerminalMenu)
        {
            if (__instance.gameObject.AddComponent<PlayerInfoSize>() == null)
            {
                __instance.gameObject.AddComponent<PlayerInfoSize>();
            }
            PlayerInfoSize infoScript = __instance.gameObject.GetComponent<PlayerInfoSize>();

            infoScript.IsSmall(true);

            HUDManager.Instance.DisplayTip("JellyJam Tiny Mod:", "Do /size to toggle being small!");
        }
    }

    public class PlayerInfoSize : MonoBehaviour
    {
        public bool isSmall = true;
        public float baseSpeed, baseJump, baseSize, baseGrab;
        private bool hasSetStats = false;

        public void ChangeSize()
        {
            GameObject player = this.gameObject;

            PlayerControllerB playerScript = player.GetComponent<PlayerControllerB>();




            isSmall = !isSmall;
            UpdateSize();

        }

        public void IsSmall(bool _isSmall)
        {
            isSmall = _isSmall;

            UpdateSize();
        }

        public void UpdateSize()
        {
            GameObject player = this.gameObject;

            PlayerControllerB playerScript = player.GetComponent<PlayerControllerB>();

            if (!hasSetStats)
            {
                baseSpeed = playerScript.movementSpeed;
                baseJump = playerScript.jumpForce;
                baseSize = 1;
                baseGrab = playerScript.grabDistance;

                hasSetStats = true;
            }

            if (isSmall)
            {
                player.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                playerScript.movementSpeed = 2.5f;
                playerScript.jumpForce = 7;
                playerScript.grabDistance = 1f;
            }
            else
            {
                player.transform.localScale = new Vector3(1f, 1f, 1f);
                playerScript.movementSpeed = baseSpeed;
                playerScript.jumpForce = baseJump;
                playerScript.grabDistance = baseGrab;
            }
        }
    }


}
