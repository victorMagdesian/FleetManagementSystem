export interface Maintenance {
  id: string;
  vehicleId: string;
  date: Date;
  description: string;
  cost: number;
}

export interface CreateMaintenanceRequest {
  vehicleId: string;
  date: Date;
  description: string;
  cost: number;
}
