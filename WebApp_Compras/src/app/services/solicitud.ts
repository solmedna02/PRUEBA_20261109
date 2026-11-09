import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export interface DetalleSolicitud {
  descripcion: string;
  cantidad: number;
  precioUnitario: number;
}

export interface CrearSolicitud {
  departamentoId: number;
  solicitanteId: number;
  justificacion: string;
  detalles: DetalleSolicitud[];
}

export interface Solicitud {
  id: number;
  departamentoId: number;
  solicitanteId: number;
  justificacion: string;
  fechaCreacion: string;
  estado: string;
  total: number;
  aprobacionDepartamento: string;
  aprobacionFinanzas: string;
  aprobacionGerencia: string;
}

@Injectable({
  providedIn: 'root'
})
export class SolicitudService {
  private http = inject(HttpClient);

  // CAMBIA EL PUERTO POR EL DE TU API .NET
  private apiUrl = 'https://localhost:7211/api/Solicitudes';

  listar() {
    return this.http.get<Solicitud[]>(this.apiUrl);
  }

  crear(datos: CrearSolicitud) {
    return this.http.post(this.apiUrl, datos);
  }

  eliminar(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`, {
      responseType: 'text'
    });
  }

  enviar(id: number) {
    return this.http.put(
      `${this.apiUrl}/${id}/enviar`,
      {},
      { responseType: 'text' }
    );
  }

  aprobar(id: number, etapa: string) {
    return this.http.put(
      `${this.apiUrl}/${id}/aprobar/${etapa}`,
      {},
      { responseType: 'text' }
    );
  }

  rechazar(id: number) {
    return this.http.put(
      `${this.apiUrl}/${id}/rechazar`,
      {},
      { responseType: 'text' }
    );
  }
}