using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AIMoba.Data;
using AIMoba.Models;
using System;
using System.Linq;

namespace MvcMovie.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new PlayerContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<PlayerContext>>()))
            {
                if (context.PlayerModel.Any())
                {
                    return;
                }

               context.SaveChanges();
            }
        }
    }
}