using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace DataLayer.Data.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            isActive = true;
            IsEnabled = true;
        }
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter a firstname")]
        [RegularExpression("[a-zA-ZäåöüÄÅÖÜß]+", ErrorMessage = "Firstname can only contain letters")]
        public string Firstname { get; set; }
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        [RegularExpression("[a-zA-ZäåöüÄÅÖÜß]+", ErrorMessage = "Lastname can only contain letters")]
        [Required(ErrorMessage = "Please enter a lastname")]
        public string Lastname { get; set; }
        [PersonalData]
        [Column(TypeName = "nvarchar(500)")]
        [StringLength(500, MinimumLength = 20, ErrorMessage = "Message must be between 20 to 500 letters")]
        public string Description { get; set; }
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Please enter the city you´r from")]
        [RegularExpression("(.*[a-zA-ZäåöüÄÅÖÜß]){3}", ErrorMessage = "Your city must atleast have three letters")]
        [Display(Name = "City")]
        [StringLength(50)]
        public string City { get; set; }
        [PersonalData]
        [Column(TypeName = "int")]
        [Required(ErrorMessage = "Please enter your age")]
        [Display(Name = "age")]
        [Range(18, 100, ErrorMessage = "You must be at least 18 years old and at the most 100.")]
        public int age { get; set; }
        [Column(TypeName = "int")]
        public bool isActive { get; set; }
        public byte[] Image { get; set; }
        [Column(TypeName = "int")]
        public bool IsEnabled { get; set; }
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public GenderType Gender { get; set; }
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public MatchPreference Preference { get; set; }
        public virtual ICollection<Interests> Interests { get; set; } = new List<Interests>();
        [JsonIgnore]
        public virtual ICollection<Friendship> SentFriendRequests { get; set; } = new List<Friendship>();
        [JsonIgnore]
        public virtual ICollection<Post> SentPost { get; set; } = new List<Post>();
        [JsonIgnore]
        public virtual ICollection<Post> ReceivedPost { get; set; } = new List<Post>();
        [JsonIgnore]
        public virtual ICollection<Friendship> ReceivedFriendRequests { get; set; } = new List<Friendship>();
        public virtual ICollection<ProfilePage> ProfilePages { get; set; } = new List<ProfilePage>();

        [JsonIgnore]
        [NotMapped]
        public virtual ICollection<Friendship> Friends
        {
            get
            {
                var friends = SentFriendRequests.Where(x => x.Approved).ToList();
                friends.AddRange(ReceivedFriendRequests.Where(x => x.Approved));
                return friends;
            }
        }

    }
}
