import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Maintenance, CreateMaintenanceRequest, Vehicle } from '../models';

@Injectable({
  providedIn: 'root'
})
export class MaintenanceService {
  private readonly endpoint = '/api/maintenance';

  constructor(private api: ApiService) {}

  getAll(): Observable<Maintenance[]> {
    return this.api.get<Maintenance[]>(this.endpoint);
  }

  getById(id: string): Observable<Maintenance> {
    return this.api.get<Maintenance>(`${this.endpoint}/${id}`);
  }

  getByVehicle(vehicleId: string): Observable<Maintenance[]> {
    return this.api.get<Maintenance[]>(`${this.endpoint}/vehicle/${vehicleId}`);
  }

  getUpcoming(days: number): Observable<Vehicle[]> {
    return this.api.get<Vehicle[]>(`${this.endpoint}/upcoming/${days}`);
  }

  create(request: CreateMaintenanceRequest): Observable<Maintenance> {
    return this.api.post<Maintenance>(this.endpoint, request);
  }
}
