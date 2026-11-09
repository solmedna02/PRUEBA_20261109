import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import {
  SolicitudService,
  CrearSolicitud
} from '../../services/solicitud';

@Component({
  selector: 'app-nueva-solicitud',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './nueva-solicitud.html'
})
export class NuevaSolicitud {
  private service = inject(SolicitudService);
  private router = inject(Router);

  mensaje = '';
  guardando = false;

  solicitud: CrearSolicitud = {
    departamentoId: 1,
    solicitanteId: 1,
    justificacion: '',
    detalles: [
      {
        descripcion: '',
        cantidad: 1,
        precioUnitario: 0
      }
    ]
  };

  get total(): number {
    return this.solicitud.detalles.reduce(
      (suma, d) => suma + d.cantidad * d.precioUnitario,
      0
    );
  }

  agregarDetalle() {
    this.solicitud.detalles.push({
      descripcion: '',
      cantidad: 1,
      precioUnitario: 0
    });
  }

  quitarDetalle(index: number) {
    if (this.solicitud.detalles.length > 1) {
      this.solicitud.detalles.splice(index, 1);
    }
  }

  guardar() {
    if (!this.solicitud.justificacion.trim() ||
        this.solicitud.detalles.some(d =>
          !d.descripcion.trim() ||
          d.cantidad <= 0 ||
          d.precioUnitario < 0)) {
      this.mensaje = 'Completa correctamente los campos';
      return;
    }

    this.guardando = true;

    this.service.crear(this.solicitud).subscribe({
      next: () => {
        this.router.navigateByUrl('/');
      },
      error: () => {
        this.mensaje = 'Error al guardar la solicitud';
        this.guardando = false;
      }
    });
  }
}