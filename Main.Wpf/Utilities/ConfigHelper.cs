using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Main.Wpf.Utilities
{
    public static class ConfigHelper
    {
        public static async Task Read()
        {
            var file = Config.File;

            LogFile.WriteLog("Read Config File ...");

            try
            {
                Config.Informations.Extension.Name = await XmlHelper.ReadString(file, "name").ConfigureAwait(false);

                Config.Settings.File = await XmlHelper.ReadString(file, "settingsFile").ConfigureAwait(false);

                if (!File.Exists(Config.Settings.File)) SettingsHelper.CreateFile();

                Config.Informations.Extension.Color = await XmlHelper.ReadString(file, "color").ConfigureAwait(false);
                Config.Informations.Extension.Theme = JsonHelper.ReadString(Config.Settings.Json, "theme");
                Config.Informations.Extension.Favicon = await XmlHelper.ReadString(file, "favicon").ConfigureAwait(false);

                string Height = await XmlHelper.ReadString(file, "height").ConfigureAwait(false);
                string Width = await XmlHelper.ReadString(file, "width").ConfigureAwait(false);
                Config.Informations.Extension.WindowHeight = ValidationHelper.IsStringValidInt(Height) ? int.Parse(Height) : 0;
                Config.Informations.Extension.WindowWidth = ValidationHelper.IsStringValidInt(Width) ? int.Parse(Width) : 0;

                Config.Menu.SingleSite = (await XmlHelper.ReadBool(file, "hideMenu").ConfigureAwait(false), await XmlHelper.ReadString(file, "singleSite_Path").ConfigureAwait(false), await XmlHelper.ReadString(file, "singleSite_StartArguments").ConfigureAwait(false));

                List<(string Title, string Icon, string Path, string StartArguments)> sites = new List<(string Title, string Icon, string Path, string StartArguments)>();
                var Titels = XmlHelper.ReadStringList(file, "site_Title");
                var Icons = XmlHelper.ReadStringList(file, "site_Icon");
                var Paths = XmlHelper.ReadStringList(file, "site_Path");
                var StartArguments = XmlHelper.ReadStringList(file, "site_StartArguments");

                for (var site = 0; site != Titels.Count; ++site)
                {
                    sites.Add((Titels[site], Icons[site], Paths[site], StartArguments[site]));
                }
                sites.Add(("", "", "", ""));
                sites.Add(("Information", "", "info.exe", ""));
                if (!Config.Login.SkipLogin) sites.Add(("Anmelden", "", "account.exe", ""));

                Config.Menu.Sites = sites;

                Config.Menu.DefaultMenuState = MenuHelper.StringToMenuState(await XmlHelper.ReadString(file, "defaultMenuState").ConfigureAwait(false));

                Config.Login.SkipLogin = await XmlHelper.ReadBool(file, "skipLogin").ConfigureAwait(false);

                Config.Updater.Extension.VersionsHistoryFile = await XmlHelper.ReadString(file, "versionsHistoryFile").ConfigureAwait(false);

                Config.Informations.Copyright.Organisation = await XmlHelper.ReadString(file, "copyright_Organisation").ConfigureAwait(false);
                Config.Informations.Copyright.Website = await XmlHelper.ReadString(file, "copyright_Website").ConfigureAwait(false);

                Config.Informations.Developer.Organisation = await XmlHelper.ReadString(file, "developer_Organisation").ConfigureAwait(false);
                Config.Informations.Developer.Website = await XmlHelper.ReadString(file, "developer_Website").ConfigureAwait(false);

                Config.Informations.Extension.SourceCode = await XmlHelper.ReadString(file, "extension_SourceCode").ConfigureAwait(false);
                Config.Informations.Extension.Website = await XmlHelper.ReadString(file, "extension_Website").ConfigureAwait(false);

                Config.Informations.Extension.IssueTracker = await XmlHelper.ReadString(file, "issueTracker").ConfigureAwait(false);

                Config.Auth0.Domain = await XmlHelper.ReadString(file, "auth0_Domain").ConfigureAwait(false);
                Config.Auth0.ClientId = await XmlHelper.ReadString(file, "auth0_ClientId").ConfigureAwait(false);
                Config.Auth0.ApiClientId = await XmlHelper.ReadString(file, "auth0_APIClientId").ConfigureAwait(false);
                Config.Auth0.ApiClientSecret = await XmlHelper.ReadString(file, "auth0_APIClientSecret").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }
        }
    }
}
