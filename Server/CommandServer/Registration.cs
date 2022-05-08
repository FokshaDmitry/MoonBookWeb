using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Commands
{
    class Registration
    {
        LibProtocol.Models.User data;
        AddDbContext add;

        public Registration(LibProtocol.Models.User data)
        {
            this.data = data;
            add = new AddDbContext();
        }

        public bool ChekLogin()
        {
            if (add.Users.Count() != 0)
            {
                foreach (var item in add.Users)
                {
                    if (item.Login == data.Login)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public LibProtocol.Response buildResponce(ref LibProtocol.Response response)
        {
            try
            {
                add.Users.Add(data);
                add.SaveChanges();
                response.succces = true;
                response.code = LibProtocol.ResponseCode.Ok;
                response.StatusTxt = "Add Ok";
            }
            catch (Exception)
            {
                response.succces = false;
                response.code = LibProtocol.ResponseCode.Error;
                response.StatusTxt = "Add False";
            }
            return response;
        }
    }
}
