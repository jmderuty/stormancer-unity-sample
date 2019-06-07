

namespace Stormancer
{
    public struct SceneAddress
    {
        ///urn format : either {sceneI} or scene:/{sceneId} scene:/{app}/{sceneId} , scene:/{account}/{app}/{sceneId} or scene:/{cluster}/{account}/{app}/{sceneId}
		public string ClusterId;
        public string App;
        public string Account;
        public string SceneId;
               
        public static SceneAddress Parse(string urn, string defaultClusterId, string defaultAccount, string defaultApp)
        {
            SceneAddress address = new SceneAddress();
            if (urn.IndexOf("scene:") != 0)
            {
                address.SceneId = urn;
            }
            else
            {
                string[] tokens = urn.Split('/');
                address.SceneId = tokens[tokens.Length - 1];

                if (tokens.Length > 2)
                {
                    address.App = tokens[tokens.Length - 2];
                }
                if (tokens.Length > 3)
                {
                    address.Account = tokens[tokens.Length - 3];
                }
                if (tokens.Length > 4)
                {
                    address.ClusterId = tokens[tokens.Length - 4];
                }
            }

            if (address.ClusterId == null)
            {
                address.ClusterId = defaultClusterId;
            }
            if (address.Account == null)
            {
                address.Account = defaultAccount;
            }
            if (address.App == null)
            {
                address.App = defaultApp;
            }
            return address;
        }

        public string toUri()
        {
            return $"scene:/{ClusterId}/{Account}/{App}/{SceneId}";
        }
    };

}
