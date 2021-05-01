using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRchat.Models.DataModels
{
    public class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            HangmanDbContext context = app.ApplicationServices
            .CreateScope().ServiceProvider.GetRequiredService<HangmanDbContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
            if (!context.Questions.Any())
            {
                context.Questions.AddRange(
                new Question
                {
                    Title = "abruptly"
                },
                new Question
                {
                    Title = "awkward"
                },
                new Question
                {
                    Title = "numbskull"
                },
                new Question
                {
                    Title = "voyeurism"
                },
                new Question
                {
                    Title = "pneumonia"
                },
                new Question
                {
                    Title = "transgress"
                },
                new Question
                {
                    Title = "wristwatch"
                },
                new Question
                {
                    Title = "thumbscrew"
                },
                new Question
                {
                    Title = "espionage"
                },
                new Question
                {
                    Title = "mnemonic"
                });
                context.SaveChanges();
            }
        }
    }
}
