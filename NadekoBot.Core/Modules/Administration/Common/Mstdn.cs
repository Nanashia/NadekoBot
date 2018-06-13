using Mastonet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NadekoBot.Modules.Administration.Services
{
    public class Mstdn
    {
        MastodonClient client;
        
        public static Lazy<Mstdn> Instance = new Lazy<Mstdn>(true);

        public Mstdn()
        {
            var appregister = new Mastonet.Entities.AppRegistration
            {
                Instance = "mstdn.kemono-friends.info",
            };

            var auth = new Mastonet.Entities.Auth
            {
                AccessToken = Environment.GetEnvironmentVariable("MSTDN_TOKEN")
            };

            client = new MastodonClient(appregister, auth);
        }

        public async Task<Mastonet.Entities.Status> post(string content, string url)
        {
            try
            {
                var wc = new System.Net.WebClient();
                Console.WriteLine("url {0}", url);
                var stream = await wc.OpenReadTaskAsync(url);
                Console.WriteLine("stream opened");
                var attachment = await client.UploadMedia(stream);
                Console.WriteLine("attachment {0}", attachment);
                return await client.PostStatus(content, Visibility.Public, mediaIds: new[] { attachment.Id });
            } catch(Exception)
            {
                throw;
            }
        }

        public async Task delete(string text)
        {
            long id;
            if(long.TryParse(text, out id))
            {
                await client.DeleteStatus(id);
            } else if(long.TryParse(text.Substring(text.LastIndexOf("/") + 1), out id))
            {
                await client.DeleteStatus(id);
            }
        }
    }
}