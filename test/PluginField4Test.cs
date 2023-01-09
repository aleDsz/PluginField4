using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using PRoCon.Core;
using PRoCon.Core.Players;
using PRoCon.Core.Players.Items;

namespace PRoConEvents.Tests
{
    [TestClass]
    public class PluginField4Test {
        [TestMethod]
        public void GetPluginVariables() {
            var expectedPluginVariables = new List<CPluginVariable>();

            expectedPluginVariables.Add(new CPluginVariable("API Key", typeof(string), "", false));

            var mock = new Mock<IAPI>();
            var pluginField4 = new PluginField4(mock.Object);

            var pluginVariables = pluginField4.GetPluginVariables();

            for (var i = 0; i < expectedPluginVariables.Count; i++) {
                var pluginVariable = pluginVariables[i];
                var expectedPluginVariable = expectedPluginVariables[i];

                Assert.AreEqual(expectedPluginVariable.Name, pluginVariable.Name);
                Assert.AreEqual(expectedPluginVariable.Value, pluginVariable.Value);
                Assert.AreEqual(expectedPluginVariable.Type, pluginVariable.Type);
                Assert.AreEqual(expectedPluginVariable.ReadOnly, pluginVariable.ReadOnly);
            }

            mock.VerifyAll();
        }

        [TestMethod]
        public void GetDisplayPluginVariables() {
            var expectedPluginVariables = new List<CPluginVariable>();

            expectedPluginVariables.Add(new CPluginVariable("API Key", typeof(string), "", false));

            var mock = new Mock<IAPI>();
            var pluginField4 = new PluginField4(mock.Object);

            var displayPluginVariables = pluginField4.GetDisplayPluginVariables();

            for (var i = 0; i < expectedPluginVariables.Count; i++) {
                var displayPluginVariable = displayPluginVariables[i];
                var expectedPluginVariable = expectedPluginVariables[i];

                Assert.AreEqual(expectedPluginVariable.Name, displayPluginVariable.Name);
                Assert.AreEqual(expectedPluginVariable.Value, displayPluginVariable.Value);
                Assert.AreEqual(expectedPluginVariable.Type, displayPluginVariable.Type);
                Assert.AreEqual(expectedPluginVariable.ReadOnly, displayPluginVariable.ReadOnly);
            }

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnPluginEnable() {
            string expectedApiKey = "123456789";

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SetApiKey(
                    It.Is<string>(v => v.Equals(expectedApiKey))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.SetPluginVariable("API Key", expectedApiKey);
            pluginField4.OnPluginEnable();

            mock.VerifyAll();
        }

        [TestMethod]
        public void GetAllEvents() {
            var expectedEvents = new string[]
            {
                "OnGlobalChat",
                "OnTeamChat",
                "OnSquadChat",
                "OnRoundOverPlayers",
                "OnRoundOverTeamScores",
                "OnRoundOver",
                "OnLoadingLevel",
                "OnLevelStarted",
                "OnPlayerKilledByAdmin",
                "OnPlayerKickedByAdmin",
                "OnPlayerMovedByAdmin",
                "OnPlayerJoin",
                "OnPlayerLeft",
                "OnPlayerAuthenticated",
                "OnPlayerKilled",
                "OnPlayerKicked",
                "OnPlayerSpawned",
                "OnPlayerTeamChange",
                "OnPlayerSquadChange",
                "OnBanAdded",
                "OnBanRemoved",
                "OnBanListLoad",
                "OnBanList",
                "OnRestartLevel",
                "OnListPlayers",
                "OnEndRound",
                "OnRunNextLevel",
                "OnCurrentLevel",
                "OnYelling",
                "OnSaying"
            };

            var mock = new Mock<IAPI>();
            var pluginField4 = new PluginField4(mock.Object);
            var allEvents = pluginField4.GetAllEvents();

            Assert.AreEqual(expectedEvents.Length, allEvents.Length);

            for (var i = 0; i < expectedEvents.Length; i++)
                Assert.AreEqual(expectedEvents[i], allEvents[i]);
        }

        [TestMethod]
        public void SetPluginVariable() {
            string expectedApiKey = "123456789";

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SetApiKey(
                    It.Is<string>(v => v.Equals(expectedApiKey))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.SetPluginVariable("API Key", expectedApiKey);
            pluginField4.OnPluginEnable();

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnGlobalChat() {
            var expectedEventName = "OnGlobalChat";
            var eventData = new Hashtable {
                { "speaker", "Foo" },
                { "message", "Bar" }
            };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnGlobalChat("Foo", "Bar");

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnTeamChat() {
            var expectedEventName = "OnTeamChat";
            var eventData = new Hashtable {
                { "speaker", "Foo" },
                { "message", "Bar" },
                { "team_id", 1 }
            };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnTeamChat("Foo", "Bar", 1);

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnSquadChat() {
            var expectedEventName = "OnSquadChat";
            var eventData = new Hashtable {
                { "speaker", "Foo" },
                { "message", "Bar" },
                { "team_id", 1 },
                { "squad_id", 1 }
            };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnSquadChat("Foo", "Bar", 1, 1);

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnRoundOverPlayers() {
            (var hashtableArray, var playerInfoList) = Helper.BuildPlayerInfoList();

            var expectedEventName = "OnRoundOverPlayers";
            var eventData = new Hashtable { { "players", hashtableArray } };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnRoundOverPlayers(playerInfoList);

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnRoundOverTeamScores() {
            (var hashtableArray, var teamScoreList) = Helper.BuildTeamScoreList();

            var expectedEventName = "OnRoundOverTeamScores";
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(hashtableArray));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnRoundOverTeamScores(teamScoreList);

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnRoundOver() {
            var expectedEventName = "OnRoundOver";
            var eventData = new Hashtable { { "team_id", 1 } };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnRoundOver(1);

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnLoadingLevel() {
            var expectedEventName = "OnLoadingLevel";
            var eventData = new Hashtable {
                { "map_name", "MP_Prison" },
                { "rounds_played", 0 },
                { "total_rounds", 2 }
            };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnLoadingLevel("MP_Prison", 0, 2);

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnLevelStarted() {
            var expectedEventName = "OnLevelStarted";
            var eventData = new Hashtable { };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnLevelStarted();

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnPlayerKilledByAdmin() {
            var expectedEventName = "OnPlayerKilledByAdmin";
            var eventData = new Hashtable {
                { "soldier_name", "Foo" }
            };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnPlayerKilledByAdmin("Foo");

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnPlayerKickedByAdmin() {
            var expectedEventName = "OnPlayerKickedByAdmin";
            var eventData = new Hashtable {
               { "soldier_name", "Foo" },
               { "reason", "Bar" }
            };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnPlayerKickedByAdmin("Foo", "Bar");

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnPlayerMovedByAdmin() {
            var expectedEventName = "OnPlayerMovedByAdmin";
            var eventData = new Hashtable {
               { "soldier_name", "Foo" },
               { "destination_team_id", 1 },
               { "destination_squad_id", 2 },
               { "is_force_killed", false }
            };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnPlayerMovedByAdmin("Foo", 1, 2, false);

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnPlayerJoin() {
            var expectedEventName = "OnPlayerJoin";
            var eventData = new Hashtable {
                { "soldier_name", "Foo" }
            };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnPlayerJoin("Foo");

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnPlayerLeft() {
            var expectedEventName = "OnPlayerLeft";
            (var eventData, var playerInfo) = Helper.BuildPlayerInfo();
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnPlayerLeft(playerInfo);

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnPlayerAuthenticated() {
            var expectedEventName = "OnPlayerAuthenticated";
            var eventData = new Hashtable {
                { "soldier_name", "Foo" },
                { "guid", "123456789" }
            };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnPlayerAuthenticated("Foo", "123456789");

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnPlayerKilled() {
            var expectedEventName = "OnPlayerKilled";
            (var eventData, var kill) = Helper.BuildKill();
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnPlayerKilled(kill);

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnPlayerKicked() {
            var expectedEventName = "OnPlayerKicked";
            var eventData = new Hashtable {
               { "soldier_name", "Foo" },
               { "reason", "Bar" }
            };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnPlayerKicked("Foo", "Bar");

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnPlayerSpawned() {
            (var hashtable, var inventory) = Helper.BuildInventory();

            var expectedEventName = "OnPlayerSpawned";
            var eventData = new Hashtable {
                { "soldier_name", "Foo" },
                { "inventory", hashtable }
            };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnPlayerSpawned("Foo", inventory);

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnPlayerTeamChange() {
            var expectedEventName = "OnPlayerTeamChange";
            var eventData = new Hashtable {
               { "soldier_name", "Foo" },
               { "team_id", 1 },
               { "squad_id", 2 }
            };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnPlayerTeamChange("Foo", 1, 2);

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnPlayerSquadChange() {
            var expectedEventName = "OnPlayerSquadChange";
            var eventData = new Hashtable {
               { "soldier_name", "Foo" },
               { "team_id", 1 },
               { "squad_id", 2 }
            };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnPlayerSquadChange("Foo", 1, 2);

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnBanAdded() {
            var expectedEventName = "OnBanAdded";
            (var eventData, var banInfo) = Helper.BuildBanInfo();
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnBanAdded(banInfo);

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnBanRemoved() {
            var expectedEventName = "OnBanRemoved";
            (var eventData, var banInfo) = Helper.BuildBanInfo();
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnBanRemoved(banInfo);

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnBanListLoad() {
            var expectedEventName = "OnBanListLoad";
            var eventData = new Hashtable { };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnBanListLoad();

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnBanList() {
            (var hashtableArray, var banInfoList) = Helper.BuildBanInfoList();

            var expectedEventName = "OnBanList";
            var eventData = new Hashtable {
                { "bans", hashtableArray }
            };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnBanList(banInfoList);

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnRestartLevel() {
            var expectedEventName = "OnRestartLevel";
            var eventData = new Hashtable { };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnRestartLevel();

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnListPlayers() {
            (var hashtablePlayerInfoList, var playerInfoList) = Helper.BuildPlayerInfoList();
            (var hashtableSubset, var playerSubset) = Helper.BuildPlayerSquadSubset();

            var expectedEventName = "OnListPlayers";
            var eventData = new Hashtable {
                { "players", hashtablePlayerInfoList[0] },
                { "subset", hashtableSubset }
            };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnListPlayers(playerInfoList, playerSubset);

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnEndRound() {
            var expectedEventName = "OnEndRound";
            var eventData = new Hashtable { { "team_id", 1 } };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnEndRound(1);

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnRunNextLevel() {
            var expectedEventName = "OnRunNextLevel";
            var eventData = new Hashtable { };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnRunNextLevel();

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnCurrentLevel() {
            var expectedEventName = "OnCurrentLevel";
            var eventData = new Hashtable { { "map_name", "MP_Prison" } };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnCurrentLevel("MP_Prison");

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnYelling() {
            (var hashtable, var playerSubset) = Helper.BuildPlayerSquadSubset();

            var expectedEventName = "OnYelling";
            var eventData = new Hashtable {
                { "message", "Foo" },
                { "duration", 10 },
                { "subset", hashtable }
            };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnYelling("Foo", 10, playerSubset);

            mock.VerifyAll();
        }

        [TestMethod]
        public void OnSaying() {
            (var hashtable, var playerSubset) = Helper.BuildPlayerSquadSubset();

            var expectedEventName = "OnSaying";
            var eventData = new Hashtable {
                { "message", "Foo" },
                { "subset", hashtable }
            };
            var expectedJsonEventData = JSON.JsonEncode(Helper.BuildEventData(eventData));

            var mock = new Mock<IAPI>();

            mock.Setup(x =>
                x.SendEvent(
                    It.Is<string>(v => v.Equals(expectedEventName)),
                    It.Is<string>(v => v.Equals(expectedJsonEventData))
                )
            );

            var pluginField4 = new PluginField4(mock.Object);

            pluginField4.OnSaying("Foo", playerSubset);

            mock.VerifyAll();
        }
    }
}
