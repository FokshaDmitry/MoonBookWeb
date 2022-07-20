namespace MoonBookWeb.Services
{
    public class Hesher
    {
        public String Hesh(string message)
        {
            using (var algo = System.Security.Cryptography.MD5.Create())
            {
                byte[] hesh = algo.ComputeHash(System.Text.Encoding.Unicode.GetBytes(message));
                var sb = new System.Text.StringBuilder();
                foreach (var h in hesh)
                {
                    sb.Append(h.ToString());
                }
                return sb.ToString();
            }
        }
    }
}
