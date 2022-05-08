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
                foreach(var reac in add.Reactions.Join(add.Posts, r => r.IdPost, p => p.Id, (r,p) => new {Ract = r, Post = p}))
                {
                    if (reac.Ract.IdPost == data.IdPost)
                    {
                        if(reac.Ract.Reaction == data.Reaction && reac.Ract.Reaction == 1)
                        {
                            reac.Ract.Reaction = 0;
                            reac.Post.Like--;
                            add.Reactions.Update(reac.Ract);
                            add.Posts.Update(reac.Post);
                            add.SaveChanges();
                            response.succces = true;
                            response.code = LibProtocol.ResponseCode.Ok;
                            return response;
                        }
                        if (reac.Ract.Reaction == data.Reaction && reac.Ract.Reaction == 2)
                        {
                            reac.Ract.Reaction = 0;
                            reac.Post.Dislike--;
                            add.Reactions.Update(reac.Ract);
                            add.Posts.Update(reac.Post);
                            add.SaveChanges();
                            response.succces = true;
                            response.code = LibProtocol.ResponseCode.Ok;
                            return response;
                        }
                        if (data.Reaction == 1)
                        {
                            reac.Ract.Reaction = 1;
                            reac.Post.Like++;
                            reac.Post.Dislike--;
                            add.Reactions.Update(reac.Ract);
                            add.Posts.Update(reac.Post);
                            add.SaveChanges();
                            response.succces = true;
                            response.code = LibProtocol.ResponseCode.Ok;
                            return response;
                        }
                        if (data.Reaction == 2)
                        {
                            reac.Ract.Reaction = 2;
                            reac.Post.Dislike++;
                            reac.Post.Like--;
                            add.Reactions.Update(reac.Ract);
                            add.Posts.Update(reac.Post);
                            add.SaveChanges();
                            response.succces = true;
                            response.code = LibProtocol.ResponseCode.Ok;
                            return response;
                        }

                    }
                }
                add.Reactions.Add(new LibProtocol.Models.Reactions { Id = Guid.NewGuid(), Reaction = data.Reaction, IdPost = data.IdPost, IdUser = data.IdUser });
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
