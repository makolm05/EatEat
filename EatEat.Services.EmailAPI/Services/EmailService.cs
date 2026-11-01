using EatEat.Services.EmailAPI.Data;
using Microsoft.EntityFrameworkCore;
using EatEat.Services.EmailAPI.Models.Dto;
using EatEat.Services.EmailAPI.Services.Interfaces;
using System.Text;
using EatEat.Services.EmailAPI.Models;

namespace EatEat.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public EmailService(DbContextOptions<AppDbContext> options)
        {
            _dbOptions = options;
        }

        public async Task EmailCart(CartDto cartDto)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("<br/>Cart Email Requested");
            message.AppendLine("<br/>Total " + cartDto.CartHeader.CartTotal);
            message.Append("<br/>");
            message.Append("<ul>");
            
            foreach(var item in cartDto.CartDetails)
            {
                message.Append("<li>");
                message.Append(item.Product.Name + " x " + item.Count);
                message.Append("</li>");
            }
            message.Append("</ul>");

            await Email(message.ToString(), cartDto.CartHeader.Email);
        }

        public async Task RegisterUserEmail(string email)
        {
            await Email($"{email} is registred", "admin@gmail.com");
        }

        private async Task Email(string message, string email)
        {
            var emailLog = new EmailLog()
            {
                Email = email,
                EmailSent = DateTime.Now,
                Message = message
            };

            await using var _db = new AppDbContext(_dbOptions);
            _db.EmailLogs.Add(emailLog);
            await _db.SaveChangesAsync();
        }
    }
}
