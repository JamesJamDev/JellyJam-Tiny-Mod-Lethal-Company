using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using LethalCompanyInputUtils;
using LethalCompanyInputUtils.Api;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace JellyJamMadness.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    public class MyInputClass
    {
        [HarmonyPatch("SubmitChat_performed")]
        [HarmonyPrefix]
        public static bool SubmitChat_performed_Prefix(HUDManager __instance)
        {
            //PlayerInfoSize infoScript = PlayerControllerBPatch.player.GetComponent<PlayerInfoSize>();
            string text = __instance.chatTextField.text;
            text = text.ToLower();

            if (!PlayerControllerBPatch.inTerminal && text.StartsWith("/"))
            {
                Debug.Log($"{text} - Input");
                if (text.StartsWith("/size"))
                {
                    if (__instance.localPlayer.gameObject.AddComponent<PlayerInfoSize>() == null)
                    {
                        __instance.localPlayer.gameObject.AddComponent<PlayerInfoSize>();
                    }
                    PlayerInfoSize infoScript = __instance.localPlayer.gameObject.GetComponent<PlayerInfoSize>();

                    infoScript.ChangeSize();


                    Debug.Log("Attempting to Change Size");
                    HUDManager.Instance.DisplayTip("Size Changed.", "Please note: This is currently only client side. Others will still see you as tiny");
                    __instance.localPlayer = GameNetworkManager.Instance.localPlayerController;
                    __instance.localPlayer.isTypingChat = false;
                    __instance.chatTextField.text = "";
                    EventSystem.current.SetSelectedGameObject(null);
                    __instance.typingIndicator.enabled = false;
                }
            }
            //__instance.ChatMessageHistory.Add();

            return true;
        }
    }
}
