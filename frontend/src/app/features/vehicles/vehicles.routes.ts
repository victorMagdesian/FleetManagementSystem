import { Routes } from '@angular/router';

export const VEHICLES_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/vehicles-page/vehicles-page.component')
      .then(m => m.VehiclesPageComponent)
  }
];
