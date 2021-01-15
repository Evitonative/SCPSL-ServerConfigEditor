using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using SCPSL_ServerConfigEditor.Properties;

namespace SCPSL_ServerConfigEditor
{
    public partial class MainMenu : Form
    {
        private List<string[]> roles;
        private List<string[]> players;

        private List<CheckBox> rolesBoxes;
        private List<List<string>> allPerms;

        private int _prevSelected;
        private int _curSelected = -1;

        public MainMenu()
        {
            InitializeComponent();
            players = new List<string[]>();
            
            configTabs.Dock = DockStyle.Fill;
            coverBox.DropDownStyle = ComboBoxStyle.DropDownList;
            hiddenBox.DropDownStyle = ComboBoxStyle.DropDownList;
            allow_central_server_commands_as_ServerConsoleCommands.DropDownStyle = ComboBoxStyle.DropDownList;
            rolePassworldSelectBox.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (var t in roleList.Items)
            {
                rolePassworldSelectBox.Items.Add(t);
            }
            playerRoleBox.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (var t in roleList.Items)
            {
                playerRoleBox.Items.Add(t);
            }

            rolePassworldSelectBox.Items.Add("");

            roles = new List<string[]>();
            roles.Add(roleOptions("owner", "SERVER OWNER", "red", "true", "false", "255", "255"));
            roles.Add(roleOptions("admin", "ADMIN", "red", "true", "false", "1", "2"));
            roles.Add(roleOptions("moderator", "MODERATOR", "silver", "true", "false", "0", "1"));
            
            allPerms = new List<List<string>>();
            for (int i = 0; i < 29; i++)
            {
                allPerms.Add(new List<string>());
            }

            rolesBoxes = new List<CheckBox>();
            rolesBoxes.Add(KickingAndShortTermBanning);
            rolesBoxes.Add(BanningUpToDay);
            rolesBoxes.Add(LongTermBanning);
            rolesBoxes.Add(ForceclassSelf);
            rolesBoxes.Add(ForceclassToSpectator);
            rolesBoxes.Add(ForceclassWithoutRestrictions);
            rolesBoxes.Add(GivingItems);
            rolesBoxes.Add(PlayersManagement);
            rolesBoxes.Add(Effects);
            rolesBoxes.Add(WarheadEvents);
            rolesBoxes.Add(RespawnEvents);
            rolesBoxes.Add(RoundEvents);
            rolesBoxes.Add(FacilityManagement);
            rolesBoxes.Add(Broadcasting);
            rolesBoxes.Add(Announcer);
            rolesBoxes.Add(AdminChat);
            rolesBoxes.Add(ViewHiddenBadges);
            rolesBoxes.Add(ViewHiddenGlobalBadges);
            rolesBoxes.Add(Overwatch);
            rolesBoxes.Add(Noclip);
            rolesBoxes.Add(GameplayData);
            rolesBoxes.Add(PlayerSensitiveDataAccess);
            rolesBoxes.Add(SetGroup);
            rolesBoxes.Add(PermissionsManagement);
            rolesBoxes.Add(ServerConfigs);
            rolesBoxes.Add(ServerConsoleCommands);
            rolesBoxes.Add(AFKImmunity);
            rolesBoxes.Add(FriendlyFireDetectorImmunity);
            rolesBoxes.Add(FriendlyFireDetectorTempDisable);
        }

        private void removeRole_Click(object sender, EventArgs e)
        {
            int selectedIndex = roleList.SelectedIndex;
            if (selectedIndex == -1) return;
            if (selectedIndex >= 1) roleList.SelectedIndex = selectedIndex - 1;
            else if (roleList.Items.Count != 0) roleList.SelectedIndex = 0;

            roles.RemoveAt(selectedIndex);
            roleList.Items.RemoveAt(selectedIndex);

            rolePassworldSelectBox.Items.Clear();
            foreach (var t in roleList.Items)
            {
                rolePassworldSelectBox.Items.Add(t);
            }
            playerRoleBox.Items.Clear();
            foreach (var t in roleList.Items)
            {
                playerRoleBox.Items.Add(t);
            }
        }

