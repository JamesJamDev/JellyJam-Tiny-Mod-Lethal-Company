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

namespace JellyJamMadness.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    public class HoardingRevamp
    {

        /*
         * the mod "MolesterLootBug" was used as reference to make a system like this work, I did not make
         * these systems myself! But I did my own take on how it works in some ways
         */

        public static float holdTime = 12;

        public class HeldInfo : MonoBehaviour
        {
            public PlayerControllerB player;
            public bool isHeld = false;
            public float canHoldTime = holdTime;
            public HoarderBugAI bug;
        }

        public class HoldingInfo : MonoBehaviour
        {
            public bool holdingPlayer = false;
            public float remainingHoldTime = holdTime;
            public PlayerControllerB playerHeld;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Awake")]
        [HarmonyPostfix]
        private static void AwakeChanged(PlayerControllerB __instance)
        {

            ((Component)(object)__instance).gameObject.AddComponent<HeldInfo>();
            HeldInfo info = ((Component)(object)__instance).GetComponent<HeldInfo>();
            info.player = __instance;
            Debug.Log("Created Player Instance for Hoarding Bug");
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPostfix]
        private static void NewUpdate(ref PlayerControllerB __instance, ref Vector3 ___serverPlayerPosition)
        {
            HeldInfo info = ((Component)(object)__instance).GetComponent<HeldInfo>();

            if (info.isHeld)
            {
                HoldingInfo bugInfo = info.bug.gameObject.GetComponent<HoldingInfo>();
                ___serverPlayerPosition = info.bug.serverPosition;
                Vector3 vector = info.bug.serverPosition - info.gameObject.transform.position;
                __instance.thisController.SimpleMove(vector * 6f + new Vector3(0, 0.5f, 0));

                info.canHoldTime -= Time.deltaTime;

                if (info.bug.isEnemyDead || info.canHoldTime <= 0 || info.player.isPlayerDead)
                {
                    bugInfo.holdingPlayer = false;
                    info.isHeld = false;
                }
            }
        }

        [HarmonyPatch(typeof(HoarderBugAI), "OnCollideWithPlayer")]
        [HarmonyPostfix]
        private static void OnBugCollide(HoarderBugAI __instance, Collider other)
        {
            HoldingInfo bugInfo = ((Component)(object)__instance).gameObject.GetComponent<HoldingInfo>();

            PlayerInfoSize infoScript = __instance.gameObject.GetComponent<PlayerInfoSize>();


            if (bugInfo.holdingPlayer == false && bugInfo.remainingHoldTime <= 0 && __instance.isEnemyDead == false)
            {
                if (other.GetComponent<PlayerControllerB>())
                {

                    Debug.Log("PLAYER HAS BEEN PICKED UP");

                    GameObject player = other.gameObject;
                    HeldInfo playerInfo = player.GetComponent<HeldInfo>();

                    playerInfo.isHeld = true;
                    bugInfo.remainingHoldTime = holdTime;
                    playerInfo.canHoldTime = holdTime;
                    playerInfo.bug = __instance;
                    bugInfo.holdingPlayer = true;
                    
                    bugInfo.playerHeld = player.GetComponent<PlayerControllerB>();
                }
            }
        }

        [HarmonyPatch(typeof(HoarderBugAI), "Start")]
        [HarmonyPostfix]
        private static void OnStart(HoarderBugAI __instance)
        {
            ((Component)(object)__instance).gameObject.AddComponent<HoldingInfo>();
        }

        [HarmonyPatch(typeof(HoarderBugAI), "Update")]
        [HarmonyPostfix]
        private static void UpdateBug(HoarderBugAI __instance)
        {
            HoldingInfo bugInfo = ((Component)(object)__instance).GetComponent<HoldingInfo>();
            if (bugInfo.holdingPlayer)
            {
                HeldInfo info = ((Component)(object)bugInfo.playerHeld).GetComponent<HeldInfo>();
                __instance.angryTimer = 1;
                __instance.angryAtPlayer = null;
                
                if (__instance.isEnemyDead)
                {
                    bugInfo.holdingPlayer = false;
                    info.isHeld = false;
                }
            }
            else
            {
                bugInfo.remainingHoldTime -= Time.deltaTime;
            }
        }
    }
}
