using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Commands
{
    public class Remove
    {
        AddDbContext add;
        Guid id;
        public Remove(Guid id)
        {
            this.id = id;
            add = new AddDbContext();
        }
        public LibProtocol.Response buildResponce(ref LibProtocol.Response response)
        {
            try
            {
                foreach (var item in add.Posts.Where(p => p.Id == id))
                {
                    add.Posts.Remove(item);
                }
                foreach (var item in add.Comments.Where(c => c.idPost == id))
                {
                    add.Comments.Remove(item);
                }
                foreach (var item in add.Reactions.Where(r => r.IdPost == id))
                {
                    add.Reactions.Remove(item);
                }
                add.SaveChanges();
                response.code = LibProtocol.ResponseCode.Ok;
                response.succces = true;
            }
            catch (Exception)
            {
                response.succces = false;
                response.code = LibProtocol.ResponseCode.Error;
                response.StatusTxt = "Remove False";
            }
            return response;
        }
    }
}
