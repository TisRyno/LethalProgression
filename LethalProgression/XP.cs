using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Collections;
using GameNetcodeStuff;
using LethalProgression.Config;
using LethalProgression.Skills;
using LethalProgression.GUI;
using LethalProgression.Patches;
using LethalProgression.Saving;
using LethalProgression.Network;
using Newtonsoft.Json;
using System.Linq;
using LethalNetworkAPI;
using System;

namespace LethalProgression
{
    internal class LC_XP : NetworkBehaviour
    {
        public LethalNetworkVariable<int> teamLevel = new LethalNetworkVariable<int>("teamLevel") {
            Value = 0
        };

        public LethalNetworkVariable<int> teamXP = new LethalNetworkVariable<int>("teamXP") {
            Value = 0
        };

        public LethalNetworkVariable<int> teamTotalValue = new LethalNetworkVariable<int>("teamTotalValue") {
            Value = 0
        };

        public LethalNetworkVariable<int> teamXPRequired = new LethalNetworkVariable<int>("teamXPRequired") {
            Value = 0
        };

        public LethalNetworkVariable<int> teamLootLevel = new LethalNetworkVariable<int>("teamLootLevel") {
            Value = 0
        };

        // New Player has Connected to Server Event
        public LethalClientEvent playerConnectClientEvent = new LethalClientEvent(identifier: "playerConnectEvent");
        public LethalServerEvent playerConnectServerEvent = new LethalServerEvent(identifier: "playerConnectEvent");

        // Server needs to re-evaluate the XP Requirements
        public LethalClientEvent evaluateXPRequirementClientEvent = new LethalClientEvent(identifier: "evaluateXPRequirementEvent");
        public LethalServerEvent evaluateXPRequirementServerEvent = new LethalServerEvent(identifier: "evaluateXPRequirementEvent");

        // Server needs to resend all player's hand slots
        public LethalClientEvent calculateAllPlayersHandSlotsClientEvent = new LethalClientEvent(identifier: "calculateAllPlayersHandSlotsEvent");
        public LethalServerEvent calculateAllPlayersHandSlotsServerEvent = new LethalServerEvent(identifier: "calculateAllPlayersHandSlotsEvent");

        // Send Host config to Clients
        public LethalServerMessage<string> sendConfigServerMessage = new LethalServerMessage<string>(identifier: "sendConfigMessage");
        public LethalClientMessage<string> sendConfigClientMessage = new LethalClientMessage<string>(identifier: "sendConfigMessage");

        // Request a Player Profile Data from Host
        public LethalServerMessage<ulong> requestProfileDataServerMessage = new LethalServerMessage<ulong>(identifier: "requestProfileDataMessage");
        public LethalClientMessage<ulong> requestProfileDataClientMessage = new LethalClientMessage<ulong>(identifier: "requestProfileDataMessage");

        // Send a Player Profile back to Client
        public LethalServerMessage<string> sendProfileDataServerMessage = new LethalServerMessage<string>(identifier: "sendProfileDataMessage");
        public LethalClientMessage<string> receiveProfileDataClientMessage = new LethalClientMessage<string>(identifier: "sendProfileDataMessage");

        // Request Host to save Player Profile data
        public LethalServerMessage<string> saveProfileDataServerMessage = new LethalServerMessage<string>(identifier: "saveProfileDataMessage");
        public LethalClientMessage<string> saveProfileDataClientMessage = new LethalClientMessage<string>(identifier: "saveProfileDataMessage");

        // Request Host to update Team Loot Level
        public LethalServerMessage<int> updateTeamLootLevelServerMessage = new LethalServerMessage<int>(identifier: "updateTeamLootLevelMessage");
        public LethalClientMessage<int> updateTeamLootLevelClientMessage = new LethalClientMessage<int>(identifier: "updateTeamLootLevelMessage");

        // Request Host to update Team XP
        public LethalServerMessage<int> updateTeamXPServerMessage = new LethalServerMessage<int>(identifier: "updateTeamXPMessage");
        public LethalClientMessage<int> updateTeamXPClientMessage = new LethalClientMessage<int>(identifier: "updateTeamXPMessage");

        // Request Clients to update player skillpoints
        public LethalServerMessage<int> updatePlayerSkillpointsServerMessage = new LethalServerMessage<int>(identifier: "updatePlayerSkillPointsMessage");
        public LethalClientMessage<int> updatePlayerSkillpointsClientMessage = new LethalClientMessage<int>(identifier: "updatePlayerSkillPointsMessage");

