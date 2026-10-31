using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Parcial1PIV.API.Models;
using Parcial1PIV.API.Settings;

namespace Parcial1PIV.API.Services
{
    public class TareaService
    {
        private readonly IMongoCollection<Tarea> _tareas;
        private readonly IMongoDatabase _database;

        public TareaService(
            IMongoClient mongoClient,
            IOptions<MongoDbSettings> settings)
        {
            var configuracion = settings.Value;

            _database = mongoClient.GetDatabase(
                configuracion.DatabaseName
            );

            _tareas = _database.GetCollection<Tarea>(
                configuracion.CollectionName
            );
        }

        // Verificar conexión con MongoDB
        public async Task PingAsync()
        {
            await _database.RunCommandAsync<BsonDocument>(
                new BsonDocument("ping", 1)
            );
        }

        // Obtener todas las tareas
        public async Task<List<Tarea>> ObtenerTodasAsync()
        {
            return await _tareas.Find(_ => true).ToListAsync();
        }

        // Obtener tarea por ID
        public async Task<Tarea?> ObtenerPorIdAsync(string id)
        {
            return await _tareas
                .Find(t => t.Id == id)
                .FirstOrDefaultAsync();
        }

        // Crear tarea
        public async Task CrearAsync(Tarea tarea)
        {
            await _tareas.InsertOneAsync(tarea);
        }

        // Actualizar tarea
        public async Task<bool> ActualizarAsync(
            string id,
            Tarea tarea)
        {
            var resultado = await _tareas.ReplaceOneAsync(
                t => t.Id == id,
                tarea
            );

            return resultado.MatchedCount > 0;
        }

        // Eliminar tarea
        public async Task<bool> EliminarAsync(string id)
        {
            var resultado = await _tareas.DeleteOneAsync(
                t => t.Id == id
            );

            return resultado.DeletedCount > 0;
        }
    }
}