        private void addRoleButton_Click(object sender, EventArgs e)
        {
            if (roleList.Items.Contains(addRoleBox.Text.ToLower()))
            {
                MessageBox.Show("This role has already been added to the list.\nRemove the old role or change the name of this one.\nThis role will not be added.", "Role is already in list",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                addRoleBox.Text = "Add role (internal name)";
                return;
            }
            roleList.Items.Add(addRoleBox.Text.ToLower());
            roles.Add(roleOptions(addRoleBox.Text));
            addRoleBox.Text = "Add role (internal name)";
            roleList.SelectedIndex = roleList.Items.Count - 1;

            rolePassworldSelectBox.Items.Clear();
            foreach (var t in roleList.Items)
            {
                rolePassworldSelectBox.Items.Add(t);
            }
            playerRoleBox.Items.Clear();
            foreach (var t in roleList.Items)
            {
                playerRoleBox.Items.Add(t);
            }
        }

        private void addRoleBox_Click(object sender, EventArgs e)
        {
            addRoleBox.SelectionStart = 0;
            addRoleBox.SelectionLength = addRoleBox.Text.Length;
        }

        public String[] roleOptions(
            string name,
            string badge = "", 
            string color = "", 
            string cover = "", 
            string hidden = "", 
            string kickPower = "", 
            string requireKickPower = ""
            )
        {
            if (badge == "") badge = name;
            List<string> roleSettings = new List<string>();

            roleSettings.Add(name);
            roleSettings.Add(badge); //Name
            roleSettings.Add(color); //Color
            roleSettings.Add(cover); //Cover
            roleSettings.Add(hidden); //hidden
            roleSettings.Add(kickPower); //kickPower
            roleSettings.Add(requireKickPower); //required KickPower

            return roleSettings.ToArray();
        }

        private void roleList_SelectedIndexChanged(object sender, EventArgs e)
        {
            _prevSelected = _curSelected;
            _curSelected = roleList.SelectedIndex;
            if(_prevSelected != -1) UpdateList(_prevSelected);
            if (roles.Count > 0 && roleList.SelectedIndex >= 0)
            {
                badgeBox.Text = roles[roleList.SelectedIndex][1];
                colorBox.Text = roles[roleList.SelectedIndex][2];
                coverBox.Text = roles[roleList.SelectedIndex][3];
                hiddenBox.Text = roles[roleList.SelectedIndex][4];
                kickPowerBox.Text = roles[roleList.SelectedIndex][5];
                requiredKickPowerBox.Text = roles[roleList.SelectedIndex][6];
                
                for (int i = 0; i < 29; i++)
                {
                    rolesBoxes[i].Checked = allPerms[i].Contains(roles[roleList.SelectedIndex][0]);
                }
            }
            else
            {
                badgeBox.Clear();
                colorBox.Clear();
                coverBox.Text = "true";
                hiddenBox.Text = "false";
                kickPowerBox.Clear();
                requiredKickPowerBox.Clear();
            }
        }

        private void colorLable_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(
                "https://cdn.discordapp.com/attachments/468799519811829761/601490386569003019/Colors.png");
        }
        
        private void UpdateList(int edit)
        {
            if (roles.Count > 0 && roleList.SelectedIndex >= 0)
            {
                roles[edit][1] = badgeBox.Text;
                roles[edit][2] = colorBox.Text;
                roles[edit][3] = coverBox.Text;
                roles[edit][4] = hiddenBox.Text;
                roles[edit][5] = kickPowerBox.Text;
                roles[edit][6] = requiredKickPowerBox.Text;

                for (int i = 0; i < 29; i++)
                {
                    allPerms[i].Remove(roles[edit][0]);
                    if (rolesBoxes[i].Checked) allPerms[i].Add(roles[edit][0]);
                }

                rolePassworldSelectBox.Items.Clear();
                foreach (var t in roleList.Items)
                {
                    rolePassworldSelectBox.Items.Add(t);
                }
            }
        }

        private void permsLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://en.scpslgame.com/index.php/Docs:Permissions");
        }

