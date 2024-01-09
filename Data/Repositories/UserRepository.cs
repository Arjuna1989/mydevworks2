using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entity;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace API.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
             _mapper =  mapper;
             _context = context;
            
        }
        public async Task CreateUser(User user)
        {
           await _context.users.AddAsync(user);
        }

        public void Delete(User user)
        {
            _context.users.Remove(user);
        }

        public async Task<UserDto> GetUserByIdAsync(int Id)
        {
             return await _context.users
                           .Where(x=>x.Id ==Id)
                           .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                           .SingleOrDefaultAsync();
        }

        public async Task<User> GetUserByNameAsync(string name)
        {
             return await _context.users.Include(p=>p.Photos).FirstOrDefaultAsync(x=>x.UserName ==name);
        }

        public async Task<UserDto> GetUserDtoByNameAsync(string name)
        {

            return await _context.users
                      .Where(x => x.UserName == name)
                      .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                      .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
           // return await _context.users.Include(p => p.Photos).ToListAsync();
              return await _context.users
                           .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                           .ToListAsync();
        }

        public async Task<bool> SaveAll()
        {
          return  await _context.SaveChangesAsync()>0;
        }
        public async Task<bool> IsUserExist(string Name){

            return await _context.users.AnyAsync(x=>x.UserName ==Name);
        }

        public void Update(User user)
        {
             _context.Entry(user).State = EntityState.Modified;
        }
    }
}