        // Request Server to update player hand slots
        public LethalServerMessage<int> updateSPHandSlotsServerMessage = new LethalServerMessage<int>(identifier: "updateSPHandSlotsMessage");
        public LethalClientMessage<int> updateSPHandSlotsClientMessage = new LethalClientMessage<int>(identifier: "updateSPHandSlotsMessage");

        // Request Client to update player hand slots
        public LethalServerMessage<PlayerHandSlotData> updatePlayerHandSlotsServerMessage = new LethalServerMessage<PlayerHandSlotData>(identifier: "updatePlayerHandSlotsMessage");
        public LethalClientMessage<PlayerHandSlotData> updatePlayerHandSlotsClientMessage = new LethalClientMessage<PlayerHandSlotData>(identifier: "updatePlayerHandSlotsMessage");
        
        // Request Server and Clients to update player jump height
        public LethalServerMessage<PlayerJumpHeightData> updatePlayerJumpForceServerMessage = new LethalServerMessage<PlayerJumpHeightData>(identifier: "updatePlayerJumpForceMessage");
        public LethalClientMessage<PlayerJumpHeightData> updatePlayerJumpForceClientMessage = new LethalClientMessage<PlayerJumpHeightData>(identifier: "updatePlayerJumpForceMessage");
        

        public int skillPoints;
        public SkillList skillList;
        public SkillsGUI guiObj;
        public bool Initialized = false;
        public bool loadedSave = false;

        public void Start()
        {
            LethalPlugin.Log.LogInfo("XP Network Behavior Made!");

            // Network Variable handlers
            teamLevel.OnValueChanged += OnTeamLevelChange;
            teamXP.OnValueChanged += OnTeamXPChange;

            // Network Events handlers
            playerConnectServerEvent.OnReceived += PlayerConnect_C2SEvent;
            evaluateXPRequirementServerEvent.OnReceived += EvaluateXPRequirements_C2SEvent;
            calculateAllPlayersHandSlotsServerEvent.OnReceived += RefreshAllPlayerHandSlots_C2SEvent;

            // Network Message handlers Server2Client
            sendConfigClientMessage.OnReceived += SendHostConfig_S2CMessage;
            receiveProfileDataClientMessage.OnReceived += LoadProfileData_S2CMessage;
            updatePlayerSkillpointsClientMessage.OnReceived += UpdateSkillPoints_S2CMessage;
            updatePlayerHandSlotsClientMessage.OnReceived += UpdatePlayerHandSlots_S2CMessage;
            updatePlayerJumpForceClientMessage.OnReceived += UpdatePlayerJumpHeight_S2CMessage;

            // Network Message handlers Client2Server
            requestProfileDataServerMessage.OnReceived += RequestSavedData_C2SMessage;
            saveProfileDataServerMessage.OnReceived += SaveProfileData_C2SMessage;
            updateTeamLootLevelServerMessage.OnReceived += UpdateTeamLootLevel_C2SMessage;
            updateTeamXPServerMessage.OnReceived += UpdateTeamXP_C2SMessage;
            updateSPHandSlotsServerMessage.OnReceived += UpdateSPHandSlots_C2SMessage;
            updatePlayerJumpForceServerMessage.OnReceived += UpdatePlayerJumpHeight_C2SMessage;

            playerConnectClientEvent.InvokeServer();
        }

        public override void OnDestroy()
        {
             // Network Variable handlers
            teamLevel.OnValueChanged -= OnTeamLevelChange;
            teamXP.OnValueChanged -= OnTeamXPChange;
            teamLootLevel.OnValueChanged -= guiObj.TeamLootHudUpdate;

            // Network Events handlers
            playerConnectServerEvent.OnReceived -= PlayerConnect_C2SEvent;
            evaluateXPRequirementServerEvent.OnReceived -= EvaluateXPRequirements_C2SEvent;
            calculateAllPlayersHandSlotsServerEvent.OnReceived -= RefreshAllPlayerHandSlots_C2SEvent;

            // Network Message handlers Server2Client
            sendConfigClientMessage.OnReceived -= SendHostConfig_S2CMessage;
            receiveProfileDataClientMessage.OnReceived -= LoadProfileData_S2CMessage;
            updatePlayerSkillpointsClientMessage.OnReceived -= UpdateSkillPoints_S2CMessage;
            updatePlayerHandSlotsClientMessage.OnReceived -= UpdatePlayerHandSlots_S2CMessage;

            // Network Message handlers Client2Server
            requestProfileDataServerMessage.OnReceived -= RequestSavedData_C2SMessage;
            saveProfileDataServerMessage.OnReceived -= SaveProfileData_C2SMessage;
            updateTeamLootLevelServerMessage.OnReceived -= UpdateTeamLootLevel_C2SMessage;
            updateTeamXPServerMessage.OnReceived -= UpdateTeamXP_C2SMessage;
            updateSPHandSlotsServerMessage.OnReceived -= UpdateSPHandSlots_C2SMessage;

            // Always invoked the base
            base.OnDestroy();
        }

