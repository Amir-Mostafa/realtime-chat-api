using System;
using System.Collections.Generic;

#nullable disable

namespace realtime.Models
{
    public partial class Message
    {
        public int Id { get; set; }
        public int? SenderId { get; set; }
        public int? ReceverId { get; set; }
        public string Msg { get; set; }
        public DateTime? Date { get; set; }
        public byte? Status { get; set; }
        public string Name { get; set; }
        public string Rname { get; set; }
    }
}
