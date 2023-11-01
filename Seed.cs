using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using AuthApp.Models;
using AuthApp.Data;

namespace AuthApp
{
    public class Seed
    {
        private readonly DataContext dataContext; 

        public Seed(DataContext context)
        {
            this.dataContext = context;
        }

        public void SeedUsers()
        {
            if (!dataContext.Users.Any())
            {
                var users = new List<User>
            {
                CreateSeededUser("John", "Doe", "johnD", "john.doe@example.com", "password123", new DateTime(1990, 1, 1)),
                CreateSeededUser("Jane", "Smith", "janeS", "jane.smith@example.com", "password456", new DateTime(1992, 6, 15))
            };

                dataContext.Users.AddRange(users);
                dataContext.SaveChanges();
            }
        }

        private User CreateSeededUser(string firstName, string lastName, string username, string email, string password, DateTime birthDate)
        {
            using (var hmac = new HMACSHA512())
            {
                return new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Username = username,
                    Email = email,
                    PasswordSalt = Convert.ToBase64String(hmac.Key),
                    PasswordHash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))),
                    BirthDate = birthDate
                };
            }
        }
    }
}
