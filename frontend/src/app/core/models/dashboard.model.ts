import { Vehicle } from './vehicle.model';

export interface DashboardStats {
  availableVehicles: number;
  inUseVehicles: number;
  inMaintenanceVehicles: number;
  activeTrips: number;
  upcomingMaintenance: Vehicle[];
}
