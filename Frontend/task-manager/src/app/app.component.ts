import { Component } from '@angular/core';
import { TaskListComponent } from './components/task-list/task-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [TaskListComponent],
  template: '<app-task-list></app-task-list>',
  styleUrl: './app.component.css'
})
export class AppComponent {}