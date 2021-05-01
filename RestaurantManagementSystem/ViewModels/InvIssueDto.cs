using RestaurantManagementSystem.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagementSystem.ViewModels
{
    public class InvIssueDto
    {
        public InvIssueDto()
        {
            listIssueDetail = new List<InvIssueDetailDto>();
            listIssueDetailsToDelete = new List<int>();
            listUser = new List<UserDto>();
        }
        [Required]
        public string IssueNo { get; set; }
        public long Id { get; set; }
        [Required]
        public DateTime? IssueDate { get; set; }
        public string IssueDateString { get; set; }
        [Required]
        public long? IssueTo { get; set; }
        [Required]
        public long? IssueBy { get; set; }
        public string IssueToEmail { get; set; }
        public string IssueByEmail { get; set; }
        [Required]
        public string Description { get; set; }
        public int IssuePlaceId { get; set; }
        public string IssuePlace { get; set; }
        public List<UserDto> listUser { get; set; }
        public List<IssuePlaceDto> IssuePlaceList { get; set; }
        public List<InvIssueDetailDto> listIssueDetail { get; set; }
        public List<int> listIssueDetailsToDelete { get; set; }
        public int ObjectState { get; set; }
    }
    public class IssuePlaceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public static List<IssuePlaceDto> ConvertEnum()
        {
            var list = new List<IssuePlaceDto>();
            foreach (IssuePlace foo in Enum.GetValues(typeof(IssuePlace)))
            {
                var obj = new IssuePlaceDto()
                {
                    Id=(int)foo,
                    Name=foo.ToString()
                };
                list.Add(obj);
            }
            return list;
        }
    }
}