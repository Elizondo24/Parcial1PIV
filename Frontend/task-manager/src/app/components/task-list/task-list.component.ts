import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../../services/task.service';
import { Task } from '../../models/task';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css'
})
export class TaskListComponent implements OnInit {

  tareas: Task[] = [];

  cargando = false;
  mensajeError = '';

  mostrarFormulario = false;
  editando = false;

  busqueda = '';
  filtroEstado = 'Todas';
  ordenamiento = '';

  tareaActual: Task = this.nuevaTarea();

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.cargarTareas();
  }

  nuevaTarea(): Task {
    return {
      titulo: '',
      descripcion: '',
      fechaLimite: '',
      prioridad: 'Media',
      estado: 'Pendiente'
    };
  }

  cargarTareas(): void {
    this.cargando = true;
    this.mensajeError = '';

    this.taskService.getTasks().subscribe({
      next: (data) => {
        this.tareas = data;
        this.aplicarFiltrosLocales();
        this.cargando = false;
      },
      error: (error) => {
        console.error(error);
        this.mensajeError = 'No se pudieron cargar las tareas.';
        this.cargando = false;
      }
    });
  }

  abrirFormulario(): void {
    this.tareaActual = this.nuevaTarea();
    this.editando = false;
    this.mostrarFormulario = true;
  }

  editarTarea(tarea: Task): void {
    this.tareaActual = {
      ...tarea,
      fechaLimite: tarea.fechaLimite.substring(0, 10)
    };

    this.editando = true;
    this.mostrarFormulario = true;
  }

  guardarTarea(): void {
    if (!this.tareaActual.titulo.trim() ||
        !this.tareaActual.fechaLimite) {
      return;
    }

    const tarea: Task = {
      ...this.tareaActual,
      fechaLimite: `${this.tareaActual.fechaLimite}T23:59:00`
    };

    if (this.editando && tarea.id) {

      this.taskService.updateTask(tarea.id, tarea).subscribe({
        next: () => {
          this.cerrarFormulario();
          this.cargarTareas();
        },
        error: (error) => {
          console.error(error);
          alert('No se pudo actualizar la tarea.');
        }
      });

    } else {

      this.taskService.createTask(tarea).subscribe({
        next: () => {
          this.cerrarFormulario();
          this.cargarTareas();
        },
        error: (error) => {
          console.error(error);
          alert('No se pudo crear la tarea.');
        }
      });

    }
  }

  cambiarEstado(tarea: Task): void {
    if (!tarea.id) return;

    const actualizada: Task = {
      ...tarea,
      estado: tarea.estado === 'Pendiente'
        ? 'Completada'
        : 'Pendiente'
    };

    this.taskService.updateTask(tarea.id, actualizada).subscribe({
      next: () => this.cargarTareas(),
      error: (error) => {
        console.error(error);
        alert('No se pudo actualizar el estado.');
      }
    });
  }

  eliminarTarea(tarea: Task): void {
    if (!tarea.id) return;

    if (!confirm(`¿Deseas eliminar "${tarea.titulo}"?`)) {
      return;
    }

    this.taskService.deleteTask(tarea.id).subscribe({
      next: () => this.cargarTareas(),
      error: (error) => {
        console.error(error);
        alert('No se pudo eliminar la tarea.');
      }
    });
  }

  cerrarFormulario(): void {
    this.mostrarFormulario = false;
    this.tareaActual = this.nuevaTarea();
  }

  aplicarFiltrosLocales(): void {
    let resultado = [...this.tareas];

    if (this.filtroEstado !== 'Todas') {
      resultado = resultado.filter(
        t => t.estado === this.filtroEstado
      );
    }

    if (this.busqueda.trim()) {
      const texto = this.busqueda.toLowerCase();

      resultado = resultado.filter(t =>
        t.titulo.toLowerCase().includes(texto) ||
        t.descripcion.toLowerCase().includes(texto)
      );
    }

    if (this.ordenamiento === 'fecha') {
      resultado.sort(
        (a, b) =>
          new Date(a.fechaLimite).getTime() -
          new Date(b.fechaLimite).getTime()
      );
    }

    if (this.ordenamiento === 'prioridad') {
      const peso: Record<string, number> = {
        Alta: 3,
        Media: 2,
        Baja: 1
      };

      resultado.sort(
        (a, b) => peso[b.prioridad] - peso[a.prioridad]
      );
    }

    this.tareasVisibles = resultado;
  }

  tareasVisibles: Task[] = [];

  get totalTareas(): number {
    return this.tareas.length;
  }

  get pendientes(): number {
    return this.tareas.filter(
      t => t.estado === 'Pendiente'
    ).length;
  }

  get completadas(): number {
    return this.tareas.filter(
      t => t.estado === 'Completada'
    ).length;
  }
}