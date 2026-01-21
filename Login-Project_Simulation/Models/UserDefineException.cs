using System;

namespace Login_Project_Simulation.Models
{
    public class NullReferenceException : Exception
    {
        public NullReferenceException(string username)
            : base($"User '{username}' not found")
        {
            Username = username;
        }

        public string Username { get; }
    }
}
