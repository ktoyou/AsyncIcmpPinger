using Microsoft.EntityFrameworkCore;
using PingerInfo.Core.DB.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace PingerInfo.Core.DB
{
    /// <summary>
    /// Модель базы данных MySql
    /// </summary>
    internal class DbApplicationContext : DbContext
    {
        /// <summary>
        /// Объекты для пингования
        /// </summary>
        public DbSet<PingObject> PingObjects { get; set; }

        public DbSet<Event> Events { get; set; }

        /// <summary>
        /// Конфигурация для подключения к базе данных
        /// </summary>
        /// <param name="configuration">Конфигурация базы данных</param>
        /// <exception cref="ArgumentNullException">Если база данных равна null или один из ее объектов null</exception>
        public DbApplicationContext(DBConfiguration configuration)
        {
            if(configuration == null) throw new ArgumentNullException(nameof(configuration));
            if(configuration.UID == null || configuration.Password == null || configuration.Server == null || configuration.DataBase == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            _connectionString = $"Server={configuration.Server};Database={configuration.DataBase};Uid={configuration.UID};Pwd={configuration.Password};";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString, 
                new MySqlServerVersion(new Version(0, 0, 0)));
        }

        private readonly string _connectionString;
    }
}
