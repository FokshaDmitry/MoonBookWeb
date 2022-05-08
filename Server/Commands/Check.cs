using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Commands
{
    public class Check
    {
        AddDbContext add;
        Guid id;
        int sum;
        List<Guid> tmpid;
        public Check(Guid id)
        { 
            this.id = id;
            sum = 0;
            tmpid = new List<Guid>();
            add = new AddDbContext();
        }
        public LibProtocol.Response buildResponce(ref LibProtocol.Response response)
        {
            try
            {
                foreach (var item in add.Posts.Where(p => p.IdUser == id))
                {
                    sum += item.Like;
                    sum += item.Dislike;
                    sum++;
                    tmpid.Add(item.Id);
                }
                foreach (var item in tmpid)
                {
                    foreach (var comment in add.Comments.Where(c => c.idPost == item))
                    {
                        sum++;
                    }
                }
                response.data = sum;
                response.code = LibProtocol.ResponseCode.Ok;
                response.succces = true;   
            }
            catch (Exception)
            {
                response.succces = false;
                response.code = LibProtocol.ResponseCode.Error;
                response.StatusTxt = "Check False";
            }
            return response;
        }
    }
}
