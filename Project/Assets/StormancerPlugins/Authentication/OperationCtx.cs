using Stormancer.Core;

namespace Stormancer.Plugins
{
    public class OperationCtx
    {
        public OperationCtx(RequestContext<IScenePeer> requestContext)
        {
            RequestContext = requestContext;
        }
        public string Operation { get; set; }
        public string OriginId { get; set; }
        public RequestContext<IScenePeer> RequestContext { get; set; }
    }
}
