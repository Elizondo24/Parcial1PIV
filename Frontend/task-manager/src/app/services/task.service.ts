import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Task } from '../models/task';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  private apiUrl = 'http://localhost:5070/api/Tareas';

  constructor(private http: HttpClient) {}

  // Obtener todas las tareas
  getTasks(): Observable<Task[]> {
    return this.http.get<Task[]>(this.apiUrl);
  }

  // Obtener tarea por ID
  getTask(id: string): Observable<Task> {
    return this.http.get<Task>(`${this.apiUrl}/${id}`);
  }

  // Filtrar tareas por estado
getTasksByStatus(estado: string): Observable<Task[]> {
  return this.http.get<Task[]>(
    `${this.apiUrl}?estado=${estado}`
  );
}

// Buscar tareas
searchTasks(buscar: string): Observable<Task[]> {
  return this.http.get<Task[]>(
    `${this.apiUrl}?buscar=${encodeURIComponent(buscar)}`
  );
}

  // Crear tarea
  createTask(task: Task): Observable<Task> {
    return this.http.post<Task>(this.apiUrl, task);
  }

  // Actualizar tarea
  updateTask(id: string, task: Task): Observable<Task> {
    return this.http.put<Task>(`${this.apiUrl}/${id}`, task);
  }

  // Eliminar tarea
  deleteTask(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}