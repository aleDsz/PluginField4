using PRoCon.Core;
using PRoCon.Core.Players;
using PRoCon.Core.Players.Items;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRoConEvents.Tests {
    static internal class Helper {
        public static Hashtable BuildEventData(Hashtable eventData) => new Hashtable { { "data", eventData } };

        public static (Hashtable, CPlayerInfo) BuildPlayerInfo() {
            var pluginField4 = new PluginField4();

            var playerInfo = new CPlayerInfo()
            {
                GUID = "123456789",
                ClanTag = "pruu",
                Deaths = 10,
                JoinTime = 123456789,
                Kdr = 1.2F,
                Kills = 12,
                Ping = 3,
                Rank = 140,
                Score = 1200,
                SessionTime = 123456789,
                SoldierName = "FooBarBaz",
                SquadID = 1,
                TeamID = 1,
                Type = 1
            };

            var hashtable = pluginField4.PlayerInfoToHastable(playerInfo);

            return (hashtable, playerInfo);
        }

        public static (Hashtable, CBanInfo) BuildBanInfo() {
            var pluginField4 = new PluginField4();
            var timeoutSubset = new TimeoutSubset(TimeoutSubset.TimeoutSubsetType.Permanent);

            var banInfo = new CBanInfo("FooBarBaz", "123456789", "127.0.0.1", timeoutSubset, "Reason");

            var hashtable = pluginField4.BanInfoToHashtable(banInfo);

            return (hashtable, banInfo);
        }

        public static (Hashtable, CPlayerSubset) BuildPlayerSubset() {
            var pluginField4 = new PluginField4();
            var playerSubset = new CPlayerSubset(CPlayerSubset.PlayerSubsetType.Player, 1, 1);
            var hashtable = pluginField4.PlayerSubsetToHashtable(playerSubset);

            return (hashtable, playerSubset);
        }

        public static (Hashtable, Inventory) BuildInventory() {
            var pluginField4 = new PluginField4();

            var specialization = new Specialization(SpecializationSlots.Kit, "Bla");
            var weapon = new Weapon(Kits.Recon, "Bla", WeaponSlots.Primary, DamageTypes.SniperRifle);

            var inventory = new Inventory(Kits.Recon);

            inventory.Weapons.Add(weapon);
            inventory.Specializations.Add(specialization);

            var hashtable = pluginField4.InventoryToHashtable(inventory);

            return (hashtable, inventory);
        }

        public static (Hashtable, Point3D) BuildLocation() {
            var point3D = new Point3D(0, 0, 0);

            var hashtable = new Hashtable {
                { "x", point3D.X },
                { "y", point3D.Y },
                { "z", point3D.Z }
            };

            return (hashtable, point3D);
        }
        
        public static (Hashtable, Kill) BuildKill() {
            (var hashtableKiller, var killer) = BuildPlayerInfo();
            (var hashtablePointKiller, var pointKiller) = BuildLocation();

            (var hashtableVictim, var victim) = BuildPlayerInfo();
            (var hashtablePointVictim, var pointVictim) = BuildLocation();

            var kill = new Kill(killer, victim, "abc", false, pointKiller, pointVictim);

            var hashtable = new Hashtable {
                { "killer", hashtableKiller },
                { "victim", hashtableVictim },
                { "distance", kill.Distance },
                { "is_headshot", kill.Headshot },
                { "is_suicide", kill.IsSuicide },
                { "damage_type", kill.DamageType },
                { "killer_location", hashtablePointKiller },
                { "victim_location", hashtablePointVictim },
                { "time_to_death", kill.TimeOfDeath.ToLongTimeString() }
            };

            return (hashtable, kill);
        }

        public static (Hashtable[], List<CPlayerInfo>) BuildPlayerInfoList(int quantity = 1) {
            var hashtableList = new List<Hashtable>();
            var playerInfoList = new List<CPlayerInfo>();

            for (int i = 0; i < quantity; i++)
            {
                (var hashtable, var playerInfo) = BuildPlayerInfo();

                playerInfoList.Add(playerInfo);
                hashtableList.Add(hashtable);
            }

            return (hashtableList.ToArray(), playerInfoList);
        }

        public static (Hashtable[], List<CBanInfo>) BuildBanInfoList(int quantity = 1) {
            var hashtableList = new List<Hashtable>();
            var banInfoList = new List<CBanInfo>();

            for (int i = 0; i < quantity; i++)
            {
                (var hashtable, var banInfo) = BuildBanInfo();

                banInfoList.Add(banInfo);
                hashtableList.Add(hashtable);
            }

            return (hashtableList.ToArray(), banInfoList);
        }

        public static (Hashtable[], List<TeamScore>) BuildTeamScoreList(int quantity = 1) {
            var hashtableList = new List<Hashtable>();
            var teamScoreList = new List<TeamScore>();

            for (int i = 0; i < quantity; i++)
            {
                var teamScore = new TeamScore(1, 1000, 2000);
                teamScoreList.Add(teamScore);

                hashtableList.Add(new Hashtable {
                    { "team_id", teamScore.TeamID },
                    { "score", teamScore.Score },
                    { "winning_score", teamScore.WinningScore }
                });
            }

            return (hashtableList.ToArray(), teamScoreList);
        }
    }
}
