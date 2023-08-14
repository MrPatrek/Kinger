using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        // This was before:
        // public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
        // Now it is:
        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();          // AsQueryable() means we do NOT EXECUTE our query yet
            var likes = _context.Likes.AsQueryable();

            if (likesParams.Predicate == "liked")       // who does userId like?
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.TargetUser);              // notice that here we return the actual user entities, not just the Id as on a line above
            }

            if (likesParams.Predicate == "likedBy")     // who likes userId?
            {
                likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);              // read comment above
            }

            var likedUsers = users.Select(user => new LikeDto           // how we want to return users:
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
                .Include(x => x.LikedUsers)                     // don't forget that other entities are not included by default unless it is explicitly stated, that's why we have this method here
                .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}