        private void OnTeamLevelChange(int newLevel)
        {
            StartCoroutine(
                WaitUntilInitialisedThenAction(HUDManagerPatch.ShowLevelUp)
            );
        }

        private void OnTeamXPChange(int newXP)
        {
            StartCoroutine(
                WaitUntilInitialisedThenAction(HUDManagerPatch.ShowXPUpdate)
            );
        }

        public IEnumerator WaitUntilInitialisedThenAction(Action callback)
        {
            yield return new WaitUntil(() => Initialized == true);

            callback();
        }

        public void PlayerConnect_C2SEvent(ulong clientId)
        {
            LethalPlugin.Log.LogInfo($"Received PlayerConnect message from {clientId}");

            IDictionary<string, string> configDict = LethalPlugin.Instance.GetAllConfigEntries();
            string serializedConfig = JsonConvert.SerializeObject(configDict);

            LethalPlugin.Log.LogInfo($"Sending config -> {serializedConfig}");

            sendConfigServerMessage.SendAllClients(serializedConfig);
        }

        public void LoadSharedData()
        {
            SaveSharedData sharedData = SaveManager.LoadSharedFile();

            if (sharedData == null)
            {
                LethalPlugin.Log.LogInfo("Shared data is null!");
                return;
            }

            LethalPlugin.Log.LogInfo("Loading Lobby shared data.");

            teamXP.Value = sharedData.xp;
            teamLevel.Value = sharedData.level;
            teamTotalValue.Value = sharedData.quota;
            
            teamXPRequired.Value = CalculateXPRequirement();

            LethalPlugin.Log.LogInfo($"{sharedData.level} current lvl, {sharedData.xp} XP, {sharedData.quota} Profit, {teamXPRequired.Value} teamXPRequired");
        }

        public IEnumerator LoadProfileData(string data)
        {
            LethalPlugin.Log.LogInfo($"Received player data from host -> {data}");

            yield return new WaitUntil(() => Initialized == true);

            if (loadedSave)
            {
                LethalPlugin.Log.LogWarning("Already loaded player data from host.");
                yield return null;
            }

            loadedSave = true;
            SaveData saveData = JsonConvert.DeserializeObject<SaveData>(data);

            skillPoints = saveData.skillPoints;
            
            LethalPlugin.Log.LogDebug($"skillPoints -> {skillPoints}");

            int skillCheck = 0;
            foreach (KeyValuePair<UpgradeType, int> skill in saveData.skillAllocation)
            {
                LethalPlugin.Log.LogDebug($"{skill.Key} -> {skill.Value}");
                skillList.skills[skill.Key].SetLevel(skill.Value, false);
                skillCheck += skill.Value;
                LethalPlugin.Log.LogDebug($"skillCheck -> {skillCheck}");
            }

            // Sanity check: If skillCheck goes over amount of skill points, reset all skills.
            //if (skillCheck > skillPoints)
            //{
            //    LethalPlugin.Log.LogInfo("Skill check is greater than skill points, resetting skills.");
            //    foreach (KeyValuePair<UpgradeType, Skill> skill in skillList.skills)
            //    {
            //        skill.Value.Reset();
            //    }
            //}

            // if the skill check is less than the current level plus five, add the difference
            if ((skillCheck + skillPoints) < teamLevel.Value + 5)
            {
                LethalPlugin.Log.LogInfo($"Skill check is less than current level, adding {teamLevel.Value + 5 - (skillCheck + skillPoints)} skill points.");
                skillPoints += teamLevel.Value + 5 - (skillCheck + skillPoints);
            }

            LP_NetworkManager.xpInstance.guiObj.UpdateAllStats();
        }
        
