/*  Copyright 2021 aleDsz 
 
    This plugin file is part of PRoCon Frostbite.

    This plugin is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This plugin is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with PRoCon Frostbite.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Net;

// Procon
using PRoCon.Core;
using PRoCon.Core.Plugin;
using PRoCon.Core.Players;
using PRoCon.Core.Players.Items;
using System.Collections;

namespace PRoConEvents {
    public interface IAPI {
        void SetApiKey(string apiKey);
        void SendEvent(string eventName, string eventData);
    }

    /// <summary>
    /// The PluginField4 API wrapper
    /// </summary>
    class API : IAPI {
        private string _baseUrl => "https://edf7972db9ea633a52a187e67c8c66d2.m.pipedream.net";
        private string _apiKey;

        /// <summary>
        /// Sets the API key
        /// </summary>
        /// <param name="apiKey">The PluginField4 API key</param>
        public void SetApiKey(string apiKey) { this._apiKey = apiKey; }

        /// <summary>
        /// Sends the event to PluginField4 API
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="eventData">The event data as stringfied JSON</param>
        public void SendEvent(string eventName, string eventData) {
            var endpoint = string.Format("/events/{0}", eventName);
            var apiKey = string.Format("Bearer {0}", this._apiKey);

            using (var webClient = new WebClient()) {
                webClient.Headers.Add("Authorization", apiKey);
                webClient.BaseAddress = this._baseUrl;

                webClient.UploadData(endpoint, "POST", new byte[0]);
            }
        }
    }

    /// <summary>
    ///	The PluginField4 plugin class
    /// </summary>
    public class PluginField4 : PRoConPluginAPI, IPRoConPluginInterface {
        private string _apiKey;
        private IAPI _api;

        public string version = "0.1.0";

        private string author = "aleDsz";
        private string description = "The Ultimate Battlefield 4 Plugin to send event data to API";
        private string name = "PluginField4 Database";
        private string website = "https://github.com/aledsz/PluginField4";

        private List<CPluginVariable> pluginList;

        public string GetPluginAuthor() => author;
        public string GetPluginDescription() => description;
        public string GetPluginName() => name;
        public string GetPluginVersion() => version;
        public string GetPluginWebsite() => website;
        public List<CPluginVariable> GetPluginVariables() => pluginList;
        public List<CPluginVariable> GetDisplayPluginVariables() => pluginList;

        public PluginField4(IAPI api = null) {
            if (api == null) {
                this._api = new API();
            } else {
                this._api = api;
            }

            this.pluginList = new List<CPluginVariable>();
            this.pluginList.Add(new CPluginVariable("API Key", typeof(string), this._apiKey, false));
        }

        /// <summary>
        /// Event triggered when plugin is disabled at PRoCon Layer
        /// </summary>
        public void OnPluginDisable() {
            this._api = null;

            this.ExecuteCommand("procon.protected.tasks.remove", "PluginField4");
        }

        /// <summary>
        /// Event triggered when plugin is enabled at PRoCon Layer
        /// </summary>
        public void OnPluginEnable() {
            this._api.SetApiKey(this._apiKey);
        }

        /// <summary>
        /// Load plugin inside PRoCon layer
        /// </summary>
        /// <param name="strHostName">Server Hostname</param>
        /// <param name="strPort">Server port as string</param>
        /// <param name="strPRoConVersion">PRoCon Server version</param>
        public void OnPluginLoaded(string strHostName, string strPort, string strPRoConVersion) {
            this.RegisterEvents(this.GetType().Name, GetAllEvents());
        }

        /// <summary>
        /// Returns a list of PRoCon events from current class
        /// </summary>
        /// <returns>The list of events to be registered</returns>
        public string[] GetAllEvents() {
            List<string> events = new List<string>();
            var type = typeof(PluginField4);

            foreach (MethodInfo methodInfo in type.GetMethods()) {
                if (methodInfo.GetBaseDefinition().DeclaringType != methodInfo.DeclaringType) {
                    events.Add(methodInfo.Name);
                }
            }

            return events.ToArray();
        }

        /// <summary>
        /// Set plugin variables into C# variables
        /// </summary>
        /// <param name="strVariable">Plugin variable name as string</param>
        /// <param name="strValue">Plugin variable value as string</param>
        public void SetPluginVariable(string strVariable, string strValue) {
            if (strVariable.Equals("API Key")) {
                this._apiKey = strValue;
            };
        }

        // Converters

        /// <summary>
        /// Converts the given <c>CPlayerInfo</c> to <c>Hashtable</c>
        /// </summary>
        /// <param name="playerInfo">The Player info</param>
        /// <returns>A Hashtable to be converted to JSON</returns>
        public Hashtable PlayerInfoToHastable(CPlayerInfo playerInfo) {
            return new Hashtable {
                { "guid", playerInfo.GUID },
                { "rank", playerInfo.Rank },
                { "clan_tag", playerInfo.ClanTag },
                { "solder_name", playerInfo.SoldierName },
                { "team_id", playerInfo.TeamID },
                { "squad_id", playerInfo.SquadID },
                { "join_time", playerInfo.JoinTime },
                { "score", playerInfo.Score },
                { "kills", playerInfo.Kills },
                { "deaths", playerInfo.Deaths },
                { "kdr", playerInfo.Kdr },
                { "ping", playerInfo.Ping },
                { "session_time", playerInfo.SessionTime }
            };
        }

        /// <summary>
        /// Converts the given <c>Inventory</c> to <c>Hashtable</c>
        /// </summary>
        /// <param name="inventory">The player Inventory</param>
        /// <returns>A Hashtable to be converted to JSON</returns>
        public Hashtable InventoryToHashtable(Inventory inventory) {
            var weapons = new List<Hashtable>();
            var specializations = new List<Hashtable>();

            foreach (var weapon in inventory.Weapons) {
                weapons.Add(WeaponToHashtable(weapon));
            }

            foreach (var specialization in inventory.Specializations) {
                specializations.Add(SpecializationToHashtable(specialization));
            }

            return new Hashtable {
                { "kit", inventory.Kit.ToString() },
                { "weapons", weapons },
                { "specializations", specializations }
            };
        }

        /// <summary>
        /// Converts the given <c>Weapon</c> to <c>Hashtable</c>
        /// </summary>
        /// <param name="weapon">The weapon</param>
        /// <returns>A Hashtable to be converted to JSON</returns>
        public Hashtable WeaponToHashtable(Weapon weapon) {
            return new Hashtable {
                { "slot", weapon.Slot.ToString() },
                { "name", weapon.Name },
                { "kit_restriction", weapon.KitRestriction.ToString() },
                { "damage", weapon.Damage.ToString() }
            };
        }

        /// <summary>
        /// Converts the given <c>Specialization</c> to <c>Hashtable</c>
        /// </summary>
        /// <param name="specialization">The specialization</param>
        /// <returns>A Hashtable to be converted to JSON</returns>
        public Hashtable SpecializationToHashtable(Specialization specialization) {
            return new Hashtable {
                { "slot", specialization.Slot.ToString() },
                { "name", specialization.Name }
            };
        }

        /// <summary>
        /// Converts the given <c>CBanInfo</c> to <c>Hashtable</c>
        /// </summary>
        /// <param name="banInfo">The ban info</param>
        /// <returns>A Hashtable to be converted to JSON</returns>
        public Hashtable BanInfoToHashtable(CBanInfo banInfo) {
            return new Hashtable {
                { "guid", banInfo.Guid },
                { "soldier_name", banInfo.SoldierName },
                { "id_type", banInfo.IdType },
                { "reason", banInfo.Reason },
                { "ban_length", TimeoutSubsetToHashtable(banInfo.BanLength) },
                { "ip_address", banInfo.IpAddress },
                { "offset", banInfo.Offset }
            };
        }

        /// <summary>
        /// Converts the given <c>TimeoutSubset</c> to <c>Hashtable</c>
        /// </summary>
        /// <param name="timeoutSubset">The timeout subset</param>
        /// <returns>A Hashtable to be converted to JSON</returns>
        public Hashtable TimeoutSubsetToHashtable(TimeoutSubset timeoutSubset) {
            return new Hashtable {
                { "timeout", timeoutSubset.Timeout },
                { "subset", timeoutSubset.Subset.ToString() }
            };
        }

        /// <summary>
        /// Converts the given <c>CPlayerSubset</c> to <c>Hashtable</c>
        /// </summary>
        /// <param name="banInfo">The player subset</param>
        /// <returns>A Hashtable to be converted to JSON</returns>
        public Hashtable PlayerSubsetToHashtable(CPlayerSubset playerSubset) {
            return new Hashtable {
                { "soldier_name", playerSubset.SoldierName },
                { "team_id", playerSubset.TeamID },
                { "squad_id", playerSubset.SquadID },
                { "subset", playerSubset.Subset }
            };
        }

        /// <summary>
        /// Converts the given <c>Kill</c> to <c>Hashtable</c>
        /// </summary>
        /// <param name="kill">The Kill</param>
        /// <returns>A Hashtable to be converted to JSON</returns>
        public Hashtable KillToHashtable(Kill kill)
        {
            return new Hashtable {
                { "killer", PlayerInfoToHastable(kill.Killer) },
                { "victim", PlayerInfoToHastable(kill.Victim) },
                { "distance", kill.Distance },
                { "is_headshot", kill.Headshot },
                { "is_suicide", kill.IsSuicide },
                { "damage_type", kill.DamageType },
                { "killer_location", LocationToHashtable(kill.KillerLocation) },
                { "victim_location", LocationToHashtable(kill.VictimLocation) },
                { "time_to_death", kill.TimeOfDeath.ToLongTimeString() }
            };
        }

        /// <summary>
        /// Converts the given <c>Point3D</c> to <c>Hashtable</c>
        /// </summary>
        /// <param name="point3D">The Point3D location</param>
        /// <returns>A Hashtable to be converted to JSON</returns>
        public Hashtable LocationToHashtable(Point3D point3D) {
            return new Hashtable {
                { "x", point3D.X },
                { "y", point3D.Y },
                { "z", point3D.Z }
            };
        }

        // PluginField4 API

        /// <summary>
        /// The local API method to send a request using a Thread
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="eventData">The event hashtable data</param>
        private void SendEvent(string eventName, Hashtable eventData) {
            var normalizedHashtable = NormalizeHashtable(eventData);
            var jsonEventData = JSON.JsonEncode(new Hashtable { { "data", normalizedHashtable } });
            var thread = new Thread(() => this._api.SendEvent(eventName, jsonEventData));

            thread.Start();
            thread.Join();
        }

        /// <summary>
        /// Normalize array fields from given Hashtable
        /// </summary>
        /// <param name="data">The hashtable to be normalized</param>
        /// <return>A normalized hashtable</return>
        private Hashtable NormalizeHashtable(Hashtable data) {
            var normalizedHashtable = new Hashtable();

            foreach (var key in data) {
                var value = data[key];

                if (value is Hashtable[]) {
                    string stringfiedArray = "[";

                    foreach (var item in (Hashtable[])value) {
                        var stringValue = JSON.JsonEncode(item);
                        stringfiedArray += string.Format("{0},", stringValue);
                    }

                    normalizedHashtable[key] = stringfiedArray;
                } else {
                    normalizedHashtable[key] = value;
                }
            }

            return normalizedHashtable;
        }

        // Chat Events

        /// <summary>
        /// Event triggered when a chat is sent to global chat
        /// </summary>
        /// <param name="speaker">The speaker who sent the message</param>
        /// <param name="message">The message sent</param>
        public override void OnGlobalChat(string speaker, string message) {
            SendEvent("OnGlobalChat", new Hashtable {
                { "speaker", speaker },
                { "message", message }
            });
        }

        /// <summary>
        /// Event triggered when a chat is sent to team chat
        /// </summary>
        /// <param name="speaker">The speaker who sent the message</param>
        /// <param name="message">The message sent</param>
        /// <param name="teamId">ID of the team who received the message</param>
        public override void OnTeamChat(string speaker, string message, int teamId) {
            SendEvent("OnTeamChat", new Hashtable {
                { "speaker", speaker },
                { "message", message },
                { "team_id", teamId }
            });
        }

        /// <summary>
        /// Event triggered when a chat is sent to squad chat
        /// </summary>
        /// <param name="speaker">The speaker who sent the message</param>
        /// <param name="message">The message sent</param>
        /// <param name="teamId">ID of the team who received the message</param>
        /// <param name="squadId">ID of the squad who received the message</param>
        public override void OnSquadChat(string speaker, string message, int teamId, int squadId) {
            SendEvent("OnSquadChat", new Hashtable {
                { "speaker", speaker },
                { "message", message },
                { "team_id", teamId },
                { "squad_id", squadId }
            });
        }

        // Round Over Events

        /// <summary>
        /// Event triggered when a round over and register the player's list
        /// </summary>
        /// <param name="playerList"></param>
        public override void OnRoundOverPlayers(List<CPlayerInfo> playerList) {
            List<Hashtable> players = new List<Hashtable>();

            foreach (var playerInfo in playerList) {
                players.Add(PlayerInfoToHastable(playerInfo));
            }

            SendEvent("OnRoundOverPlayers", new Hashtable {
                { "players", players.ToArray() }
            });
        }

        /// <summary>
        /// Event triggered when a round over and register the team scores
        /// </summary>
        /// <param name="listTeamScores"></param>
        public override void OnRoundOverTeamScores(List<TeamScore> listTeamScores) {
            List<Hashtable> teamScores = new List<Hashtable>();

            foreach (var teamScore in listTeamScores)
            {
                teamScores.Add(new Hashtable {
                    { "team_id", teamScore.TeamID },
                    { "score", teamScore.Score },
                    { "winning_score", teamScore.WinningScore }
                });
            }

            SendEvent("OnRoundOverTeamScores", new Hashtable {
                { "team_scores", teamScores.ToArray() }
            });
        }

        /// <summary>
        /// Event triggered when a round over and register the ID of the winning team
        /// </summary>
        /// <param name="winningTeamId"></param>
        public override void OnRoundOver(int winningTeamId) {
            SendEvent("OnRoundOver", new Hashtable {
                { "team_id", winningTeamId }
            });
        }

        // Levels Events

        /// <summary>
        /// Event triggered when the level is loading
        /// </summary>
        /// <param name="mapName">Name of the map</param>
        /// <param name="quantityRoundsPlayed">Quantity of rounds played from given map</param>
        /// <param name="quantityRoundsTotal">Total of rounds from given map</param>
        public override void OnLoadingLevel(string mapName, int roundsPlayed, int roundsTotal) {
            SendEvent("OnLoadingLevel", new Hashtable {
                { "map_name", mapName },
                { "rounds_played", roundsPlayed },
                { "total_rounds", roundsTotal }
            });
        }

        /// <summary>
        /// Event triggered when the level loads
        /// </summary>
        public override void OnLevelStarted() {
            SendEvent("OnLevelStarted", new Hashtable { });
        }

        // Player Actions

        /// <summary>
        /// Event triggered when a player is killed by an administrator
        /// </summary>
        /// <param name="soldierName">The player name who have been killed</param>
        public override void OnPlayerKilledByAdmin(string soldierName) {
            SendEvent("OnPlayerKilledByAdmin", new Hashtable {
                { "soldier_name", soldierName }
            });
        }

        /// <summary>
        /// Event triggered when a player is kicked by an administrator
        /// </summary>
        /// <param name="soldierName">The player name who have been kicked</param>
        /// <param name="reason">The kick reason</param>
        public override void OnPlayerKickedByAdmin(string soldierName, string reason) {
            SendEvent("OnPlayerKickedByAdmin", new Hashtable {
                { "soldier_name", soldierName },
                { "reason", reason }
            });
        }

        /// <summary>
        /// Event triggered when a player is team swaped by an administrator
        /// </summary>
        /// <param name="soldierName">The player name who have been team swaped</param>
        /// <param name="destinationTeamId">The team destination ID</param>
        /// <param name="destinationSquadId">The squad destination ID</param>
        /// <param name="isForceKilled">Flag if the player is forced to be dead to move</param>
        public override void OnPlayerMovedByAdmin(string soldierName, int destinationTeamId, int destinationSquadId, bool isForceKilled) {
            SendEvent("OnPlayerMovedByAdmin", new Hashtable {
                { "soldier_name", soldierName },
                { "destination_team_id", destinationTeamId },
                { "destination_squad_id", destinationSquadId },
                { "is_force_killed", isForceKilled }
            });
        }

        // Game Server Request Events

        /// <summary>
        /// Event triggered when a player joins the server
        /// </summary>
        /// <param name="soldierName">The player name who joined the server</param>
        public override void OnPlayerJoin(string soldierName) {
            SendEvent("OnPlayerJoin", new Hashtable {
                { "soldier_name", soldierName }
            });
        }

        /// <summary>
        /// Event triggered when a player lefts the server
        /// </summary>
        /// <param name="playerInfo">The player info who left the server</param>
        public override void OnPlayerLeft(CPlayerInfo playerInfo) {
            SendEvent("OnPlayerLeft", PlayerInfoToHastable(playerInfo));
        }

        /// <summary>
        /// Event triggered when player is authenticated with EA servers
        /// </summary>
        /// <param name="soldierName">The player name who have been authenticated</param>
        /// <param name="guid">The EA GUID from player who have been authenticated</param>
        public override void OnPlayerAuthenticated(string soldierName, string guid) {
            SendEvent("OnPlayerAuthenticated", new Hashtable {
                { "guid", guid },
                { "soldier_name", soldierName }
            });
        }

        /// <summary>
        /// Event triggered when a player kills/dies by another player
        /// </summary>
        /// <param name="kill">The killer/victim info</param>
        public override void OnPlayerKilled(Kill kill) {
            SendEvent("OnPlayerKilled", KillToHashtable(kill));
        }

        /// <summary>
        /// Event triggered when a player is kicked by another actor
        /// </summary>
        /// <param name="soldierName">The player name who have been kicked</param>
        /// <param name="reason">The kick reason</param>
        public override void OnPlayerKicked(string soldierName, string reason) {
            SendEvent("OnPlayerKicked", new Hashtable {
                { "soldier_name", soldierName },
                { "reason", reason },
            });
        }

        /// <summary>
        /// Event triggered when a player spawns
        /// </summary>
        /// <param name="soldierName">The player name who have been spawned</param>
        /// <param name="spawnedInventory">The player loadout</param>
        public override void OnPlayerSpawned(string soldierName, Inventory spawnedInventory) {
            SendEvent("OnPlayerSpawned", new Hashtable {
                { "soldier_name", soldierName },
                { "inventory", InventoryToHashtable(spawnedInventory) }
            });
        }

        /// <summary>
        /// Event triggered when a player team swaps
        /// </summary>
        /// <param name="soldierName">The player name who have been team swaped</param>
        /// <param name="teamId">The team destination ID</param>
        /// <param name="squadId">The squad destination ID</param>
        public override void OnPlayerTeamChange(string soldierName, int teamId, int squadId) {
            SendEvent("OnPlayerTeamChange", new Hashtable {
                { "soldier_name", soldierName },
                { "team_id", teamId },
                { "squad_id", squadId }
            });
        }

        /// <summary>
        /// Event triggered when a player squad swaps
        /// </summary>
        /// <param name="soldierName">The player name who have been team swaped</param>
        /// <param name="teamId">The team destination ID</param>
        /// <param name="squadId">The squad destination ID</param>
        public override void OnPlayerSquadChange(string soldierName, int teamId, int squadId) {
            SendEvent("OnPlayerSquadChange", new Hashtable {
                { "soldier_name", soldierName },
                { "team_id", teamId },
                { "squad_id", squadId }
            });
        }

        // Banlist

        /// <summary>
        /// Event triggered when a ban is added
        /// </summary>
        /// <param name="banInfo">The ban info</param>
        public override void OnBanAdded(CBanInfo banInfo) {
            SendEvent("OnBanAdded", BanInfoToHashtable(banInfo));
        }

        /// <summary>
        /// Event triggered when a ban is removed
        /// </summary>
        /// <param name="banInfo">The ban info</param>
        public override void OnBanRemoved(CBanInfo banInfo) {
            SendEvent("OnBanRemoved", BanInfoToHashtable(banInfo));
        }

        /// <summary>
        /// Event triggered when the ban is have been loaded
        /// </summary>
        public override void OnBanListLoad() { SendEvent("OnBanListLoad", new Hashtable { }); }

        /// <summary>
        /// Event triggered when the ban list is requested
        /// </summary>
        /// <param name="banList">The ban info's list</param>
        public override void OnBanList(List<CBanInfo> banList) {
            var bans = new List<Hashtable>();

            foreach (var banInfo in banList) {
                bans.Add(BanInfoToHashtable(banInfo));
            }
            
            SendEvent("OnBanList", new Hashtable {
                { "bans", bans.ToArray() }
            });
        }

        // Map Functions

        /// <summary>
        /// Event triggered when a level have been restarted
        /// </summary>
        public override void OnRestartLevel() { SendEvent("OnRestartLevel", new Hashtable { }); }

        /// <summary>
        /// Event triggered when a player's list is requested
        /// </summary>
        /// <param name="playerList">The player's list from current round</param>
        /// <param name="playerSubset">The player subset</param>
        public override void OnListPlayers(List<CPlayerInfo> playerList, CPlayerSubset playerSubset) {
            var players = new List<Hashtable>();

            foreach (var playerInfo in playerList) {
                players.Add(PlayerInfoToHastable(playerInfo));
            }

            SendEvent("OnListPlayers", new Hashtable {
                { "players", players },
                { "subset", PlayerSubsetToHashtable(playerSubset) }
            });
        }

        /// <summary>
        /// Event triggered when a round ends
        /// </summary>
        /// <param name="winningTeamId">The ID of the winning team</param>
        public override void OnEndRound(int winningTeamId) {
            SendEvent("OnEndRound", new Hashtable {
                { "team_id", winningTeamId }
            });
        }

        /// <summary>
        /// Event triggered when is requested to change level
        /// </summary>
        public override void OnRunNextLevel() { SendEvent("OnRunNextLevel", new Hashtable { }); }

        /// <summary>
        /// Event triggered when is requested the current level
        /// </summary>
        /// <param name="mapName"></param>
        public override void OnCurrentLevel(string mapName) {
            SendEvent("OnCurrentLevel", new Hashtable {
                { "map_name", mapName }
            });
        }

        // Global/Login

        /// <summary>
        /// Event triggered when a message is sent to be yelled
        /// </summary>
        /// <param name="message">The message sent to be yelled</param>
        /// <param name="messageDuration">The yelling message duration</param>
        /// <param name="playerSubset">The player subset</param>
        public override void OnYelling(string message, int messageDuration, CPlayerSubset playerSubset) {
            SendEvent("OnYelling", new Hashtable {
                { "message", message },
                { "duration", messageDuration },
                { "subset", PlayerSubsetToHashtable(playerSubset) }
            });
        }

        /// <summary>
        /// Event triggered when a message is sent to be displayed on global chat
        /// </summary>
        /// <param name="message">The message sent to be displayed on global chat</param>
        /// <param name="playerSubset">The player subset</param>
        public override void OnSaying(string message, CPlayerSubset playerSubset) {
            SendEvent("OnSaying", new Hashtable {
                { "message", message },
                { "subset", PlayerSubsetToHashtable(playerSubset) }
            });
        }
    }
}
