using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Parcial1PIV.API.Models
{
    public class Tarea
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(100)]
        [BsonElement("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(500)]
        [BsonElement("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [BsonElement("fechaLimite")]
        public DateTime FechaLimite { get; set; }

        [BsonElement("prioridad")]
        public string Prioridad { get; set; } = "Media";

        [BsonElement("estado")]
        public string Estado { get; set; } = "Pendiente";

        [BsonElement("fechaCreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}