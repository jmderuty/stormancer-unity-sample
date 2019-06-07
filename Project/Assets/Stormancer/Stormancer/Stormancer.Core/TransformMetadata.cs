using System.Collections.Generic;

namespace Stormancer
{
    public struct TransformMetadata
    {
        public Dictionary<string, string> SceneMetadata;
        public string SceneId;
        public string RouteName;
        public int SceneHandle;
        public int RouteHandle;
        public bool DontEncrypt;

		private Scene _scene;

        public TransformMetadata(Scene scene)
        {
            _scene = scene;
            SceneHandle = 0;
            RouteHandle = 0;
            DontEncrypt = false;
            SceneMetadata = new Dictionary<string, string>();
            SceneId = "";
            RouteName = "";
        }
    };
}
