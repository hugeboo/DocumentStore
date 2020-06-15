using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace DocumentStore.Model
{
    public sealed class Document
    {
        [BsonId]
        [Required]
        public string Id { get; set; }
        [Required]
        public string Data { get; set; }
    }
}
