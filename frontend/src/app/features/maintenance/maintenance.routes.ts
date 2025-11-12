import { Routes } from '@angular/router';

export const MAINTENANCE_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/maintenance-page/maintenance-page.component')
      .then(m => m.MaintenancePageComponent)
  }
];
