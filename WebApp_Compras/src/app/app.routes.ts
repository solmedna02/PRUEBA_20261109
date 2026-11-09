
import { Routes } from '@angular/router';
import { Solicitudes } from './pages/solicitudes/solicitudes';
import { NuevaSolicitud } from './pages/nueva-solicitud/nueva-solicitud';

export const routes: Routes = [
  { path: '', component: Solicitudes },
  { path: 'nueva', component: NuevaSolicitud },
  { path: '**', redirectTo: '' }
];