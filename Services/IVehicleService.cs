using Wheels_in_Csharp.Models;

namespace Wheels_in_Csharp.Services.Memory
{
    public interface IVehicleService
    {
        IList<Vehicle> ObterTodos();
        Vehicle Obter(int id);
        void Incluir(Vehicle vehicle);
        void Alterar(Vehicle vehicle);
        void Excluir(int id);
        IList<VehicleStatus> ObterTodosStatus();
    }
}