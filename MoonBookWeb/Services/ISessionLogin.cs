﻿namespace MoonBookWeb.Services
{
    public interface ISessionLogin
    {
        public User user { get; set; }
        public void Set(string id);
    }
}