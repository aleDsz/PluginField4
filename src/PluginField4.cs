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

using System;

// Procon
using PRoCon.Core;
using PRoCon.Core.Plugin;
using PRoCon.Core.Plugin.Commands;
using PRoCon.Core.Players;
using PRoCon.Core.Players.Items;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Collections;

namespace PRoConEvents {
    /// <summary>
    /// The PluginField4 API wrapper
    /// </summary>
    class API {
        private string _apiKey;

        public API(string apiKey) {
            this._apiKey = apiKey;
        }

        /// <summary>
        /// Sends the event to PluginField4 API
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="eventData">The event data as stringfied JSON</param>
        public void SendEvent(string eventName, string eventData) {
            // 
        }
    }

	/// <summary>
	///	The PluginField4 plugin class
	/// </summary>
	public class PluginField4 : PRoConPluginAPI, IPRoConPluginInterface {
        private string _apiKey;
        private API _api;

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

        public PluginField4() {
            this.pluginList = new List<CPluginVariable>();
            this.pluginList.Add(new CPluginVariable("API|API Key", typeof(string), this._apiKey, false));
        }

        /// <summary>
        /// Event triggered when plugin is disabled at PRoCon Layer
        /// </summary>
        public void OnPluginDisable() {
            this._api = null;
        }

