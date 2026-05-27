namespace ProyectoITV.DTO;

/// <summary>
/// DTO que encapsula la información de una cita
/// </summary>
/// /// <param name="Id">Identificador único de la cita</param>
/// <param name="Dni">DNI del propietario.</param>
/// <param name="Matricula">Matrícula del vehículo</param>
/// <param name="FechaInspeccion">Fecha de la inspección</param>
/// <param name="Marca">Marca del vehículo</param>
/// <param name="Modelo">Modelo del vehículo</param>
/// <param name="Motor">Tipo del motor.</param>
/// <param name="FechaMatriculacion">Fecha de la matriculación del vehículo</param>
public record CitaDTO(int Id, string Dni, string Matricula, string FechaInspeccion, string Marca, string Modelo, string Motor, string FechaMatriculacion);