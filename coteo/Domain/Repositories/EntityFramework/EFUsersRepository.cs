﻿using coteo.Areas.Identity.Data;
using coteo.Domain.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace coteo.Domain.Repositories.EntityFramework
{
    public class EFUsersRepository : IUsersRepository
    {
        private readonly AppDbContext _context;

        public EFUsersRepository(AppDbContext context)
        {
            _context = context;
        }

        public void DeleteUser(string id)
        {
            _context.Users.Remove(new ApplicationUser() { Id = id });
        }

        public string GetShortNameById(string id)
        {
            string[] fullName = _context.Users.FirstOrDefault(x => x.Id == id).FullName.Split(' ');
            return $"{fullName[0]} {fullName[1][0]}. {fullName[2][0]}.";
        }

        public ApplicationUser GetUserById(string id)
        {
            return _context.Users.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<ApplicationUser> GetUsers()
        {
            return _context.Users;
        }

        public void SaveUser(ApplicationUser entity)
        {
            if (entity.Id == default)
                _context.Entry(entity).State = EntityState.Added;
            else
                _context.Entry(entity).State = EntityState.Modified;
        }
    }
}