﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;

namespace ToDoList.Model.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly ScheduleDbContext _context;
        public UserRepository(ScheduleDbContext context)
        {
            _context = context;
        }

        public void Add(User entity)
        {
            if (entity == null) throw new ArgumentNullException();

            _context.Users.Add(entity);
            _context.SaveChangesAsync();
        }

        public void AddRange(IEnumerable<User> entities)
        {
            if (entities == null) throw new ArgumentNullException();
            if (!entities.Any()) throw new ArgumentException("No entities on the list");

            _context.Users.AddRange(entities);
            _context.SaveChangesAsync();
        }

        public void Remove(Guid id)
        {
            var users = this.Find(x => x.Id == id).ToList();

            if (users.Count != 0)
            {
                _context.Users.Remove(users[0]);
                _context.SaveChangesAsync();
            }
        }

        public void Remove(User entity)
        {
            if (entity == null) throw new ArgumentNullException();

            _context.Users.Remove(entity);
            _context.SaveChangesAsync();
        }

        public IEnumerable<User> Find(Expression<Func<User, bool>> predicate)
        {          
            return _context.Users.Where(predicate).ToList();
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public User GetById(Guid id)
        {
            var users = _context.Users.Where(x => x.Id == id).ToList();
            if (users.Count > 0) return users[0];
            else return null;
        }

        public void RemoveRange(IEnumerable<User> entities)
        {
            _context.Users.RemoveRange(entities);
            _context.SaveChangesAsync();
        }

        public void Update(User entity)
        {
            var user = _context.Users.Find(entity);

            user.Id = entity.Id;
            user.Karma = entity.Karma;
            user.Name = entity.Name;

            _context.SaveChangesAsync();
        }
    }
}
