import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Vehicle, CreateVehicleRequest, UpdateVehicleRequest } from '../models';

@Injectable({
  providedIn: 'root'
})
export class VehicleService {
  private readonly endpoint = '/api/vehicles';

  constructor(private api: ApiService) {}

  getAll(): Observable<Vehicle[]> {
    return this.api.get<Vehicle[]>(this.endpoint);
  }

  getById(id: string): Observable<Vehicle> {
    return this.api.get<Vehicle>(`${this.endpoint}/${id}`);
  }

  getAvailable(): Observable<Vehicle[]> {
    return this.api.get<Vehicle[]>(`${this.endpoint}/available`);
  }

  create(request: CreateVehicleRequest): Observable<Vehicle> {
    return this.api.post<Vehicle>(this.endpoint, request);
  }

  update(id: string, request: UpdateVehicleRequest): Observable<Vehicle> {
    return this.api.put<Vehicle>(`${this.endpoint}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
