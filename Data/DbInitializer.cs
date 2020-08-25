using System.Text;
using System.Reflection.Metadata;
using System.Collections.Generic;
using Bogus;
using Skrawl.API.Data;
using Skrawl.API.Data.Models;
using System;
using System.Linq;
using Skrawl.API.Services;
using Skrawl.API.Infrastructure;

namespace Skrawl.API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SkrawlContext context)
        {
            // context.Database.EnsureCreated();

            // Look for any notes.
            if (context.Notes.Any() && context.Users.Any())
            {
                return;   // DB has been seeded
            }

            var passwordService = new PasswordService();
            byte[] salt = null;

            byte[] password = passwordService.HashPassword(Encoding.UTF8.GetBytes("password"), ref salt);

            List<User> users = new Faker<User>()
                .StrictMode(false)
                .Rules((faker, user) =>
                {
                    user.Email = faker.Person.Email;
                    user.Username = faker.Person.UserName;
                    user.Password = password;
                    user.Salt = salt;
                    user.Role = faker.PickRandom<string>(new[] {Policies.Admin, Policies.User});
                })
                .Generate(10);

            context.Users.AddRange(users);
            context.SaveChanges();

            List<Note> notes = new Faker<Note>()
                .StrictMode(false)
                .Rules((faker, note) => 
                {
                    note.Title = faker.Lorem.Sentence(2, 1);
                    note.Body = faker.Lorem.Paragraph();
                    note.UserId = faker.PickRandom<User>(users).Id;
                })
                .Generate(100);

            context.Notes.AddRange(notes);
            context.SaveChanges();
        }
    }
}