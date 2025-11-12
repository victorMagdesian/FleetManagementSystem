import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadChildren: () => import('./features/dashboard/dashboard.routes')
          .then(m => m.DASHBOARD_ROUTES)
      },
      {
        path: 'vehicles',
        loadChildren: () => import('./features/vehicles/vehicles.routes')
          .then(m => m.VEHICLES_ROUTES)
      },
      {
        path: 'drivers',
        loadChildren: () => import('./features/drivers/drivers.routes')
          .then(m => m.DRIVERS_ROUTES)
      },
      {
        path: 'trips',
        loadChildren: () => import('./features/trips/trips.routes')
          .then(m => m.TRIPS_ROUTES)
      },
      {
        path: 'maintenance',
        loadChildren: () => import('./features/maintenance/maintenance.routes')
          .then(m => m.MAINTENANCE_ROUTES)
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
