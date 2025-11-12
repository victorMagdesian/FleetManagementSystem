export interface Vehicle {
  id: string;
  plate: string;
  model: string;
  year: number;
  mileage: number;
  lastMaintenanceDate: Date;
  nextMaintenanceDate: Date;
  status: VehicleStatus;
}

export enum VehicleStatus {
  Available = 'Available',
  InUse = 'InUse',
  InMaintenance = 'InMaintenance'
}

export interface CreateVehicleRequest {
  plate: string;
  model: string;
  year: number;
  mileage: number;
}

export interface UpdateVehicleRequest {
  model: string;
  year: number;
  mileage: number;
}
