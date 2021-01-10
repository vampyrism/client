using System.Collections;
using UnityEngine;
using Discord;
using System.ComponentModel;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

namespace Assets.Scripts
{
    public class DiscordController : MonoBehaviour
    {
        public static DiscordController instance;
        public Discord.Discord discord;
        private bool ready = false;

        // Use this for initialization
        void Start()
        {
            DiscordController.instance = this;
            System.Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", "0");
            this.discord = new Discord.Discord(785086130143887391, (System.UInt64)Discord.CreateFlags.Default);
            this.discord.SetLogHook(Discord.LogLevel.Debug, (Discord.LogLevel level, string message) =>
            {
                Debug.Log(message);
            });
            var activityManager = discord.GetActivityManager();

            var activity = new Discord.Activity
            {
                State = "Chilling in the menu"
            };

            activityManager.UpdateActivity(activity, (res) =>
            {
                if (res == Discord.Result.Ok)
                {
                    Debug.Log("Discord Controller is OK!");
                }
            });

            activityManager.OnActivityJoin += secret =>
            {
                secret = secret.Split(':')[1];
                Menu.instance.LobbyId = secret;
                Menu.instance.hideMenu();
                SceneManager.LoadScene("Game");
            };

            this.ready = true;
        }

        public void JoinLobby(string id, string secret)
        {
            var activity = new Discord.Activity
            {
                State = "In-Game",
                Details = "Surviving...for now.",
                Instance = true,
                Party =
                    {
                        Id = id,
                        Size =
                        {
                            CurrentSize = 1,
                            MaxSize = 150
                        }
                    },
                Secrets =
                    {
                        Join = "secret:" + id
                    }
            };

            discord.GetActivityManager().UpdateActivity(activity, (res) =>
            {
                if (res == Discord.Result.Ok)
                {
                    Debug.Log("Discord Controller is OK!");
                }
                else
                {
                    Debug.LogWarning("Couldn't set user activity due to " + res);
                }
            });
        }

        Action<string> callback;
        public void GetAndSetTextComponentName(Action<string> callback)
        {
            this.callback = callback;
            try
            {
                Debug.Log("Name is " + discord.GetUserManager().GetCurrentUser().Username);
                this.callback(discord.GetUserManager().GetCurrentUser().Username);
            } catch(Discord.ResultException e) { }

            discord.GetUserManager().OnCurrentUserUpdate += DiscordController_OnCurrentUserUpdate;
        }

        private void DiscordController_OnCurrentUserUpdate()
        {
            Debug.Log("Name is " + discord.GetUserManager().GetCurrentUser().Username);
            this.callback(discord.GetUserManager().GetCurrentUser().Username);
        }

        // Update is called once per frame
        void Update()
        {
            discord.RunCallbacks();
        }

        private void OnDestroy()
        {
            discord.Dispose();
        }
    }
}