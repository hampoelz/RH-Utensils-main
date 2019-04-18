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

        private static bool showLogin;

        public static bool ShowLogin
        {
            get
            {
                return showLogin;
            }
            set
            {
                if (showLogin == value) return;

                showLogin = value;
            }
        }
    }
}