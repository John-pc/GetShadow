using GetShadows.Request;

namespace GetShadows
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var url = @"http://www.ishadowsocks.com";
            if (args.Length > 0)
                url = args[0];
            var request = new GetRequest();
            request.GetContent(url);
            request.SerachInfo(url);
        }
    }
}