using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.CommandServer
{
    public class Reaction
    {
        LibProtocol.Models.Reactions data;
        AddDbContext add;
        public Reaction(LibProtocol.Models.Reactions data)
        {
            this.data = data;
            add = new AddDbContext();
        }

        public LibProtocol.Response buildResponce(ref LibProtocol.Response response)
        {
            try
            {
                var qerty = add.Reactions.Where(r => r.IdPost == data.IdPost).Join(add.Posts, r => r.IdPost, p => p.Id, (r, p) => new { Ract = r, Post = p });

                foreach (var reac in qerty)
                {
                    if (reac.Ract.IdUser == data.IdUser)
                    {
                        if(reac.Ract.Reaction == data.Reaction && reac.Ract.Reaction == 1)
                        {
                            reac.Ract.Reaction = 0;
                            reac.Post.Like--;
                            add.Posts.Update(reac.Post);
                            add.Reactions.Update(reac.Ract);
                        }
                        else if (reac.Ract.Reaction == data.Reaction && reac.Ract.Reaction == 2)
                        {
                            reac.Ract.Reaction = 0;
                            reac.Post.Dislike--;
                            add.Posts.Update(reac.Post);
                            add.Reactions.Update(reac.Ract);
                        }
                        else if (reac.Ract.Reaction != data.Reaction && data.Reaction == 1)
                        {
                            reac.Ract.Reaction = 1;
                            reac.Post.Like++;
                            if (reac.Post.Dislike != 0) reac.Post.Dislike--;
                            add.Posts.Update(reac.Post);
                            add.Reactions.Update(reac.Ract);
                        }
                        else if (reac.Ract.Reaction != data.Reaction && data.Reaction == 2)
                        {
                            reac.Ract.Reaction = 2;
                            reac.Post.Dislike++;
                            if(reac.Post.Like != 0) reac.Post.Like--;
                            add.Posts.Update(reac.Post);
                            add.Reactions.Update(reac.Ract);
                        }
                    }
                    else
                    {
                        if (data.Reaction == 1) reac.Post.Like++;
                        else reac.Post.Dislike++;

                        add.Posts.Update(reac.Post);
                        add.Reactions.Add(data);
                    }
                }
                if (qerty.Count() == 0)
                {
                    var q = add.Posts.Where(p => p.Id == data.IdPost).Single();
                    if (data.Reaction == 1) q.Like++;
                    else q.Dislike++;
                    
                    add.Reactions.Add(data);
                    add.Posts.Update(q);
                }
                add.SaveChanges();
                response.succces = true;
                response.code = LibProtocol.ResponseCode.Ok;
            }
            catch (Exception)
            {
                response.succces = false;
                response.code = LibProtocol.ResponseCode.Error;
                response.StatusTxt = "Post False";
            }
            return response;
        }
    }
}
