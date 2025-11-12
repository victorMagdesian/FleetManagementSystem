import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Driver, CreateDriverRequest, UpdateDriverRequest } from '../models';

@Injectable({
  providedIn: 'root'
})
export class DriverService {
  private readonly endpoint = '/api/drivers';

  constructor(private api: ApiService) {}

  getAll(): Observable<Driver[]> {
    return this.api.get<Driver[]>(this.endpoint);
  }

  getById(id: string): Observable<Driver> {
    return this.api.get<Driver>(`${this.endpoint}/${id}`);
  }

  getActive(): Observable<Driver[]> {
    return this.api.get<Driver[]>(`${this.endpoint}/active`);
  }

  create(request: CreateDriverRequest): Observable<Driver> {
    return this.api.post<Driver>(this.endpoint, request);
  }

  update(id: string, request: UpdateDriverRequest): Observable<Driver> {
    return this.api.put<Driver>(`${this.endpoint}/${id}`, request);
  }

  activate(id: string): Observable<void> {
    return this.api.post<void>(`${this.endpoint}/${id}/activate`, {});
  }

  deactivate(id: string): Observable<void> {
    return this.api.post<void>(`${this.endpoint}/${id}/deactivate`, {});
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
