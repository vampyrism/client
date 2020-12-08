using System.Collections;
using UnityEngine;
using Discord;

namespace Assets.Scripts
{
    public class DiscordController : MonoBehaviour
    {
        public Discord.Discord discord;

        // Use this for initialization
        void Start()
        {
            this.discord = new Discord.Discord(785086130143887391, (System.UInt64)Discord.CreateFlags.Default);
            this.discord.SetLogHook(Discord.LogLevel.Debug, (Discord.LogLevel level, string message) =>
            {
                Debug.Log(message);
            });
            var activityManager = discord.GetActivityManager();

            var activity = new Discord.Activity
            {
                State = "In-Game",
                Details = "With tons of other players",
                /*Party = new Discord.ActivityParty
                {
                    Id = "secretysecretparty",
                    Size = new Discord.PartySize
                    {
                        CurrentSize = 1,
                        MaxSize = 100
                    }
                },
                Secrets = new Discord.ActivitySecrets
                {
                    Match = "secretmatchsecret",
                    Join = "secretjoinsecret",
                    Spectate = "secretspectatesecret"
                }*/
            };

            activityManager.UpdateActivity(activity, (res) =>
            {
                if (res == Discord.Result.Ok)
                {
                    Debug.LogError("Everything is fine!");
                }
            });

            var lobbyManager = discord.GetLobbyManager();
            var txn = lobbyManager.GetLobbyCreateTransaction();

            // Set lobby information
            txn.SetCapacity(100);
            txn.SetType(Discord.LobbyType.Public);

            lobbyManager.CreateLobby(txn, (Discord.Result result, ref Discord.Lobby lobby) =>
            {
                Debug.Log("lobby " + lobby.Id + " created with secret " + lobby.Secret);

                var a = new Discord.Activity
                {
                    Party =
                    {
                        Id = lobby.Id.ToString(),
                        Size =
                        {
                            CurrentSize = 1,
                            MaxSize = (int) lobby.Capacity
                        }
                    },
                    Secrets =
                    {
                        Join = lobbyManager.GetLobbyActivitySecret(lobby.Id)
                    }
                };

                activityManager.UpdateActivity(a, (Discord.Result res) =>
                {
                });

                /*lobbyManager.ConnectVoice(lobby.Id, (Discord.Result res) =>
                {
                    Debug.Log("Voice status: " + res);
                });*/
            });

            lobbyManager.OnSpeaking += (System.Int64 lobby, System.Int64 userId, bool speaking) => {
                Debug.Log("In lobby " + lobby + " user with id " + userId + " is speaking " + speaking);
            };
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