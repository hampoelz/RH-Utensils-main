using System.Threading.Tasks;

namespace Main.Wpf.Functions
{
    public static class Login
    {
        private static bool skipLogin;

        public static bool SkipLogin
        {
            get
            {
                return skipLogin;
            }
            set
            {
                if (skipLogin == value) return;

                skipLogin = value;
            }
        }

        public static class LoggedIn
        {
            public static async Task Set(bool value)
            {
                if (Informations.Extension.Name == "RH Utensils") return;

                Config._isChanging = true;
                await Xml.SetString(Config.File, "config/loggedIn", value.ToString()).ConfigureAwait(false);
                Config._isChanging = false;
            }

            public static async Task<bool> Get()
            {
                if ( Informations.Extension.Name == "RH Utensils") return false;

                return await Xml.ReadBool(Config.File, "loggedIn").ConfigureAwait(false);
            }
        }

        public static class FirstRun
        {
            public static async Task Set(bool value)
            {
                if (Informations.Extension.Name == "RH Utensils") return;

                Config._isChanging = true;
                await Xml.SetString(Config.File, "config/firstRun", value.ToString()).ConfigureAwait(false);
                Config._isChanging = false;
            }

            public static async Task<bool> Get()
            {
                if (Informations.Extension.Name == "RH Utensils") return false;

                return await Xml.ReadBool(Config.File, "firstRun").ConfigureAwait(false);
            }
        }
    }
}