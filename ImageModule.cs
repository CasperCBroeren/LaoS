using LaoS.Interfaces;
using Nancy;
using System.Collections.Generic;
using System.IO;

namespace LaoS
{
    public class ImageModule : Nancy.NancyModule
    {
        private static Dictionary<string, byte[]> cache = new Dictionary<string, byte[]>();
        private ISlackApi slackApi;

        public ImageModule(ISlackApi slackApi)
        {
            this.slackApi = slackApi;
            Get("/img", async (args) =>
            {
                var team = Request.Query["team"];
                var imgId = Request.Query["imgId"];
                var imgName = Request.Query["imgName"];
                string cacheKey = $"{team}_{imgId}_{imgName}";
                if (cache.ContainsKey(cacheKey))
                { 
                    return Response.FromStream(new MemoryStream(cache[cacheKey]), "image/jpg");
                }
                else
                {
                    var image = await this.slackApi.FetchImage(imgId, imgName, team);
                    cache.Add(cacheKey, image);
                    return Response.FromStream(new MemoryStream(image), "image/jpg");
                }
            });
        }
    }
}
