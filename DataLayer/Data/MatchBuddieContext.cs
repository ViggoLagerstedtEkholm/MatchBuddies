
using DataLayer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace DataLayer.Data
{
    public class MatchBuddieContext : IdentityDbContext<User>
    {
        public MatchBuddieContext(DbContextOptions<MatchBuddieContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Friendship>().HasKey(e => new { e.ApplicationUserId, e.FriendId });

            builder.Entity<Friendship>()
               .HasOne(x => x.ApplicationUser)
               .WithMany(y => y.SentFriendRequests)
               .HasForeignKey(x => x.ApplicationUserId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Friendship>()
                .HasOne(x => x.Friend)
                .WithMany(y => y.ReceivedFriendRequests)
                .HasForeignKey(x => x.FriendId);

            builder.Entity<Post>().HasKey(e => e.PostID);

            builder.Entity<Post>()
               .HasOne(x => x.User)
               .WithMany(y => y.SentPost)
               .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Post>()
                .HasOne(x => x.Reciver)
                .WithMany(y => y.ReceivedPost)
                .HasForeignKey(x => x.ReciverId);

            builder.Entity<User>()
                .HasMany(x => x.ProfilePages)
                .WithMany(x => x.Users)

                .UsingEntity<ProfilePageUser>
                (vp => vp.HasOne<ProfilePage>().WithMany(),
                vp => vp.HasOne<User>().WithMany()).Property(vp => vp.Date);
        }

        public DbSet<User> User { get; set; }
        public DbSet<Friendship> Requests { get; set; }
        public DbSet<Interests> Interests { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<ProfilePage> ProfilePage { get; set; }
        public DbSet<ProfilePageUser> VisitedPages { get; set; }
        public DbSet<RelationType> RelationType { get; set; }
    }
}
