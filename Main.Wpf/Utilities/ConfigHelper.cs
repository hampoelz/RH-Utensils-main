using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;

namespace Main.Wpf.Utilities
{
    public static class ConfigHelper
    {
        public static bool _loaded;

        public static async Task Read()
        {
            var file = Config.File;

            LogFile.WriteLog("Read Config File ...");

            try
            {
                Config.Informations.Extension.Name = await XmlHelper.ReadString(file, "name").ConfigureAwait(false);

                Config.Settings.File = await XmlHelper.ReadString(file, "settingsFile").ConfigureAwait(false);

                Config.Informations.Extension.Color = await XmlHelper.ReadString(file, "color").ConfigureAwait(false);
                Config.Informations.Extension.Theme = JsonHelper.ReadString(Config.Settings.Json, "theme");
                Config.Informations.Extension.Favicon =
                    await XmlHelper.ReadString(file, "favicon").ConfigureAwait(false);

                var height = await XmlHelper.ReadString(file, "height").ConfigureAwait(false);
                var width = await XmlHelper.ReadString(file, "width").ConfigureAwait(false);
                Config.Informations.Extension.WindowHeight =
                    ValidationHelper.IsStringValidInt(height) ? int.Parse(height) : 0;
                Config.Informations.Extension.WindowWidth =
                    ValidationHelper.IsStringValidInt(width) ? int.Parse(width) : 0;

                Config.Login.SkipLogin = await XmlHelper.ReadBool(file, "skipLogin").ConfigureAwait(false);

                Config.Menu.SingleSite = (await XmlHelper.ReadBool(file, "hideMenu").ConfigureAwait(false),
                    await XmlHelper.ReadString(file, "singleSite_Path").ConfigureAwait(false),
                    await XmlHelper.ReadString(file, "singleSite_StartArguments").ConfigureAwait(false));

                var sites = new List<MenuItem>();
                var titels = XmlHelper.ReadStringList(file, "site_Title");
                var icons = XmlHelper.ReadStringList(file, "site_Icon");
                var paths = XmlHelper.ReadStringList(file, "site_Path");
                var startArguments = XmlHelper.ReadStringList(file, "site_StartArguments");

                for (var site = 0; site != titels.Count; ++site)
                    if (titels[site] == "null" || paths[site] == "null") sites.Add(new MenuItem {Space = true});
                    else
                        sites.Add(new MenuItem
                        {
                            Title = titels[site],
                            Icon = Enum.TryParse(icons[site], out PackIconKind icon) ? icon : PackIconKind.Application,
                            Path = paths[site], StartArguments = startArguments[site]
                        });
                sites.Add(new MenuItem {Space = true});
                sites.Add(new MenuItem
                    {Title = "Information", Icon = PackIconKind.InformationOutline, Path = "info.exe"});
                if (!Config.Login.SkipLogin)
                    sites.Add(new MenuItem
                        {Title = "Anmelden", Icon = PackIconKind.AccountPlusOutline, Path = "account.exe"});

                await Config.Menu.SetSites(sites);

                Config.Menu.DefaultMenuState =
                    MenuHelper.StringToMenuState(await XmlHelper.ReadString(file, "defaultMenuState")
                        .ConfigureAwait(false));

                Config.Updater.Extension.VersionsHistoryFile =
                    await XmlHelper.ReadString(file, "versionsHistoryFile").ConfigureAwait(false);

                Config.Informations.Copyright.Organisation =
                    await XmlHelper.ReadString(file, "copyright_Organisation").ConfigureAwait(false);
                Config.Informations.Copyright.Website =
                    await XmlHelper.ReadString(file, "copyright_Website").ConfigureAwait(false);

                Config.Informations.Developer.Organisation =
                    await XmlHelper.ReadString(file, "developer_Organisation").ConfigureAwait(false);
                Config.Informations.Developer.Website =
                    await XmlHelper.ReadString(file, "developer_Website").ConfigureAwait(false);

                Config.Informations.Extension.SourceCode =
                    await XmlHelper.ReadString(file, "extension_SourceCode").ConfigureAwait(false);
                Config.Informations.Extension.Website =
                    await XmlHelper.ReadString(file, "extension_Website").ConfigureAwait(false);

                Config.Informations.Extension.IssueTracker =
                    await XmlHelper.ReadString(file, "issueTracker").ConfigureAwait(false);

                Config.Auth0.Domain = await XmlHelper.ReadString(file, "auth0_Domain").ConfigureAwait(false);
                Config.Auth0.ClientId = await XmlHelper.ReadString(file, "auth0_ClientId").ConfigureAwait(false);
                Config.Auth0.ApiClientId = await XmlHelper.ReadString(file, "auth0_APIClientId").ConfigureAwait(false);
                Config.Auth0.ApiClientSecret =
                    await XmlHelper.ReadString(file, "auth0_APIClientSecret").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex);
            }

            _loaded = true;
        }
    }
}