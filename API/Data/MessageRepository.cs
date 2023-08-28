using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(x => x.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u =>
                    u.RecipientUsername == messageParams.Username &&
                    u.RecipientDeleted == false
                ),
                "Outbox" => query.Where(
                    u => u.SenderUsername == messageParams.Username &&
                    u.SenderDeleted == false
                ),
                _ => query.Where(u =>                   // basically the default parameter, which is "Unread"
                    u.RecipientUsername == messageParams.Username &&
                    u.RecipientDeleted == false &&
                    u.DateRead == null
                )
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            var query = _context.Messages           // it was 'var messages' before...
                // If we use Projection (NOW we do when we return), then we do not need to use Include():
                // At the same time, we want to remove all redundant selects which we do not need (lots of columns from asp net identity), so that's why we comment these Include-s of Users:
                // .Include(u => u.Sender).ThenInclude(p => p.Photos)              // ThenInclude() means we Include(), but not from Messages, but from the Sender, which is AppUser
                // .Include(u => u.Recipient).ThenInclude(p => p.Photos)           // same
                .Where(m =>
                    m.RecipientUsername == currentUserName &&
                    m.RecipientDeleted == false &&
                    m.SenderUsername == recipientUserName
                    ||
                    m.RecipientUsername == recipientUserName &&
                    m.SenderUsername == currentUserName &&
                    m.SenderDeleted == false
                ).OrderBy(m => m.MessageSent)
                // Now we do not exeucte query, so comment:
                // .ToListAsync();     // the query is performed here
                .AsQueryable();
            
            var unreadMessages = query
                .Where(m => m.DateRead == null && m.RecipientUsername == currentUserName)
                .ToList();
            
            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }

                // Below operation is unsafe, any data save shoud occur outside the repository (i.e., using UnitOfWork)
                // await _context.SaveChangesAsync();           // should be DELETED because now executed inside MessageHub
            }

            // OLD one (inefficient, too many redundant select-s (especially columns from asp net identity) inside query below):
            // return _mapper.Map<IEnumerable<MessageDto>>(messages);
            return await query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }
    }
}