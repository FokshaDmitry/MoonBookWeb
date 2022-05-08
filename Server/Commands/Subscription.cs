using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Commands
{
    public class Subscription
    {
        LibProtocol.Models.Subscriptions data;
        AddDbContext add;
        public Subscription(LibProtocol.Models.Subscriptions data)
        {
            this.data = data;
            add = new AddDbContext();

        }

        public LibProtocol.Response buildResponce(ref LibProtocol.Response response)
        {
            try
            {
                add.Subscriptions.Add(data);
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
