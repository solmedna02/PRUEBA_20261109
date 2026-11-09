import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  SolicitudService,
  Solicitud
} from '../../services/solicitud';

@Component({
  selector: 'app-solicitudes',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './solicitudes.html',
  styleUrl: './solicitudes.css'
})
export class Solicitudes implements OnInit {

  private service = inject(SolicitudService);

  solicitudes: Solicitud[] = [];
  mensaje = '';

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.service.listar().subscribe({
      next: (datos) => {
        this.solicitudes = datos;
      },
      error: () => {
        this.mensaje = 'Error al cargar las solicitudes';
      }
    });
  }

  enviar(id: number): void {
    this.service.enviar(id).subscribe({
      next: () => this.cargar(),
      error: () => alert('No se pudo enviar la solicitud')
    });
  }

  aprobar(solicitud: Solicitud): void {
    let etapa = 'departamento';

    if (solicitud.aprobacionDepartamento === 'Aprobado') {
      etapa = 'finanzas';
    }

    if (solicitud.aprobacionFinanzas === 'Aprobado') {
      etapa = 'gerencia';
    }

    this.service.aprobar(solicitud.id, etapa).subscribe({
      next: () => this.cargar(),
      error: () => alert('No se pudo aprobar')
    });
  }

  rechazar(id: number): void {
    this.service.rechazar(id).subscribe({
      next: () => this.cargar(),
      error: () => alert('No se pudo rechazar')
    });
  }

  eliminar(id: number): void {
    if (!confirm('¿Eliminar esta solicitud?')) return;

    this.service.eliminar(id).subscribe({
      next: () => this.cargar(),
      error: () => alert('No se pudo eliminar')
    });
  }
}