        public int CalculateXPRequirement()
        {
            // First, we need to check how many players.
            int playerCount = StartOfRound.Instance.connectedPlayersAmount;
            int quota = TimeOfDay.Instance.timesFulfilledQuota;

            int initialXPCost = int.Parse(SkillConfig.hostConfig["XP Minimum"]);
            int maxXPCost = int.Parse(SkillConfig.hostConfig["XP Maximum"]);

            int personScale = int.Parse(SkillConfig.hostConfig["Person Multiplier"]);
            int personValue = playerCount * personScale;
            int req = initialXPCost + personValue;

            // Quota multiplier
            int quotaMult = int.Parse(SkillConfig.hostConfig["Quota Multiplier"]);
            int quotaVal = quota * quotaMult;

            req += (int)(req * (quotaVal / 100f));

            if (req > maxXPCost)
            {
                req = maxXPCost;
            }

            LethalPlugin.Log.LogInfo($"{playerCount} players, {quota} quotas, {initialXPCost} initial cost, {personValue} person value, {quotaVal} quota value, {req} total cost.");
            
            return req;
        }

        public int GetXP()
        {
            return teamXP.Value;
        }

        public int GetLevel()
        {
            return teamLevel.Value;
        }

        public int GetProfit()
        {
            return teamTotalValue.Value;
        }

        public int GetSkillPoints()
        {
            return skillPoints;
        }

        public void SetSkillPoints(int num)
        {
            skillPoints = num;
        }

        public void EvaluateXPRequirements_C2SEvent(ulong clientId)
        {
            StartCoroutine(XPRequirementCoroutine());
        }

        public IEnumerator XPRequirementCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            teamXPRequired.Value = CalculateXPRequirement();
        }

        public void UpdateTeamXP_C2SMessage(int xpToAdd, ulong clientId)
        {
            // Update XP values
            teamXP.Value += xpToAdd;
            teamTotalValue.Value += xpToAdd;

            int newXP = GetXP();

            // If we have enough XP to level up.
            if (newXP >= teamXPRequired.Value)
            {
                // How many times do we level up?
                int levelUps = 0;

                while (newXP >= teamXPRequired.Value)
                {
                    levelUps++;
                    newXP -= teamXPRequired.Value;
                }

                // Update level value
                teamXP.Value = newXP;
                teamLevel.Value += levelUps;

                updatePlayerSkillpointsServerMessage.SendAllClients(levelUps);
            }
        }

        public void UpdateSkillPoints_S2CMessage(int pointsToAdd)
        {
            skillPoints += pointsToAdd;
        }

        /////////////////////////////////////////////////
        /// Team Loot Upgrade Sync
        /////////////////////////////////////////////////
        
        public void UpdateTeamLootLevel_C2SMessage(int change, ulong clientId) {
            int currentLootLevel = teamLootLevel.Value;

            if (currentLootLevel + change < 0) {
                teamLootLevel.Value = 0;
            } else {
                teamLootLevel.Value += change;
            }

            LethalPlugin.Log.LogInfo($"Changed team loot value by {change} turning into {teamLootLevel.Value}.");
        }

        /////////////////////////////////////////////////
        /// Hand Slot Sync
        /////////////////////////////////////////////////

        public void UpdateSPHandSlots_C2SMessage(int slotChange, ulong clientId)
        {
            if (LethalPlugin.ReservedSlots)
                return;
            
            LethalPlugin.Log.LogInfo($"C2S Received update for Player {clientId} to add {slotChange} slots");
            
            updatePlayerHandSlotsServerMessage.SendAllClients(new PlayerHandSlotData(clientId, slotChange));
        }

        public void UpdatePlayerHandSlots_S2CMessage(PlayerHandSlotData data)
        {
            LethalPlugin.Log.LogInfo($"S2C Received update for Player {data.clientId} to add {data.additionalSlots} slots");
            SetHandSlot(data.clientId, data.additionalSlots);
        }