        /// <summary>
        /// Event triggered when plugin is enabled at PRoCon Layer
        /// </summary>
        public void OnPluginEnable() {
            this._api = new API(this._apiKey);
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
        private string[] GetAllEvents() {
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
            object _ = strVariable switch
            {
                "API|API Key" => this._apiKey = strValue,
                _ => null,
            };
        }

        // PluginField4 API

        /// <summary>
        /// The local API method to send a request using a Thread
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="eventData">The event data</param>
        private void SendEvent(string eventName, object eventData) {
            var jsonEventData = ConvertToJson(eventData);
            new Thread(() => this._api.SendEvent(eventName, jsonEventData)).Start();
        }

        /// <summary>
        /// Converts an objec into a stringfied JSON
        /// </summary>
        /// <param name="eventData">The given event data to be converted</param>
        private string ConvertToJson(object eventData) {
            return "{}";
        }

        // Chat Events

        /// <summary>
        /// Event triggered when a chat is sent to global chat
        /// </summary>
        /// <param name="speaker">The speaker who sent the message</param>
        /// <param name="message">The message sent</param>
        public override void OnGlobalChat(string speaker, string message) { SendEvent("OnGlobalChat", new object { }); }

        /// <summary>
        /// Event triggered when a chat is sent to team chat
        /// </summary>
        /// <param name="speaker">The speaker who sent the message</param>
        /// <param name="message">The message sent</param>
        /// <param name="teamId">ID of the team who received the message</param>
        public override void OnTeamChat(string speaker, string message, int teamId) { SendEvent("OnTeamChat", new object { }); }

        /// <summary>
        /// Event triggered when a chat is sent to squad chat
        /// </summary>
        /// <param name="speaker">The speaker who sent the message</param>
        /// <param name="message">The message sent</param>
        /// <param name="teamId">ID of the team who received the message</param>
        /// <param name="squadId">ID of the squad who received the message</param>
        public override void OnSquadChat(string speaker, string message, int teamId, int squadId) { SendEvent("OnSquadChat", new object { }); }

        // Round Over Events

        /// <summary>
        /// Event triggered when a round over and register the player's list
        /// </summary>
        /// <param name="playerList"></param>
        public override void OnRoundOverPlayers(List<CPlayerInfo> playerList) { SendEvent("OnRoundOverPlayers", new object { }); }

        /// <summary>
        /// Event triggered when a round over and register the team scores
        /// </summary>
        /// <param name="listTeamScores"></param>
        public override void OnRoundOverTeamScores(List<TeamScore> listTeamScores) { SendEvent("OnRoundOverTeamScores", new object { }); }

        /// <summary>
        /// Event triggered when a round over and register the ID of the winning team
        /// </summary>
        /// <param name="winningTeamId"></param>
        public override void OnRoundOver(int winningTeamId) { SendEvent("OnRoundOver", new object { }); }

        // Levels Events

        /// <summary>
        /// Event triggered when the level is loading
        /// </summary>
        /// <param name="mapName">Name of the map</param>
        /// <param name="quantityRoundsPlayed">Quantity of rounds played from given map</param>
        /// <param name="quantityRoundsTotal">Total of rounds from given map</param>
        public override void OnLoadingLevel(string mapName, int roundsPlayed, int roundsTotal) { SendEvent("OnLoadingLevel", new object { }); }

        /// <summary>
        /// Event triggered when the level loads
        /// </summary>
        public override void OnLevelStarted() { SendEvent("OnLevelStarted", new object { }); }

        // Player Actions

        /// <summary>
        /// Event triggered when a player is killed by an administrator
        /// </summary>
        /// <param name="soldierName">The player name who have been killed</param>
        public override void OnPlayerKilledByAdmin(string soldierName) { SendEvent("OnPlayerKilledByAdmin", new object { }); }

        /// <summary>
        /// Event triggered when a player is kicked by an administrator
        /// </summary>
        /// <param name="soldierName">The player name who have been kicked</param>
        /// <param name="reason">The kick reason</param>
        public override void OnPlayerKickedByAdmin(string soldierName, string reason) { SendEvent("OnPlayerKickedByAdmin", new object { }); }

        /// <summary>
        /// Event triggered when a player is team swaped by an administrator
        /// </summary>
        /// <param name="soldierName">The player name who have been team swaped</param>
        /// <param name="destinationTeamId">The team destination ID</param>
        /// <param name="destinationSquadId">The squad destination ID</param>
        /// <param name="isForceKilled">Flag if the player is forced to be dead to move</param>
        public override void OnPlayerMovedByAdmin(string soldierName, int destinationTeamId, int destinationSquadId, bool isForceKilled) {
            SendEvent("OnPlayerMovedByAdmin", new object { });
        }

        // Game Server Request Events

        /// <summary>
        /// Event triggered when a player joins the server
        /// </summary>
        /// <param name="soldierName">The player name who joined the server</param>
        public override void OnPlayerJoin(string soldierName) { SendEvent("OnPlayerJoin", new object { }); }

        /// <summary>
        /// Event triggered when a player lefts the server
        /// </summary>
        /// <param name="playerInfo">The player info who left the server</param>
        public override void OnPlayerLeft(CPlayerInfo playerInfo) { SendEvent("OnPlayerLeft", new object { }); }

        /// <summary>
        /// Event triggered when player is authenticated with EA servers
        /// </summary>
        /// <param name="soldierName">The player name who have been authenticated</param>
        /// <param name="guid">The EA GUID from player who have been authenticated</param>
        public override void OnPlayerAuthenticated(string soldierName, string guid) { SendEvent("OnPlayerAuthenticated", new object { }); }

        /// <summary>
        /// Event triggered when a player kills/dies by another player
        /// </summary>
        /// <param name="kill">The killer/victim info</param>
        public override void OnPlayerKilled(Kill kill) { SendEvent("OnPlayerKilled", new object { }); }

        /// <summary>
        /// Event triggered when a player is kicked by another actor
        /// </summary>
        /// <param name="soldierName">The player name who have been kicked</param>
        /// <param name="reason">The kick reason</param>
        public override void OnPlayerKicked(string soldierName, string reason) { SendEvent("OnPlayerKicked", new object { }); }

        /// <summary>
        /// Event triggered when a player spawns
        /// </summary>
        /// <param name="soldierName">The player name who have been spawned</param>
        /// <param name="spawnedInventory">The player loadout</param>
        public override void OnPlayerSpawned(string soldierName, Inventory spawnedInventory) { SendEvent("OnPlayerSpawned", new object { }); }

        /// <summary>
        /// Event triggered when a player team swaps
        /// </summary>
        /// <param name="soldierName">The player name who have been team swaped</param>
        /// <param name="teamId">The team destination ID</param>
        /// <param name="squadId">The squad destination ID</param>
        public override void OnPlayerTeamChange(string soldierName, int teamId, int squadId) { SendEvent("OnPlayerTeamChange", new object { }); }

        /// <summary>
        /// Event triggered when a player squad swaps
        /// </summary>
        /// <param name="soldierName">The player name who have been team swaped</param>
        /// <param name="teamId">The team destination ID</param>
        /// <param name="squadId">The squad destination ID</param>
        public override void OnPlayerSquadChange(string soldierName, int teamId, int squadId) { SendEvent("OnPlayerSquadChange", new object { }); }

        // Banlist

        /// <summary>
        /// Event triggered when a ban is added
        /// </summary>
        /// <param name="banInfo">The ban info</param>
        public override void OnBanAdded(CBanInfo banInfo) { SendEvent("OnBanAdded", new object { }); }

        /// <summary>
        /// Event triggered when a ban is removed
        /// </summary>
        /// <param name="banInfo">The ban info</param>
        public override void OnBanRemoved(CBanInfo banInfo) { SendEvent("OnBanRemoved", new object { }); }

        /// <summary>
        /// Event triggered when the ban is have been cleared
        /// </summary>
        public override void OnBanListClear() { SendEvent("OnBanListClear", new object { }); }

        /// <summary>
        /// Event triggered when the ban is have been saved
        /// </summary>
        public override void OnBanListSave() { SendEvent("OnBanListSave", new object { }); }

        /// <summary>
        /// Event triggered when the ban is have been loaded
        /// </summary>
        public override void OnBanListLoad() { SendEvent("OnBanListLoad", new object { }); }

        /// <summary>
        /// Event triggered when the ban list is requested
        /// </summary>
        /// <param name="banList">The ban info's list</param>
        public override void OnBanList(List<CBanInfo> banList) { SendEvent("OnBanList", new object { }); }

        // Map Functions

        /// <summary>
        /// Event triggered when a level have been restarted
        /// </summary>
        public override void OnRestartLevel() { SendEvent("OnRestartLevel", new object { }); }

        /// <summary>
        /// Event triggered when a player's list is requested
        /// </summary>
        /// <param name="playerList">The player's list from current round</param>
        /// <param name="playerSubset">The player subset</param>
        public override void OnListPlayers(List<CPlayerInfo> playerList, CPlayerSubset playerSubset) { SendEvent("OnListPlayers", new object { }); }

        /// <summary>
        /// Event triggered when a round ends
        /// </summary>
        /// <param name="winningTeamId">The ID of the winning team</param>
        public override void OnEndRound(int winningTeamId) { SendEvent("OnEndRound", new object { }); }

        /// <summary>
        /// Event triggered when is requested to change level
        /// </summary>
        public override void OnRunNextLevel() { SendEvent("OnRunNextLevel", new object { }); }

        /// <summary>
        /// Event triggered when is requested the current level
        /// </summary>
        /// <param name="mapName"></param>
        public override void OnCurrentLevel(string mapName) { SendEvent("OnCurrentLevel", new object { }); }

        // Global/Login

        /// <summary>
        /// Event triggered when a message is sent to be yelled
        /// </summary>
        /// <param name="message">The message sent to be yelled</param>
        /// <param name="messageDuration">The yelling message duration</param>
        /// <param name="playerSubset">The player subset</param>
        public override void OnYelling(string message, int messageDuration, CPlayerSubset playerSubset) { SendEvent("OnYelling", new object { }); }

        /// <summary>
        /// Event triggered when a message is sent to be displayed on global chat
        /// </summary>
        /// <param name="message">The message sent to be displayed on global chat</param>
        /// <param name="playerSubset">The player subset</param>
        public override void OnSaying(string message, CPlayerSubset playerSubset) { SendEvent("OnSaying", new object { }); }
    }
}
