export interface Task {
  id?: string;
  titulo: string;
  descripcion: string;
  fechaLimite: string;
  prioridad: string;
  estado: string;
  fechaCreacion?: string;
}