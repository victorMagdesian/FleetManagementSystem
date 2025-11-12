export interface Trip {
  id: string;
  vehicleId: string;
  driverId: string;
  route: string;
  startDate: Date;
  endDate?: Date;
  distance: number;
}

export interface StartTripRequest {
  vehicleId: string;
  driverId: string;
  route: string;
}

export interface EndTripRequest {
  distance: number;
}
