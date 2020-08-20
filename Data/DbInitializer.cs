using System.Collections.Generic;
using Bogus;
using Skrawl.API.Data;
using Skrawl.API.Data.Models;
using System;
using System.Linq;

namespace Skrawl.API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SkrawlContext context)
        {
            context.Database.EnsureCreated();

            // Look for any notes.
            if (context.Notes.Any())
            {
                return;   // DB has been seeded
            }

            List<Note> notes = new Faker<Note>()
                .StrictMode(false)
                .Rules((faker, note) => 
                {
                    note.Title = faker.Lorem.Sentence();
                    note.Body = faker.Lorem.Paragraph();
                })
                .Generate(10);

            context.Notes.AddRange(notes);
            context.SaveChanges();
        }
    }
}