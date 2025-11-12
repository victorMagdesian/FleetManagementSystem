import { Routes } from '@angular/router';

export const TRIPS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/trips-page/trips-page.component')
      .then(m => m.TripsPageComponent)
  }
];