        private void addPlayerButton_Click(object sender, EventArgs e)
        {
            string player = addPlayerBox.Text;
            string platform;
            string otherPlatform;
            if (isDiscordId.Checked)
            {
                platform = "discord";
                otherPlatform = "steam";
            }
            else
            {
                platform = "steam";
                otherPlatform = "discord";
            }

            if (playerList.Items.Contains($"{player}@{platform}"))
            {
                MessageBox.Show(
                    $"This {player}@{platform} has already been created.\nIs this a {platform}ID or did you mean to use {otherPlatform}?\nThis player will not be added.",
                    "Player already created", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (playerRoleBox.Text == "")
            {
                MessageBox.Show(
                    $"You have not selected a role for this player.", "No role selected.", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                playerList.Items.Add($"{player}@{platform}");

                players.Add(new[] { $"{player}@{platform}", playerRoleBox.Text });

                playerRoleBox.Text = "";
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = playerList.SelectedIndex;
            if (selectedIndex == -1) return;
            if (selectedIndex >= 1) roleList.SelectedIndex -= 1;
            else if (playerList.Items.Count != 0) playerList.SelectedIndex = 0;

            players.RemoveAt(selectedIndex);
            playerList.Items.RemoveAt(selectedIndex);
        }

        private async void ExportButton_Click(object sender, EventArgs e)
        {
            await SaveFile();
        }

        private Task SaveFile()
        {
            UpdateList(_curSelected);

            List<string> export = new List<string>();
            export.Add("#Let's assign roles (you can modify them and create custom roles below)");
            export.Add("#UserID format is SteamId64Here@steam, DiscordUserIDHere@discord, etc...");
            export.Add("Members:");
            foreach (var p in players)
            {
                export.Add($" - {p[0]}: {p[1]}");
            }
            export.Add("");
            export.Add("#Should Secret Lab development staff be able to use the Remote Admin?");
            export.Add($"enable_staff_access: {enable_staff_access.Checked.ToString()}");
            export.Add("");
            export.Add("#Should Secret Lab CEO and managers be able to use Remote Admin? (We do not abuse our powers)");
            export.Add($"enable_manager_access: {enable_manager_access.Checked.ToString()}");
            export.Add("");
            export.Add("#Allow remote admin access for the banning team, to allow them searching and fightung cheaters globally");
            export.Add($"enable_banteam_access: {enable_banteam_access.Checked.ToString()}");
            export.Add("");
            export.Add("#Enable reserved slots for the banning team (they are restricted by reserved slots limit set in the gameplay config)");
            export.Add($"enable_banteam_reserved_slots: {enable_banteam_reserved_slots.Checked.ToString()}");
            export.Add("");
            export.Add("#Allow the banning team to bypass georestrictions on the server");
            export.Add($"enable_banteam_bypass_geoblocking: {enable_banteam_bypass_geoblocking.Checked.ToString()}");
            export.Add("");
            export.Add("#Let's define roles");
            export.Add("#use color \"none\" to disable badge");
            export.Add("#you can add own roles too");
            export.Add("#cover means that this local badge is more important than a global badge and will cover it");
            export.Add("#hidden means that this local badge is hiddeny by default (always you can use \"hidetag\" and \"showtag\" commands in game console or text-based remote admin)");
            export.Add("#kick_power is the power for kicking and banning that the member of this group has (can be from 0 to 255)");
            export.Add("#required_kick_power is the required kick power to kick or ban a member of this group (can be from 0 to 255)");
            export.Add("");
            for (int i = 0; i < roleList.Items.Count; i++)
            {
                export.Add($"{roles[i][0]}_badge: {roles[i][1]}");
                export.Add($"{roles[i][0]}_color: {roles[i][2]}");
                export.Add($"{roles[i][0]}_cover: {roles[i][3]}");
                export.Add($"{roles[i][0]}_hidden: {roles[i][4]}");
                export.Add($"{roles[i][0]}_kick_power: {roles[i][5]}");
                export.Add($"{roles[i][0]}_required_kick_power: {roles[i][6]}");
                export.Add("");
            }
            export.Add("#And add them to the roles list");
            export.Add("Roles:");
            for (int i = 0; i < roleList.Items.Count; i++)
            {
                export.Add($"- {roles[i][0]}");
            }
            export.Add("");
            export.Add("#Let's setup permissions for roles");
            export.Add("#More info can be found on our wiki: https://en.scpslgame.com/index.php/Docs:Permissions");
            export.Add("Permissions:");
            export.Add($" - KickingAndShortTermBanning: [{string.Join(", ", allPerms[0])}]");
            export.Add($" - BanningUpToDay: [{string.Join(", ", allPerms[1])}]");
            export.Add($" - LongTermBanning: [{string.Join(", ", allPerms[2])}]");
            export.Add($" - ForceclassSelf: [{string.Join(", ", allPerms[3])}]");
            export.Add($" - ForceclassToSpectator: [{string.Join(", ", allPerms[4])}]");
            export.Add($" - ForceclassWithoutRestrictions: [{string.Join(", ", allPerms[5])}]");
            export.Add($" - GivingItems: [{string.Join(", ", allPerms[6])}]");
            export.Add($" - PlayersManagement: [{string.Join(", ", allPerms[7])}]");
            export.Add($" - Effects: [{string.Join(", ", allPerms[8])}]");
            export.Add($" - WarheadEvents: [{string.Join(", ", allPerms[9])}]");
            export.Add($" - RespawnEvents: [{string.Join(", ", allPerms[10])}]");
            export.Add($" - RoundEvents: [{string.Join(", ", allPerms[11])}]");
            export.Add($" - FacilityManagement: [{string.Join(", ", allPerms[12])}]");
            export.Add($" - Broadcasting: [{string.Join(", ", allPerms[13])}]");
            export.Add($" - Announcer: [{string.Join(", ", allPerms[14])}]");
            export.Add($" - AdminChat: [{string.Join(", ", allPerms[15])}]");
            export.Add($" - ViewHiddenBadges: [{string.Join(", ", allPerms[16])}]");
            export.Add($" - ViewHiddenGlobalBadges: [{string.Join(", ", allPerms[17])}]");
            export.Add($" - Overwatch: [{string.Join(", ", allPerms[18])}]");
            export.Add($" - Noclip: [{string.Join(", ", allPerms[19])}]");
            export.Add($" - GameplayData: [{string.Join(", ", allPerms[20])}]");
            export.Add($" - PlayerSensitiveDataAccess: [{string.Join(", ", allPerms[21])}]");
            export.Add($" - SetGroup: [{string.Join(", ", allPerms[22])}]");
            export.Add($" - PermissionsManagement: [{string.Join(", ", allPerms[23])}]");
            export.Add($" - ServerConfigs: [{string.Join(", ", allPerms[24])}]");
            export.Add($" - ServerConsoleCommands: [{string.Join(", ", allPerms[25])}]");
            export.Add($" - AFKImmunity: [{string.Join(", ", allPerms[26])}]");
            export.Add($" - FriendlyFireDetectorImmunity: [{string.Join(", ", allPerms[27])}]");
            export.Add($" - FriendlyFireDetectorTempDisable: [{string.Join(", ", allPerms[28])}]");
            export.Add("");
            export.Add("#Set to \"none\" in order to disable password.");
            export.Add("#WE DON'T RECOMMEND USING PASSWORD!!!");
            export.Add("#SETUP STEAMID AUTHENTICATION INSTEAD (of the top of this config file)!");
            export.Add($"override_password:: {raPassword}");
            export.Add($"override_password_role: {rolePassworldSelectBox.SelectedItem}");
            export.Add("");
            export.Add("#Allows running central server commands (they are prefixed with \"!\") using \"sudo\"/\"cron\" command in RA (requires ServerConsoleCommands permission).");
            export.Add("#Don't turn on unless you fully trust all people with this permission, they needs to run that commands from RA and you know what are you doing");
            export.Add($"allow_central_server_commands_as_ServerConsoleCommands: {allow_central_server_commands_as_ServerConsoleCommands.Text}");
            export.Add("");
            //TODO

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = "Export as";
            saveFile.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFile.FileName = "config_remoteadmin";


            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                StreamWriter txtOutput = new StreamWriter(saveFile.FileName);
                txtOutput.Write(string.Join(System.Environment.NewLine, export));
                txtOutput.Close();
            }
            return Task.CompletedTask;
        }

        private async void MainMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Do you want to export the file before you exit.", "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                await SaveFile();
                e.Cancel = false;
            }
            else if (result == DialogResult.No)
            {
                e.Cancel = false;
            }
            else e.Cancel = true;
        }
    }
}