        public void SetHandSlot(ulong playerID, int additionalSlots)
        {
            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                if (player.playerClientId == playerID)
                {
                    int newAmount = 4 + additionalSlots;
                    List<GrabbableObject> objects = player.ItemSlots.ToList<GrabbableObject>();

                    if (player.currentItemSlot > newAmount - 1)
                    {
                        HandSlots.SwitchItemSlots(player, newAmount - 1);
                    }

                    for (int i = 0; i < objects.Count; i++)
                    {
                        if (i > newAmount - 1)
                        {
                            // In a slot that is getting removed, drop it instead
                            if (objects[i] != null)
                            {
                                HandSlots.SwitchItemSlots(player, i);
                                player.DiscardHeldObject();
                            }
                        }
                    }

                    player.ItemSlots = new GrabbableObject[newAmount];
                    for (int i = 0; i < objects.Count; i++)
                    {
                        if (objects[i] == null)
                        {
                            continue;
                        }
                        player.ItemSlots[i] = objects[i];
                    }
                    LethalPlugin.Log.LogDebug($"Player {playerID} has {player.ItemSlots.Length} slots after setting.");

                    if (player == GameNetworkManager.Instance.localPlayerController)
                    {
                        LethalPlugin.Log.LogDebug($"Updating HUD slots.");
                        HandSlots.UpdateHudSlots();
                    }
                    break;
                }
            }
        }

        // When joining.
        public void RefreshAllPlayerHandSlots_C2SEvent(ulong clientId)
        {
            if (LethalPlugin.ReservedSlots)
                return;

            if (!LP_NetworkManager.xpInstance.skillList.IsSkillValid(UpgradeType.HandSlot))
                return;

            // Compile a list of all players and their handslots.
            foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
                if (!player.gameObject.activeSelf)
                    continue;

                ulong playerID = player.playerClientId;
                int additionalSlots = player.ItemSlots.Length - 4;

                updatePlayerHandSlotsServerMessage.SendAllClients(new PlayerHandSlotData(playerID, additionalSlots));
            }
        }

        public void SendHostConfig_S2CMessage(string serializedConfig)
        {
            IDictionary<string, string> configs = JsonConvert.DeserializeObject<IDictionary<string, string>>(serializedConfig);
            // Apply configs.
            foreach (KeyValuePair<string, string> entry in configs)
            {
                SkillConfig.hostConfig[entry.Key] = entry.Value;
                LethalPlugin.Log.LogInfo($"Loaded host config: {entry.Key} = {entry.Value}");
            }

            if (!Initialized)
            {
                Initialized = true;
                LP_NetworkManager.xpInstance = this;
                
                skillList = new SkillList();
                skillList.InitializeSkills();

                if (GameNetworkManager.Instance.isHostingGame)
                {
                    LoadSharedData();
                }

                guiObj = new SkillsGUI();
                teamLootLevel.OnValueChanged += guiObj.TeamLootHudUpdate;

                skillPoints = teamLevel.Value + 5;

                calculateAllPlayersHandSlotsClientEvent.InvokeServer();

                evaluateXPRequirementClientEvent.InvokeServer();
            }
        }

        // Loading
        public void RequestSavedData_C2SMessage(ulong steamID, ulong clientId)
        {
            string saveData = SaveManager.LoadPlayerFile(steamID);

            PlayerControllerB player = clientId.GetPlayerController();

            sendProfileDataServerMessage.SendClient(saveData, player.actualClientId);
        }

        public void LoadProfileData_S2CMessage(string saveData)
        {
            LethalPlugin.Log.LogInfo($"Received LoadProfileData_S2CMessage -> {saveData}");

            if (saveData == null)
            {
                return;
            }

            StartCoroutine(LoadProfileData(saveData));
        }

        public void SaveProfileData_C2SMessage(string data, ulong clientId)
        {
            SaveProfileData profileData = JsonConvert.DeserializeObject<SaveProfileData>(data);

            LethalPlugin.Log.LogInfo($"Received SaveData request for {profileData.steamId} with data -> {JsonConvert.SerializeObject(profileData.saveData)}");

            SaveManager.Save(profileData.steamId, profileData.saveData);
            SaveManager.SaveShared(teamXP.Value, teamLevel.Value, teamTotalValue.Value);
        }

        public void UpdatePlayerJumpHeight_C2SMessage(PlayerJumpHeightData playerData, ulong clientId)
        {
            LethalPlugin.Log.LogInfo($"Received request for {clientId} [{playerData.clientId}] to set jump skill to {playerData.jumpSkillValue}");
            
            updatePlayerJumpForceServerMessage.SendAllClients(playerData);
        }

        public void UpdatePlayerJumpHeight_S2CMessage(PlayerJumpHeightData playerData)
        {
            PlayerControllerB playerController = playerData.clientId.GetPlayerController();
            Skill skill = LP_NetworkManager.xpInstance.skillList.skills[UpgradeType.JumpHeight];

            // 10 is 100%. So if 1 level adds 1% more, then it is 10 * 1.01.
            float addedJump = playerData.jumpSkillValue * skill.GetMultiplier() / 100f * 13f;

            playerController.jumpForce = 13f + addedJump;

            LethalPlugin.Log.LogInfo($"Updating client {playerData.clientId} jump height. Adding {addedJump} resulting in {playerController.jumpForce} jump force");
        }
    }
}