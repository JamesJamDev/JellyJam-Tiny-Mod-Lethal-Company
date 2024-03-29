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
        public static Rigidbody playerRigidbody;

        [HarmonyPatch("Update")]
        [HarmonyPostfix] // Pre-fix is done before the actual game code
        public static void UpdatePatch(ref float ___sprintMeter, ref Transform ___thisPlayerBody, ref float ___movementSpeed, ref float ___jumpForce, ref float ___grabDistance, ref Transform ___localVisor, PlayerControllerB __instance, ref bool ___inTerminalMenu, ref bool ___isPlayerDead)
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

                //infoScript = new PlayerInfoSize();
                //player.AddComponent<PlayerInfoSize>();

                //if (player.GetComponent<PlayerInfoSize>())
                //{
                //    Debug.Log("MOD FOUND");
                //}

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

            if (infoScript.isSmall)
            {
                // Apply our own gravity when small
                playerRigidbody.AddForce(Physics.gravity * 0.75f, ForceMode.Acceleration);
            }


            // Remove spawn timer to prevent getting stuck when spawned in
            if (infoScript.spawnTimer >= 0 && !___isPlayerDead)
            {
                infoScript.spawnTimer -= Time.deltaTime;
                infoScript.UpdateSize();
            }

            // Refresh spawn timer if player is dead
            if (___isPlayerDead && infoScript.spawnTimer != 5)
            {
                infoScript.spawnTimer = 5f;
            }


        }
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void NewStart(ref float ___sprintMeter, ref Transform ___thisPlayerBody, ref float ___movementSpeed, ref float ___jumpForce, ref float ___grabDistance, ref Transform ___localVisor, PlayerControllerB __instance, ref bool ___inTerminalMenu, ref Rigidbody ___playerRigidbody)
        {
            if (__instance.gameObject.AddComponent<PlayerInfoSize>() == null)
            {
                __instance.gameObject.AddComponent<PlayerInfoSize>();
            }

            if (player == null)
            {
                player = ___thisPlayerBody.gameObject;
            }

            infoScript = __instance.gameObject.GetComponent<PlayerInfoSize>();

            playerRigidbody = player.GetComponent<Rigidbody>();

            infoScript.IsSmall(true);

            infoScript.spawnTimer = 5f;



            player.transform.localScale = new Vector3(1f, 1f, 1f);

            HUDManager.Instance.DisplayTip("JellyJam Tiny Mod:", "Do /size to toggle being small!");
        }
    }

    public class PlayerInfoSize : MonoBehaviour
    {
        public bool isSmall = true;
        public float baseSpeed, baseJump, baseSize, baseGrab;
        private bool hasSetStats = false;
        public float spawnTimer = 1.5f;

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

            Rigidbody rb = player.GetComponent<Rigidbody>();

            if (spawnTimer > 0) { return; }

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
                player.transform.localScale = new Vector3(0.33f, 0.33f, 0.33f);
                playerScript.movementSpeed = 2.7f;
                playerScript.jumpForce = 8;
                playerScript.grabDistance = 1f;
                rb.useGravity = false; // We are going to use our own gravity
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
