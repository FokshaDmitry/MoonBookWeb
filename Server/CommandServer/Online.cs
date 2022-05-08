using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.CommandServer
{
    public class Online
    {
        AddDbContext add;
        LibProtocol.Online online;
        Guid id;
        public Online(Guid id)
        {
            this.id = id;
            online = new LibProtocol.Online();
            add = new AddDbContext();
        }
        public LibProtocol.Response buildResponce(ref LibProtocol.Response response)
        {
            try
            {
                foreach (var Online in add.Users.GroupJoin(add.Posts, u => id, p => p.IdUser, (u, p) => new {User = u, Post = p}))
                {
                    online.users.Add(Online.User);
                    online.posts.AddRange(Online.Post);
                    response.data = new LibProtocol.Online { posts = online.posts, users = online.users };
                    response.succces = true;
                    response.code = LibProtocol.ResponseCode.Ok;
                }
                foreach (var Online in add.Posts.GroupJoin(add.Comments, p => id, c => c.idPost, (p, c) => new {Post = p, Com = c }))
                { 
                    foreach (var Coment in Online.Com.Join(add.Users, c => c.idUser, u => u.Id, (c, u) => new { Com = c, Use = u}))
                    {
                        online.users.Add(Coment.Use);
                        online.comments.Add(Coment.Com);
                    }
                    response.data = new LibProtocol.Online { users = online.users, comments = online.comments};
                    response.succces = true;
                    response.code = LibProtocol.ResponseCode.Ok;
                }
            }
            catch (Exception)
            {
                response.succces = false;
                response.code = LibProtocol.ResponseCode.Error;
                response.StatusTxt = "Online False";
            }
            return response;
        }
    }
}
