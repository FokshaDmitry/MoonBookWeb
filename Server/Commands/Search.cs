using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Commands
{
    public class Search
    {
        LibProtocol.Models.User data;
        AddDbContext add;
        LibProtocol.Online online;
        public Search(LibProtocol.Models.User data)
        {
            this.data = data;
            add = new AddDbContext();
            online = new LibProtocol.Online();
            online.users = new List<LibProtocol.Models.User>();
            online.subscriptions = new List<LibProtocol.Models.Subscriptions>();
        }

        public LibProtocol.Response buildResponce(ref LibProtocol.Response response)
        {
            try
            {
                foreach (var item in add.Users)
                {
                    if ($"{item.Name.ToLower()}{item.Surname.ToLower()}" == data.Name.Replace(" ", "").ToLower())
                    {
                        online.users.Add(item);
                    }
                }
                foreach (var item in add.Subscriptions.Where(s => s.IdUser == data.Id))
                {
                    online.subscriptions.Add(item);
                }
                if (online.users.Count() == 0)
                {
                    response.succces = false;
                    response.code = LibProtocol.ResponseCode.Error;
                    response.StatusTxt = "No matvhes found";
                }
            }
            catch (Exception)
            {
                response.succces = false;
                response.code = LibProtocol.ResponseCode.Error;
                response.StatusTxt = "Wrong Search";
                return response;
            }

            response.data = online;
            response.succces = true;
            response.code = LibProtocol.ResponseCode.Ok;
            return response;
        }
    }
}
