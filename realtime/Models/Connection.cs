using System;
using System.Collections.Generic;

#nullable disable

namespace realtime.Models
{
    public partial class Connection
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ConId { get; set; }
        public int? SendId { get; set; }
    }
}
